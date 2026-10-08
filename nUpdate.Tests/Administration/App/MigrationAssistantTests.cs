using Avalonia.Headless.XUnit;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Operations;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.App;

public sealed class MigrationAssistantTests
{
    private readonly AppTestContext _context = new();
    private bool _cleanedUp;

    private static LegacyFeedEntry Entry(string literal, bool statistics = false) =>
        new(LegacyVersion.Parse(literal), literal) { UseStatistics = statistics, PackageUri = new Uri($"https://updates.example.com/demo/{literal}/p.zip") };

    private static MigrationPackage Ready(string literal, bool statistics = false, string? source = null, IReadOnlyList<string>? skipped = null, IReadOnlyList<string>? warnings = null) =>
        MigrationPackage.Ready(Entry(literal, statistics), source ?? $"https://updates.example.com/demo/{literal}/p.zip", "/tmp/p.zip", 2048, 3, skipped ?? [],
            new LegacyOperationConversion([new TerminateProcessOperation { ProcessName = "app" }], warnings ?? []));

    /// <summary>A plan with a migrated, a ready and a broken package, as the migrator would prepare it.</summary>
    private MigrationPlan Plan(UpdateProject project, UpdateFeed? feed = null, params MigrationPackage[] packages) =>
        new(project.Id, true, feed, packages.Length > 0 ? packages : [MigrationPackage.Migrated(Entry("0.9.0.0")), Ready("1.0.0.0", statistics: true), MigrationPackage.Failed(Entry("1.1.0.0b2"), "https://updates.example.com/demo/1.1.0.0b2/p.zip", "404")],
            () => _cleanedUp = true);

    private async Task<MigrationViewModel> AssistantAsync(UpdateProject project, ProjectSecrets secrets, MigrationPlan plan)
    {
        _context.Migrator.PrepareAsync(project, secrets, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>()).Returns(plan);
        var assistant = _context.Factory.Create<MigrationViewModel>(project, secrets);
        await assistant.InitializeAsync();
        return assistant;
    }

    [Fact]
    public async Task Assistant_ExplainsWhatChangesAndWhatStays()
    {
        var project = AppTestContext.NewProject();
        project.LegacyProjectFile = "/old/Demo.nupdproj";
        var assistant = await AssistantAsync(project, AppTestContext.NewSecrets(), Plan(project));

        assistant.Title.ShouldBe("Move Demo to nUpdate 5");
        assistant.IsPrepared.ShouldBeTrue();
        assistant.IsOverviewStep.ShouldBeTrue();
        assistant.ServerState.ShouldBe([
            "updates.json with 3 packages, which applications built with nUpdate 3 or 4 read.",
            "No nupdate.json yet.",
            "Packages of updates.json report their downloads to statistics.php.",
        ]);
        assistant.AddedItems.Count.ShouldBe(3); // packages, feed, local copies; no statistics for this project
        assistant.AddedItems[0].ShouldStartWith("packages/<version>/<platform>.zip for every package you select");
        assistant.AddedItems[1].ShouldContain(project.FeedUri.ToString());
        assistant.KeptItems.ShouldBe([
            "updates.json and the package folders 0.9.0.0/, 1.0.0.0/, 1.1.0.0b2/: installed copies of your application that still run nUpdate 3 or 4 keep updating from them.",
            "statistics.php and its tables: those copies keep reporting their downloads.",
            "Your project file of nUpdate Administration 4 (/old/Demo.nupdproj) and its package copies, so you can keep publishing to updates.json with it in the meantime.",
        ]);

        // A project with statistics, an existing feed, no statistics in the old feed and no remembered project file.
        var other = AppTestContext.NewProject(statistics: true);
        var feed = new UpdateFeed { ProjectId = other.Id, Packages = [new PackageInfo { Version = new UpdateVersion("0.9.0") }] };
        var second = await AssistantAsync(other, AppTestContext.NewSecrets(statistics: true), Plan(other, feed, Ready("1.0.0.0")));
        second.ServerState.ShouldBe(["updates.json with 1 package, which applications built with nUpdate 3 or 4 read.", "nupdate.json with 1 package, which applications built with nUpdate 5 read."]);
        second.AddedItems.ShouldContain(i => i.StartsWith("nupdate-statistics.php and nupdate-statistics.config.php", StringComparison.Ordinal));
        second.KeptItems.Count.ShouldBe(2);
        second.Packages.Single().Include.ShouldBeTrue();
        second.MigrationSummary.ShouldContain("nupdate.json is written with 2 packages.");
        second.MigrationSummary.ShouldContain("nupdate-statistics.php is uploaded and the versions are registered in it.");
        var both = await AssistantAsync(other, AppTestContext.NewSecrets(statistics: true), Plan(other, null, Ready("1.0.0.0"), Ready("1.1.0.0")));
        both.MigrationSummary[0].ShouldBe("2 packages (1.0.0, 1.1.0) are repacked, signed and uploaded to packages/.");
        both.Packages[0].Include = false;
        both.Packages[1].Include = false;
        both.MigrationSummary.ShouldContain("nupdate-statistics.php is uploaded.");
        second.KeptItems[1].ShouldStartWith("The project and the package copies of nUpdate Administration 4");

        var empty = await AssistantAsync(other, AppTestContext.NewSecrets(statistics: true), new MigrationPlan(other.Id, false, null, []));
        empty.ServerState[0].ShouldStartWith("No updates.json");
        empty.KeptItems[0].ShouldContain("if there are any");
        empty.SelectionSummary.ShouldBe("updates.json lists no packages.");
    }

