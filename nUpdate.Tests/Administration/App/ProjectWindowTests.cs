using System.IO.Abstractions.TestingHelpers;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Exceptions;
using nUpdate.Packaging;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.App;

/// <summary>The project window: package list and commands, overview data, statistics, history, the legacy migration and the dialogs it opens.</summary>
public class ProjectWindowTests
{
    private readonly AppTestContext _context = new();

    private static UpdatePackage Package(string version, bool released, string description = "",
        DateTimeOffset? createdAt = null) =>
        new()
        {
            Version = new UpdateVersion(version),
            Released = released,
            Description = description,
            CreatedAt = createdAt ?? new DateTimeOffset(2026, 10, 1, 12, 0, 0, TimeSpan.Zero)
        };

    [Fact]
    public async Task ProjectViewModel_ListsPackagesAndRunsCommands()
    {
        var project = AppTestContext.NewProject(statistics: true);
        project.Packages.Add(Package("1.0.0", true, "first"));
        project.Packages.Add(Package("1.1.0", false));
        project.Log.Add(new LogEntry { Kind = LogEntryKind.Upload, Version = new UpdateVersion("1.0.0"), User = "u" });
        project.Log.Add(new LogEntry { Kind = LogEntryKind.Create, At = DateTimeOffset.MinValue });
        var secrets = AppTestContext.NewSecrets(statistics: true);
        var viewModel = _context.Factory.Create<ProjectViewModel>(project, secrets);

        viewModel.Title.ShouldBe("Demo - nUpdate Administration");
        viewModel.Packages.Select(p => p.Version).ShouldBe(["1.1.0", "1.0.0"]);
        viewModel.Packages[1].State.ShouldBe("Released");
        viewModel.Packages[0].State.ShouldBe("Local only");
        viewModel.Packages[1].Description.ShouldBe("first");
        viewModel.Packages[0].Created.ShouldNotBe("-");
        new PackageItemViewModel(new UpdatePackage { CreatedAt = DateTimeOffset.MinValue }).Created.ShouldBe("-");
        viewModel.History.Count.ShouldBe(2);
        viewModel.History[0].Kind.ShouldBe("Upload");
        viewModel.History[0].Version.ShouldBe("1.0.0");
        viewModel.History[0].User.ShouldBe("u");
        viewModel.History[0].Time.ShouldNotBe("-");
        viewModel.History[1].Time.ShouldBe("-");
        viewModel.History[1].Version.ShouldBe("-");
        viewModel.FeedUrl.ShouldBe("https://updates.example.com/demo/nupdate.json");
        viewModel.Folder.ShouldBe(project.Folder);
        viewModel.PublicKey.ShouldBe(project.PublicKey);
        viewModel.ProjectId.ShouldBe(project.Id.ToString());
        viewModel.TransferSummary.ShouldBe("Ftp user@ftp.example.com:21/demo");
        viewModel.StatisticsEnabled.ShouldBeTrue();
        viewModel.HasSelectedPackage.ShouldBeFalse();
        viewModel.EditPackageCommand.CanExecute(null).ShouldBeFalse();
        viewModel.MigrationText.ShouldBe("Not checked yet.");

        await viewModel.CopyPublicKeyCommand.ExecuteAsync(null);
        await viewModel.CopyFeedUrlCommand.ExecuteAsync(null);
        await _context.Clipboard.Received().SetTextAsync(project.PublicKey);
        await _context.Clipboard.Received().SetTextAsync(viewModel.FeedUrl);

        viewModel.SelectedPackage = viewModel.Packages[0];
        viewModel.HasSelectedPackage.ShouldBeTrue();
        viewModel.CanPublishSelected.ShouldBeTrue();
        _context.Dialogs.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(false);
        await viewModel.PublishPackageCommand.ExecuteAsync(null);
        await _context.Publisher.DidNotReceive().PublishExistingAsync(Arg.Any<UpdateProject>(),
            Arg.Any<ProjectSecrets>(), Arg.Any<UpdateVersion>(), Arg.Any<IProgress<PipelineProgress>>(),
            Arg.Any<CancellationToken>());
        _context.Dialogs.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);
        await viewModel.PublishPackageCommand.ExecuteAsync(null);
        await _context.Publisher.Received().PublishExistingAsync(project, secrets, new UpdateVersion("1.1.0"),
            Arg.Any<IProgress<PipelineProgress>>(), Arg.Any<CancellationToken>());
        viewModel.SelectedPackage.ShouldNotBeNull();

        _context.Publisher.PublishExistingAsync(Arg.Any<UpdateProject>(), Arg.Any<ProjectSecrets>(),
                Arg.Any<UpdateVersion>(), Arg.Any<IProgress<PipelineProgress>>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new PipelineException("Loading the current feed", new MigrationRequiredException(), []));
        await viewModel.PublishPackageCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowErrorAsync("Error while publishing the package",
            Arg.Is<string>(m => m.Contains("Migrate")));

