using System.Globalization;
using System.IO.Abstractions;
using System.IO.Compression;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.TransferInterface;
using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Tests.Administration.Support;
using nUpdate.Tests.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public class PublishServiceTests
{
    private readonly AdminTestContext _context = new();
    private readonly PublishService _service;
    private readonly List<UpdateFeed> _uploadedFeeds = [];

    public PublishServiceTests()
    {
        _service = new PublishService(_context.FileSystem, _context.Builder, _context.Signer, _context.Feeds,
            _context.TransferFactory, _context.Statistics, _context.Store, _context.Logger, _context.ContentReader,
            () => AdminTestContext.Now);
        _context.Transfer.UploadFileAsync(Arg.Any<string>(), "nupdate.json", null, Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                _uploadedFeeds.Add(FeedLoader.Parse(_context.FileSystem.File.ReadAllText(call.ArgAt<string>(0))));
                return Task.CompletedTask;
            });
        _context.ServeLegacyFeed(null);
    }

    private PublishRequest Request(UpdateProject project, ProjectSecrets secrets, string version = "1.1.0",
        bool publish = true)
    {
        var definition = new PackageDefinition(new UpdateVersion(version));
        var package = definition.GetOrAddPlatform("any");
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "app.dll",
            _context.AddSourceFile($"app-{version}.dll", "new code")));
        package.Operations.Add(
            new TerminateProcessOperation { ProcessName = "helper", RunBeforeFileReplacement = true });
        var request = new PublishRequest(project, secrets, definition)
        {
            Description = "desc",
            Publish = publish,
            Necessary = true,
            RolloutConditionMode = RolloutConditionMode.All
        };
        request.Changelog[new CultureInfo("en")] = "Fixes";
        request.Changelog[new CultureInfo("de-DE")] = "Korrekturen";
        request.RolloutConditions.Add(new RolloutCondition("R", "east"));
        request.RolloutConditions.Add(new RolloutCondition("", "ignored"));
        request.UnsupportedVersions.Add(new UpdateVersion("0.9.0"));
        return request;
    }

    private static PackageInfo Entry(string version) => new()
    {
        Version = new UpdateVersion(version),
        Files =
        [
            new PackageFile
            {
                Path = $"packages/{version}/any.zip", Size = 1, Sha512 = "h",
                Signature = new PackageSignature { Value = "s" }
            }
        ],
    };

    private static UpdateFeed Feed(params PackageInfo[] packages) => new()
    { ProjectId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), Packages = packages.ToList() };

    [Fact]
    public async Task CreatePackage_BuildsSignsUploadsAndSaves()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        _context.ServeFeed(Feed(Entry("1.0.0")));
        var progress = new List<PipelineProgress>();

        var package =
            await _service.CreatePackageAsync(Request(project, secrets), new SyncProgress<PipelineProgress>(progress));

        package.Released.ShouldBeTrue();
        package.Description.ShouldBe("desc");
        package.CreatedAt.ShouldBe(AdminTestContext.Now);
        project.Packages.ShouldContain(package);
        var version = new UpdateVersion("1.1.0");
        var packagePath = project.PackageFilePath(version, "any");
        _context.FileSystem.File.Exists(packagePath).ShouldBeTrue();
        _context.FileSystem.File
            .Exists(_context.FileSystem.Path.Combine(project.PlatformDirectory(version, "any"), "manifest.json"))
            .ShouldBeTrue();

        var entry = (await _context.Feeds.LoadEntryAsync(project, version))!;
        var file = entry.Files.Single();
        file.Platform.ShouldBe("any");
        file.Path.ShouldBe("packages/1.1.0/any.zip");
        file.Size.ShouldBe(_context.FileSystem.FileInfo.New(packagePath).Length);
        file.Sha512.ShouldBe(_context.Signer.Hash(packagePath));
        entry.PublishedAt.ShouldBe(AdminTestContext.Now);
        entry.Statistics!.Url.ShouldBe("nupdate-statistics.php");
        entry.Statistics.Enabled.ShouldBeTrue();
        entry.Necessary.ShouldBeTrue();
        entry.UnsupportedVersions.ShouldBe([new UpdateVersion("0.9.0")]);
        entry.Rollout.Mode.ShouldBe(RolloutConditionMode.All);
        entry.Rollout.Conditions.Single().Key.ShouldBe("R");
        file.Touches.ShouldBe([OperationArea.Processes]);
        entry.GetChangelog(new CultureInfo("de-DE")).ShouldBe("Korrekturen");
        file.Signature.Algorithm.ShouldBe("rsa-pss-sha512");
        _context.Signer.Verify(packagePath, TestKeys.PublicKey, file.Signature.Value).ShouldBeTrue();

        await _context.Transfer.Received().CreateDirectoryAsync("packages/1.1.0", Arg.Any<CancellationToken>());
        await _context.Transfer.Received()
            .UploadFileAsync(packagePath, "packages/1.1.0/any.zip", null, Arg.Any<CancellationToken>());
        _uploadedFeeds.Single().ProjectId.ShouldBe(project.Id);
        _uploadedFeeds.Single().Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0", "1.1.0"]);
        await _context.Statistics.Received().RegisterVersionAsync(
            Arg.Is<StatisticsEndpoint>(e =>
                e.AdminSecret == "admin-secret" &&
                e.Uri.ToString() == "https://updates.example.com/demo/nupdate-statistics.php"), project.Id, version,
            Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DisposeAsync();

        (await _context.Store.LoadAsync(project.Path)).Project.Packages.Single().Version.ShouldBe(version);
        project.Log.Select(l => l.Kind).ShouldBe([LogEntryKind.Create, LogEntryKind.Upload]);
        progress.First().StepName.ShouldBe("Building the package");
        progress.Last().StepName.ShouldBe("Done");
    }

    [Fact]
    public async Task CreatePackage_LocalOnlyDoesNotTouchServer()
    {
        var project = _context.NewProject();
        var package =
            await _service.CreatePackageAsync(Request(project, AdminTestContext.NewSecrets(), publish: false));
        package.Released.ShouldBeFalse();
        _context.Http.Requests.ShouldBeEmpty();
        await _context.Transfer.DidNotReceive().UploadFileAsync(Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<IProgress<TransferProgress>>(), Arg.Any<CancellationToken>());
        await _context.Statistics.DidNotReceiveWithAnyArgs().RegisterVersionAsync(default!, default, default!, default);
        project.Log.Single().Kind.ShouldBe(LogEntryKind.Create);
        (await _context.Feeds.LoadEntryAsync(project, new UpdateVersion("1.1.0")))!.Statistics.ShouldBeNull();
    }

    [Fact]
    public async Task CreatePackage_RefusesWhileOnlyTheLegacyFeedExists()
    {
        var project = _context.NewProject();
        _context.ServeFeed(null);
        _context.ServeLegacyFeed("[]");
        var ex = await Should.ThrowAsync<PipelineException>(() =>
            _service.CreatePackageAsync(Request(project, AdminTestContext.NewSecrets())));
        ex.InnerException.ShouldBeOfType<MigrationRequiredException>().Message.ShouldContain("Migrate");
        project.Packages.ShouldBeEmpty();
        _context.FileSystem.Directory.Exists(project.PackageDirectory(new UpdateVersion("1.1.0"))).ShouldBeFalse();
        new MigrationRequiredException("m").Message.ShouldBe("m");
        new MigrationRequiredException("m", new InvalidOperationException()).InnerException.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreatePackage_StartsANewFeedWhenTheServerHasNone()
    {
        var project = _context.NewProject();
        _context.ServeFeed(null);
        await _service.CreatePackageAsync(Request(project, AdminTestContext.NewSecrets()));
        _uploadedFeeds.Single().Packages.Single().Version.ShouldBe(new UpdateVersion("1.1.0"));
    }

    [Fact]
    public async Task CreatePackage_RollsBackWhenUploadFails()
    {
        var project = _context.NewProject(statistics: true);
        _context.ServeFeed(null);
        _context.Transfer.UploadFileAsync(Arg.Any<string>(),
                Arg.Is<string>(p => p.EndsWith(".zip", StringComparison.Ordinal)), null, Arg.Any<CancellationToken>())
            .Returns(_ => throw new TransferException("disk full"));

        var ex = await Should.ThrowAsync<PipelineException>(() =>
            _service.CreatePackageAsync(Request(project, AdminTestContext.NewSecrets(statistics: true))));

        ex.FailedStep.ShouldBe("Uploading the package");
        ex.CompensationErrors.ShouldBeEmpty();
        project.Packages.ShouldBeEmpty();
        _context.FileSystem.Directory.Exists(project.PackageDirectory(new UpdateVersion("1.1.0"))).ShouldBeFalse();
        _context.FileSystem.File.Exists(project.Path).ShouldBeFalse();
    }

    [Fact]
    public async Task CreatePackage_RollsBackStatisticsAndFeedWhenRegistrationFails()
    {
        var project = _context.NewProject(statistics: true);
        _context.ServeFeed(Feed(Entry("1.0.0")));
        _context.Statistics.RegisterVersionAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<Guid>(),
                Arg.Any<UpdateVersion>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new StatisticsException("no db"));

        var ex = await Should.ThrowAsync<PipelineException>(() =>
            _service.CreatePackageAsync(Request(project, AdminTestContext.NewSecrets(statistics: true))));

        ex.FailedStep.ShouldBe("Registering the version in the statistics");
        _uploadedFeeds.Count.ShouldBe(2);
        _uploadedFeeds[1].Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0"]);
        await _context.Transfer.Received().DeleteDirectoryAsync("packages/1.1.0", Arg.Any<CancellationToken>());
        project.Packages.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreatePackage_RollsBackEverythingWhenSavingFails()
    {
        var project = _context.NewProject(statistics: true);
        _context.ServeFeed(Feed(Entry("1.0.0")));
        _context.FileSystem.AddFile(project.Path,
            new System.IO.Abstractions.TestingHelpers.MockFileData("locked") { Attributes = FileAttributes.ReadOnly });

        var ex = await Should.ThrowAsync<PipelineException>(() =>
            _service.CreatePackageAsync(Request(project, AdminTestContext.NewSecrets(statistics: true))));

        ex.FailedStep.ShouldBe("Saving the project");
        ex.CompensationErrors.ShouldBeEmpty();
        project.Packages.ShouldBeEmpty();
        await _context.Statistics.Received().DeleteVersionAsync(Arg.Any<StatisticsEndpoint>(), project.Id,
            new UpdateVersion("1.1.0"), Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteDirectoryAsync("packages/1.1.0", Arg.Any<CancellationToken>());
        _uploadedFeeds.Count.ShouldBe(2);
        _uploadedFeeds[1].Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0"]);
        _context.FileSystem.Directory.Exists(project.PackageDirectory(new UpdateVersion("1.1.0"))).ShouldBeFalse();
    }

    [Fact]
    public async Task CreatePackage_RollbackRemovesAFeedThatDidNotExistBefore()
    {
        var project = _context.NewProject(statistics: true);
        _context.ServeFeed(null);
        _context.Statistics.RegisterVersionAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<Guid>(),
                Arg.Any<UpdateVersion>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new HttpRequestException("statistics down"));

        await Should.ThrowAsync<PipelineException>(() =>
            _service.CreatePackageAsync(Request(project, AdminTestContext.NewSecrets(statistics: true))));

        _uploadedFeeds.Count.ShouldBe(1);
        await _context.Transfer.Received().DeleteFileAsync("nupdate.json", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreatePackage_RollbackRestoresAnEmptyFeedThatExistedBefore()
    {
        var project = _context.NewProject(statistics: true);
        _context.ServeFeed(Feed());
        _context.Statistics.RegisterVersionAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<Guid>(),
                Arg.Any<UpdateVersion>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new HttpRequestException("statistics down"));

        await Should.ThrowAsync<PipelineException>(() =>
            _service.CreatePackageAsync(Request(project, AdminTestContext.NewSecrets(statistics: true))));

        _uploadedFeeds.Count.ShouldBe(2);
        _uploadedFeeds[1].Packages.ShouldBeEmpty();
        await _context.Transfer.DidNotReceive().DeleteFileAsync("nupdate.json", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreatePackage_ConnectFailureDisposesProvider()
    {
        var project = _context.NewProject();
        _context.ServeFeed(null);
        _context.Transfer.ConnectAsync(Arg.Any<CancellationToken>())
            .Returns(_ => throw new TransferException("refused"));
#pragma warning disable CA2012 // configuring a substitute, not consuming the ValueTask
        _context.Transfer.DisposeAsync().Returns(_ => new ValueTask(Task.Delay(1)));
#pragma warning restore CA2012
        var ex = await Should.ThrowAsync<PipelineException>(() =>
            _service.CreatePackageAsync(Request(project, AdminTestContext.NewSecrets())));
        ex.InnerException.ShouldBeOfType<TransferException>();
        await _context.Transfer.Received().DisposeAsync();
    }

    [Fact]
    public async Task CreatePackage_ValidatesRequests()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        await Should.ThrowAsync<ArgumentNullException>(() => _service.CreatePackageAsync(null!));
        (await Should.ThrowAsync<ArgumentException>(() =>
                _service.CreatePackageAsync(Request(project, secrets, version: "0.0.0-beta.1")))).Message
            .ShouldContain("reserved");

        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.1.0") });
        await Should.ThrowAsync<ArgumentException>(() => _service.CreatePackageAsync(Request(project, secrets)));
        project.Packages.Clear();

        var noChangelog = Request(project, secrets);
        noChangelog.Changelog.Clear();
        await Should.ThrowAsync<ArgumentException>(() => _service.CreatePackageAsync(noChangelog));
        noChangelog.Changelog[new CultureInfo("en")] = " ";
        await Should.ThrowAsync<ArgumentException>(() => _service.CreatePackageAsync(noChangelog));

        var empty = new PublishRequest(project, secrets, new PackageDefinition(new UpdateVersion("1.1.0")));
        empty.Changelog[new CultureInfo("en")] = "x";
        (await Should.ThrowAsync<ArgumentException>(() => _service.CreatePackageAsync(empty))).Message.ShouldContain(
            "at least one platform");
        empty.Package.GetOrAddPlatform("linux");
        (await Should.ThrowAsync<ArgumentException>(() => _service.CreatePackageAsync(empty))).Message.ShouldContain(
            "The linux package needs at least one file or operation.");
        empty.Package.Platforms[0].Operations.Add(new StopServiceOperation { ServiceName = "svc" });
        (await Should.ThrowAsync<ArgumentException>(() => _service.CreatePackageAsync(empty))).Message.ShouldContain(
            "The linux package contains registry or service operations");

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _service.CreatePackageAsync(Request(project, AdminTestContext.NewSecrets())));

        var invalidUrl = _context.NewProject();
        invalidUrl.UpdateUrl = "not a url";
        (await Should.ThrowAsync<ArgumentException>(() => _service.CreatePackageAsync(Request(invalidUrl, secrets))))
            .Message.ShouldContain("not a url");

        var withoutKey = Request(project, new ProjectSecrets { TransferPassword = "x", StatisticsAdminSecret = "s" });
        (await Should.ThrowAsync<PipelineException>(() => _service.CreatePackageAsync(withoutKey))).InnerException
            .ShouldBeOfType<InvalidOperationException>();

        Should.Throw<ArgumentNullException>(() =>
            new PublishRequest(null!, secrets, new PackageDefinition(new UpdateVersion("1.0.0"))));
        Should.Throw<ArgumentNullException>(() =>
            new PublishRequest(project, null!, new PackageDefinition(new UpdateVersion("1.0.0"))));
        Should.Throw<ArgumentNullException>(() => new PublishRequest(project, secrets, null!));
        Should.Throw<ArgumentNullException>(() => PublishService.BuildEntry(null!, project));
        Should.Throw<ArgumentNullException>(() => PublishService.BuildEntry(Request(project, secrets), null!));
        Should.Throw<ArgumentNullException>(() => PublishService.Replace(null!, new PackageInfo()));
        Should.Throw<ArgumentNullException>(() => PublishService.Replace([], null!));
        Should.Throw<ArgumentNullException>(() => PublishService.StatisticsUrl(null!));
        Should.Throw<ArgumentNullException>(() => PublishService.StatisticsUri(null!));
        Should.Throw<ArgumentNullException>(() => PublishService.Endpoint(project, null!));
    }

    [Fact]
    public async Task CreatePackage_BuildsSignsAndUploadsOneFilePerPlatform()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeFeed(null);
        var request = Request(project, secrets);
        var windows = request.Package.GetOrAddPlatform("win-x64");
        windows.Files.Add(new PackageFileEntry(PackageRoot.Program, "app.exe",
            _context.AddSourceFile("app.exe", "windows build")));
        windows.Operations.Add(new SetRegistryValuesOperation
        { Key = "HKEY_CURRENT_USER\\Software\\Demo", Values = [RegistryValue.DWord("Installed", 1)] });
        var progress = new List<PipelineProgress>();

        await _service.CreatePackageAsync(request, new SyncProgress<PipelineProgress>(progress));

        var version = new UpdateVersion("1.1.0");
        var entry = (await _context.Feeds.LoadEntryAsync(project, version))!;
        entry.Files.Select(f => (f.Platform, f.Path))
            .ShouldBe([("any", "packages/1.1.0/any.zip"), ("win-x64", "packages/1.1.0/win-x64.zip")]);
        entry.Files[1].Touches.ShouldBe([OperationArea.Registry]);
        foreach (var file in entry.Files)
        {
            var path = project.PackageFilePath(version, file.Platform);
            _context.Signer.Verify(path, TestKeys.PublicKey, file.Signature.Value).ShouldBeTrue();
            await _context.Transfer.Received().UploadFileAsync(path, file.Path, null, Arg.Any<CancellationToken>());
        }

        _uploadedFeeds.Single().Packages.Single().Files.Count.ShouldBe(2);
        progress.Select(p => p.StepName).ShouldContain("Building the package");
        progress.Select(p => p.StepName).ShouldContain("Signing the package");
        progress.Select(p => p.StepName).ShouldContain("Uploading the package");
    }

    [Fact]
    public async Task CreatePackage_RemovesTheFilesOfEarlierPlatformsWhenALaterOneFails()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeFeed(null);
        var request = Request(project, secrets);
        request.Package.GetOrAddPlatform("win-x64").Files
            .Add(new PackageFileEntry(PackageRoot.Program, "app.exe", "/missing/app.exe"));

        var ex = await Should.ThrowAsync<PipelineException>(() => _service.CreatePackageAsync(request));

        ex.FailedStep.ShouldBe("Building the package");
        _context.FileSystem.Directory.Exists(project.PackageDirectory(new UpdateVersion("1.1.0")))
            .ShouldBeFalse(); // not even the any package
        project.Packages.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreatePackage_RemovesTheUploadsOfEarlierPlatformsWhenALaterOneFails()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeFeed(null);
        var request = Request(project, secrets);
        request.Package.GetOrAddPlatform("linux").Files.Add(new PackageFileEntry(PackageRoot.Program, "app",
            _context.AddSourceFile("app-linux", "linux")));
        _context.Transfer.UploadFileAsync(Arg.Any<string>(), "packages/1.1.0/linux.zip",
                Arg.Any<IProgress<TransferProgress>?>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new TransferException("quota"));

        var ex = await Should.ThrowAsync<PipelineException>(() => _service.CreatePackageAsync(request));

        ex.FailedStep.ShouldBe("Uploading the package");
        await _context.Transfer.Received().UploadFileAsync(Arg.Any<string>(), "packages/1.1.0/any.zip", null,
            Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteDirectoryAsync("packages/1.1.0", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishExisting_UploadsALocalPackage()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        await _service.CreatePackageAsync(Request(project, secrets, publish: false));
        _context.ServeFeed(Feed(Entry("1.0.0")));

        await _service.PublishExistingAsync(project, secrets, new UpdateVersion("1.1.0"));

        project.Packages.Single().Released.ShouldBeTrue();
        _uploadedFeeds.Single().Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0", "1.1.0"]);
        (await _context.Feeds.LoadEntryAsync(project, new UpdateVersion("1.1.0")))!.PublishedAt.ShouldBe(
            AdminTestContext.Now);
        project.Log.Select(l => l.Kind).ShouldBe([LogEntryKind.Create, LogEntryKind.Upload]);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _service.PublishExistingAsync(project, secrets, new UpdateVersion("1.1.0")));
        await Should.ThrowAsync<InvalidOperationException>(() =>
            _service.PublishExistingAsync(project, secrets, new UpdateVersion("9.9.0")));
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _service.PublishExistingAsync(null!, secrets, new UpdateVersion("1.1.0")));
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _service.PublishExistingAsync(project, null!, new UpdateVersion("1.1.0")));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.PublishExistingAsync(project, secrets, null!));
    }

    [Fact]
    public async Task PublishExisting_RequiresPackageFileEntryAndSecret()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        var version = new UpdateVersion("2.0.0");
        project.Packages.Add(new UpdatePackage { Version = version });
        (await Should.ThrowAsync<InvalidOperationException>(() =>
            _service.PublishExistingAsync(project, secrets, version))).Message.ShouldContain("feed entry");

        var entry = Entry("2.0.0");
        entry.Files.Add(new PackageFile { Platform = "linux-x64", Path = "packages/2.0.0/linux-x64.zip" });
        entry.Files[0].Platform = "win";
        await _context.Feeds.SaveEntryAsync(project, entry);
        _context.FileSystem.AddFile(project.PackageFilePath(version, "win"),
            new System.IO.Abstractions.TestingHelpers.MockFileData("zip"));
        (await Should.ThrowAsync<FileNotFoundException>(() => _service.PublishExistingAsync(project, secrets, version)))
            .Message.ShouldContain("The linux-x64 package file of 2.0.0 is missing.");

        project.Statistics.Enabled = true;
        (await Should.ThrowAsync<InvalidOperationException>(() =>
            _service.PublishExistingAsync(project, secrets, version))).Message.ShouldContain("admin secret");
    }

    [Fact]
    public async Task DeletePackage_RemovesReleasedPackagesEverywhere()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        _context.ServeFeed(null);
        await _service.CreatePackageAsync(Request(project, secrets));
        _context.ServeFeed(Feed(Entry("1.0.0"), Entry("1.1.0")));
        _uploadedFeeds.Clear();

        await _service.DeletePackageAsync(project, secrets, new UpdateVersion("1.1.0"));

        _uploadedFeeds.Single().Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0"]);
        await _context.Transfer.Received().DeleteDirectoryAsync("packages/1.1.0", Arg.Any<CancellationToken>());
        await _context.Statistics.Received().DeleteVersionAsync(Arg.Any<StatisticsEndpoint>(), project.Id,
            new UpdateVersion("1.1.0"), Arg.Any<CancellationToken>());
        // The statistics go before the zip: a failing statistics API leaves the package downloadable instead of half-deleted.
        Received.InOrder(() =>
        {
            _context.Statistics.DeleteVersionAsync(Arg.Any<StatisticsEndpoint>(), project.Id,
                new UpdateVersion("1.1.0"), Arg.Any<CancellationToken>());
            _context.Transfer.DeleteDirectoryAsync("packages/1.1.0", Arg.Any<CancellationToken>());
        });
        _context.FileSystem.Directory.Exists(project.PackageDirectory(new UpdateVersion("1.1.0"))).ShouldBeFalse();
        project.Packages.ShouldBeEmpty();
        project.Log.Last().Kind.ShouldBe(LogEntryKind.Delete);

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _service.DeletePackageAsync(project, secrets, new UpdateVersion("1.1.0")));
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _service.DeletePackageAsync(null!, secrets, new UpdateVersion("1.1.0")));
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _service.DeletePackageAsync(project, null!, new UpdateVersion("1.1.0")));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.DeletePackageAsync(project, secrets, null!));
    }

    [Fact]
    public async Task DeletePackage_RequiresAdminSecretForStatistics()
    {
        var project = _context.NewProject(statistics: true);
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.0.0"), Released = true });
        _context.ServeFeed(Feed(Entry("1.0.0")));
        // The secret is checked before anything is removed, so the server and the project stay untouched.
        await Should.ThrowAsync<InvalidOperationException>(() =>
            _service.DeletePackageAsync(project, AdminTestContext.NewSecrets(), new UpdateVersion("1.0.0")));
        _uploadedFeeds.ShouldBeEmpty();
        project.Packages.Single().Released.ShouldBeTrue();
    }

    [Fact]
    public async Task DeletePackage_RestoresTheFeedWhenALaterStepFails()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.0.0"), Released = true });
        _context.ServeFeed(Feed(Entry("1.0.0"), Entry("0.9.0")));
        _context.Transfer.DeleteDirectoryAsync("packages/1.0.0", Arg.Any<CancellationToken>())
            .Returns(_ => throw new TransferException("refused"));

        var ex = await Should.ThrowAsync<PipelineException>(() =>
            _service.DeletePackageAsync(project, secrets, new UpdateVersion("1.0.0")));

        ex.FailedStep.ShouldBe("Deleting the package from the server");
        _uploadedFeeds.Count.ShouldBe(2);
        _uploadedFeeds[0].Packages.Select(p => p.Version.ToString()).ShouldBe(["0.9.0"]);
        _uploadedFeeds[1].Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0", "0.9.0"]);
        await _context.Statistics.Received().RegisterVersionAsync(Arg.Any<StatisticsEndpoint>(), project.Id,
            new UpdateVersion("1.0.0"), Arg.Any<CancellationToken>());
        project.Packages.Single().Released.ShouldBeTrue();
    }

    [Fact]
    public async Task DeletePackage_AndUpdateEntry_RefuseWhileOnlyTheLegacyFeedExists()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.0.0"), Released = true });
        _context.ServeFeed(null);
        _context.ServeLegacyFeed("[]");

        var deletion = await Should.ThrowAsync<PipelineException>(() =>
            _service.DeletePackageAsync(project, secrets, new UpdateVersion("1.0.0")));
        deletion.InnerException.ShouldBeOfType<MigrationRequiredException>();
        project.Packages.Single().Released.ShouldBeTrue();

        var update =
            await Should.ThrowAsync<PipelineException>(() =>
                _service.UpdateEntryAsync(project, secrets, Entry("1.0.0")));
        update.InnerException.ShouldBeOfType<MigrationRequiredException>();
        _uploadedFeeds.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeletePackage_ReleasedWithoutRemoteFeedWritesAnEmptyOneAndRestoresByDeleting()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.0.0"), Released = true });
        _context.ServeFeed(null);
        _context.Transfer.DeleteDirectoryAsync("packages/1.0.0", Arg.Any<CancellationToken>())
            .Returns(_ => throw new TransferException("refused"));

        await Should.ThrowAsync<PipelineException>(() =>
            _service.DeletePackageAsync(project, secrets, new UpdateVersion("1.0.0")));

        _uploadedFeeds.Single().Packages.ShouldBeEmpty();
        await _context.Transfer.Received().DeleteFileAsync("nupdate.json", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeletePackage_UnreleasedOnlyTouchesLocalFiles()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        await _service.CreatePackageAsync(Request(project, secrets, publish: false));
        await _service.DeletePackageAsync(project, secrets, new UpdateVersion("1.1.0"));
        await _context.Transfer.DidNotReceive().DeleteFileAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        project.Packages.ShouldBeEmpty();
    }

    [Fact]
    public async Task UpdateEntry_UpdatesLocalAndRemote()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeFeed(null);
        await _service.CreatePackageAsync(Request(project, secrets));
        var remote = Entry("1.1.0");
        remote.Necessary = true;
        _context.ServeFeed(Feed(remote, Entry("1.0.0")));
        _uploadedFeeds.Clear();

        var entry = (await _context.Feeds.LoadEntryAsync(project, new UpdateVersion("1.1.0")))!;
        entry.Necessary = false;
        await _service.UpdateEntryAsync(project, secrets, entry);

        _uploadedFeeds.Single().Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0", "1.1.0"]);
        _uploadedFeeds.Single().Packages[1].Necessary.ShouldBeFalse();
        (await _context.Feeds.LoadEntryAsync(project, new UpdateVersion("1.1.0")))!.Necessary.ShouldBeFalse();
        project.Log.Last().Kind.ShouldBe(LogEntryKind.Edit);

        _context.ServeFeed(null);
        _uploadedFeeds.Clear();
        await _service.UpdateEntryAsync(project, secrets, entry);
        _uploadedFeeds.Single().Packages.Single().Version.ShouldBe(new UpdateVersion("1.1.0"));

        project.Packages[0].Released = false;
        _uploadedFeeds.Clear();
        await _service.UpdateEntryAsync(project, secrets, entry);
        _uploadedFeeds.ShouldBeEmpty();

        await Should.ThrowAsync<InvalidOperationException>(() =>
            _service.UpdateEntryAsync(project, secrets, new PackageInfo { Version = new UpdateVersion("7.0.0") }));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.UpdateEntryAsync(null!, secrets, entry));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.UpdateEntryAsync(project, null!, entry));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.UpdateEntryAsync(project, secrets, null!));
    }

    [Fact]
    public async Task CreatePackage_ConnectFailureWithSynchronousDispose()
    {
        var project = _context.NewProject();
        _context.ServeFeed(null);
        _context.Transfer.ConnectAsync(Arg.Any<CancellationToken>())
            .Returns(_ => throw new TransferException("refused"));
        var ex = await Should.ThrowAsync<PipelineException>(() =>
            _service.CreatePackageAsync(Request(project, AdminTestContext.NewSecrets())));
        ex.InnerException.ShouldBeOfType<TransferException>();
        await _context.Transfer.Received().DisposeAsync();
    }

    [Fact]
    public void StatisticsUri_UsesExplicitStatisticsEndpoint()
    {
        var project = _context.NewProject(statistics: true);
        project.Statistics.EndpointUrl = "https://stats.example.com/api.php";
        var entry = PublishService.BuildEntry(Request(project, AdminTestContext.NewSecrets(statistics: true)), project);
        entry.Statistics!.Url.ShouldBe("https://stats.example.com/api.php");
        PublishService.StatisticsUri(project).ToString().ShouldBe("https://stats.example.com/api.php");
        project.Statistics.EndpointUrl = null;
        PublishService.StatisticsUri(project).ToString()
            .ShouldBe("https://updates.example.com/demo/nupdate-statistics.php");
        project.UpdateUrl = "invalid";
        Should.Throw<InvalidOperationException>(() => PublishService.StatisticsUri(project));
    }

    [Fact]
    public async Task CreatePackage_StampsTheCurrentTimeWithoutAClock()
    {
        var c = _context;
        var service = new PublishService(c.FileSystem, c.Builder, c.Signer, c.Feeds, c.TransferFactory, c.Statistics,
            c.Store, c.Logger, c.ContentReader);
        var before = DateTimeOffset.UtcNow;

        var package = await service.CreatePackageAsync(Request(c.NewProject(), AdminTestContext.NewSecrets(),
            publish: false));

        package.CreatedAt.ShouldBeInRange(before, DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Constructor_ValidatesArguments()
    {
        var c = _context;
        Should.Throw<ArgumentNullException>(() =>
            new PublishService(null!, c.Builder, c.Signer, c.Feeds, c.TransferFactory, c.Statistics, c.Store,
                c.Logger));
        Should.Throw<ArgumentNullException>(() => new PublishService(c.FileSystem, null!, c.Signer, c.Feeds,
            c.TransferFactory, c.Statistics, c.Store, c.Logger));
        Should.Throw<ArgumentNullException>(() => new PublishService(c.FileSystem, c.Builder, null!, c.Feeds,
            c.TransferFactory, c.Statistics, c.Store, c.Logger));
        Should.Throw<ArgumentNullException>(() => new PublishService(c.FileSystem, c.Builder, c.Signer, null!,
            c.TransferFactory, c.Statistics, c.Store, c.Logger));
        Should.Throw<ArgumentNullException>(() =>
            new PublishService(c.FileSystem, c.Builder, c.Signer, c.Feeds, null!, c.Statistics, c.Store, c.Logger));
        Should.Throw<ArgumentNullException>(() =>
            new PublishService(c.FileSystem, c.Builder, c.Signer, c.Feeds, c.TransferFactory, null!, c.Store,
                c.Logger));
        Should.Throw<ArgumentNullException>(() => new PublishService(c.FileSystem, c.Builder, c.Signer, c.Feeds,
            c.TransferFactory, c.Statistics, null!, c.Logger));
        Should.Throw<ArgumentNullException>(() => new PublishService(c.FileSystem, c.Builder, c.Signer, c.Feeds,
            c.TransferFactory, c.Statistics, c.Store, null!));
        Should.Throw<ArgumentNullException>(() => new PublishService(c.FileSystem, c.Builder, c.Signer, c.Feeds,
            c.TransferFactory, c.Statistics, c.Store, c.Logger, null!));
        Should.Throw<ArgumentNullException>(() => new PublishService(c.FileSystem, c.Builder, c.Signer, c.Feeds,
            c.TransferFactory, c.Statistics, c.Store, c.Logger, c.ContentReader, null!));
        Should.Throw<ArgumentNullException>(() => new PublishService(null!, c.Builder, c.Signer, c.Feeds,
            c.TransferFactory, c.Statistics, c.Store, c.Logger, c.ContentReader));
        new PublishService(c.FileSystem, c.Builder, c.Signer, c.Feeds, c.TransferFactory, c.Statistics, c.Store,
            c.Logger).ShouldNotBeNull();
        new PublishService(c.FileSystem, c.Builder, c.Signer, c.Feeds, c.TransferFactory, c.Statistics, c.Store,
            c.Logger, c.ContentReader).ShouldNotBeNull();
    }

    private static readonly UpdateVersion Rebuilt = new("1.1.0");

    /// <summary>Creates the package 1.1.0 of <see cref="Request" /> plus a file for each further platform, and serves the feed with it when published.</summary>
    private async Task<(UpdateProject Project, ProjectSecrets Secrets, PackageInfo Entry)> CreateAsync(bool publish,
        params string[] platforms)
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeFeed(Feed(Entry("1.0.0")));
        var request = Request(project, secrets, publish: publish);
        foreach (var platform in platforms)
        {
            request.Package.GetOrAddPlatform(platform).Files.Add(new PackageFileEntry(PackageRoot.Program,
                $"app-{platform}", _context.AddSourceFile($"app-{platform}", $"{platform} build")));
        }

        await _service.CreatePackageAsync(request);
        var entry = (await _context.Feeds.LoadEntryAsync(project, Rebuilt))!;
        _context.ServeFeed(Feed(Entry("1.0.0"), entry));
        _uploadedFeeds.Clear();
        _context.Transfer.ClearReceivedCalls();
        return (project, secrets, entry);
    }

    /// <summary>A request to rebuild 1.1.0 with other metadata than <see cref="Request" />.</summary>
    private static PublishRequest RebuildRequest(UpdateProject project, ProjectSecrets secrets,
        PackageDefinition definition)
    {
        var request = new PublishRequest(project, secrets, definition)
        {
            Description = "rebuilt",
        };
        request.Changelog[new CultureInfo("en")] = "Fixes again";
        return request;
    }

    /// <summary>Opens 1.1.0 and changes the file of every platform, so each builds to another hash.</summary>
    private async Task<PackageDefinition> OpenAndChangeAsync(UpdateProject project)
    {
        var definition = await _service.OpenPackageAsync(project, Rebuilt, "/edit");
        foreach (var file in definition.Platforms.SelectMany(p => p.Files))
            _context.FileSystem.File.AppendAllText(file.SourcePath, " fixed");
        return definition;
    }

    private string Hash(UpdateProject project, string platform) =>
        _context.Signer.Hash(project.PackageFilePath(Rebuilt, platform));

    private IEnumerable<string> PackageDirectoryEntries(UpdateProject project) => _context.FileSystem.Directory
        .EnumerateFileSystemEntries(project.PackageDirectory(Rebuilt))
        .Select(e => _context.FileSystem.Path.GetFileName(e)).Order(StringComparer.Ordinal);

    [Fact]
    public async Task OpenPackage_ExtractsTheFilesAndOperationsOfEveryPlatform()
    {
        var (project, _, _) = await CreateAsync(publish: false, "win-x64");
        var work = _context.FileSystem.Path.Combine(_context.FileSystem.Path.GetTempPath(), "edit");

        var definition = await _service.OpenPackageAsync(project, Rebuilt, work);

        definition.Version.ShouldBe(Rebuilt);
        definition.Platforms.Select(p => p.Platform).ShouldBe(["any", "win-x64"]);
        var any = definition.Platforms[0];
        var file = any.Files.Single();
        (file.Root, file.RelativePath).ShouldBe((PackageRoot.Program, "app.dll"));
        file.SourcePath.ShouldBe(_context.FileSystem.Path.Combine(work, "any", "Program", "app.dll"));
        _context.FileSystem.File.ReadAllText(file.SourcePath).ShouldBe("new code");
        file.UnixMode.ShouldNotBeNull(); // as stored, so a rebuild on Windows keeps it
        any.Operations.Single().ShouldBeOfType<TerminateProcessOperation>().ProcessName.ShouldBe("helper");
        var windows = definition.Platforms[1];
        windows.Files.Single().SourcePath.ShouldBe(
            _context.FileSystem.Path.Combine(work, "win-x64", "Program", "app-win-x64"));
        windows.Operations.ShouldBeEmpty();
    }

    [Fact]
    public async Task OpenPackage_TakesNoOperationsFromAPackageWithoutManifest()
    {
        var (project, _, _) = await CreateAsync(publish: false);
        _context.FileSystem.File.WriteAllBytes(project.PackageFilePath(Rebuilt, "any"),
            TestPackages.WithoutManifest([1, 2, 3]));

        var definition = await _service.OpenPackageAsync(project, Rebuilt, "/edit");

        definition.Platforms.Single().Files.Single().RelativePath.ShouldBe("payload.bin");
        definition.Platforms.Single().Operations.ShouldBeEmpty();
    }

    [Fact]
    public async Task OpenPackage_LeavesTheModeToTheBuilderWhenTheZipStoresNone()
    {
        var (project, _, _) = await CreateAsync(publish: false);
        using (var stream = _context.FileSystem.File.Create(project.PackageFilePath(Rebuilt, "any")))
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
        {
            var entry = archive.CreateEntry("Program/app.dll");
            entry.ExternalAttributes = 0; // as written on Windows
            await using var output = entry.Open();
            output.WriteByte(1);
        }

        var definition = await _service.OpenPackageAsync(project, Rebuilt, "/edit");

        definition.Platforms.Single().Files.Single().UnixMode.ShouldBeNull();
    }

    [Fact]
    public async Task OpenPackage_RequiresThePackageItsEntryAndEveryPackageFile()
    {
        var (project, _, _) = await CreateAsync(publish: false, "win-x64");
        var missing = project.PackageFilePath(Rebuilt, "win-x64");
        _context.FileSystem.File.Delete(missing);

        var ex = await Should.ThrowAsync<FileNotFoundException>(() =>
            _service.OpenPackageAsync(project, Rebuilt, "/edit"));

        ex.Message.ShouldStartWith("The win-x64 package file of 1.1.0 is missing");
        ex.FileName.ShouldBe(missing);
        _context.FileSystem.Directory.Exists("/edit").ShouldBeFalse(); // not even the any package is extracted
        (await Should.ThrowAsync<InvalidOperationException>(() =>
            _service.OpenPackageAsync(project, new UpdateVersion("9.0.0"), "/edit"))).Message.ShouldContain("9.0.0");
        _context.FileSystem.File.Delete(
            _context.FileSystem.Path.Combine(project.PackageDirectory(Rebuilt), IFeedStore.EntryFileName));
        (await Should.ThrowAsync<InvalidOperationException>(() => _service.OpenPackageAsync(project, Rebuilt, "/edit")))
            .Message.ShouldContain("feed entry");
        await Should.ThrowAsync<ArgumentNullException>(() => _service.OpenPackageAsync(null!, Rebuilt, "/edit"));
        await Should.ThrowAsync<ArgumentNullException>(() => _service.OpenPackageAsync(project, null!, "/edit"));
        await Should.ThrowAsync<ArgumentException>(() => _service.OpenPackageAsync(project, Rebuilt, " "));
    }

    [Fact]
    public async Task RebuildPackage_UploadsUnderANewNameBeforeSwitchingTheFeed()
    {
        var (project, secrets, previous) = await CreateAsync(publish: true);
        previous.PublishedAt = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);
        await _context.Feeds.SaveEntryAsync(project, previous);
        var definition = await OpenAndChangeAsync(project);
        definition.Platforms[0].Operations.Clear();
        var progress = new List<PipelineProgress>();

        var package = await _service.RebuildPackageAsync(RebuildRequest(project, secrets, definition), ["any"],
            new SyncProgress<PipelineProgress>(progress));

        package.ShouldBeSameAs(project.Packages.Single());
        package.Released.ShouldBeTrue();
        package.Description.ShouldBe("rebuilt");
        var packagePath = project.PackageFilePath(Rebuilt, "any");
        var entry = (await _context.Feeds.LoadEntryAsync(project, Rebuilt))!;
        var file = entry.Files.Single();
        file.Path.ShouldBe("packages/1.1.0/any-r2.zip");
        file.Size.ShouldBe(_context.FileSystem.FileInfo.New(packagePath).Length);
        file.Sha512.ShouldBe(Hash(project, "any"));
        file.Sha512.ShouldNotBe(previous.Files.Single().Sha512);
        file.Touches.ShouldBeEmpty();
        _context.Signer.Verify(packagePath, TestKeys.PublicKey, file.Signature.Value).ShouldBeTrue();
        entry.PublishedAt.ShouldBe(previous.PublishedAt);
        entry.GetChangelog(new CultureInfo("de-DE")).ShouldBe("Fixes again");
        var content = await _context.ContentReader.ReadAsync(packagePath);
        content.Manifest!.Operations.ShouldBeEmpty();
        content.Entries.Single().RelativePath.ShouldBe("app.dll");

        Received.InOrder(() =>
        {
            _context.Transfer.UploadFileAsync(packagePath, "packages/1.1.0/any-r2.zip", null,
                Arg.Any<CancellationToken>());
            _context.Transfer.UploadFileAsync(Arg.Any<string>(), "nupdate.json", null, Arg.Any<CancellationToken>());
            _context.Transfer.DeleteFileAsync("packages/1.1.0/any.zip", Arg.Any<CancellationToken>());
        });
        var published = _uploadedFeeds.Single().Packages;
        published.Select(p => p.Version.ToString()).ShouldBe(["1.0.0", "1.1.0"]);
        published[1].Files.Single().Sha512.ShouldBe(file.Sha512);
        published[1].PublishedAt.ShouldBe(previous.PublishedAt);
        await _context.Statistics.DidNotReceiveWithAnyArgs().RegisterVersionAsync(default!, default, default!, default);

        project.Log.Last().Kind.ShouldBe(LogEntryKind.Rebuild);
        (await _context.Store.LoadAsync(project.Path)).Project.Log.Last().Kind.ShouldBe(LogEntryKind.Rebuild);
        PackageDirectoryEntries(project).ShouldBe(["any", "feed-entry.json"]); // the backup is gone
        progress.Select(p => p.StepName).ShouldBe([
            "Loading the current feed", "Building the package", "Uploading the package",
            "Updating the local feed entry", "Uploading the feed",
            "Deleting the replaced package files from the server", "Saving the project", "Done"
        ]);
    }

    [Fact]
    public async Task RebuildPackage_KeepsUnchangedPlatformsAddsNewOnesAndRemovesDroppedOnes()
    {
        var (project, secrets, previous) = await CreateAsync(publish: true, "linux", "win-x64");
        var linux = previous.Files.Single(f => f.Platform == "linux");
        var definition = new PackageDefinition(Rebuilt);
        definition.GetOrAddPlatform("any").Files.Add(new PackageFileEntry(PackageRoot.Program, "app.dll",
            _context.AddSourceFile("app-2.dll", "any 2")));
        definition.GetOrAddPlatform("linux"); // kept as it is; its files are not needed
        definition.GetOrAddPlatform("osx").Files.Add(new PackageFileEntry(PackageRoot.Program, "App",
            _context.AddSourceFile("App", "mac")));

        await _service.RebuildPackageAsync(RebuildRequest(project, secrets, definition), ["osx", "any"]);

        var entry = (await _context.Feeds.LoadEntryAsync(project, Rebuilt))!;
        entry.Files.Select(f => (f.Platform, f.Path)).ShouldBe([
            ("any", "packages/1.1.0/any-r2.zip"), ("linux", "packages/1.1.0/linux.zip"),
            ("osx", "packages/1.1.0/osx-r2.zip")
        ]);
        (entry.Files[1].Sha512, entry.Files[1].Signature.Value).ShouldBe((linux.Sha512, linux.Signature.Value));
        Hash(project, "linux").ShouldBe(linux.Sha512);
        Hash(project, "osx").ShouldBe(entry.Files[2].Sha512);
        PackageDirectoryEntries(project).ShouldBe(["any", "feed-entry.json", "linux", "osx"]);
        await _context.Transfer.Received().UploadFileAsync(project.PackageFilePath(Rebuilt, "osx"),
            "packages/1.1.0/osx-r2.zip", null, Arg.Any<CancellationToken>());
        await _context.Transfer.DidNotReceive().UploadFileAsync(Arg.Any<string>(),
            Arg.Is<string>(p => p.Contains("linux")), Arg.Any<IProgress<TransferProgress>?>(),
            Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteFileAsync("packages/1.1.0/any.zip", Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteFileAsync("packages/1.1.0/win-x64.zip", Arg.Any<CancellationToken>());
        await _context.Transfer.DidNotReceive().DeleteFileAsync("packages/1.1.0/linux.zip",
            Arg.Any<CancellationToken>());
        _uploadedFeeds.Single().Packages[1].Files.Select(f => f.Platform).ShouldBe(["any", "linux", "osx"]);
    }

    [Fact]
    public async Task RebuildPackage_RebuildsALocalPackageOnlyLocally()
    {
        var (project, secrets, _) = await CreateAsync(publish: false);
        var request = RebuildRequest(project, secrets, await OpenAndChangeAsync(project));
        request.Publish = true; // a local package stays local

        var package = await _service.RebuildPackageAsync(request, ["any"]);

        package.Released.ShouldBeFalse();
        var entry = (await _context.Feeds.LoadEntryAsync(project, Rebuilt))!;
        entry.Files.Single().Path.ShouldBe("packages/1.1.0/any.zip"); // nothing on the server can be confused with it
        entry.Files.Single().Sha512.ShouldBe(Hash(project, "any"));
        _context.Http.Requests.ShouldBeEmpty();
        _context.Transfer.ReceivedCalls().ShouldBeEmpty();
        project.Log.Select(l => l.Kind).ShouldBe([LogEntryKind.Create, LogEntryKind.Rebuild]);
        PackageDirectoryEntries(project).ShouldBe(["any", "feed-entry.json"]);
    }

    [Fact]
    public async Task RebuildPackage_RestoresTheLocalPackageWhenBuildingFails()
    {
        var (project, secrets, previous) = await CreateAsync(publish: true, "linux");
        var definition = new PackageDefinition(Rebuilt);
        definition.GetOrAddPlatform("any").Files.Add(new PackageFileEntry(PackageRoot.Program, "app.dll",
            _context.AddSourceFile("app-2.dll", "any 2")));
        definition.GetOrAddPlatform("linux").Files
            .Add(new PackageFileEntry(PackageRoot.Program, "app", "/missing/app"));

        var ex = await Should.ThrowAsync<PipelineException>(() =>
            _service.RebuildPackageAsync(RebuildRequest(project, secrets, definition), ["any", "linux"]));

        ex.FailedStep.ShouldBe("Building the package");
        ex.CompensationErrors.ShouldBeEmpty();
        Hash(project, "any").ShouldBe(previous.Files[0].Sha512);
        Hash(project, "linux").ShouldBe(previous.Files[1].Sha512);
        _context.FileSystem.File.Exists(_context.FileSystem.Path.Combine(project.PlatformDirectory(Rebuilt, "linux"),
            PackageLayout.ManifestFileName)).ShouldBeTrue();
        PackageDirectoryEntries(project).ShouldBe(["any", "feed-entry.json", "linux"]);
        (await _context.Feeds.LoadEntryAsync(project, Rebuilt))!.Files.Select(f => f.Path)
            .ShouldBe(["packages/1.1.0/any.zip", "packages/1.1.0/linux.zip"]);
        _context.Transfer.ReceivedCalls().ShouldBeEmpty();
        project.Packages.Single().Description.ShouldBe("desc");
    }

    [Fact]
    public async Task RebuildPackage_DeletesTheNewFilesWhenTheFeedCannotBeUploaded()
    {
        var (project, secrets, previous) = await CreateAsync(publish: true);
        _context.Transfer.UploadFileAsync(Arg.Any<string>(), "nupdate.json", null, Arg.Any<CancellationToken>())
            .Returns(_ => throw new TransferException("disk full"));

        var ex = await Should.ThrowAsync<PipelineException>(async () =>
            await _service.RebuildPackageAsync(RebuildRequest(project, secrets, await OpenAndChangeAsync(project)),
                ["any"]));

        ex.FailedStep.ShouldBe("Uploading the feed");
        ex.CompensationErrors.ShouldBeEmpty();
        await _context.Transfer.Received().DeleteFileAsync("packages/1.1.0/any-r2.zip", Arg.Any<CancellationToken>());
        await _context.Transfer.DidNotReceive().DeleteFileAsync("packages/1.1.0/any.zip",
            Arg.Any<CancellationToken>());
        var entry = (await _context.Feeds.LoadEntryAsync(project, Rebuilt))!;
        (entry.Files.Single().Path, entry.Files.Single().Sha512)
            .ShouldBe(("packages/1.1.0/any.zip", previous.Files.Single().Sha512));
        Hash(project, "any").ShouldBe(previous.Files.Single().Sha512);
        project.Packages.Single().Description.ShouldBe("desc");
        project.Log.Last().Kind.ShouldBe(LogEntryKind.Upload);
    }

    [Fact]
    public async Task RebuildPackage_PutsTheReplacedFilesBackBeforeRestoringTheFeed()
    {
        var (project, secrets, previous) = await CreateAsync(publish: true, "linux");
        var restored = new Dictionary<string, string>();
        _context.Transfer.UploadFileAsync(Arg.Any<string>(),
                Arg.Is<string>(p => p == "packages/1.1.0/any.zip" || p == "packages/1.1.0/linux.zip"), null,
                Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                restored[call.ArgAt<string>(1)] = _context.Signer.Hash(call.ArgAt<string>(0));
                return Task.CompletedTask;
            });
        _context.Transfer.DeleteFileAsync("packages/1.1.0/linux.zip", Arg.Any<CancellationToken>())
            .Returns(_ => throw new TransferException("refused"));

        var ex = await Should.ThrowAsync<PipelineException>(async () =>
            await _service.RebuildPackageAsync(RebuildRequest(project, secrets, await OpenAndChangeAsync(project)),
                ["any", "linux"]));

        ex.FailedStep.ShouldBe("Deleting the replaced package files from the server");
        ex.CompensationErrors.ShouldBeEmpty();
        restored.ShouldBe(new Dictionary<string, string>
        {
            ["packages/1.1.0/any.zip"] = previous.Files[0].Sha512,
            ["packages/1.1.0/linux.zip"] = previous.Files[1].Sha512, // its deletion may have got half-way
        });
        Received.InOrder(() =>
        {
            var transfer = _context.Transfer;
            transfer.UploadFileAsync(Arg.Any<string>(), "packages/1.1.0/any-r2.zip", null,
                Arg.Any<CancellationToken>());
            transfer.UploadFileAsync(Arg.Any<string>(), "packages/1.1.0/linux-r2.zip", null,
                Arg.Any<CancellationToken>());
            transfer.UploadFileAsync(Arg.Any<string>(), "nupdate.json", null, Arg.Any<CancellationToken>());
            transfer.DeleteFileAsync("packages/1.1.0/any.zip", Arg.Any<CancellationToken>());
            transfer.DeleteFileAsync("packages/1.1.0/linux.zip", Arg.Any<CancellationToken>());
            transfer.UploadFileAsync(Arg.Any<string>(), "packages/1.1.0/any.zip", null, Arg.Any<CancellationToken>());
            transfer.UploadFileAsync(Arg.Any<string>(), "packages/1.1.0/linux.zip", null, Arg.Any<CancellationToken>());
            transfer.UploadFileAsync(Arg.Any<string>(), "nupdate.json", null, Arg.Any<CancellationToken>());
            transfer.DeleteFileAsync("packages/1.1.0/any-r2.zip", Arg.Any<CancellationToken>());
            transfer.DeleteFileAsync("packages/1.1.0/linux-r2.zip", Arg.Any<CancellationToken>());
        });
        _uploadedFeeds.Count.ShouldBe(2);
        _uploadedFeeds[1].Packages[1].Files.Select(f => f.Path)
            .ShouldBe(["packages/1.1.0/any.zip", "packages/1.1.0/linux.zip"]);
        Hash(project, "linux").ShouldBe(previous.Files[1].Sha512);
        PackageDirectoryEntries(project).ShouldBe(["any", "feed-entry.json", "linux"]);
    }

    [Fact]
    public async Task RebuildPackage_RollsBackEverythingWhenSavingFails()
    {
        var (project, secrets, previous) = await CreateAsync(publish: true);
        _context.ServeFeed(null); // released, but the server lost its feed
        var definition = await OpenAndChangeAsync(project);
        _context.FileSystem.File.SetAttributes(project.Path, FileAttributes.ReadOnly);

        var ex = await Should.ThrowAsync<PipelineException>(() =>
            _service.RebuildPackageAsync(RebuildRequest(project, secrets, definition), ["any"]));

        ex.FailedStep.ShouldBe("Saving the project");
        ex.CompensationErrors.ShouldBeEmpty();
        project.Log.Select(l => l.Kind).ShouldBe([LogEntryKind.Create, LogEntryKind.Upload]);
        _uploadedFeeds.Single().Packages.Single().Files.Single().Path.ShouldBe("packages/1.1.0/any-r2.zip");
        await _context.Transfer.Received().DeleteFileAsync("nupdate.json", Arg.Any<CancellationToken>());
        await _context.Transfer.Received().UploadFileAsync(Arg.Any<string>(), "packages/1.1.0/any.zip", null,
            Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteFileAsync("packages/1.1.0/any-r2.zip", Arg.Any<CancellationToken>());
        Hash(project, "any").ShouldBe(previous.Files.Single().Sha512);
        (await _context.Feeds.LoadEntryAsync(project, Rebuilt))!.Files.Single().Path
            .ShouldBe("packages/1.1.0/any.zip");
        project.Packages.Single().Description.ShouldBe("desc");
    }

    [Theory]
    [InlineData(typeof(IOException))]
    [InlineData(typeof(UnauthorizedAccessException))]
    public async Task RebuildPackage_SucceedsWhenTheBackupCannotBeDeleted(Type exceptionType)
    {
        var (project, secrets, previous) = await CreateAsync(publish: false);
        var mock = _context.FileSystem;
        var directory = Substitute.For<IDirectory>();
        directory.Exists(Arg.Any<string>()).Returns(call => mock.Directory.Exists(call.Arg<string>()));
        directory.CreateDirectory(Arg.Any<string>())
            .Returns(call => mock.Directory.CreateDirectory(call.Arg<string>()));
        directory.When(d => d.Move(Arg.Any<string>(), Arg.Any<string>()))
            .Do(call => mock.Directory.Move(call.ArgAt<string>(0), call.ArgAt<string>(1)));
        directory.When(d => d.Delete(Arg.Any<string>(), true))
            .Do(_ => throw (Exception)Activator.CreateInstance(exceptionType, "in use")!);
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.Directory.Returns(directory);
        fileSystem.File.Returns(mock.File);
        fileSystem.FileInfo.Returns(mock.FileInfo);
        fileSystem.Path.Returns(mock.Path);
        var service = new PublishService(fileSystem, _context.Builder, _context.Signer, _context.Feeds,
            _context.TransferFactory, _context.Statistics, _context.Store, _context.Logger, _context.ContentReader,
            () => AdminTestContext.Now);

        var package = await service.RebuildPackageAsync(
            RebuildRequest(project, secrets, await OpenAndChangeAsync(project)), ["any"]);

        package.Description.ShouldBe("rebuilt");
        var backup = mock.Directory.GetDirectories(project.PackageDirectory(Rebuilt), ".backup-*").Single();
        _context.Signer.Hash(mock.Path.Combine(backup, "any", "any.zip")).ShouldBe(previous.Files.Single().Sha512);
    }

    [Fact]
    public async Task RebuildPackage_ValidatesRequests()
    {
        var (project, secrets, _) = await CreateAsync(publish: false);
        var definition = await _service.OpenPackageAsync(project, Rebuilt, "/edit");
        await Should.ThrowAsync<ArgumentNullException>(() => _service.RebuildPackageAsync(null!, ["any"]));
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _service.RebuildPackageAsync(RebuildRequest(project, secrets, definition), null!));

        var other = new PackageDefinition(new UpdateVersion("9.0.0"));
        other.GetOrAddPlatform("any").Operations.Add(new TerminateProcessOperation { ProcessName = "x" });
        (await Should.ThrowAsync<InvalidOperationException>(() =>
            _service.RebuildPackageAsync(RebuildRequest(project, secrets, other), ["any"]))).Message.ShouldContain(
            "no package 9.0.0");
        (await Should.ThrowAsync<ArgumentException>(() =>
                _service.RebuildPackageAsync(RebuildRequest(project, secrets, definition), ["any", "linux"])))
            .Message.ShouldContain("The linux package is to be built, but the package has no such platform.");

        var withoutChangelog = RebuildRequest(project, secrets, definition);
        withoutChangelog.Changelog.Clear();
        await Should.ThrowAsync<ArgumentException>(() => _service.RebuildPackageAsync(withoutChangelog, ["any"]));
        var invalidUrl = _context.NewProject();
        invalidUrl.UpdateUrl = "not a url";
        invalidUrl.Packages.Add(new UpdatePackage { Version = Rebuilt });
        (await Should.ThrowAsync<ArgumentException>(() =>
                _service.RebuildPackageAsync(RebuildRequest(invalidUrl, secrets, definition), ["any"])))
            .Message.ShouldContain("not a url");
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("2.0.0") });
        (await Should.ThrowAsync<ArgumentException>(() => _service.RebuildPackageAsync(
                RebuildRequest(project, secrets, new PackageDefinition(new UpdateVersion("2.0.0"))), [])))
            .Message.ShouldContain("at least one platform");

        var empty = new PackageDefinition(Rebuilt);
        empty.GetOrAddPlatform("linux");
        (await Should.ThrowAsync<ArgumentException>(() =>
                _service.RebuildPackageAsync(RebuildRequest(project, secrets, empty), ["linux"])))
            .Message.ShouldContain("The linux package needs at least one file or operation.");
        empty.Platforms[0].Operations.Add(new StopServiceOperation { ServiceName = "svc" });
        (await Should.ThrowAsync<ArgumentException>(() =>
                _service.RebuildPackageAsync(RebuildRequest(project, secrets, empty), ["linux"])))
            .Message.ShouldContain("registry or service operations");
        (await Should.ThrowAsync<ArgumentException>(() =>
                _service.RebuildPackageAsync(RebuildRequest(project, secrets, empty), [])))
            .Message.ShouldContain("The linux package has no package file yet, so it has to be built.");

        (await Should.ThrowAsync<InvalidOperationException>(() => _service.RebuildPackageAsync(
                RebuildRequest(project, new ProjectSecrets { TransferPassword = "x" }, definition), ["any"])))
            .Message.ShouldContain("private key");
        _context.FileSystem.File.Delete(
            _context.FileSystem.Path.Combine(project.PackageDirectory(Rebuilt), IFeedStore.EntryFileName));
        (await Should.ThrowAsync<InvalidOperationException>(() =>
                _service.RebuildPackageAsync(RebuildRequest(project, secrets, definition), ["any"])))
            .Message.ShouldContain("feed entry");
    }

    [Fact]
    public void RevisionPath_CountsUpFromTheCurrentName()
    {
        var version = new UpdateVersion("2.0.0");
        PublishService.RevisionPath(version, "win-x64", "packages/2.0.0/win-x64.zip")
            .ShouldBe("packages/2.0.0/win-x64-r2.zip");
        PublishService.RevisionPath(version, "win-x64", "packages/2.0.0/win-x64-r2.zip")
            .ShouldBe("packages/2.0.0/win-x64-r3.zip");
        PublishService.RevisionPath(version, "win", "win-r9.zip").ShouldBe("packages/2.0.0/win-r10.zip");
        PublishService.RevisionPath(version, "x64", "packages/2.0.0/win-x64-r5.zip")
            .ShouldBe("packages/2.0.0/x64-r2.zip"); // another platform's name
        PublishService.RevisionPath(version, "any", "https://mirror.example.com/any-r4.zip")
            .ShouldBe("packages/2.0.0/any-r5.zip");
        PublishService.RevisionPath(version, "linux", string.Empty).ShouldBe("packages/2.0.0/linux-r2.zip");
        Should.Throw<ArgumentNullException>(() => PublishService.RevisionPath(null!, "any", string.Empty));
        Should.Throw<ArgumentNullException>(() => PublishService.RevisionPath(version, null!, string.Empty));
        Should.Throw<ArgumentNullException>(() => PublishService.RevisionPath(version, "any", null!));
    }
}