    [Fact]
    public async Task Assistant_ListsThePackagesAndLetsTheUserChoose()
    {
        var project = AppTestContext.NewProject();
        var assistant = await AssistantAsync(project, AppTestContext.NewSecrets(), Plan(project, null,
            MigrationPackage.Migrated(Entry("0.9.0.0")),
            Ready("1.0.0.0", source: "/data/Projects/Demo/1.0.0.0/p.zip", skipped: ["notes.txt"], warnings: ["Operation 2 is left out."]),
            Ready("1.1.0.0b2"),
            MigrationPackage.Failed(Entry("1.2.0.0"), null, "The package is neither on this computer nor named in updates.json.")));
        await assistant.ContinueCommand.ExecuteAsync(null);
        assistant.IsPackagesStep.ShouldBeTrue();

        var packages = assistant.Packages;
        packages.Select(p => p.Title).ShouldBe(["0.9.0.0 → 0.9.0", "1.0.0.0 → 1.0.0", "1.1.0.0b2 → 1.1.0-beta.2", "1.2.0.0 → 1.2.0"]);
        packages[0].AlreadyMigrated.ShouldBeTrue();
        packages[0].Details.ShouldBe("Already in nupdate.json.");
        packages[0].HasNotes.ShouldBeFalse();
        packages[1].Details.ShouldBe("3 files, 1 operation, 2.00 KB, from this computer (/data/Projects/Demo/1.0.0.0/p.zip).");
        packages[1].Notes.ShouldBe(["Operation 2 is left out.", "Left out because they are outside the folders the installer knows: notes.txt."]);
        packages[2].Details.ShouldStartWith("3 files, 1 operation, 2.00 KB, downloaded from https://");
        packages[3].Details.ShouldBe("Cannot be migrated.");
        packages[3].Notes.Single().ShouldContain("neither on this computer");
        packages[3].CanInclude.ShouldBeFalse();
        assistant.SelectionSummary.ShouldBe("2 of 3 packages to migrate selected.");

        var changes = new List<string?>();
        packages[1].PropertyChanged += (_, e) => changes.Add(e.PropertyName);
        packages[1].Include = false;
        packages[1].Include = false;
        changes.ShouldBe([nameof(MigrationPackageItemViewModel.Include)]);
        packages[3].Include = true;
        packages[3].Include.ShouldBeFalse();
        assistant.SelectionSummary.ShouldBe("1 of 3 packages to migrate selected.");
        new MigrationPackageItemViewModel(MigrationPackage.Failed(Entry("2.0.0.0"), "https://x/p.zip", "gone")).Details.ShouldBe("Cannot be migrated from https://x/p.zip.");
        Should.Throw<ArgumentNullException>(() => new MigrationPackageItemViewModel(null!));
    }

