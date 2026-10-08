using System.Globalization;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Security;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.TransferInterface;
using nUpdate.Installer;
using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Platform;
using nUpdate.Tests.Integration.Support;
using nUpdate.Tests.Support;
using nUpdate.UpdateInstaller;
using nUpdate.UpdateInstaller.Abstractions;
using nUpdate.Updating;

namespace nUpdate.Tests.Integration;

/// <summary>Administration publishes over FTP or SFTP, the client library updates over HTTP, the installer applies, the statistics count, legacy projects migrate.</summary>
[Collection(ServerCollectionFixture.Name)]
[Trait("Category", "Integration")]
public sealed class EndToEndTests : IDisposable
{
    private const string ProjectPassword = "integration-password";
    private readonly IntegrationContext _context;

    public EndToEndTests(ServerFixture server)
    {
        _context = new IntegrationContext(server);
    }

    public void Dispose() => _context.Dispose();

    private static UpdateManagerServices ClientServices(string product, string? executablePath = null, IProcessLauncher? launcher = null)
    {
        var applicationInfo = Substitute.For<IApplicationInfo>();
        applicationInfo.ProductName.Returns(product + "-" + Guid.NewGuid().ToString("N"));
        applicationInfo.ExecutablePath.Returns(executablePath);
        applicationInfo.DeclaredVersion.Returns("1.0.0");
        applicationInfo.UserAgentProduct.Returns(product + "/1.0");
        var systemInformation = Substitute.For<ISystemInformation>();
        systemInformation.RuntimeIdentifier.Returns("win-x64");
        systemInformation.OperatingSystemName.Returns("Windows 11");
        return new UpdateManagerServices
        {
            ApplicationInfo = applicationInfo,
            SystemInformation = systemInformation,
            ProcessLauncher = launcher ?? Substitute.For<IProcessLauncher>(),
            ApplicationTerminator = Substitute.For<IApplicationTerminator>(),
        };
    }

