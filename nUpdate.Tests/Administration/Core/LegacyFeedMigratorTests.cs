using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Net;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.TransferInterface;
using nUpdate.Operations;
using nUpdate.Tests.Administration.Support;
using nUpdate.Tests.Support;
using nUpdate.Updating;
using static nUpdate.Tests.Administration.Support.LegacyTestData;

namespace nUpdate.Tests.Administration.Core;

public class LegacyFeedMigratorTests
{
    private readonly AdminTestContext _context = new();
    private readonly List<UpdateFeed> _uploadedFeeds = [];
    private readonly List<string> _uploadedFiles = [];

    public LegacyFeedMigratorTests()
    {
        _context.Transfer.UploadFileAsync(Arg.Any<string>(), Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                _uploadedFiles.Add(call.ArgAt<string>(1));
                if (call.ArgAt<string>(1) == "nupdate.json")
                    _uploadedFeeds.Add(FeedLoader.Parse(_context.FileSystem.File.ReadAllText(call.ArgAt<string>(0))));
                return Task.CompletedTask;
            });
    }

    /// <summary>Prepares the migration and runs it with every ready package, as the assistant does when the user keeps the selection.</summary>
    private async Task<IReadOnlyList<UpdateVersion>> MigrateAsync(UpdateProject project, ProjectSecrets secrets,
        IProgress<PipelineProgress>? progress = null)
    {
        using var plan = await _context.Migrator.PrepareAsync(project, secrets);
        return await _context.Migrator.RunAsync(project, secrets, plan, progress);
    }

    /// <summary>An entry of updates.json for a package at <paramref name="uri" />, signed over <paramref name="zip" />; the zip is served there unless <paramref name="serve" /> is false.</summary>
    private string Entry(string literal, string uri, byte[] zip, bool serve = true)
    {
        if (serve)
            _context.Http.Bytes(uri, zip);
        return $$"""{"LiteralVersion":"{{literal}}","UpdatePackageUri":"{{uri}}","Signature":"{{Sign(zip)}}"}""";
    }

    [Fact]
    public async Task Check_ReportsBothFeeds()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeLegacyFeed("[]");
        _context.ServeFeed(null);
        var status = await _context.Migrator.CheckAsync(project, secrets);
        status.LegacyFeedPresent.ShouldBeTrue();
        status.FeedPresent.ShouldBeFalse();
        status.NeedsMigration.ShouldBeTrue();

        _context.ServeFeed(new UpdateFeed { ProjectId = project.Id });
        (await _context.Migrator.CheckAsync(project, secrets)).NeedsMigration.ShouldBeFalse();
        _context.ServeLegacyFeed(null);
        (await _context.Migrator.CheckAsync(project, secrets)).LegacyFeedPresent.ShouldBeFalse();
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Migrator.CheckAsync(null!, secrets));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Migrator.CheckAsync(project, null!));
    }

    [Fact]
    public async Task Run_RepacksSignsAndUploadsNextToTheOldFilesWithoutTouchingThem()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        project.Packages.Add(new UpdatePackage
        { Version = new UpdateVersion("1.0.0"), Released = true, Description = "first" });
        var firstZip = LegacyZip(true, ("Program/app.exe", "v1"), ("Program/sub/lib.dll", "l"),
            ("operations.json.bak", "x"));
        var secondZip = LegacyZip(false, ("AppData/settings.json", "{}"));
        _context.ServeLegacyFeed(LegacyFeedJson("false", Sign(firstZip), Sign(secondZip)));
        _context.ServeFeed(null);
        // 1.0.0.0 is available locally in the layout of the 5.0 pre-releases; 1.1.0.0b2 has to be downloaded.
        var legacyFolder = _context.FileSystem.Path.Combine(project.Folder, "1.0.0.0");
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(legacyFolder, $"{project.Id}.zip"),
            new MockFileData(firstZip));
        _context.Http.Bytes("https://updates.example.com/demo/1.1.0.0b2/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip",
            secondZip);
        var progress = new List<PipelineProgress>();

        var migrated = await MigrateAsync(project, secrets, new SyncProgress<PipelineProgress>(progress));

        migrated.Select(v => v.ToString()).ShouldBe(["1.0.0", "1.1.0-beta.2"]);
        var first = new UpdateVersion("1.0.0");
        var second = new UpdateVersion("1.1.0-beta.2");
        _context.FileSystem.File.Exists(project.PackageFilePath(first, "win")).ShouldBeTrue();
        _context.FileSystem.File.Exists(project.PackageFilePath(second, "win-x64")).ShouldBeTrue();
        // The old copy stays for nUpdate Administration 4, which may keep publishing to updates.json for a while.
        _context.FileSystem.Directory.Exists(legacyFolder).ShouldBeTrue();

        var content =
            await new PackageContentReader(_context.FileSystem).ReadAsync(project.PackageFilePath(first, "win"));
        content.Entries.Select(e => e.RelativePath).ShouldBe(["app.exe", "sub/lib.dll"]);
        content.Manifest!.ProjectId.ShouldBe(project.Id);
        content.Manifest.Version.ShouldBe(first);
        content.Manifest.CreatedAt.ShouldBe(AdminTestContext.Now);
        content.Manifest.Platform.ShouldBe("win");
        content.Manifest.Operations.Count.ShouldBe(10); // the C# script is left out
        using (var archive =
               new ZipArchive(
                   new MemoryStream(_context.FileSystem.File.ReadAllBytes(project.PackageFilePath(first, "win")))))
            archive.Entries.Select(e => e.FullName).ShouldNotContain("operations.json");
        var secondContent =
            await new PackageContentReader(_context.FileSystem).ReadAsync(project.PackageFilePath(second, "win-x64"));
        secondContent.Manifest!.Operations.Single().ShouldBeOfType<TerminateProcessOperation>();
        secondContent.Manifest.Platform.ShouldBe("win-x64");
        _context.FileSystem.File
            .Exists(_context.FileSystem.Path.Combine(project.PlatformDirectory(second, "win-x64"), "manifest.json"))
            .ShouldBeTrue();

        var entry = (await _context.Feeds.LoadEntryAsync(project, second))!;
        var file = entry.Files.Single();
        file.Platform.ShouldBe("win-x64");
        file.Path.ShouldBe("packages/1.1.0-beta.2/win-x64.zip");
        file.Size.ShouldBe(_context.FileSystem.FileInfo.New(project.PackageFilePath(second, "win-x64")).Length);
        file.Sha512.ShouldBe(_context.Signer.Hash(project.PackageFilePath(second, "win-x64")));
        _context.Signer.Verify(project.PackageFilePath(second, "win-x64"), TestKeys.PublicKey, file.Signature.Value)
            .ShouldBeTrue();
        file.Touches.ShouldBe([OperationArea.Processes]);
        entry.Necessary.ShouldBeTrue();
        entry.Rollout.Conditions.Single().Key.ShouldBe("R");
        entry.UnsupportedVersions.ShouldBe([new UpdateVersion("0.9.0")]);
        entry.Changelog["en"].ShouldBe("Beta");
        entry.Statistics!.Enabled.ShouldBeTrue();
        entry.Statistics.Url.ShouldBe("nupdate-statistics.php");
        (await _context.Feeds.LoadEntryAsync(project, first))!.Statistics!.Enabled.ShouldBeFalse();

        _uploadedFiles.ShouldBe([
            "packages/1.0.0/win.zip", "packages/1.1.0-beta.2/win-x64.zip", "nupdate.json", "nupdate-statistics.php",
            "nupdate-statistics.config.php"
        ]);
        await _context.Transfer.Received().CreateDirectoryAsync("packages/1.0.0", Arg.Any<CancellationToken>());
        await _context.Transfer.Received().CreateDirectoryAsync("packages/1.1.0-beta.2", Arg.Any<CancellationToken>());
        _uploadedFeeds.Single().ProjectId.ShouldBe(project.Id);
        _uploadedFeeds.Single().Packages.Select(p => p.Version).ShouldBe([first, second]);
        _uploadedFeeds.Single().Packages.ShouldAllBe(p => p.PublishedAt == AdminTestContext.Now);
        await _context.Statistics.Received()
            .VerifyAsync(Arg.Is<StatisticsEndpoint>(e => e.AdminSecret == "admin-secret" && e.Project == project),
                Arg.Any<CancellationToken>());
        await _context.Statistics.Received().RegisterVersionAsync(Arg.Any<StatisticsEndpoint>(), project.Id, first,
            Arg.Any<CancellationToken>());
        await _context.Statistics.Received().RegisterVersionAsync(Arg.Any<StatisticsEndpoint>(), project.Id, second,
            Arg.Any<CancellationToken>());
        await _context.Transfer.DidNotReceive().DeleteFileAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _context.Transfer.DidNotReceive().DeleteDirectoryAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        project.Packages.Select(p => $"{p.Version}:{p.Released}:{p.Description}")
            .ShouldBe(["1.0.0:True:first", "1.1.0-beta.2:True:"]);
        project.Log.Select(l => l.Kind).ShouldBe([LogEntryKind.Migrate, LogEntryKind.Migrate]);
        (await _context.Store.LoadAsync(project.Path)).Project.Packages.Count.ShouldBe(2);
        progress.Select(p => p.StepName).ShouldBe([
            "Repacking 1.0.0", "Uploading 1.0.0", "Repacking 1.1.0-beta.2", "Uploading 1.1.0-beta.2",
            "Uploading the feed", "Setting up the statistics", "Registering the versions in the statistics",
            "Updating the project", "Done"
        ]);
    }

    [Fact]
    public async Task Run_SkipsPackagesAlreadyInTheNewFeedAndDoesNothingWithoutLegacyFeed()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        var second = LegacyZip(false);
        _context.ServeLegacyFeed(LegacyFeedJson("false", "old", Sign(second)));
        var existing = new PackageInfo
        {
            Version = new UpdateVersion("1.0.0"),
            Files =
            [
                new PackageFile
                {
                    Platform = "win", Path = "packages/1.0.0/win.zip", Size = 1, Sha512 = "h",
                    Signature = new PackageSignature { Value = "s" }
                }
            ]
        };
        _context.ServeFeed(new UpdateFeed { ProjectId = project.Id, Packages = [existing] });
        _context.Http.Bytes("https://updates.example.com/demo/1.1.0.0b2/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip",
            second);

        using (var plan = await _context.Migrator.PrepareAsync(project, secrets))
        {
            plan.LegacyFeedPresent.ShouldBeTrue();
            plan.ExistingFeed.ShouldNotBeNull();
            plan.IsComplete.ShouldBeFalse();
            plan.Packages.Select(p => (p.Version.ToString(), p.AlreadyMigrated, p.Include))
                .ShouldBe([("1.0.0", true, false), ("1.1.0-beta.2", false, true)]);
            plan.Pending.Single().LiteralVersion.ShouldBe("1.1.0.0b2");
            plan.Packages[0].Include = true; // a migrated package cannot be included again
            plan.Packages[0].Include.ShouldBeFalse();
            plan.Packages[0].CanInclude.ShouldBeFalse();
            (await _context.Migrator.RunAsync(project, secrets, plan)).ShouldBe([new UpdateVersion("1.1.0-beta.2")]);
        }

        _uploadedFeeds.Single().Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0", "1.1.0-beta.2"]);
        await _context.Statistics.DidNotReceiveWithAnyArgs().RegisterVersionAsync(default!, default, default!, default);

        // Everything is in nupdate.json: the plan says so and running it changes nothing.
        _uploadedFeeds.Clear();
        _context.ServeFeed(new UpdateFeed
        {
            ProjectId = project.Id,
            Packages =
            [
                existing, new PackageInfo { Version = new UpdateVersion("1.1.0-beta.2"), Files = existing.Files }
            ]
        });
        using (var plan = await _context.Migrator.PrepareAsync(project, secrets))
        {
            plan.IsComplete.ShouldBeTrue();
            (await _context.Migrator.RunAsync(project, secrets, plan)).ShouldBeEmpty();
        }

        _context.ServeLegacyFeed(null);
        using (var plan = await _context.Migrator.PrepareAsync(project, secrets))
        {
            plan.LegacyFeedPresent.ShouldBeFalse();
            plan.Packages.ShouldBeEmpty();
            (await _context.Migrator.RunAsync(project, secrets, plan)).ShouldBeEmpty();
        }

        _uploadedFeeds.ShouldBeEmpty();
    }

    [Fact]
    public async Task Run_RollsBackUploadsAndLocalFoldersWhenAStepFails()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        var first = LegacyZip(true);
        var second = LegacyZip(false);
        _context.ServeLegacyFeed(LegacyFeedJson("false", Sign(first), Sign(second)));
        _context.ServeFeed(null);
        _context.Http.Bytes("https://updates.example.com/demo/1.0.0.0/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip", first);
        _context.Http.Bytes("https://updates.example.com/demo/1.1.0.0b2/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip",
            second);
        _context.Transfer.UploadFileAsync(Arg.Any<string>(), "nupdate.json", null, Arg.Any<CancellationToken>())
            .Returns(_ => throw new TransferException("quota"));

        var ex = await Should.ThrowAsync<PipelineException>(() => MigrateAsync(project, secrets));

        ex.FailedStep.ShouldBe("Uploading the feed");
        ex.CompensationErrors.ShouldBeEmpty();
        await _context.Transfer.Received().DeleteDirectoryAsync("packages/1.0.0", Arg.Any<CancellationToken>());
        await _context.Transfer.Received().DeleteDirectoryAsync("packages/1.1.0-beta.2", Arg.Any<CancellationToken>());
        await _context.Transfer.DidNotReceive().DeleteFileAsync("nupdate.json", Arg.Any<CancellationToken>());
        _context.FileSystem.Directory.Exists(project.PackagesDirectory).ShouldBeTrue();
        _context.FileSystem.Directory.GetDirectories(project.PackagesDirectory).ShouldBeEmpty();
        project.Packages.ShouldBeEmpty();
    }

    [Fact]
    public async Task Run_RestoresAnExistingFeedWhenStatisticsFail()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        var second = LegacyZip(false);
        _context.ServeLegacyFeed(LegacyFeedJson("false", "old", Sign(second)));
        var existing = new PackageInfo
        {
            Version = new UpdateVersion("1.0.0"),
            Files =
            [
                new PackageFile
                {
                    Platform = "win", Path = "packages/1.0.0/win.zip", Size = 1, Sha512 = "h",
                    Signature = new PackageSignature { Value = "s" }
                }
            ]
        };
        _context.ServeFeed(new UpdateFeed { ProjectId = project.Id, Packages = [existing] });
        _context.Http.Bytes("https://updates.example.com/demo/1.1.0.0b2/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip",
            second);
        _context.Statistics
            .RegisterVersionAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<Guid>(), Arg.Any<UpdateVersion>(),
                Arg.Any<CancellationToken>()).Returns(_ => throw new StatisticsException("db"));

        var ex = await Should.ThrowAsync<PipelineException>(() => MigrateAsync(project, secrets));

        ex.FailedStep.ShouldBe("Registering the versions in the statistics");
        _uploadedFeeds.Count.ShouldBe(2);
        _uploadedFeeds[1].Packages.Select(p => p.Version.ToString()).ShouldBe(["1.0.0"]);
    }

    [Fact]
    public async Task Run_DeletesTheFeedWhenTheServerHadNoneAndALaterStepFails()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        var first = LegacyZip(true);
        var second = LegacyZip(false);
        _context.ServeLegacyFeed(LegacyFeedJson("false", Sign(first), Sign(second)));
        _context.ServeFeed(null);
        _context.Http.Bytes("https://updates.example.com/demo/1.0.0.0/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip", first);
        _context.Http.Bytes("https://updates.example.com/demo/1.1.0.0b2/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip",
            second);
        _context.Statistics
            .RegisterVersionAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<Guid>(), Arg.Any<UpdateVersion>(),
                Arg.Any<CancellationToken>()).Returns(_ => throw new StatisticsException("db"));

        await Should.ThrowAsync<PipelineException>(() => MigrateAsync(project, secrets));

        _uploadedFeeds.Count.ShouldBe(1);
        await _context.Transfer.Received().DeleteFileAsync("nupdate.json", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Run_RestoresAnEmptyFeedTheServerAlreadyHad()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        var first = LegacyZip(true);
        var second = LegacyZip(false);
        _context.ServeLegacyFeed(LegacyFeedJson("false", Sign(first), Sign(second)));
        _context.ServeFeed(new UpdateFeed { ProjectId = project.Id });
        _context.Http.Bytes("https://updates.example.com/demo/1.0.0.0/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip", first);
        _context.Http.Bytes("https://updates.example.com/demo/1.1.0.0b2/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip",
            second);
        _context.Statistics.VerifyAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new StatisticsException("old script"));

        var ex = await Should.ThrowAsync<PipelineException>(() => MigrateAsync(project, secrets));

        ex.FailedStep.ShouldBe("Setting up the statistics");
        _uploadedFeeds.Count.ShouldBe(2);
        _uploadedFeeds[1].Packages.ShouldBeEmpty();
        await _context.Transfer.DidNotReceive().DeleteFileAsync("nupdate.json", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Run_RollsBackTheProjectWhenSavingFails()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("1.0.0"), Released = false });
        var first = LegacyZip(true, ("Program/app.exe", "v1"));
        var second = LegacyZip(false);
        _context.ServeLegacyFeed(LegacyFeedJson("false", Sign(first), Sign(second)));
        _context.ServeFeed(null);
        var legacyFolder = _context.FileSystem.Path.Combine(_context.Paths.Root, "Projects", project.Name, "1.0.0.0");
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(legacyFolder, $"{project.Id}.zip"),
            new MockFileData(first));
        _context.Http.Bytes("https://updates.example.com/demo/1.1.0.0b2/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip",
            second);
        project.Path =
            _context.FileSystem.Path.Combine(_context.FileSystem.Path.GetTempPath(), "locked", "project.nupdproj");
        _context.FileSystem.AddDirectory(project.Path); // a directory where the file should go makes the save fail

        var ex = await Should.ThrowAsync<PipelineException>(() => MigrateAsync(project, secrets));

        ex.FailedStep.ShouldBe("Updating the project");
        project.Packages.Select(p => $"{p.Version}:{p.Released}").ShouldBe(["1.0.0:False"]);
        project.Log.ShouldBeEmpty();
        _context.FileSystem.Directory.Exists(legacyFolder).ShouldBeTrue();
        await _context.Transfer.Received().DeleteFileAsync("nupdate.json", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Run_TakesTheLocalCopyFromTheDataFolderOfNUpdate4()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        var zip = LegacyZip(false, ("Program/app.exe", "v1"), ("Program/../escape.txt", "x"),
            ("Nowhere/file.txt", "y"));
        _context.ServeLegacyFeed(
            $"[{Entry("1.0.0.0", "https://updates.example.com/demo/missing.zip", zip, serve: false)}]");
        _context.ServeFeed(null);
        var legacyFolder = _context.FileSystem.Path.Combine(_context.Paths.Root, "Projects", project.Name, "1.0.0.0");
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(legacyFolder, $"{project.Id}.zip"),
            new MockFileData(zip));

        using (var plan = await _context.Migrator.PrepareAsync(project, secrets))
        {
            var package = plan.Packages.Single();
            package.Source.ShouldBe(_context.FileSystem.Path.Combine(legacyFolder, $"{project.Id}.zip"));
            package.FileCount.ShouldBe(1);
            package.SkippedEntries.ShouldBe(["Program/../escape.txt", "Nowhere/file.txt"]);
            (await _context.Migrator.RunAsync(project, secrets, plan)).ShouldBe([new UpdateVersion("1.0.0")]);
        }

        _context.FileSystem.Directory.Exists(legacyFolder).ShouldBeTrue();
        var content =
            await new PackageContentReader(_context.FileSystem).ReadAsync(
                project.PackageFilePath(new UpdateVersion("1.0.0"), "win"));
        content.Entries.Select(e => e.RelativePath).ShouldBe(["app.exe"]);
    }

    [Fact]
    public async Task Prepare_ReportsPackagesThatCannotBeMigratedInsteadOfFailing()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeFeed(null);
        byte[] badOperations;
        using (var stream = new MemoryStream())
        {
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
            {
                using var writer = new StreamWriter(archive.CreateEntry("operations.json").Open());
                writer.Write("{broken");
            }

            badOperations = stream.ToArray();
        }

        var genuine = LegacyZip(false, ("Program/a.txt", "genuine"));
        _context.ServeLegacyFeed($$"""
                                   [{"LiteralVersion":"1.0","UpdatePackageUri":null,"Signature":"c2ln"},
                                    {"LiteralVersion":"1.1","UpdatePackageUri":"https://updates.example.com/demo/missing.zip","Signature":"c2ln"},
                                    {{Entry("1.2", "https://updates.example.com/demo/broken.zip", [1, 2, 3])}},
                                    {"LiteralVersion":"1.3","UpdatePackageUri":"https://updates.example.com/demo/slow.zip","Signature":"c2ln"},
                                    {{Entry("1.4", "https://updates.example.com/demo/badops.zip", badOperations)}},
                                    {"LiteralVersion":"1.5","UpdatePackageUri":"https://updates.example.com/demo/unsigned.zip"},
                                    {"LiteralVersion":"1.7"},
                                    {{Entry("1.6", "https://updates.example.com/demo/tampered.zip", genuine, serve: false)}}]
                                   """);
        _context.Http.Text(HttpMethod.Get, "https://updates.example.com/demo/missing.zip", "gone",
            HttpStatusCode.NotFound);
        _context.Http.On(r => r.RequestUri!.ToString() == "https://updates.example.com/demo/slow.zip",
            (_, _) => throw new TaskCanceledException("timeout"));
        _context.Http.Bytes("https://updates.example.com/demo/tampered.zip",
            LegacyZip(false, ("Program/a.txt", "changed on the way")));

        var progress = new List<PipelineProgress>();
        using var plan =
            await _context.Migrator.PrepareAsync(project, secrets, new SyncProgress<PipelineProgress>(progress));

        plan.Packages.Select(p => p.Version.ToString())
            .ShouldBe(["1.0.0", "1.1.0", "1.2.0", "1.3.0", "1.4.0", "1.5.0", "1.6.0", "1.7.0"]);
        plan.Packages.ShouldAllBe(p => !p.CanInclude && !p.Include && p.Problem != null);
        plan.Packages[0].Problem!.ShouldContain("neither on this computer nor named in updates.json");
        plan.Packages[0].Source.ShouldBeNull();
        plan.Packages[1].Problem!.ShouldContain("404");
        plan.Packages[1].Source.ShouldBe("https://updates.example.com/demo/missing.zip");
        plan.Packages[2].Problem.ShouldNotBeNullOrEmpty();
        plan.Packages[3].Problem.ShouldBe("The download did not finish in time.");
        plan.Packages[4].Problem.ShouldNotBeNullOrEmpty();
        plan.Packages[5].Problem!.ShouldContain("no signature");
        plan.Packages[6].Problem!.ShouldContain("does not carry the signature");
        plan.Packages[7].Problem!.ShouldContain("no signature");
        plan.Packages[7].Source.ShouldBeNull();
        plan.Packages[0].Include = true;
        plan.Packages[0].Include.ShouldBeFalse();
        progress.Select(p => p.StepName).ShouldBe([
            "Reading the feeds on the server", "Reading 1.0.0", "Reading 1.1.0", "Reading 1.2.0", "Reading 1.3.0",
            "Reading 1.4.0", "Reading 1.5.0", "Reading 1.6.0", "Reading 1.7.0"
        ]);

        // Nothing can be carried over and the server has no nupdate.json: the migration starts an empty one.
        (await _context.Migrator.RunAsync(project, secrets, plan)).ShouldBeEmpty();
        _uploadedFeeds.Single().Packages.ShouldBeEmpty();
        _uploadedFiles.ShouldBe(["nupdate.json"]);
        project.Log.ShouldBeEmpty();
    }

    [Fact]
    public async Task Prepare_FlagsDuplicateVersionsOwnPackagesAndUnreadableEntries()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeFeed(null);
        var zip = LegacyZip(false, ("Program/a.txt", "a"));
        _context.ServeLegacyFeed($$"""
                                   [{{Entry("1.0.0.0", "https://updates.example.com/demo/a.zip", zip)}},
                                    {{Entry("1.0", "https://updates.example.com/demo/b.zip", zip)}},
                                    {{Entry("2.0.0.0", "https://updates.example.com/demo/c.zip", zip)}},
                                    {"LiteralVersion":"not a version","UpdatePackageUri":"https://updates.example.com/demo/d.zip"}]
                                   """);
        // The user created 2.0.0 in this project already, for example the release that moves the users to nUpdate 5.
        _context.FileSystem.AddFile(project.PackageFilePath(new UpdateVersion("2.0.0"), "any"),
            new MockFileData("mine"));

        using var plan = await _context.Migrator.PrepareAsync(project, secrets);

        plan.UnreadableVersions.ShouldBe(["not a version"]);
        plan.Packages.Select(p => (p.LiteralVersion, p.CanInclude))
            .ShouldBe([("1.0.0.0", true), ("1.0", false), ("2.0.0.0", false)]);
        plan.Packages[1].Problem
            .ShouldBe("updates.json lists this version twice, as 1.0.0.0 and 1.0; only 1.0.0.0 is migrated.");
        plan.Packages[2].Problem!.ShouldContain("has a package 2.0.0 of its own already");
        _context.FileSystem.File.ReadAllText(project.PackageFilePath(new UpdateVersion("2.0.0"), "any"))
            .ShouldBe("mine");
    }

    [Fact]
    public async Task Prepare_DownloadsThePackageWhenTheLocalCopyDoesNotCarryItsSignature()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        var genuine = LegacyZip(false, ("Program/a.txt", "genuine"));
        _context.ServeLegacyFeed($"[{Entry("1.0.0.0", "https://updates.example.com/demo/p.zip", genuine)}]");
        _context.ServeFeed(null);
        var local = _context.FileSystem.Path.Combine(_context.Paths.LegacyProjectDataDirectory(project.Name), "1.0.0.0",
            $"{project.Id}.zip");
        _context.FileSystem.AddFile(local, new MockFileData(LegacyZip(false, ("Program/a.txt", "edited later"))));

        using var plan = await _context.Migrator.PrepareAsync(project, secrets);

        plan.Packages.Single().Source.ShouldBe("https://updates.example.com/demo/p.zip");
        plan.Packages.Single().CanInclude.ShouldBeTrue();
    }

    [Fact]
    public async Task Run_LeavesOutDeselectedPackages()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        var first = LegacyZip(true);
        var second = LegacyZip(false);
        _context.ServeLegacyFeed(LegacyFeedJson("false", Sign(first), Sign(second)));
        _context.ServeFeed(null);
        _context.Http.Bytes("https://updates.example.com/demo/1.0.0.0/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip", first);
        _context.Http.Bytes("https://updates.example.com/demo/1.1.0.0b2/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip",
            second);

        using var plan = await _context.Migrator.PrepareAsync(project, secrets);
        plan.LegacyStatisticsUsed.ShouldBeTrue();
        plan.Packages[0].Warnings.Count.ShouldBe(6);
        plan.Packages[0].Operations.Count.ShouldBe(10);
        plan.Packages[0].Size.ShouldBeGreaterThan(0);
        plan.Packages[1].Warnings.ShouldBeEmpty();
        plan.Packages[0].Include = false;
        plan.Included.Single().Version.ShouldBe(new UpdateVersion("1.1.0-beta.2"));

        (await _context.Migrator.RunAsync(project, secrets, plan)).ShouldBe([new UpdateVersion("1.1.0-beta.2")]);
        _uploadedFeeds.Single().Packages.Select(p => p.Version.ToString()).ShouldBe(["1.1.0-beta.2"]);
        await _context.Statistics.DidNotReceive().RegisterVersionAsync(Arg.Any<StatisticsEndpoint>(), project.Id,
            new UpdateVersion("1.0.0"), Arg.Any<CancellationToken>());
        project.Packages.Select(p => p.Version.ToString()).ShouldBe(["1.1.0-beta.2"]);
    }

    [Fact]
    public async Task Run_WithNothingSelectedOnlyStartsTheFeedAndTheStatistics()
    {
        var project = _context.NewProject(statistics: true);
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        _context.ServeLegacyFeed($"[{Entry("1.0.0.0", "https://updates.example.com/demo/p.zip", LegacyZip(false))}]");
        _context.ServeFeed(null);
        using var plan = await _context.Migrator.PrepareAsync(project, secrets);
        plan.Packages.Single().Include = false;
        var progress = new List<PipelineProgress>();

        // Without a private key: nothing has to be signed.
        (await _context.Migrator.RunAsync(project,
            new ProjectSecrets
            { TransferPassword = "x", StatisticsAdminSecret = "admin-secret", StatisticsDatabasePassword = "pw" },
            plan, new SyncProgress<PipelineProgress>(progress))).ShouldBeEmpty();

        progress.Select(p => p.StepName).ShouldBe(["Uploading the feed", "Setting up the statistics", "Done"]);
        _uploadedFeeds.Single().Packages.ShouldBeEmpty();
        await _context.Statistics.DidNotReceiveWithAnyArgs().RegisterVersionAsync(default!, default, default!, default);
    }

    [Fact]
    public async Task Prepare_KeepsDownloadsUntilThePlanIsDisposedAndCleansUpWhenCancelled()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        var q = LegacyZip(false);
        _context.ServeLegacyFeed(
            $"[{Entry("1.0.0.0", "https://updates.example.com/demo/p.zip", LegacyZip(false, ("Program/a.txt", "a")))},{Entry("1.1.0.0", "https://updates.example.com/demo/q.zip", q, serve: false)}]");
        _context.ServeFeed(null);

        var plan = await PrepareWithQAsync(q);
        var downloaded = plan.Packages[0].SourcePath!;
        _context.FileSystem.File.Exists(downloaded).ShouldBeTrue();
        plan.Packages[0].Source.ShouldBe("https://updates.example.com/demo/p.zip");
        plan.Dispose();
        _context.FileSystem.File.Exists(downloaded).ShouldBeFalse();
        plan.Dispose();

        using var cancellation = new CancellationTokenSource();
        _context.Http.On(r => r.RequestUri!.ToString() == "https://updates.example.com/demo/q.zip", (_, _) =>
        {
            cancellation.Cancel();
            throw new TaskCanceledException();
        });
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _context.Migrator.PrepareAsync(project, secrets, null, cancellation.Token));
        _context.FileSystem.Directory.GetDirectories(_context.FileSystem.Path.GetTempPath(), "nupdate-migration-*")
            .ShouldBeEmpty();

        async Task<MigrationPlan> PrepareWithQAsync(byte[] zip)
        {
            _context.Http.Bytes("https://updates.example.com/demo/q.zip", zip);
            return await _context.Migrator.PrepareAsync(project, secrets);
        }
    }

    [Fact]
    public async Task Run_RequiresPrivateKeyStatisticsSecretAndAPlanOfTheProject()
    {
        var project = _context.NewProject();
        _context.ServeLegacyFeed($"[{Entry("1.0.0.0", "https://updates.example.com/demo/p.zip", LegacyZip(false))}]");
        _context.ServeFeed(null);
        using var plan = await _context.Migrator.PrepareAsync(project, AdminTestContext.NewSecrets());

        (await Should.ThrowAsync<InvalidOperationException>(() =>
            _context.Migrator.RunAsync(project, new ProjectSecrets(), plan))).Message.ShouldContain("private key");
        var withStatistics = _context.NewProject(statistics: true);
        await Should.ThrowAsync<InvalidOperationException>(() =>
            _context.Migrator.RunAsync(withStatistics, AdminTestContext.NewSecrets(), plan));
        var other = _context.NewProject();
        other.Id = Guid.NewGuid();
        (await Should.ThrowAsync<ArgumentException>(() =>
                _context.Migrator.RunAsync(other, AdminTestContext.NewSecrets(), plan))).Message
            .ShouldContain("another project");

        _context.Http.Text(HttpMethod.Get, "https://updates.example.com/demo/updates.json", "boom",
            HttpStatusCode.InternalServerError);
        await Should.ThrowAsync<HttpRequestException>(() =>
            _context.Migrator.PrepareAsync(project, AdminTestContext.NewSecrets()));

        await Should.ThrowAsync<ArgumentNullException>(() =>
            _context.Migrator.PrepareAsync(null!, AdminTestContext.NewSecrets()));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Migrator.PrepareAsync(project, null!));
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _context.Migrator.RunAsync(null!, AdminTestContext.NewSecrets(), plan));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Migrator.RunAsync(project, null!, plan));
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _context.Migrator.RunAsync(project, AdminTestContext.NewSecrets(), null!));
        Should.Throw<ArgumentNullException>(() => LegacyFeedMigrator.BuildEntry(null!, project));
        Should.Throw<ArgumentNullException>(() =>
            LegacyFeedMigrator.BuildEntry(new LegacyFeedEntry(new UpdateVersion("1.0.0"), "1.0"), null!));
    }

    [Fact]
    public async Task Run_ReadsEmptyOperationsFilesAndConnectFailures()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeFeed(null);
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            archive.CreateEntry("operations.json");
            using var writer = new StreamWriter(archive.CreateEntry("Program/a.txt").Open());
            writer.Write("a");
        }

        _context.ServeLegacyFeed(
            $"[{Entry("1.0.0.0", "https://updates.example.com/demo/1.0.0.0/p.zip", stream.ToArray())}]");
        _context.Transfer.ConnectAsync(Arg.Any<CancellationToken>())
            .Returns(_ => throw new TransferException("refused"));

        var ex = await Should.ThrowAsync<PipelineException>(() => MigrateAsync(project, secrets));

        ex.FailedStep.ShouldBe("Uploading 1.0.0");
        ex.InnerException.ShouldBeOfType<TransferException>();
        await _context.Transfer.Received().DisposeAsync();
    }

    [Fact]
    public async Task FindLegacyFiles_ListsWhatNUpdate4LeftOnTheServerAndOnThisComputer()
    {
        var project = _context.NewProject();
        var oldFolder = _context.FileSystem.Path.Combine(_context.FileSystem.Path.GetTempPath(), "old", "Legacy");
        project.LegacyProjectFile = _context.FileSystem.Path.Combine(oldFolder, "Legacy.nupdproj");
        var secrets = AdminTestContext.NewSecrets();
        _context.ServeLegacyFeed(LegacyFeedJson("false", "old", "old"));
        _context.Transfer.FileExistsAsync("statistics.php", Arg.Any<CancellationToken>()).Returns(true);
        var zip = $"{project.Id}.zip";
        var preRelease = _context.FileSystem.Path.Combine(project.Folder, "1.0.0.0");
        var dataFolder = _context.Paths.LegacyProjectDataDirectory(project.Name);
        var besideOldFile = _context.FileSystem.Path.Combine(oldFolder, "1.1.0.0b2");
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(preRelease, zip), new MockFileData("z"));
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(dataFolder, "1.1.0.0b2", zip),
            new MockFileData("z"));
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(besideOldFile, zip), new MockFileData("z"));
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(dataFolder, "1.0.0.0", "other-project.zip"),
            new MockFileData("z"));
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(dataFolder, "statistics.php"),
            new MockFileData("<?php"));

        var files = await _context.Migrator.FindLegacyFilesAsync(project, secrets);

        files.ServerFiles.ShouldBe(["updates.json", "statistics.php"]);
        files.ServerDirectories.ShouldBe(["1.0.0.0", "1.1.0.0b2"]);
        // Only the version folders that hold this project's zip; never the data folder as a whole.
        files.LocalDirectories.ShouldBe([
            preRelease, besideOldFile, _context.FileSystem.Path.Combine(dataFolder, "1.1.0.0b2")
        ]);
        files.IsEmpty.ShouldBeFalse();
        files.ServerOnly().LocalDirectories.ShouldBeEmpty();
        files.ServerOnly().ServerDirectories.Count.ShouldBe(2);

        // A folder that holds the project itself or its old file is never offered for deletion.
        var inside = _context.NewProject();
        inside.Path = _context.FileSystem.Path.Combine(dataFolder, "1.1.0.0b2", "nested", "project.nupdproj");
        inside.LegacyProjectFile = _context.FileSystem.Path.Combine(preRelease, "Legacy.nupdproj");
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(inside.Folder, "1.0.0.0", zip),
            new MockFileData("z"));
        (await _context.Migrator.FindLegacyFilesAsync(inside, secrets)).LocalDirectories.ShouldBe([
            _context.FileSystem.Path.Combine(inside.Folder, "1.0.0.0")
        ]);
        var nearOldFile = _context.NewProject();
        nearOldFile.LegacyProjectFile = _context.FileSystem.Path.Combine(dataFolder, "1.0.0.0", "Old.nupdproj");
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(dataFolder, "1.0.0.0", zip),
            new MockFileData("z"));
        (await _context.Migrator.FindLegacyFilesAsync(nearOldFile, secrets)).LocalDirectories.ShouldNotContain(
            _context.FileSystem.Path.Combine(dataFolder, "1.0.0.0"));
        var isTheFolder = _context.NewProject();
        isTheFolder.Path = _context.FileSystem.Path.Combine(dataFolder, "1.0.0.0", "project.nupdproj");
        (await _context.Migrator.FindLegacyFilesAsync(isTheFolder, secrets)).LocalDirectories.ShouldNotContain(
            _context.FileSystem.Path.Combine(dataFolder, "1.0.0.0"));

        // Without the old feed and script there is nothing to retire.
        _context.ServeLegacyFeed(null);
        _context.Transfer.FileExistsAsync("statistics.php", Arg.Any<CancellationToken>()).Returns(false);
        (await _context.Migrator.FindLegacyFilesAsync(project, secrets)).IsEmpty.ShouldBeTrue();
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Migrator.FindLegacyFilesAsync(null!, secrets));
        await Should.ThrowAsync<ArgumentNullException>(() => _context.Migrator.FindLegacyFilesAsync(project, null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("..")]
    [InlineData("...")]
    [InlineData("Demo.")]
    [InlineData("Demo ")]
    [InlineData("a/b")]
    [InlineData("a\\b")]
    [InlineData("c:")]
    public async Task FindLegacyFiles_NeverLooksOutsideTheProjectFolderOfNUpdate4(string name)
    {
        var project = _context.NewProject();
        project.Name = name;
        _context.ServeLegacyFeed(LegacyFeedJson("false", "old", "old"));
        // Where an unchecked name would lead: the data folder itself or a sibling of the project's folder.
        _context.FileSystem.AddFile(
            _context.FileSystem.Path.Combine(_context.Paths.Root, "1.0.0.0", $"{project.Id}.zip"),
            new MockFileData("z"));
        _context.FileSystem.AddFile(
            _context.FileSystem.Path.Combine(_context.Paths.LegacyProjectsDirectory, "1.0.0.0", $"{project.Id}.zip"),
            new MockFileData("z"));
        _context.FileSystem.AddFile(
            _context.FileSystem.Path.Combine(_context.Paths.LegacyProjectsDirectory, name, "1.0.0.0",
                $"{project.Id}.zip"), new MockFileData("z"));

        (await _context.Migrator.FindLegacyFilesAsync(project, AdminTestContext.NewSecrets())).LocalDirectories
            .ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteLegacyFiles_RemovesTheFeedLastSoAnInterruptedRunCanBeRepeated()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        var local = _context.FileSystem.Path.Combine(_context.Paths.Root, "Projects", "Demo");
        _context.FileSystem.AddFile(_context.FileSystem.Path.Combine(local, "1.0.0.0", "x.zip"), new MockFileData("z"));

        await _context.Migrator.DeleteLegacyFilesAsync(project, secrets,
            new LegacyFiles(["statistics.php", "updates.json"], ["1.0.0.0"], [local, "/does/not/exist"]));

        Received.InOrder(() =>
        {
            _context.Transfer.DeleteDirectoryAsync("1.0.0.0", Arg.Any<CancellationToken>());
            _context.Transfer.DeleteFileAsync("statistics.php", Arg.Any<CancellationToken>());
            _context.Transfer.DeleteFileAsync("updates.json", Arg.Any<CancellationToken>());
        });
        _context.FileSystem.Directory.Exists(local).ShouldBeFalse();

        // The connection drops before updates.json is gone: the feed is still there to list the folders for the next attempt.
        _context.Transfer.ClearReceivedCalls();
        _context.Transfer.DeleteDirectoryAsync("1.1.0.0", Arg.Any<CancellationToken>())
            .Returns(_ => throw new TransferException("dropped"));
        await Should.ThrowAsync<TransferException>(() =>
            _context.Migrator.DeleteLegacyFilesAsync(project, secrets,
                new LegacyFiles(["updates.json"], ["1.1.0.0"], [])));
        await _context.Transfer.DidNotReceive().DeleteFileAsync("updates.json", Arg.Any<CancellationToken>());

        // Only local files: no connection.
        _context.TransferFactory.ClearReceivedCalls();
        await _context.Migrator.DeleteLegacyFilesAsync(project, secrets, new LegacyFiles([], [], []));
        _context.TransferFactory.DidNotReceiveWithAnyArgs().Create(default!, default!);
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _context.Migrator.DeleteLegacyFilesAsync(null!, secrets, new LegacyFiles([], [], [])));
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _context.Migrator.DeleteLegacyFilesAsync(project, null!, new LegacyFiles([], [], [])));
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _context.Migrator.DeleteLegacyFilesAsync(project, secrets, null!));
        Should.Throw<ArgumentNullException>(() => new LegacyFiles(null!, [], []));
        Should.Throw<ArgumentNullException>(() => new LegacyFiles([], null!, []));
        Should.Throw<ArgumentNullException>(() => new LegacyFiles([], [], null!));
    }

    [Fact]
    public void Constructor_ValidatesArguments()
    {
        var c = _context;
        Should.Throw<ArgumentNullException>(() => new LegacyFeedMigrator(null!, c.Paths, c.HttpClientFactory, c.Feeds,
            c.Signer, c.TransferFactory, c.Statistics, c.Store, c.Logger));
        Should.Throw<ArgumentNullException>(() => new LegacyFeedMigrator(c.FileSystem, null!, c.HttpClientFactory,
            c.Feeds, c.Signer, c.TransferFactory, c.Statistics, c.Store, c.Logger));
        Should.Throw<ArgumentNullException>(() => new LegacyFeedMigrator(c.FileSystem, c.Paths, null!, c.Feeds,
            c.Signer, c.TransferFactory, c.Statistics, c.Store, c.Logger));
        Should.Throw<ArgumentNullException>(() => new LegacyFeedMigrator(c.FileSystem, c.Paths, c.HttpClientFactory,
            null!, c.Signer, c.TransferFactory, c.Statistics, c.Store, c.Logger));
        Should.Throw<ArgumentNullException>(() => new LegacyFeedMigrator(c.FileSystem, c.Paths, c.HttpClientFactory,
            c.Feeds, null!, c.TransferFactory, c.Statistics, c.Store, c.Logger));
        Should.Throw<ArgumentNullException>(() => new LegacyFeedMigrator(c.FileSystem, c.Paths, c.HttpClientFactory,
            c.Feeds, c.Signer, null!, c.Statistics, c.Store, c.Logger));
        Should.Throw<ArgumentNullException>(() => new LegacyFeedMigrator(c.FileSystem, c.Paths, c.HttpClientFactory,
            c.Feeds, c.Signer, c.TransferFactory, null!, c.Store, c.Logger));
        Should.Throw<ArgumentNullException>(() => new LegacyFeedMigrator(c.FileSystem, c.Paths, c.HttpClientFactory,
            c.Feeds, c.Signer, c.TransferFactory, c.Statistics, null!, c.Logger));
        Should.Throw<ArgumentNullException>(() => new LegacyFeedMigrator(c.FileSystem, c.Paths, c.HttpClientFactory,
            c.Feeds, c.Signer, c.TransferFactory, c.Statistics, c.Store, null!));
        Should.Throw<ArgumentNullException>(() => new LegacyFeedMigrator(c.FileSystem, c.Paths, c.HttpClientFactory,
            c.Feeds, c.Signer, c.TransferFactory, c.Statistics, c.Store, c.Logger, null!));
        new LegacyFeedMigrator(c.FileSystem, c.Paths, c.HttpClientFactory, c.Feeds, c.Signer, c.TransferFactory,
            c.Statistics, c.Store, c.Logger).ShouldNotBeNull();
        new MigrationStatus(false, false).NeedsMigration.ShouldBeFalse();
    }
}