    [Fact]
    public async Task Assistant_ChecksTheStatisticsBeforeTheMigration()
    {
        var project = AppTestContext.NewProject(statistics: true);
        project.Statistics.Database = null;
        var secrets = AppTestContext.NewSecrets(statistics: true);
        secrets.StatisticsDatabasePassword = null;
        var assistant = await AssistantAsync(project, secrets, Plan(project));
        assistant.Step = MigrationViewModel.StatisticsStep;

        assistant.StatisticsEnabled.ShouldBeTrue();
        assistant.StatisticsDetails.ShouldBe([
            "Endpoint: https://updates.example.com/demo/nupdate-statistics.php",
            "Database: not set",
            "Database password: missing",
            "Admin secret: present",
        ]);
        await assistant.ContinueCommand.ExecuteAsync(null);
        assistant.ErrorMessage!.ShouldContain("database settings");
        assistant.IsStatisticsStep.ShouldBeTrue();

        project.Statistics.Database = new StatisticsDatabaseSettings { Host = "db", Name = "stats", Username = "u" };
        await assistant.ContinueCommand.ExecuteAsync(null);
        assistant.ErrorMessage!.ShouldContain("database password");

        secrets.StatisticsDatabasePassword = "pw";
        secrets.StatisticsAdminSecret = null;
        assistant.StatisticsDetails.ShouldContain("Admin secret: missing");
        assistant.StatisticsDetails.ShouldContain("Database: stats on db, user u");
        await assistant.ContinueCommand.ExecuteAsync(null);
        assistant.ErrorMessage!.ShouldContain("admin secret");

        secrets.StatisticsAdminSecret = "s";
        assistant.StatisticsProblem.ShouldBeNull();
        await assistant.ContinueCommand.ExecuteAsync(null);
        assistant.IsMigrateStep.ShouldBeTrue();
        assistant.ErrorMessage.ShouldBeNull();

        var withoutStatistics = await AssistantAsync(AppTestContext.NewProject(), AppTestContext.NewSecrets(), Plan(project));
        withoutStatistics.StatisticsDetails.ShouldBeEmpty();
        withoutStatistics.StatisticsProblem.ShouldBeNull();
    }

