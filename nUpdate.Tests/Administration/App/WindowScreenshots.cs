using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.App;

/// <summary>
///     Renders every window with sample data and writes PNGs when NUPDATE_SCREENSHOTS names a folder. A design review
///     tool rather than a test: without the variable it only checks that every window renders a frame.
/// </summary>
public class WindowScreenshots
{
    private readonly AppTestContext _context = new();

    private static string? OutputDirectory => Environment.GetEnvironmentVariable("NUPDATE_SCREENSHOTS");

    [AvaloniaFact]
    public void EveryWindow_RendersAFrame()
    {
        var project = SampleProject();
        var secrets = AppTestContext.NewSecrets(statistics: true);
        _context.Store.ListAsync(Arg.Any<CancellationToken>()).Returns(
        [
            new ProjectRegistration(Guid.NewGuid(), "Trade Updater", @"C:\Users\dominic\Documents\nUpdate Projects\Trade Updater\project.nupdproj"),
            new ProjectRegistration(Guid.NewGuid(), "Media Tool", @"C:\Users\dominic\Documents\nUpdate Projects\Media Tool\project.nupdproj"),
        ]);
        _context.Statistics.GetStatisticsAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(new ProjectStatistics
        {
            Total = 4812,
            Versions =
            [
                new VersionStatistics { Version = new UpdateVersion("2.1.0"), Downloads = 1930, ByOperatingSystem = { ["Windows 11"] = 1341, ["Windows 10"] = 462, ["Windows Server 2022"] = 98, ["Windows 8.1"] = 29 } },
                new VersionStatistics { Version = new UpdateVersion("2.0.1"), Downloads = 1512 },
                new VersionStatistics { Version = new UpdateVersion("2.0.0"), Downloads = 1104 },
                new VersionStatistics { Version = new UpdateVersion("1.9.4"), Downloads = 266 },
            ],
        });

        var main = _context.Factory.Create<MainWindowViewModel>();
        main.RefreshCommand.ExecuteAsync(null).GetAwaiter().GetResult();
        main.SelectedProject = main.Projects[0];
        Capture("main", new MainWindow { DataContext = main });
        CloseAll();

        var wizard = _context.Factory.Create<NewProjectViewModel>();
        wizard.Name = "Trade Updater";
        wizard.UpdateUrl = "https://updates.example.com/trade-updater/";
        Capture("new-project-general", new NewProjectWindow { DataContext = wizard });
        wizard.ContinueCommand.ExecuteAsync(null).GetAwaiter().GetResult();
        wizard.ContinueCommand.ExecuteAsync(null).GetAwaiter().GetResult();
        wizard.Transfer.Host = "updates.example.com";
        wizard.Transfer.Username = "deploy";
        wizard.Transfer.SftpPrivateKeyPath = "~/.ssh/id_ed25519";
        wizard.Transfer.Directory = "/updates/trade-updater";
        Capture("new-project-transfer", new NewProjectWindow { DataContext = wizard });
        CloseAll();

        wizard.Step = 4;
        Capture("new-project-security", new NewProjectWindow { DataContext = wizard });
        CloseAll();

        Capture("settings", new ProjectSettingsWindow { DataContext = _context.Factory.Create<ProjectSettingsViewModel>(project, secrets) });
        CloseAll();

        var projectViewModel = _context.Factory.Create<ProjectViewModel>(project, secrets);
        projectViewModel.SelectedPackage = projectViewModel.Packages[0];
        projectViewModel.Migration = new nUpdate.Administration.Core.Migration.MigrationStatus(true, false);
        var projectWindow = new ProjectWindow { DataContext = projectViewModel };
        Capture("project-packages-migration", projectWindow, w => ((ProjectWindow)w).Nav.SelectedIndex = 0);
        projectViewModel.Migration = new nUpdate.Administration.Core.Migration.MigrationStatus(false, true);
        Capture("project-packages", projectWindow, w => ((ProjectWindow)w).Nav.SelectedIndex = 0);
        Capture("project-overview", projectWindow, w => ((ProjectWindow)w).Nav.SelectedIndex = 1);
        projectViewModel.RefreshStatisticsCommand.ExecuteAsync(null).GetAwaiter().GetResult();
        Capture("project-statistics", projectWindow, w => ((ProjectWindow)w).Nav.SelectedIndex = 2);
        Capture("project-history", projectWindow, w => ((ProjectWindow)w).Nav.SelectedIndex = 3);
        projectWindow.Close();

        var editor = _context.Factory.Create<PackageEditorViewModel>(project, secrets);
        editor.Description = "New importer and dark mode";
        foreach (var platform in new[] { "win-x64", "linux-x64", "osx-arm64" })
        {
            editor.NewPlatform = new PlatformChoice(platform);
            editor.AddPlatformCommand.Execute(null);
        }

        editor.RemovePlatformCommand.Execute(editor.Platforms[0]);
        editor.SelectedPlatform = editor.Platforms[0];
        editor.AddFile(nUpdate.Packaging.PackageRoot.Program, "TradeUpdater.exe", @"C:\build\TradeUpdater.exe");
        editor.AddFile(nUpdate.Packaging.PackageRoot.Program, "plugins/Importer.dll", @"C:\build\plugins\Importer.dll");
        editor.AddOperationOfKindCommand.Execute(OperationKind.FromType(nUpdate.Operations.TerminateProcessOperation.TypeName));
        editor.AddOperationOfKindCommand.Execute(OperationKind.FromType(nUpdate.Operations.StartProcessOperation.TypeName));
        var editorWindow = new PackageEditorWindow { DataContext = editor };
        Capture("package-general", editorWindow);
        editor.SelectedSection = editor.Sections.Single(s => s.Key == "files");
        Capture("package-files", editorWindow);
        editor.SelectedSection = editor.Sections.Single(s => s.Key == "operations");
        Capture("package-operations", editorWindow);
        editorWindow.Close();

        Capture("credentials", new CredentialsWindow { DataContext = _context.Factory.Create<CredentialsViewModel>(project, new ProjectSecrets(), CredentialsMode.Secrets) });
        var locked = SampleProject();
        locked.Secrets = nUpdate.Administration.Core.Projects.ProjectSecretsProtection.Protect(secrets, "project-password");
        Capture("unlock", new CredentialsWindow { DataContext = _context.Factory.Create<CredentialsViewModel>(locked, new ProjectSecrets(), CredentialsMode.ProjectPassword) });
        Capture("project-password", new ProjectPasswordWindow { DataContext = _context.Factory.Create<ProjectPasswordViewModel>() });
        Capture("popup", new MessageWindow("Uploading the package failed", "The SFTP server \"updates.example.com:22\" could not be reached: connection timed out. Everything done so far was rolled back.", MessageWindow.Kind.Error, "Close", null));
        CloseAll();

        // The migration assistant for a project of nUpdate 4 with three published packages.
        var legacyProject = SampleProject();
        legacyProject.LegacyProjectFile = @"C:\Users\dominic\Documents\Trade Updater.nupdproj";
        _context.Migrator.PrepareAsync(legacyProject, secrets, Arg.Any<IProgress<nUpdate.Administration.Core.Publishing.PipelineProgress>?>(), Arg.Any<CancellationToken>()).Returns(MigrationSample(legacyProject));
        var assistant = _context.Factory.Create<MigrationViewModel>(legacyProject, secrets);
        assistant.InitializeAsync().GetAwaiter().GetResult();
        var assistantWindow = new MigrationWindow { DataContext = assistant };
        Capture("migration-overview", assistantWindow);
        assistant.Step = MigrationViewModel.PackagesStep;
        Capture("migration-packages", assistantWindow);
        assistant.Migrated = true;
        assistant.Step = MigrationViewModel.SideBySideStep;
        Capture("migration-side-by-side", assistantWindow);
        assistantWindow.Close();
    }