        viewModel.SelectedPackage = viewModel.Packages[1];
        viewModel.CanPublishSelected.ShouldBeFalse();
        await viewModel.DeletePackageCommand.ExecuteAsync(null);
        await _context.Publisher.Received().DeletePackageAsync(project, secrets, new UpdateVersion("1.0.0"),
            Arg.Any<IProgress<PipelineProgress>>(), Arg.Any<CancellationToken>());
        viewModel.SelectedPackage = viewModel.Packages[0];
        _context.Publisher.DeletePackageAsync(Arg.Any<UpdateProject>(), Arg.Any<ProjectSecrets>(),
                Arg.Any<UpdateVersion>(), Arg.Any<IProgress<PipelineProgress>>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new PipelineException("Deleting", new IOException("io"), []));
        await viewModel.DeletePackageCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowErrorAsync("Error while deleting the package", Arg.Any<string>());
        _context.Dialogs.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(false);
        _context.Publisher.ClearReceivedCalls();
        await viewModel.DeletePackageCommand.ExecuteAsync(null);
        await _context.Publisher.DidNotReceiveWithAnyArgs()
            .DeletePackageAsync(default!, default!, default!, default, default);
    }

    [Fact]
    public async Task Project_SearchesPackagesAndSummarisesThem()
    {
        var project = AppTestContext.NewProject();
        var viewModel = _context.Factory.Create<ProjectViewModel>(project, AppTestContext.NewSecrets());
        viewModel.PackagesSummary.ShouldBe("0 packages released");
        viewModel.PackageCountText.ShouldBe("0 packages");

        project.Packages.Add(Package("1.0.0", true, "First release",
            new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero)));
        project.Packages.Add(Package("1.1.0", true, "Dark mode",
            new DateTimeOffset(2026, 10, 1, 12, 0, 0, TimeSpan.Zero)));
        project.Packages.Add(Package("1.2.0-beta.1", false, "Beta"));
        viewModel.Refresh();
        viewModel.Packages.Count.ShouldBe(3);
        viewModel.PackageCountText.ShouldBe("3 packages");
        viewModel.PackagesSummary.ShouldStartWith("2 packages released · newest 1.1.0 on ");

        viewModel.SearchText = "dark";
        viewModel.Packages.Select(p => p.Version).ShouldBe(["1.1.0"]);
        viewModel.SearchText = "1.";
        viewModel.Packages.Count.ShouldBe(3);
        viewModel.SearchText = "beta";
        viewModel.Packages.Select(p => p.Version).ShouldBe(["1.2.0-beta.1"]);
        viewModel.SearchText = "nothing";
        viewModel.Packages.ShouldBeEmpty();
        viewModel.SearchText = "";
        viewModel.Packages.Count.ShouldBe(3);

        project.Packages.Clear();
        project.Packages.Add(Package("1.0.0", true));
        viewModel.Refresh();
        viewModel.PackageCountText.ShouldBe("1 package");
        viewModel.PackagesSummary.ShouldStartWith("1 package released · newest 1.0.0 on ");

        viewModel.SourceLanguages.ShouldBe(["C#", "Visual Basic"]);
        viewModel.SelectedSourceLanguage.ShouldBe("C#");
        viewModel.SourceSnippet.ShouldStartWith("var manager = new UpdateManager(");
        viewModel.SourceSnippet.ShouldContain(project.PublicKey);
        viewModel.SourceSnippetPreview.ShouldNotContain(project.PublicKey);
        viewModel.SourceSnippetPreview.ShouldContain("…");
        viewModel.SelectedSourceLanguage = "Visual Basic";
        viewModel.SourceSnippet.ShouldStartWith("Dim manager As New UpdateManager(");
        await viewModel.CopySourceCommand.ExecuteAsync(null);
        await _context.Clipboard.Received().SetTextAsync(viewModel.SourceSnippet);

        project.PublicKey = "short";
        viewModel.SourceSnippetPreview.ShouldBe(viewModel.SourceSnippet);
    }

    [Fact]
    public async Task ProjectViewModel_EditsPackagesAndOpensDialogs()
    {
        var project = AppTestContext.NewProject();
        project.Packages.Add(Package("1.0.0", true));
        var viewModel = _context.Factory.Create<ProjectViewModel>(project, AppTestContext.NewSecrets());
        viewModel.SelectedPackage = viewModel.Packages[0];

        _context.Feeds.LoadEntryAsync(project, Arg.Any<UpdateVersion>(), Arg.Any<CancellationToken>())
            .Returns((PackageInfo?)null);
        await viewModel.EditPackageCommand.ExecuteAsync(null);
        await _context.Dialogs.Received()
            .ShowErrorAsync("Error while opening the package", Arg.Is<string>(m => m.Contains("missing")));
        await _context.Dialogs.DidNotReceive().ShowDialogAsync(Arg.Any<PackageEditorViewModel>());

        // The package is extracted into a temporary folder, which is deleted once the editor is closed.
        _context.Feeds.LoadEntryAsync(project, Arg.Any<UpdateVersion>(), Arg.Any<CancellationToken>())
            .Returns(new PackageInfo { Version = new UpdateVersion("1.0.0"), Changelog = { ["en"] = "x" } });
        var extractedTo = new List<string>();
        _context.Publisher.OpenPackageAsync(project, Arg.Any<UpdateVersion>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>()).Returns(call =>
        {
            var directory = call.ArgAt<string>(2);
            extractedTo.Add(directory);
            _context.FileSystem.AddFile(Path.Combine(directory, "any", "Program", "a.dll"), new MockFileData("a"));
            var content = new PackageDefinition(new UpdateVersion("1.0.0"));
            content.GetOrAddPlatform("any").Files.Add(new PackageFileEntry(PackageRoot.Program, "a.dll",
                Path.Combine(directory, "any", "Program", "a.dll")));
            return content;
        });
        _context.Dialogs.ShowDialogAsync(Arg.Any<PackageEditorViewModel>()).Returns(true);
        await viewModel.EditPackageCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowDialogAsync(Arg.Is<PackageEditorViewModel>(e =>
            e.IsEditMode && e.Files.Single().Display == "Program/a.dll" && e.Files.Single().Size == 1));
        _context.FileSystem.Directory.Exists(extractedTo.Single()).ShouldBeFalse();

        // Without its package files the package opens for its feed entry only.
        _context.Publisher.OpenPackageAsync(project, Arg.Any<UpdateVersion>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>()).ThrowsAsync(new FileNotFoundException("The any package file is missing."));
        await viewModel.EditPackageCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowDialogAsync(Arg.Is<PackageEditorViewModel>(e => !e.CanChangeContent));

        // Any other failure is reported, and what was extracted so far is deleted.
        _context.Publisher.OpenPackageAsync(project, Arg.Any<UpdateVersion>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>()).Returns<Task<PackageDefinition>>(call =>
        {
            var directory = call.ArgAt<string>(2);
            extractedTo.Add(directory);
            _context.FileSystem.AddFile(Path.Combine(directory, "any", "Program", "a.dll"), new MockFileData("a"));
            throw new IOException("The disk is full.");
        });
        await viewModel.EditPackageCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowErrorAsync("Error while opening the package", "The disk is full.");
        _context.FileSystem.Directory.Exists(extractedTo[^1]).ShouldBeFalse();
        _context.Publisher.OpenPackageAsync(project, Arg.Any<UpdateVersion>(), Arg.Any<string>(),
            Arg.Any<CancellationToken>()).Returns(call =>
        {
            var directory = call.ArgAt<string>(2);
            extractedTo.Add(directory);
            _context.FileSystem.AddFile(Path.Combine(directory, "any", "Program", "a.dll"), new MockFileData("a"));
            var content = new PackageDefinition(new UpdateVersion("1.0.0"));
            content.GetOrAddPlatform("any").Files.Add(new PackageFileEntry(PackageRoot.Program, "a.dll",
                Path.Combine(directory, "any", "Program", "a.dll")));
            return content;
        });

        // A file that cannot be deleted leaves the folder behind without failing.
        _context.Dialogs.ShowDialogAsync(Arg.Any<PackageEditorViewModel>()).Returns(call =>
        {
            _context.FileSystem.File.SetAttributes(Path.Combine(extractedTo[^1], "any", "Program", "a.dll"),
                FileAttributes.ReadOnly);
            return Task.FromResult(false);
        });
        await viewModel.EditPackageCommand.ExecuteAsync(null);
        _context.FileSystem.Directory.Exists(extractedTo[^1]).ShouldBeTrue();

        await viewModel.AddPackageCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowDialogAsync(Arg.Is<PackageEditorViewModel>(e => e.IsCreateMode));

        var closed = new List<bool>();
        viewModel.CloseRequested += (_, a) => closed.Add(a);
        _context.Dialogs.ShowDialogAsync(Arg.Any<ProjectSettingsViewModel>()).Returns(true);
        await viewModel.OpenSettingsCommand.ExecuteAsync(null);
        closed.ShouldBeEmpty();
        _context.Migrator.ClearReceivedCalls();
        _context.Dialogs.ShowDialogAsync(Arg.Any<ProjectSettingsViewModel>()).Returns(call =>
        {
            // The assistant ran from the settings, which were then cancelled: the migrated package still shows up.
            project.Packages.Add(Package("7.0.0", true));
            typeof(ProjectSettingsViewModel).GetProperty(nameof(ProjectSettingsViewModel.Migrated))!.SetValue(
                call.Arg<ProjectSettingsViewModel>(), true);
            return Task.FromResult(false);
        });
        await viewModel.OpenSettingsCommand.ExecuteAsync(null);
        await _context.Migrator.Received(1)
            .CheckAsync(project, Arg.Any<ProjectSecrets>(), Arg.Any<CancellationToken>());
        viewModel.Packages.ShouldContain(p => p.Version == "7.0.0");
        _context.Dialogs.ShowDialogAsync(Arg.Any<ProjectSettingsViewModel>()).Returns(call =>
        {
            typeof(ProjectSettingsViewModel).GetProperty(nameof(ProjectSettingsViewModel.Deleted))!.SetValue(
                call.Arg<ProjectSettingsViewModel>(), true);
            return Task.FromResult(true);
        });
        await viewModel.OpenSettingsCommand.ExecuteAsync(null);
        closed.ShouldBe([true]);
    }

    [Fact]
    public async Task Project_OffersToTrustAnUnknownCertificateAndRetries()
    {
        var project = AppTestContext.NewProject();
        project.Transfer.Protocol = TransferProtocol.FtpsExplicit;
        project.Packages.Add(Package("1.0.0", false));
        var secrets = AppTestContext.NewSecrets();
        var viewModel = _context.Factory.Create<ProjectViewModel>(project, secrets);
        viewModel.SelectedPackage = viewModel.Packages.Single();
        var calls = 0;
        _context.Publisher.PublishExistingAsync(project, secrets, Arg.Any<UpdateVersion>(),
                Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>())
            .Returns(_ => calls++ == 0
                ? Task.FromException(new PipelineException("Uploading the package",
                    new UntrustedServerException("unknown certificate", "abc123", "CN=server"), []))
                : Task.CompletedTask);
        _context.Dialogs.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);

        await viewModel.PublishPackageCommand.ExecuteAsync(null);

        calls.ShouldBe(2);
        project.Transfer.TrustedCertificateFingerprint.ShouldBe("abc123");
        viewModel.ErrorMessage.ShouldBeNull();
        await _context.Dialogs.Received().ConfirmAsync("Unknown certificate", Arg.Is<string>(m => m.Contains("abc123")),
            "Trust", Arg.Any<string>());

        // Declining keeps the error; SFTP stores the host key instead.
        project.Transfer.Protocol = TransferProtocol.Sftp;
        project.Transfer.TrustedCertificateFingerprint = null;
        calls = 0;
        _context.Dialogs.ConfirmAsync("Unknown host key", Arg.Any<string>(), "Trust", Arg.Any<string>()).Returns(false);
        _context.Dialogs.ConfirmAsync("Publish package", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);
        await viewModel.PublishPackageCommand.ExecuteAsync(null);
        calls.ShouldBe(1);
        project.Transfer.TrustedHostKeyFingerprint.ShouldBeNull();
        await _context.Dialogs.Received().ShowErrorAsync("Error while publishing the package", Arg.Any<string>());

        // Other failures are not retried.
        calls = 0;
        _context.Publisher.PublishExistingAsync(project, secrets, Arg.Any<UpdateVersion>(),
                Arg.Any<IProgress<PipelineProgress>?>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                calls++;
                return Task.FromException(new PipelineException("Uploading the package",
                    new TransferException("refused"), []));
            });
        await viewModel.PublishPackageCommand.ExecuteAsync(null);
        calls.ShouldBe(1);
    }

    [Fact]
    public async Task Project_CopiesClientSourceAndLoadsStatisticsOnOpen()
    {
        var project = AppTestContext.NewProject(statistics: true);
        var viewModel = _context.Factory.Create<ProjectViewModel>(project, AppTestContext.NewSecrets(statistics: true));
        viewModel.StatisticsStatus.ShouldBe("Statistics have not been loaded yet.");

        await viewModel.CopyCSharpSourceCommand.ExecuteAsync(null);
        await _context.Clipboard.Received().SetTextAsync(Arg.Is<string>(s =>
            s.StartsWith("var manager = new UpdateManager(", StringComparison.Ordinal)));
        await viewModel.CopyVisualBasicSourceCommand.ExecuteAsync(null);
        await _context.Clipboard.Received().SetTextAsync(Arg.Is<string>(s =>
            s.StartsWith("Dim manager As New UpdateManager(", StringComparison.Ordinal)));

        await viewModel.OnOpenedAsync();
        viewModel.StatisticsStatus.ShouldBe("across 0 versions");
        viewModel.NeedsMigration.ShouldBeFalse();
        viewModel.LegacyFeedPresent.ShouldBeFalse();
        viewModel.MigrationText.ShouldBe("No updates.json of nUpdate 3 or 4 on the server.");
        await _context.Dialogs.DidNotReceive().ShowDialogAsync(Arg.Any<MigrationViewModel>());

        var disabled =
            _context.Factory.Create<ProjectViewModel>(AppTestContext.NewProject(), AppTestContext.NewSecrets());
        disabled.StatisticsStatus.ShouldBe("Statistics are disabled for this project.");
    }

    [Fact]
    public async Task Project_OpensTheMigrationAssistantWhenOnlyTheLegacyFeedExists()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        var viewModel = _context.Factory.Create<ProjectViewModel>(project, secrets);
        _context.Migrator.CheckAsync(project, secrets, Arg.Any<CancellationToken>())
            .Returns(new MigrationStatus(true, false), new MigrationStatus(true, true));
        _context.Dialogs.ShowDialogAsync(Arg.Any<MigrationViewModel>()).Returns(true);

        await viewModel.OnOpenedAsync();

        await _context.Dialogs.Received(1)
            .ShowDialogAsync(Arg.Is<MigrationViewModel>(m => m.Project == project && m.Secrets == secrets));
        viewModel.NeedsMigration.ShouldBeFalse();
        viewModel.LegacyFeedPresent.ShouldBeTrue();
        viewModel.MigrationText.ShouldContain("retire the old setup");

        // The banner stays while the migration is pending; the overview and the banner open the assistant again.
        _context.Migrator.CheckAsync(project, secrets, Arg.Any<CancellationToken>())
            .Returns(new MigrationStatus(true, false));
        await viewModel.CheckMigrationCommand.ExecuteAsync(null);
        viewModel.NeedsMigration.ShouldBeTrue();
        viewModel.MigrationText.ShouldContain("nothing can be published");
        await viewModel.MigrateCommand.ExecuteAsync(null);
        await _context.Dialogs.Received(2).ShowDialogAsync(Arg.Any<MigrationViewModel>());

        // Nothing to migrate: opening the project does not open the assistant.
        _context.Dialogs.ClearReceivedCalls();
        _context.Migrator.CheckAsync(project, secrets, Arg.Any<CancellationToken>())
            .Returns(new MigrationStatus(true, true));
        await viewModel.OnOpenedAsync();
        await _context.Dialogs.DidNotReceive().ShowDialogAsync(Arg.Any<MigrationViewModel>());

        // The check tolerates an unreachable server.
        _context.Migrator.CheckAsync(project, secrets, Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("offline"));
        await viewModel.CheckMigrationCommand.ExecuteAsync(null);
        viewModel.Migration.ShouldBeNull();
        viewModel.FeedStatus.ShouldBe("offline");
    }

    [Fact]
    public async Task Project_RetiresTheLegacySetupAfterListingWhatGoes()
    {
        var project = AppTestContext.NewProject();
        var secrets = AppTestContext.NewSecrets();
        var viewModel = _context.Factory.Create<ProjectViewModel>(project, secrets);
        var files = new LegacyFiles(["updates.json", "statistics.php"], ["1.0.0.0"], ["/data/Projects/Demo"]);
        _context.Migrator.FindLegacyFilesAsync(project, secrets, Arg.Any<CancellationToken>()).Returns(files);
        _context.Dialogs.ConfirmAsync("Retire the nUpdate 4 setup", Arg.Any<string>(), "Delete", Arg.Any<string>())
            .Returns(false);

        await viewModel.RetireLegacySetupCommand.ExecuteAsync(null);

        await _context.Dialogs.Received().ConfirmAsync("Retire the nUpdate 4 setup",
            Arg.Is<string>(m =>
                m.Contains("no longer find updates") &&
                m.Contains("On the server: updates.json, statistics.php, 1.0.0.0/") &&
                m.Contains("/data/Projects/Demo")), "Delete", Arg.Any<string>());
        await _context.Migrator.DidNotReceiveWithAnyArgs()
            .DeleteLegacyFilesAsync(default!, default!, default!, default);

        _context.Dialogs.ConfirmAsync("Retire the nUpdate 4 setup", Arg.Any<string>(), "Delete", Arg.Any<string>())
            .Returns(true);
        _context.Migrator.CheckAsync(project, secrets, Arg.Any<CancellationToken>())
            .Returns(new MigrationStatus(false, true));
        await viewModel.RetireLegacySetupCommand.ExecuteAsync(null);
        await _context.Migrator.Received()
            .DeleteLegacyFilesAsync(project, secrets, files, Arg.Any<CancellationToken>());
        viewModel.LegacyFeedPresent.ShouldBeFalse();

        _context.Migrator.DeleteLegacyFilesAsync(project, secrets, files, Arg.Any<CancellationToken>())
            .ThrowsAsync(new TransferException("refused"));
        await viewModel.RetireLegacySetupCommand.ExecuteAsync(null);
        await _context.Dialogs.Received()
            .ShowErrorAsync("Error while deleting the files of nUpdate 3 and 4", "refused");

        _context.Migrator.FindLegacyFilesAsync(project, secrets, Arg.Any<CancellationToken>())
            .Returns(new LegacyFiles([], [], []));
        await viewModel.RetireLegacySetupCommand.ExecuteAsync(null);
        await _context.Dialogs.Received().ShowInfoAsync("Retire the nUpdate 4 setup",
            Arg.Is<string>(m => m.Contains("Neither the server nor this computer")));

        _context.Migrator.FindLegacyFilesAsync(project, secrets, Arg.Any<CancellationToken>())
            .ThrowsAsync(new TransferException("offline"));
        await viewModel.RetireLegacySetupCommand.ExecuteAsync(null);
        await _context.Dialogs.Received()
            .ShowErrorAsync("Error while looking for the files of nUpdate 3 and 4", "offline");

        // An unknown host key outside a pipeline is offered for trust like inside one.
        var calls = 0;
        project.Transfer.Protocol = TransferProtocol.Sftp;
        _context.Migrator.FindLegacyFilesAsync(project, secrets, Arg.Any<CancellationToken>())
            .Returns(_ =>
                calls++ == 0
                    ? Task.FromException<LegacyFiles>(new UntrustedServerException("unknown host key", "fp-1",
                        "ssh-ed25519"))
                    : Task.FromResult(new LegacyFiles([], [], [])));
        _context.Dialogs.ConfirmAsync("Unknown host key", Arg.Any<string>(), "Trust", Arg.Any<string>()).Returns(true);
        await viewModel.RetireLegacySetupCommand.ExecuteAsync(null);
        project.Transfer.TrustedHostKeyFingerprint.ShouldBe("fp-1");
        calls.ShouldBe(2);

        ProjectViewModel.DescribeRetirement(new LegacyFiles(["updates.json"], [], []))
            .ShouldNotContain("On this computer");
        ProjectViewModel.DescribeRetirement(new LegacyFiles([], [], ["/x"])).ShouldNotContain("On the server");
        Should.Throw<ArgumentNullException>(() => ProjectViewModel.DescribeRetirement(null!));
    }

    [Fact]
    public async Task Project_ChecksTheRemoteFeed()
    {
        var project = AppTestContext.NewProject();
        var viewModel = _context.Factory.Create<ProjectViewModel>(project, AppTestContext.NewSecrets());
        viewModel.FeedStatus.ShouldBe("Not checked yet.");
        viewModel.FeedReachable.ShouldBeFalse();

        _context.Feeds.LoadRemoteAsync(project, Arg.Any<ProjectSecrets>(), Arg.Any<CancellationToken>())
            .Returns(new UpdateFeed { Packages = [new PackageInfo { Version = new UpdateVersion("1.0.0") }] });
        await viewModel.CheckFeedCommand.ExecuteAsync(null);
        viewModel.FeedStatus.ShouldBe("Reachable, 1 package");
        viewModel.FeedReachable.ShouldBeTrue();

        _context.Feeds.LoadRemoteAsync(project, Arg.Any<ProjectSecrets>(), Arg.Any<CancellationToken>())
            .Returns(new UpdateFeed());
        await viewModel.CheckFeedCommand.ExecuteAsync(null);
        viewModel.FeedStatus.ShouldBe("Reachable, 0 packages");

        _context.Feeds.LoadRemoteAsync(project, Arg.Any<ProjectSecrets>(), Arg.Any<CancellationToken>())
            .Returns((UpdateFeed?)null);
        await viewModel.CheckFeedCommand.ExecuteAsync(null);
        viewModel.FeedStatus.ShouldBe("No nupdate.json on the server yet.");
        viewModel.FeedReachable.ShouldBeFalse();

        _context.Feeds.LoadRemoteAsync(project, Arg.Any<ProjectSecrets>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("404"));
        await viewModel.CheckFeedCommand.ExecuteAsync(null);
        viewModel.FeedStatus.ShouldBe("404");
        _context.Feeds.LoadRemoteAsync(project, Arg.Any<ProjectSecrets>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidFeedException("broken feed"));
        await viewModel.CheckFeedCommand.ExecuteAsync(null);
        viewModel.FeedStatus.ShouldBe("broken feed");
    }

    [Fact]
    public async Task ProjectViewModel_LoadsStatistics()
    {
        var project = AppTestContext.NewProject(statistics: true);
        var secrets = AppTestContext.NewSecrets(statistics: true);
        var viewModel = _context.Factory.Create<ProjectViewModel>(project, secrets);
        _context.Statistics.GetStatisticsAsync(Arg.Any<StatisticsEndpoint>(), project.Id, Arg.Any<CancellationToken>())
            .Returns(new ProjectStatistics
            {
                Total = 1,
                Versions =
                [
                    new VersionStatistics { Version = new UpdateVersion("1.1.0"), Downloads = 1 },
                    new VersionStatistics { Version = new UpdateVersion("1.0.0") }
                ]
            });

        await viewModel.RefreshStatisticsCommand.ExecuteAsync(null);
        viewModel.TotalDownloads.ShouldBe(1);
        viewModel.StatisticsStatus.ShouldBe("across 2 versions");
        viewModel.VersionStatistics.Select(v => v.Version.ToString()).ShouldBe(["1.0.0", "1.1.0"]);

        _context.Statistics.GetStatisticsAsync(Arg.Any<StatisticsEndpoint>(), project.Id, Arg.Any<CancellationToken>())
            .Returns(new ProjectStatistics { Total = 5, Versions = [new VersionStatistics()] });
        await viewModel.RefreshStatisticsCommand.ExecuteAsync(null);
        viewModel.StatisticsStatus.ShouldBe("across 1 version");

        _context.Statistics.GetStatisticsAsync(Arg.Any<StatisticsEndpoint>(), project.Id, Arg.Any<CancellationToken>())
            .ThrowsAsync(new StatisticsException("api down"));
        await viewModel.RefreshStatisticsCommand.ExecuteAsync(null);
        viewModel.StatisticsStatus.ShouldBe("api down");

        project.UpdateUrl = "invalid";
        project.Statistics.EndpointUrl = null;
        await viewModel.RefreshStatisticsCommand.ExecuteAsync(null);
        viewModel.StatisticsStatus!.ShouldContain("invalid");

        secrets.StatisticsAdminSecret = null;
        await viewModel.RefreshStatisticsCommand.ExecuteAsync(null);
        viewModel.StatisticsStatus!.ShouldContain("admin secret is missing");

        var disabled =
            _context.Factory.Create<ProjectViewModel>(AppTestContext.NewProject(), AppTestContext.NewSecrets());
        await disabled.RefreshStatisticsCommand.ExecuteAsync(null);
        disabled.StatisticsStatus.ShouldContain("disabled");
    }

    [AvaloniaFact]
    public void ProjectWindow_BindsPackagesStatisticsAndTheMigrationBanner()
    {
        var project = AppTestContext.NewProject(statistics: true);
        project.Packages.Add(Package("1.0.0", true));
        var viewModel = _context.Factory.Create<ProjectViewModel>(project, AppTestContext.NewSecrets(statistics: true));
        var window = new ProjectWindow { DataContext = viewModel };
        window.Show();
        window.PackageGrid.ItemsSource.ShouldBe(viewModel.Packages);
        window.FeedUrlBox.Text.ShouldBe(viewModel.FeedUrl);
        window.TransferBox.Text.ShouldBe(viewModel.TransferSummary);
        window.MigrationBanner.IsVisible.ShouldBeFalse();
        viewModel.Migration = new MigrationStatus(true, false);
        window.MigrationBanner.IsVisible.ShouldBeTrue();
        window.RetireLegacySetupButton.IsVisible.ShouldBeTrue();
        window.MigrationAssistantButton.IsVisible.ShouldBeTrue();
        viewModel.VersionStatistics.Add(new VersionStatistics { Version = new UpdateVersion("1.0.0"), Downloads = 3 });
        window.StatisticsGrid.ItemsSource.ShouldBe(viewModel.VersionStatistics);
        window.HistoryGrid.ItemsSource.ShouldBe(viewModel.History);
        window.DataContext =
            _context.Factory.Create<ProjectViewModel>(project, AppTestContext.NewSecrets(statistics: true));
        window.DataContext = null;
        window.Close();
    }

    [AvaloniaFact]
    public void ProjectWindow_OffersItsActionsInTheMacOsMenuBar()
    {
        var viewModel = _context.Factory.Create<ProjectViewModel>(AppTestContext.NewProject(), AppTestContext.NewSecrets());
        var window = new ProjectWindow { DataContext = viewModel };
        window.Show();
        var menus = NativeMenu.GetMenu(window)!.Items.Cast<NativeMenuItem>().ToList();
        menus.Select(m => m.Header).ShouldBe(["Project", "Package"]);
        menus[0].Menu!.Items.OfType<NativeMenuItem>().Where(i => i is not NativeMenuItemSeparator).Select(i => (i.Header, i.Command)).ShouldBe([
            ("Check the Feed", viewModel.CheckFeedCommand),
            ("Migration Assistant…", viewModel.MigrateCommand),
            ("Copy Client Code", viewModel.CopySourceCommand),
            ("Project Settings…", viewModel.OpenSettingsCommand),
            ("Close Project", viewModel.CancelCommand),
        ]);
        menus[1].Menu!.Items.OfType<NativeMenuItem>().Where(i => i is not NativeMenuItemSeparator).Select(i => (i.Header, i.Command)).ShouldBe([
            ("New Package…", viewModel.AddPackageCommand),
            ("Edit Package…", viewModel.EditPackageCommand),
            ("Publish Package", viewModel.PublishPackageCommand),
            ("Delete Package…", viewModel.DeletePackageCommand),
        ]);
        window.Close();
    }

    [Fact]
    public void PackageDetails_DescribeTheLocalFeedEntry()
    {
        var package = new UpdatePackage
        { Version = new UpdateVersion("2.0.0"), Description = "Two", Released = true, CreatedAt = DateTimeOffset.MinValue };
        var withoutEntry = new PackageDetailsViewModel(package, null);
        (withoutEntry.Version, withoutEntry.Description, withoutEntry.Released).ShouldBe(("2.0.0", "Two", true));
        withoutEntry.Platforms.ShouldBeEmpty();
        new[] { withoutEntry.Size, withoutEntry.Rollout, withoutEntry.AfterInstall, withoutEntry.Created }
            .ShouldAllBe(text => text == "-");
        withoutEntry.HasChangelog.ShouldBeFalse();

        var entry = new PackageInfo
        {
            Version = new UpdateVersion("2.0.0"),
            Necessary = true,
            AfterInstall = AfterInstall.Close,
            Changelog = { ["en"] = "Fixes" },
            Rollout = new RolloutSettings { Conditions = [new RolloutCondition("Region", "EU")] },
            Files =
            [
                new PackageFile { Platform = "win-x64", Size = 1024 * 1024 },
                new PackageFile { Platform = "linux-x64", Size = 1024 * 1024 },
            ],
        };
        package.CreatedAt = DateTimeOffset.UtcNow;
        var details = new PackageDetailsViewModel(package, entry);
        details.Platforms.ShouldBe(["win-x64", "linux-x64"]);
        details.Size.ShouldBe(nUpdate.Ui.ByteSizeFormatter.Format(2 * 1024 * 1024, System.Globalization.CultureInfo.CurrentCulture));
        details.Rollout.ShouldBe("1 condition · necessary");
        details.AfterInstall.ShouldBe("Leave the application closed");
        details.Created.ShouldNotBe("-");
        details.Changelog.ShouldBe("Fixes");
        details.HasChangelog.ShouldBeTrue();

        entry.Necessary = false;
        entry.AfterInstall = null;
        entry.Rollout.Conditions.Add(new RolloutCondition("Tier", "beta"));
        details.Rollout.ShouldBe("2 conditions");
        details.AfterInstall.ShouldBe("As the application decides");
        entry.Rollout.Conditions.Clear();
        details.Rollout.ShouldBe("Everyone");
    }

    [Fact]
    public async Task ProjectViewModel_LoadsTheDetailsOfTheSelectedPackage()
    {
        var project = AppTestContext.NewProject();
        project.Packages.Add(Package("1.0.0", true));
        project.Packages.Add(Package("2.0.0", false));
        var first = new TaskCompletionSource<PackageInfo?>();
        _context.Feeds.LoadEntryAsync(project, new UpdateVersion("1.0.0"), Arg.Any<CancellationToken>()).Returns(first.Task);
        _context.Feeds.LoadEntryAsync(project, new UpdateVersion("2.0.0"), Arg.Any<CancellationToken>())
            .Returns(new PackageInfo { Version = new UpdateVersion("2.0.0"), Files = [new PackageFile { Platform = "any" }] });
        var viewModel = _context.Factory.Create<ProjectViewModel>(project, AppTestContext.NewSecrets());
        viewModel.SelectedDetails.ShouldBeNull();

        // The entry of 1.0.0 arrives only after 2.0.0 was selected: the details stay with 2.0.0.
        viewModel.SelectedPackage = viewModel.Packages.Single(p => p.Version == "1.0.0");
        viewModel.SelectedDetails!.Version.ShouldBe("1.0.0");
        viewModel.SelectedDetails.Size.ShouldBe("-");
        viewModel.SelectedPackage = viewModel.Packages.Single(p => p.Version == "2.0.0");
        viewModel.SelectedDetails!.Platforms.ShouldBe(["any"]);
        first.SetResult(new PackageInfo { Version = new UpdateVersion("1.0.0") });
        await Task.Yield();
        viewModel.SelectedDetails!.Version.ShouldBe("2.0.0");

        // An entry that cannot be read leaves the details without it.
        _context.Feeds.LoadEntryAsync(project, new UpdateVersion("1.0.0"), Arg.Any<CancellationToken>())
            .ThrowsAsync(new IOException("locked"));
        viewModel.SelectedPackage = viewModel.Packages.Single(p => p.Version == "1.0.0");
        viewModel.SelectedDetails!.Size.ShouldBe("-");
        viewModel.Initials.ShouldBe("D");
        viewModel.PackageCount.ShouldBe(2);
        viewModel.SelectedPackage = null;
        viewModel.SelectedDetails.ShouldBeNull();
    }
}