    [Fact]
    public async Task Assistant_SummarisesAndRunsTheMigration()
    {
        var project = AppTestContext.NewProject(statistics: true);
        var secrets = AppTestContext.NewSecrets(statistics: true);
        var plan = Plan(project);
        var assistant = await AssistantAsync(project, secrets, plan);
        assistant.BackCommand.CanExecute(null).ShouldBeFalse();
        await assistant.ContinueCommand.ExecuteAsync(null);
        await assistant.ContinueCommand.ExecuteAsync(null);
        await assistant.ContinueCommand.ExecuteAsync(null);
        assistant.IsMigrateStep.ShouldBeTrue();
        assistant.BackCommand.CanExecute(null).ShouldBeTrue();
        assistant.ContinueText.ShouldBe("Start migration");
        assistant.MigrationSummary.ShouldBe([
            "1 package (1.0.0) is repacked, signed and uploaded to packages/.",
            "nupdate.json is written with 1 package.",
            "nupdate-statistics.php is uploaded and the versions are registered in it.",
            "The project is saved with the packages marked as released.",
            "Nothing of nUpdate 3 and 4 is changed. If a step fails, what this migration uploaded is removed again.",
        ]);

        // A failure keeps the assistant on this step and explains it.
        _context.Migrator.RunAsync(project, secrets, plan, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<IReadOnlyList<UpdateVersion>>(new PipelineException("Uploading 1.0.0", new TransferException("quota"), [])), _ => Task.FromResult<IReadOnlyList<UpdateVersion>>([new UpdateVersion("1.0.0")]));
        await assistant.ContinueCommand.ExecuteAsync(null);
        assistant.IsMigrateStep.ShouldBeTrue();
        assistant.ErrorMessage!.ShouldContain("quota");
        assistant.Migrated.ShouldBeFalse();

        await assistant.ContinueCommand.ExecuteAsync(null);
        assistant.Migrated.ShouldBeTrue();
        assistant.MigratedVersions.ShouldBe([new UpdateVersion("1.0.0")]);
        assistant.IsSideBySideStep.ShouldBeTrue();
        assistant.Steps.Take(4).ShouldAllBe(s => s.IsDone);
        assistant.BackCommand.CanExecute(null).ShouldBeFalse();
        assistant.ContinueText.ShouldBe("Done");
        assistant.SideBySideIntroduction.ShouldStartWith("The migration is done. 1 package of updates.json is not in nupdate.json, so applications built with nUpdate 5 do not see it;");
        assistant.CanReload.ShouldBeFalse();
        assistant.IsMigrating.ShouldBeFalse();

        var closed = new List<bool>();
        assistant.CloseRequested += (_, accepted) => closed.Add(accepted);
        await assistant.ContinueCommand.ExecuteAsync(null);
        closed.ShouldBe([true]);
        _cleanedUp.ShouldBeTrue();
    }

    [Fact]
    public async Task Assistant_StartsAnEmptyFeedOrSkipsTheRunWhenNothingIsSelected()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        var assistant = await AssistantAsync(project, secrets, Plan(project));
        assistant.Packages[1].Include = false;
        assistant.Step = MigrationViewModel.MigrateStep;
        assistant.NothingToMigrate.ShouldBeFalse();
        assistant.MigrationSummary[0].ShouldContain("nupdate.json starts empty");
        assistant.MigrationSummary.ShouldNotContain(l => l.Contains("project is saved", StringComparison.Ordinal));