    private static nUpdate.Administration.Core.Migration.MigrationPlan MigrationSample(UpdateProject project)
    {
        static nUpdate.Administration.Core.Migration.LegacyFeedEntry Entry(string literal) =>
            new(nUpdate.Administration.Core.Migration.LegacyVersion.Parse(literal), literal) { UseStatistics = true, PackageUri = new Uri($"https://updates.example.com/trade-updater/{literal}/package.zip") };
        static nUpdate.Administration.Core.Migration.MigrationPackage Ready(string literal, int files, long size, params string[] warnings) =>
            nUpdate.Administration.Core.Migration.MigrationPackage.Ready(Entry(literal), $"https://updates.example.com/trade-updater/{literal}/package.zip", "package.zip", size, files, [],
                new nUpdate.Administration.Core.Migration.LegacyOperationConversion([new nUpdate.Operations.TerminateProcessOperation { ProcessName = "TradeUpdater" }], warnings));
        return new(project.Id, true, null,
        [
            Ready("2.0.0.0", 14, 18_400_000),
            Ready("2.0.1.0", 3, 2_100_000),
            Ready("2.1.0.0", 9, 7_350_000, "Operation 2 (Scripts Create) has no counterpart in nUpdate 5 and is left out."),
        ]);
    }

    private static void CloseAll()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            foreach (var window in desktop.Windows.ToList())
                window.Close();
        }
    }

    private static UpdateProject SampleProject()
    {
        var project = AppTestContext.NewProject(statistics: true);
        project.Name = "Trade Updater";
        project.UpdateUrl = "https://updates.example.com/trade-updater/";
        project.Transfer = new TransferSettings { Protocol = TransferProtocol.Sftp, Port = 22, Host = "updates.example.com", Username = "deploy", Directory = "/updates/trade-updater" };
        project.AssemblyVersionPath = @"C:\Projects\Trade Updater\bin\Release\TradeUpdater.exe";
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("2.1.0"), Description = "Dark mode, faster start-up", Released = true, CreatedAt = new DateTimeOffset(2026, 10, 5, 14, 12, 0, TimeSpan.Zero) });
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("2.0.1"), Description = "Fixes the crash when exporting reports", Released = true, CreatedAt = new DateTimeOffset(2026, 9, 21, 9, 40, 0, TimeSpan.Zero) });
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("2.0.0"), Description = "New reporting module", Released = true, CreatedAt = new DateTimeOffset(2026, 9, 2, 17, 3, 0, TimeSpan.Zero) });
        project.Packages.Add(new UpdatePackage { Version = new UpdateVersion("2.2.0-beta.1"), Description = "Beta of the new importer", Released = false, CreatedAt = new DateTimeOffset(2026, 10, 6, 11, 0, 0, TimeSpan.Zero) });
        project.Log.Add(new LogEntry { Kind = LogEntryKind.Create, At = new DateTimeOffset(2026, 10, 5, 14, 10, 0, TimeSpan.Zero), Version = new UpdateVersion("2.1.0"), User = "dominic" });
        project.Log.Add(new LogEntry { Kind = LogEntryKind.Upload, At = new DateTimeOffset(2026, 10, 5, 14, 12, 0, TimeSpan.Zero), Version = new UpdateVersion("2.1.0"), User = "dominic" });
        return project;
    }

    [AvaloniaFact]
    public async Task InstallerAndUpdateDialogs_RenderAFrame()
    {
        var options = new nUpdate.Installer.InstallerOptions { Application = new nUpdate.Installer.ApplicationOptions { Name = "Trade Updater" } };
        var installer = new nUpdate.UpdateInstaller.UI.Avalonia.InstallerWindowViewModel(new nUpdate.UpdateInstaller.InstallerSession(options, "/tmp/nUpdate Installer/Trade Updater/install.log"));
        installer.Report(64, "Copying TradeUpdater.exe...");
        var installerWindow = new nUpdate.UpdateInstaller.UI.Avalonia.InstallerWindow(installer);
        Capture("installer", installerWindow);
        installer.AskAboutLockedFile("/opt/trade-updater/TradeUpdater.dll", _ => { });
        Capture("installer-locked-file", installerWindow);
        installer.ShowError(new UnauthorizedAccessException("Access to the path '/opt/trade-updater/TradeUpdater.dll' is denied."), () => { });
        Capture("installer-error", installerWindow);
        installer.Finish();

        var services = new nUpdate.Tests.Library.Support.TestServices();
        services.ApplicationInfo.ProductName.Returns("Trade Updater");
        var feed = new UpdateFeed();
        feed.Packages.Add(new PackageInfo
        {
            Version = new UpdateVersion("2.2.0"),
            Changelog = { ["en"] = "- A new importer for CSV and Excel files\n- Dark mode follows the system\n- Starts twice as fast" },
            Files = [new PackageFile { Path = "packages/2.2.0/any.zip", Size = 18_400_000, Sha512 = "x", Signature = new PackageSignature { Value = "s" }, Touches = [nUpdate.Operations.OperationArea.Processes] }],
        });
        services.Http.Text(HttpMethod.Get, "https://updates.example.com/trade-updater/nupdate.json", nUpdate.Serializer.Serialize(feed));
        using var manager = new UpdateManager(new Uri("https://updates.example.com/trade-updater/nupdate.json"), nUpdate.Tests.Support.TestKeys.PublicKey,
            currentVersion: new UpdateVersion("2.1.0"), services: services.Build());
        (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
        var dialog = new nUpdate.UI.Avalonia.Views.UpdateDialog(new nUpdate.UI.Avalonia.ViewModels.NewUpdateDialogViewModel(manager));
        Capture("update-dialog", dialog);
        dialog.Close();
    }

    private static void Capture(string name, Window window, Action<Window>? arrange = null)
    {
        if (!window.IsVisible)
            window.Show();
        arrange?.Invoke(window);
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
        var frame = window.CaptureRenderedFrame();
        frame.ShouldNotBeNull();
        if (OutputDirectory is { } directory)
        {
            Directory.CreateDirectory(directory);
#pragma warning disable CS0618 // the encoder-options overload adds nothing for a review PNG
            frame.Save(Path.Combine(directory, name + ".png"));
#pragma warning restore CS0618
        }

    }
}