    [DockerFact]
    public async Task PublishUpdateInstallAndCount()
    {
        await _context.Server.ResetAsync();
        var transfer = _context.FtpSettings();
        await using (var learn = await _context.ConnectTrustedAsync(transfer, IntegrationContext.FtpCredentials))
            await learn.ListAsync(string.Empty, false);

        // --- Administration: create the project (uploads nupdate-statistics.php, checks the API) ---
        var created = await _context.Projects.CreateAsync(new NewProjectRequest
        {
            Name = "Integration",
            Folder = _context.ProjectFolder("Integration"),
            UpdateUrl = _context.Server.HttpBaseUrl,
            Transfer = transfer,
            Secrets = new ProjectSecrets { TransferPassword = ServerFixture.FtpPassword, StatisticsDatabasePassword = ServerFixture.DbPassword },
            Statistics = new StatisticsSettings { Enabled = true, Database = new StatisticsDatabaseSettings { Host = "mysql", Name = ServerFixture.DbName, Username = ServerFixture.DbUser } },
            ProjectPassword = ProjectPassword,
        });
        var project = created.Project;
        var secrets = created.Secrets;
        project.Path.ShouldBe(Path.Combine(_context.ProjectFolder("Integration"), "project.nupdproj"));
        using (var unauthorized = await _context.HttpClient.GetAsync(_context.Server.HttpBaseUrl + "nupdate-statistics.php/v2/projects/" + project.Id + "/statistics"))
        {
            unauthorized.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
            (await unauthorized.Content.ReadAsStringAsync()).ShouldContain("\"code\":\"unauthorized\"");
        }

        using (var info = await _context.HttpClient.GetAsync(_context.Server.HttpBaseUrl + "nupdate-statistics.php/v2"))
            (await info.Content.ReadAsStringAsync()).ShouldContain("\"version\":2");

        // --- Administration: publish a package ---
        var definition = new PackageDefinition(new UpdateVersion("1.1.0"));
        var windows = definition.GetOrAddPlatform("win");
        windows.Files.Add(new PackageFileEntry(PackageRoot.Program, "app.exe", _context.WriteFile("app.exe", "version 1.1")));
        windows.Files.Add(new PackageFileEntry(PackageRoot.Program, "lib/helper.dll", _context.WriteFile("helper.dll", "helper 1.1")));
        windows.Operations.Add(new DeleteFilesOperation { Directory = "%program%", Files = ["obsolete.dll"] });
        definition.GetOrAddPlatform("linux").Files.Add(new PackageFileEntry(PackageRoot.Program, "app", _context.WriteFile("app", "linux 1.1")));
        var request = new PublishRequest(project, secrets, definition) { Description = "First update" };
        request.Changelog[new CultureInfo("en")] = "Everything is better.";
        var package = await _context.Publisher.CreatePackageAsync(request);
        package.Released.ShouldBeTrue();

        var remote = (await _context.Feeds.LoadRemoteAsync(project, secrets))!;
        remote.ProjectId.ShouldBe(project.Id);
        remote.Packages.Single().Version.ShouldBe(new UpdateVersion("1.1.0"));
        remote.Packages.Single().Files.Select(f => f.Path).ShouldBe(["packages/1.1.0/win.zip", "packages/1.1.0/linux.zip"]);
        remote.Packages.Single().Files[0].Touches.ShouldBe([OperationArea.Files]);
        remote.Packages.Single().Files[1].Touches.ShouldBeEmpty();
        (await _context.StatusAsync("packages/1.1.0/win.zip")).ShouldBe(System.Net.HttpStatusCode.OK);
        (await _context.StatusAsync("packages/1.1.0/linux.zip")).ShouldBe(System.Net.HttpStatusCode.OK);

        // --- Client: check, download, verify, start the installer ---
        var appDirectory = Path.Combine(_context.Root, "app");
        Directory.CreateDirectory(appDirectory);
        File.WriteAllText(Path.Combine(appDirectory, "app.exe"), "version 1.0");
        File.WriteAllText(Path.Combine(appDirectory, "obsolete.dll"), "old");

        var launcher = Substitute.For<IProcessLauncher>();
        string? optionsPath = null;
        launcher.Start(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(call =>
        {
            optionsPath = call.ArgAt<string>(1).Trim('"');
            return true;
        });
        var installerDirectory = Path.Combine(_context.Root, "installer");
        Directory.CreateDirectory(installerDirectory);
        File.WriteAllText(Path.Combine(installerDirectory, "installer.exe"), "stub");

        using var manager = new UpdateManager(project.FeedUri, TestKeys.PublicKey, services: ClientServices("IntegrationApp", Path.Combine(appDirectory, "app.exe"), launcher))
        {
            InstallerPath = Path.Combine(installerDirectory, "installer.exe"),
            AfterInstall = AfterInstall.KeepRunning,
        };
        (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
        manager.AvailableUpdates.Single().Version.ShouldBe(new UpdateVersion("1.1.0"));
        manager.TotalDownloadSize.ShouldBeGreaterThan(0);
        await manager.DownloadAsync();
        (await manager.VerifyAsync()).ShouldBeTrue();
        manager.StartInstaller().ShouldBeTrue();
        optionsPath.ShouldNotBeNull();

        var options = InstallerOptionsReader.Read(new System.IO.Abstractions.FileSystem(), optionsPath);
        options.Format.ShouldBe(2);
        options.Packages.Single().Path.ShouldEndWith("1.1.0.zip");
        var reporter = Substitute.For<IProgressReporter>();
        var specialFolders = Substitute.For<ISpecialFolders>();
        specialFolders.ApplicationData.Returns(Path.Combine(_context.Root, "appdata"));
        specialFolders.Temp.Returns(Path.Combine(_context.Root, "temp"));
        specialFolders.Desktop.Returns(Path.Combine(_context.Root, "desktop"));
        var installerServices = new InstallerServices { ProcessService = Substitute.For<IProcessService>(), SpecialFolders = specialFolders, EnvironmentInfo = Substitute.For<IEnvironmentInfo>() };
        var result = new InstallEngine(installerServices).Run(options, reporter);
        result.Succeeded.ShouldBeTrue(result.Error?.ToString());
        File.ReadAllText(Path.Combine(appDirectory, "app.exe")).ShouldBe("version 1.1");
        File.ReadAllText(Path.Combine(appDirectory, "lib", "helper.dll")).ShouldBe("helper 1.1");
        File.Exists(Path.Combine(appDirectory, "obsolete.dll")).ShouldBeFalse();
        reporter.Received().Terminate();

        // --- Statistics: the download was counted, the admin can read it ---
        var endpoint = PublishService.Endpoint(project, secrets);
        var statistics = await _context.Statistics.GetStatisticsAsync(endpoint, project.Id);
        statistics.Total.ShouldBe(1);
        statistics.Versions.Single().Version.ShouldBe(new UpdateVersion("1.1.0"));
        statistics.Versions.Single().ByOperatingSystem["Windows 11"].ShouldBe(1);

        // --- Administration: edit the entry, then delete the package ---
        var entry = remote.Packages.Single();
        entry.Necessary = true;
        await _context.Publisher.UpdateEntryAsync(project, secrets, entry);
        (await _context.Feeds.LoadRemoteAsync(project, secrets))!.Packages.Single().Necessary.ShouldBeTrue();

        await _context.Publisher.DeletePackageAsync(project, secrets, new UpdateVersion("1.1.0"));
        (await _context.Feeds.LoadRemoteAsync(project, secrets))!.Packages.ShouldBeEmpty();
        (await _context.Statistics.GetStatisticsAsync(endpoint, project.Id)).Total.ShouldBe(0);
        (await _context.StatusAsync("packages/1.1.0/win.zip")).ShouldBe(System.Net.HttpStatusCode.NotFound);
        (await _context.StatusAsync("packages/1.1.0/linux.zip")).ShouldBe(System.Net.HttpStatusCode.NotFound);

        // --- Administration: reopen the project from disk with its password and delete it including the server files ---
        (await _context.Store.LoadAsync(project.Path)).SecretsState.ShouldBe(SecretsState.PasswordRequired);
        (await _context.Passwords.GetAsync(project.Id)).ShouldBe(ProjectPassword);
        var reopened = await _context.Store.LoadAsync(project.Path, ProjectPassword);
        reopened.SecretsState.ShouldBe(SecretsState.Loaded);
        reopened.Secrets.TransferPassword.ShouldBe(ServerFixture.FtpPassword);
        await _context.Projects.DeleteAsync(reopened.Project, reopened.Secrets, deleteLocalFiles: true, deleteServerFiles: true);
        (await _context.StatusAsync("nupdate-statistics.php")).ShouldBe(System.Net.HttpStatusCode.NotFound);
        (await _context.StatusAsync("nupdate.json")).ShouldBe(System.Net.HttpStatusCode.NotFound);
        Directory.Exists(project.Folder).ShouldBeFalse();
    }

    [DockerFact]
    public async Task PublishOverSftpAndUpdateFromAMirror()
    {
        await _context.Server.ResetAsync();
        var transfer = _context.SftpSettings();
        await using (var learn = await _context.ConnectTrustedAsync(transfer, IntegrationContext.SftpCredentials))
            await learn.ListAsync(string.Empty, false);

        var created = await _context.Projects.CreateAsync(new NewProjectRequest
        {
            Name = "Sftp",
            Folder = _context.ProjectFolder("Sftp"),
            UpdateUrl = _context.Server.HttpBaseUrl,
            Transfer = transfer,
            Secrets = new ProjectSecrets { TransferPassword = ServerFixture.SftpPassword },
            Statistics = new StatisticsSettings { Enabled = false },
        });
        var definition = new PackageDefinition(new UpdateVersion("2.0.0-beta.1"));
        definition.GetOrAddPlatform("any").Files.Add(new PackageFileEntry(PackageRoot.Program, "readme.txt", _context.WriteFile("readme.txt", "sftp")));
        var request = new PublishRequest(created.Project, created.Secrets, definition);
        request.Changelog[new CultureInfo("en")] = "Mirror test.";
        await _context.Publisher.CreatePackageAsync(request);

        // A mirror: the feed names the package by an absolute URL, which clients honour as it is.
        var entry = (await _context.Feeds.LoadRemoteAsync(created.Project, created.Secrets))!.Packages.Single();
        entry.Files.Single().Path = _context.Server.HttpBaseUrl + "packages/2.0.0-beta.1/any.zip";
        await _context.Publisher.UpdateEntryAsync(created.Project, created.Secrets, entry);

        using var manager = new UpdateManager(created.Project.FeedUri, TestKeys.PublicKey, services: ClientServices("SftpApp")) { MinimumStability = Stability.Any };
        (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
        manager.AvailableUpdates.Single().Files.Single().Path.ShouldStartWith(_context.Server.HttpBaseUrl);
        await manager.DownloadAsync();
        (await manager.VerifyAsync()).ShouldBeTrue();
        manager.DeleteDownloads();
    }

    [DockerFact]
    public async Task MigratesAProjectPublishedByNUpdate4()
    {
        await _context.Server.ResetAsync();
        var transfer = _context.FtpSettings();
        var projectId = Guid.Parse("12345678-1234-1234-1234-123456789abc");

        // --- The server as nUpdate 4 left it: updates.json plus 1.0.0.0/<id>.zip; the project file on disk in the v3 layout ---
        await using (var ftp = await _context.ConnectTrustedAsync(transfer, IntegrationContext.FtpCredentials))
            await LegacyServer.PublishAsync(ftp, _context, projectId, "1.0.0.0", LegacyServer.Zip(("Program/app.exe", "version 1.0")));
        var legacyFolder = Path.Combine(_context.Root, "legacy-project");
        Directory.CreateDirectory(legacyFolder);
        var legacyFile = Path.Combine(legacyFolder, "Legacy.nupdproj");
        var json = nUpdate.Tests.Administration.Core.ProjectStoreTests.LegacyProjectJson(true, LegacyAesCredentialDecryptor.BuiltInKeyPassword, LegacyAesCredentialDecryptor.BuiltInIvPassword)
            .Replace("\"UseStatistics\": true", "\"UseStatistics\": false", StringComparison.Ordinal)
            .Replace("\"UpdateUrl\": \"https://updates.example.com/legacy\"", $"\"UpdateUrl\": \"{_context.Server.HttpBaseUrl}\"", StringComparison.Ordinal)
            .Replace("\"FtpHost\": \"ftp.example.com\"", $"\"FtpHost\": \"{_context.Server.FtpHost}\"", StringComparison.Ordinal)
            .Replace("\"FtpPort\": 2121", $"\"FtpPort\": {_context.Server.FtpPort}", StringComparison.Ordinal)
            .Replace("\"FtpProtocol\": 1", "\"FtpProtocol\": 0", StringComparison.Ordinal)
            .Replace("\"FtpUsePassiveMode\": false", "\"FtpUsePassiveMode\": true", StringComparison.Ordinal)
            .Replace("\"FtpDirectory\": \"/updates\"", "\"FtpDirectory\": \"/\"", StringComparison.Ordinal)
            .Replace("\"FtpUsername\": \"ftpuser\"", $"\"FtpUsername\": \"{ServerFixture.FtpUser}\"", StringComparison.Ordinal)
            .Replace("\"Proxy\": { \"Address\": \"http://proxy:8080\"", "\"Proxy\": { \"Address\": \"\"", StringComparison.Ordinal);
        await File.WriteAllTextAsync(legacyFile, json);

        // --- Opening converts the file; the secrets of the old project are recovered ---
        var loaded = await _context.Store.LoadAsync(legacyFile);
        loaded.Migrated.ShouldBeTrue();
        loaded.Project.Id.ShouldBe(projectId);
        loaded.Project.PublicKey.ShouldBe(TestKeys.PublicKey);
        loaded.Secrets.PrivateKey.ShouldBe(TestKeys.PrivateKey);
        loaded.Secrets.TransferPassword = ServerFixture.FtpPassword; // the fixture file carries another password
        loaded.Project.Transfer.TrustedCertificateFingerprint = transfer.TrustedCertificateFingerprint;
        await _context.Projects.SaveMigratedAsync(loaded.Project, loaded.Secrets, ProjectPassword);
        var project = loaded.Project;
        var secrets = loaded.Secrets;
        // The old file sat in a folder that is not the project's own, so the converted project gets one named after it.
        project.Path.ShouldBe(Path.Combine(legacyFolder, "Legacy", "project.nupdproj"));
        File.Exists(legacyFile).ShouldBeTrue();
        (await _context.Store.LoadAsync(project.Path, ProjectPassword)).Secrets.PrivateKey.ShouldBe(TestKeys.PrivateKey);

        // --- Nothing can be published until the server is migrated ---
        (await _context.Migrator.CheckAsync(project, secrets)).NeedsMigration.ShouldBeTrue();
        var definition = new PackageDefinition(new UpdateVersion("1.1.0"));
        definition.GetOrAddPlatform("win").Files.Add(new PackageFileEntry(PackageRoot.Program, "app.exe", _context.WriteFile("app-1.1.exe", "version 1.1")));
        var request = new PublishRequest(project, secrets, definition);
        request.Changelog[new CultureInfo("en")] = "After the migration.";
        var refused = await Should.ThrowAsync<PipelineException>(() => _context.Publisher.CreatePackageAsync(request));
        refused.InnerException.ShouldBeOfType<MigrationRequiredException>();
        project.Packages.Count.ShouldBe(2);

        // --- The migration repacks, re-signs and uploads the package and writes nupdate.json; the old files stay ---
        using (var plan = await _context.Migrator.PrepareAsync(project, secrets))
        {
            plan.Packages.Single().LiteralVersion.ShouldBe("1.0.0.0");
            plan.Packages.Single().Source.ShouldBe(_context.Server.HttpBaseUrl + "1.0.0.0/" + projectId + ".zip");
            (await _context.Migrator.RunAsync(project, secrets, plan)).ShouldBe([new UpdateVersion("1.0.0")]);
        }

        var status = await _context.Migrator.CheckAsync(project, secrets);
        status.NeedsMigration.ShouldBeFalse();
        status.LegacyFeedPresent.ShouldBeTrue();
        (await _context.StatusAsync("1.0.0.0/" + projectId + ".zip")).ShouldBe(System.Net.HttpStatusCode.OK);
        var feed = (await _context.Feeds.LoadRemoteAsync(project, secrets))!;
        var entry = feed.Packages.Single();
        entry.Version.ShouldBe(new UpdateVersion("1.0.0"));
        entry.Necessary.ShouldBeTrue();
        entry.Changelog["de-DE"].ShouldBe("Alte Version");
        var file = entry.Files.Single();
        file.Platform.ShouldBe("win");
        file.Touches.ShouldBe([OperationArea.Files, OperationArea.Processes]);
        _context.Signer.Verify(project.PackageFilePath(entry.Version, "win"), project.PublicKey, file.Signature.Value).ShouldBeTrue();
        var content = await new PackageContentReader(_context.FileSystem).ReadAsync(project.PackageFilePath(entry.Version, "win"));
        content.Manifest!.Operations.Select(o => o.Type).ShouldBe(["deleteFiles", "terminateProcess"]);
        project.FindPackage(entry.Version)!.Released.ShouldBeTrue();

        // --- The feed checks out the way a client sees it ---
        var check = await new FeedChecker(_context.FileSystem, _context.HttpClientFactory, _context.Feeds, _context.Signer, _context.Statistics).CheckAsync(project, secrets);
        check.Succeeded.ShouldBeTrue(string.Join("; ", check.Packages.Select(p => p.Problem)) + check.FeedProblem);

        // --- A 5.0 client updates from the migrated feed ---
        using (var manager = new UpdateManager(project.FeedUri, project.PublicKey, services: ClientServices("LegacyApp"), currentVersion: new UpdateVersion("0.9.0")))
        {
            (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
            manager.AvailableUpdates.Single().Version.ShouldBe(new UpdateVersion("1.0.0"));
            await manager.DownloadAsync();
            (await manager.VerifyAsync()).ShouldBeTrue();
            manager.DeleteDownloads();
        }

        // --- Publishing works now, and the legacy files can be removed once every client has moved ---
        await _context.Publisher.CreatePackageAsync(request);
        (await _context.Feeds.LoadRemoteAsync(project, secrets))!.Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0", "1.1.0"]);
        var legacy = await _context.Migrator.FindLegacyFilesAsync(project, secrets);
        legacy.ServerFiles.ShouldBe(["updates.json"]);
        legacy.ServerDirectories.ShouldBe(["1.0.0.0"]);
        await _context.Migrator.DeleteLegacyFilesAsync(project, secrets, legacy);
        (await _context.StatusAsync("updates.json")).ShouldBe(System.Net.HttpStatusCode.NotFound);
        (await _context.StatusAsync("1.0.0.0/" + projectId + ".zip")).ShouldBe(System.Net.HttpStatusCode.NotFound);
        (await _context.Migrator.CheckAsync(project, secrets)).LegacyFeedPresent.ShouldBeFalse();
    }
}