        var feed = new UpdateFeed { ProjectId = project.Id };
        var withFeed = await AssistantAsync(project, secrets, Plan(project, feed, Ready("1.0.0.0")));
        withFeed.Packages[0].Include = false;
        withFeed.Step = MigrationViewModel.MigrateStep;
        withFeed.NothingToMigrate.ShouldBeTrue();
        withFeed.ContinueText.ShouldBe("Continue");
        withFeed.MigrationSummary.Single().ShouldContain("nothing to do");
        await withFeed.ContinueCommand.ExecuteAsync(null);
        withFeed.IsSideBySideStep.ShouldBeTrue();
        withFeed.Migrated.ShouldBeFalse();
        await _context.Migrator.DidNotReceiveWithAnyArgs().RunAsync(default!, default!, default!, default, default);
    }

    [Fact]
    public async Task Assistant_OffersToTrustAnUnknownServerDuringTheMigration()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        var plan = Plan(project);
        var assistant = await AssistantAsync(project, secrets, plan);
        assistant.Step = MigrationViewModel.MigrateStep;
        _context.Migrator.RunAsync(project, secrets, plan, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<IReadOnlyList<UpdateVersion>>(new PipelineException("Uploading 1.0.0", new UntrustedServerException("unknown certificate", "fp", "CN=ftp"), [])), _ => Task.FromResult<IReadOnlyList<UpdateVersion>>([]));
        _context.Dialogs.ConfirmAsync("Unknown certificate", Arg.Any<string>(), "Trust", Arg.Any<string>()).Returns(true);

        await assistant.ContinueCommand.ExecuteAsync(null);

        project.Transfer.TrustedCertificateFingerprint.ShouldBe("fp");
        assistant.Migrated.ShouldBeTrue();
    }

    [Fact]
    public async Task Assistant_OpensAtTheGuideWhenNothingIsLeftToMigrate()
    {
        var project = AppTestContext.NewProject();
        var feed = new UpdateFeed { ProjectId = project.Id };
        var assistant = await AssistantAsync(project, AppTestContext.NewSecrets(), Plan(project, feed, MigrationPackage.Migrated(Entry("1.0.0.0b3"))));

        assistant.IsSideBySideStep.ShouldBeTrue();
        assistant.SelectionSummary.ShouldBe("Every package of updates.json is in nupdate.json already.");
        assistant.SideBySideIntroduction.ShouldStartWith("Every package of updates.json is in nupdate.json. Follow these steps");
        assistant.ClientSnippet.ShouldContain(project.FeedUri.ToString());
        // The build that moves the users over declares a version above every old package, never the installed one.
        assistant.VersionExample.ShouldContain("ApplicationVersion(\"1.1.0\")");
        assistant.VersionExample.ShouldContain("what nUpdate Administration 4 calls 1.1.0.0");
        assistant.VersionExample.ShouldContain("higher than 1.0.0.0b3");
        assistant.BridgeRelease.ShouldContain("(1.1.0.0 for 1.1.0)");
        assistant.BridgeRelease.ShouldContain("Publish the build from step 2 once with nUpdate Administration 4,");
        assistant.KeepOldFiles.ShouldContain(".nupdproj");
        project.LegacyProjectFile = "/old/Demo.nupdproj";
        assistant.BridgeRelease.ShouldContain("the project file /old/Demo.nupdproj");
        assistant.KeepOldFiles.ShouldContain("statistics.php");
        assistant.Retirement.ShouldContain("Retire the nUpdate 4 setup");

        await assistant.CopySnippetCommand.ExecuteAsync(null);
        await _context.Clipboard.Received().SetTextAsync(assistant.ClientSnippet);

        var empty = await AssistantAsync(project, AppTestContext.NewSecrets(), new MigrationPlan(project.Id, false, feed, []));
        empty.VersionExample.ShouldContain("ApplicationVersion(\"1.2.0\")");
        empty.VersionExample.ShouldNotContain("higher than");
        empty.BridgeRelease.ShouldContain("(1.2.0.0 for 1.2.0)");
    }

    [Fact]
    public async Task Assistant_ChecksTheNewFeedLikeAClient()
    {
        var project = AppTestContext.NewProject(statistics: true);
        var secrets = AppTestContext.NewSecrets(statistics: true);
        var assistant = await AssistantAsync(project, secrets, Plan(project));
        assistant.CheckStatus.ShouldBe("Not checked yet.");
        var uri = new Uri("https://updates.example.com/demo/packages/1.0.0/win.zip");
        _context.FeedChecker.CheckAsync(project, secrets, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>()).Returns(
            new FeedCheckResult(null, [new PackageCheck(new UpdateVersion("1.0.0"), "win", uri, null)], true, null),
            new FeedCheckResult("There is no nupdate.json.", [new PackageCheck(new UpdateVersion("1.1.0"), "win-x64", uri, "The SHA-512 hash does not match the feed.")], true, "no PATH_INFO"),
            new FeedCheckResult(null, [], false, null));

        await assistant.CheckFeedCommand.ExecuteAsync(null);
        assistant.CheckSucceeded.ShouldBe(true);
        assistant.CheckStatus.ShouldBe("An application built with nUpdate 5 can update from this server.");
        assistant.CheckResults.ShouldBe(["✓ 1.0.0 (win): downloaded, size, hash, signature and manifest are fine.", "✓ nupdate-statistics.php answers."]);

        await assistant.CheckFeedCommand.ExecuteAsync(null);
        assistant.CheckSucceeded.ShouldBe(false);
        assistant.CheckStatus.ShouldContain("would run into the problems");
        assistant.CheckResults.ShouldBe(["✗ There is no nupdate.json.", "✗ 1.1.0 (win-x64): The SHA-512 hash does not match the feed.", "✗ nupdate-statistics.php: no PATH_INFO"]);

        await assistant.CheckFeedCommand.ExecuteAsync(null);
        assistant.CheckResults.ShouldBeEmpty();

        _context.FeedChecker.CheckAsync(project, secrets, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException("broken"));
        await assistant.CheckFeedCommand.ExecuteAsync(null);
        assistant.CheckSucceeded.ShouldBe(false);
        assistant.CheckStatus.ShouldBe("broken");
    }

    [Fact]
    public async Task Assistant_ReportsAServerItCannotReadAndTriesAgain()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        _context.Migrator.PrepareAsync(project, secrets, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<MigrationPlan>(new HttpRequestException("offline")), _ => Task.FromResult(Plan(project)));
        var assistant = _context.Factory.Create<MigrationViewModel>(project, secrets);
        assistant.ServerState.ShouldBeEmpty();
        assistant.MigrationSummary.ShouldBeEmpty();
        assistant.NothingToMigrate.ShouldBeFalse();

        await assistant.InitializeAsync();
        assistant.IsPrepared.ShouldBeFalse();
        assistant.ErrorMessage.ShouldBe("The server could not be read: offline");
        await assistant.ContinueCommand.ExecuteAsync(null);
        assistant.ErrorMessage!.ShouldContain("Reload");
        assistant.IsOverviewStep.ShouldBeTrue();

        await assistant.ReloadCommand.ExecuteAsync(null);
        assistant.IsPrepared.ShouldBeTrue();
        assistant.ErrorMessage.ShouldBeNull();
        await assistant.InitializeAsync(); // once read, the plan stays
        await _context.Migrator.Received(2).PrepareAsync(project, secrets, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>());

        await assistant.ContinueCommand.ExecuteAsync(null);
        assistant.BackCommand.Execute(null);
        assistant.IsOverviewStep.ShouldBeTrue();
        assistant.Dispose();
        _cleanedUp.ShouldBeTrue();

        _context.Factory.Create<MigrationViewModel>(project, secrets).Dispose(); // nothing prepared, nothing to delete
        var c = _context;
        Should.Throw<ArgumentNullException>(() => new MigrationViewModel(null!, c.FeedChecker, c.Dialogs, c.Clipboard, project, secrets));
        Should.Throw<ArgumentNullException>(() => new MigrationViewModel(c.Migrator, null!, c.Dialogs, c.Clipboard, project, secrets));
        Should.Throw<ArgumentNullException>(() => new MigrationViewModel(c.Migrator, c.FeedChecker, null!, c.Clipboard, project, secrets));
        Should.Throw<ArgumentNullException>(() => new MigrationViewModel(c.Migrator, c.FeedChecker, c.Dialogs, null!, project, secrets));
        Should.Throw<ArgumentNullException>(() => new MigrationViewModel(c.Migrator, c.FeedChecker, c.Dialogs, c.Clipboard, null!, secrets));
        Should.Throw<ArgumentNullException>(() => new MigrationViewModel(c.Migrator, c.FeedChecker, c.Dialogs, c.Clipboard, project, null!));
    }

    [Fact]
    public async Task Assistant_ReadsTheServerAgainOnRequestAndDropsAPlanThatArrivesAfterClosing()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        var cleaned = new List<string>();
        MigrationPlan NewPlan(string name, params string[] unreadable) => new(project.Id, true, null, [Ready("1.0.0.0")], () => cleaned.Add(name), unreadable);
        _context.Migrator.PrepareAsync(project, secrets, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>())
            .Returns(NewPlan("first"), NewPlan("second", "garbage"));
        var assistant = _context.Factory.Create<MigrationViewModel>(project, secrets);
        await assistant.InitializeAsync();
        assistant.Packages[0].Include = false;
        assistant.CanReload.ShouldBeTrue();

        await assistant.ReloadCommand.ExecuteAsync(null);

        cleaned.ShouldBe(["first"]);
        assistant.Packages.Single().Include.ShouldBeTrue();
        assistant.ServerState.ShouldContain("updates.json has entries whose version nUpdate 5 cannot read; they are left out: garbage.");

        // Closed while the server is read: the plan that comes in afterwards is deleted at once.
        var pending = new TaskCompletionSource<MigrationPlan>();
        _context.Migrator.PrepareAsync(project, secrets, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>()).Returns(pending.Task);
        var closing = _context.Factory.Create<MigrationViewModel>(project, secrets);
        var reading = closing.InitializeAsync();
        closing.Dispose();
        pending.SetResult(NewPlan("late"));
        await reading;
        cleaned.ShouldBe(["first", "late"]);
        closing.IsPrepared.ShouldBeFalse();
        await closing.InitializeAsync();
        await _context.Migrator.Received(3).PrepareAsync(project, secrets, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>());
    }

    [AvaloniaFact]
    public async Task MigrationWindow_StaysOpenWhileTheMigrationRuns()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        var plan = Plan(project);
        _context.Migrator.PrepareAsync(project, secrets, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>()).Returns(plan);
        var running = new TaskCompletionSource<IReadOnlyList<UpdateVersion>>();
        _context.Migrator.RunAsync(project, secrets, plan, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>()).Returns(running.Task);
        var assistant = _context.Factory.Create<MigrationViewModel>(project, secrets);
        var window = new MigrationWindow { DataContext = assistant };
        window.Show();
        await Task.Yield();
        assistant.Step = MigrationViewModel.MigrateStep;

        var migrating = assistant.ContinueCommand.ExecuteAsync(null);
        assistant.IsMigrating.ShouldBeTrue();
        window.Close();
        window.IsVisible.ShouldBeTrue();
        _cleanedUp.ShouldBeFalse();

        running.SetResult([new UpdateVersion("1.0.0")]);
        await migrating;
        assistant.IsMigrating.ShouldBeFalse();
        window.Close();
        window.IsVisible.ShouldBeFalse();
        _cleanedUp.ShouldBeTrue();
    }

    [AvaloniaFact]
    public async Task MigrationWindow_ShowsEveryStepAndPreparesWhenOpened()
    {
        var project = AppTestContext.NewProject(statistics: true);
        var secrets = AppTestContext.NewSecrets(statistics: true);
        _context.Migrator.PrepareAsync(project, secrets, Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>()).Returns(Plan(project));
        var assistant = _context.Factory.Create<MigrationViewModel>(project, secrets);
        var window = new MigrationWindow { DataContext = assistant };

        window.Show();
        await Task.Yield();

        assistant.IsPrepared.ShouldBeTrue();
        window.ServerStateList.ItemCount.ShouldBe(3);
        window.ReloadButton.IsVisible.ShouldBeTrue();
        foreach (var step in new[] { MigrationViewModel.PackagesStep, MigrationViewModel.StatisticsStep, MigrationViewModel.MigrateStep, MigrationViewModel.SideBySideStep })
        {
            assistant.Step = step;
            window.UpdateLayout();
        }

        window.PackageList.ItemCount.ShouldBe(3);
        window.ClientSnippetText.Text.ShouldBe(assistant.ClientSnippet);
        window.Close();
        _cleanedUp.ShouldBeTrue();

        // A window without its view model opens and closes quietly; the rail shows the steps it is given.
        var bare = new MigrationWindow();
        bare.Show();
        bare.Close();
        new nUpdate.Administration.Views.Controls.WizardRail { Steps = assistant.Steps }.Steps.ShouldBe(assistant.Steps);
    }
}
