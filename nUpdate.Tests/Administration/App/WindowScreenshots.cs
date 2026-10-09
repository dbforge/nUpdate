using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
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
            new ProjectRegistration(Guid.NewGuid(), "Aurora",
                @"C:\Users\dominic\Documents\nUpdate Projects\Aurora\project.nupdproj"),
            new ProjectRegistration(Guid.NewGuid(), "Media Tool",
                @"C:\Users\dominic\Documents\nUpdate Projects\Media Tool\project.nupdproj"),
        ]);
        var mediaTool = AppTestContext.NewProject();
        mediaTool.Name = "Media Tool";
        mediaTool.UpdateUrl = "https://downloads.mediatool.app/updates/";
        mediaTool.Transfer = new TransferSettings { Protocol = TransferProtocol.FtpsExplicit, Host = "downloads.mediatool.app" };
        mediaTool.Packages.Add(new UpdatePackage { Version = new UpdateVersion("3.4.1"), Released = true });
        mediaTool.Packages.Add(new UpdatePackage { Version = new UpdateVersion("3.5.0-beta.2"), Released = false });
        _context.Store.LoadAsync(Arg.Is<string>(p => p.Contains("Aurora")), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new ProjectLoadResult(project, secrets, migrated: false, SecretsState.Loaded));
        _context.Store.LoadAsync(Arg.Is<string>(p => p.Contains("Media Tool")), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new ProjectLoadResult(mediaTool, secrets, migrated: false, SecretsState.Loaded));
        _context.Statistics
            .GetStatisticsAsync(Arg.Any<StatisticsEndpoint>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(
                new ProjectStatistics
                {
                    Total = 4812,
                    Versions =
                    [
                        new VersionStatistics
                        {
                            Version = new UpdateVersion("2.1.0"), Downloads = 1930,
                            ByOperatingSystem =
                            {
                                ["Windows 11"] = 1341, ["Windows 10"] = 462, ["Windows Server 2022"] = 98,
                                ["Windows 8.1"] = 29
                            }
                        },
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
        wizard.Name = "Aurora";
        wizard.UpdateUrl = "https://updates.example.com/aurora/";
        Capture("new-project-general", new NewProjectWindow { DataContext = wizard });
        wizard.ContinueCommand.ExecuteAsync(null).GetAwaiter().GetResult();
        wizard.ContinueCommand.ExecuteAsync(null).GetAwaiter().GetResult();
        wizard.Transfer.Host = "updates.example.com";
        wizard.Transfer.Username = "deploy";
        wizard.Transfer.SftpPrivateKeyPath = "~/.ssh/id_ed25519";
        wizard.Transfer.Directory = "/updates/aurora";
        Capture("new-project-transfer", new NewProjectWindow { DataContext = wizard });
        CloseAll();

        wizard.Step = 4;
        Capture("new-project-security", new NewProjectWindow { DataContext = wizard });
        CloseAll();

        Capture("settings",
            new ProjectSettingsWindow
            { DataContext = _context.Factory.Create<ProjectSettingsViewModel>(project, secrets) });
        CloseAll();

        _context.Feeds.LoadEntryAsync(project, Arg.Any<UpdateVersion>(), Arg.Any<CancellationToken>()).Returns(new PackageInfo
        {
            Version = new UpdateVersion("2.2.0-beta.1"),
            Changelog = { ["en"] = "- A new importer for CSV and Excel files\n- Dark mode follows the system\n- Starts twice as fast" },
            Files =
            {
                new PackageFile { Platform = "win-x64", Size = 18_400_000 },
                new PackageFile { Platform = "linux-x64", Size = 17_900_000 },
                new PackageFile { Platform = "osx-arm64", Size = 19_100_000 },
            },
        });
        var projectViewModel = _context.Factory.Create<ProjectViewModel>(project, secrets);
        projectViewModel.SelectedPackage = projectViewModel.Packages[0];
        _context.Migrator.CheckAsync(project, secrets, Arg.Any<CancellationToken>())
            .Returns(new nUpdate.Administration.Core.Migration.MigrationStatus(true, false));
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
        editor.AddFile(nUpdate.Packaging.PackageRoot.Program, "Aurora.exe", @"C:\build\Aurora.exe");
        editor.AddFile(nUpdate.Packaging.PackageRoot.Program, "plugins/Importer.dll", @"C:\build\plugins\Importer.dll");
        editor.AddOperationOfKindCommand.Execute(
            OperationKind.FromType(nUpdate.Operations.TerminateProcessOperation.TypeName));
        editor.AddOperationOfKindCommand.Execute(
            OperationKind.FromType(nUpdate.Operations.StartProcessOperation.TypeName));
        var editorWindow = new PackageEditorWindow { DataContext = editor };
        Capture("package-general", editorWindow);
        editor.SelectedSection = editor.Sections.Single(s => s.Key == "files");
        Capture("package-files", editorWindow);
        editor.SelectedSection = editor.Sections.Single(s => s.Key == "operations");
        Capture("package-operations", editorWindow);
        editorWindow.Close();

        // Editing the published 2.1.0: one file replaced, one added, one removed, and a migration step after the files.
        var content = new nUpdate.Administration.Core.Packages.PackageDefinition(new UpdateVersion("2.1.0"));
        foreach (var platform in new[] { "win-x64", "linux-x64", "osx-arm64" })
        {
            var files = content.GetOrAddPlatform(platform).Files;
            foreach (var (root, path, size) in new[]
                     {
                         (nUpdate.Packaging.PackageRoot.Program, "Aurora.exe", 2_516_582),
                         (nUpdate.Packaging.PackageRoot.Program, "Aurora.Core.dll", 1_153_434),
                         (nUpdate.Packaging.PackageRoot.Program, "Aurora.Reports.dll", 655_360),
                         (nUpdate.Packaging.PackageRoot.Program, "Legacy.Export.dll", 215_040),
                         (nUpdate.Packaging.PackageRoot.AppData, "Aurora/settings.default.json", 4_096),
                     })
            {
                var source = $"/edit/{platform}/{root}/{path}";
                _context.FileSystem.AddFile(source, new System.IO.Abstractions.TestingHelpers.MockFileData(new byte[size]));
                files.Add(new nUpdate.Administration.Core.Packages.PackageFileEntry(root, path, source));
            }
        }

        content.Platforms[0].Operations.Add(new nUpdate.Operations.TerminateProcessOperation
        { ProcessName = "Aurora.Tray", RunBeforeFileReplacement = true });
        _context.FileSystem.AddFile("/build/Aurora.Core.dll", new System.IO.Abstractions.TestingHelpers.MockFileData(new byte[1_220_000]));
        _context.FileSystem.AddFile("/build/plugins/Importer.dll", new System.IO.Abstractions.TestingHelpers.MockFileData(new byte[389_120]));
        var published = new PackageInfo
        { Version = new UpdateVersion("2.1.0"), Changelog = { ["en"] = "Dark mode, faster start-up", ["de"] = "Dunkelmodus" } };
        var editing = _context.Factory.Create<PackageEditorViewModel>(project, secrets, new ExistingPackage(published, content));
        editing.AddFile(nUpdate.Packaging.PackageRoot.Program, "Aurora.Core.dll", "/build/Aurora.Core.dll");
        editing.AddFile(nUpdate.Packaging.PackageRoot.Program, "plugins/Importer.dll", "/build/plugins/Importer.dll");
        editing.RemoveFileCommand.Execute(editing.Files.Single(f => f.RelativePath == "Legacy.Export.dll"));
        editing.AddOperationOfKindCommand.Execute(
            OperationKind.FromType(nUpdate.Operations.StartProcessOperation.TypeName));
        editing.SelectedOperation!.Value = "%program%/Aurora.Migrate.exe";
        editing.SelectedOperation.SecondValue = "--from 2.0 --quiet";
        editing.SelectedOperation.WaitForExit = true;
        editing.SelectedOperation.FailOnError = true;
        var editingWindow = new PackageEditorWindow { DataContext = editing };
        editing.SelectedSection = editing.Sections.Single(s => s.Key == "files");
        Capture("package-edit-files", editingWindow);
        editing.SelectedSection = editing.Sections.Single(s => s.Key == "operations");
        Capture("package-edit-operations", editingWindow);
        editingWindow.Close();

        // The same windows in dark mode.
        Avalonia.Application.Current!.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Dark;
        try
        {
            Capture("main-dark", new MainWindow { DataContext = main });
            CloseAll();
            var darkProjectWindow = new ProjectWindow { DataContext = projectViewModel };
            Capture("project-packages-dark", darkProjectWindow, w => ((ProjectWindow)w).Nav.SelectedIndex = 0);
            Capture("project-statistics-dark", darkProjectWindow, w => ((ProjectWindow)w).Nav.SelectedIndex = 2);
            darkProjectWindow.Close();
            var darkEditor = new PackageEditorWindow { DataContext = editing };
            Capture("package-edit-files-dark", darkEditor, _ => editing.SelectedSection = editing.Sections.Single(s => s.Key == "files"));
            Capture("package-edit-operations-dark", darkEditor,
                _ => editing.SelectedSection = editing.Sections.Single(s => s.Key == "operations"));
            darkEditor.Close();
            Capture("settings-dark",
                new ProjectSettingsWindow { DataContext = _context.Factory.Create<ProjectSettingsViewModel>(project, secrets) });
            CloseAll();
        }
        finally
        {
            Avalonia.Application.Current.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Default;
        }

        Capture("credentials",
            new CredentialsWindow
            {
                DataContext =
                    _context.Factory.Create<CredentialsViewModel>(project, new ProjectSecrets(),
                        CredentialsMode.Secrets)
            });
        var locked = SampleProject();
        locked.Secrets =
            nUpdate.Administration.Core.Projects.ProjectSecretsProtection.Protect(secrets, "project-password");
        Capture("unlock",
            new CredentialsWindow
            {
                DataContext = _context.Factory.Create<CredentialsViewModel>(locked, new ProjectSecrets(),
                    CredentialsMode.ProjectPassword)
            });
        Capture("project-password",
            new ProjectPasswordWindow { DataContext = _context.Factory.Create<ProjectPasswordViewModel>() });
        Capture("popup",
            new MessageWindow("Uploading the package failed",
                "The SFTP server \"updates.example.com:22\" could not be reached: connection timed out. Everything done so far was rolled back.",
                MessageWindow.Kind.Error, "Close", null));
        CloseAll();

        // The migration assistant for a project of nUpdate 4 with three published packages.
        var legacyProject = SampleProject();
        legacyProject.LegacyProjectFile = @"C:\Users\dominic\Documents\Aurora.nupdproj";
        _context.Migrator
            .PrepareAsync(legacyProject, secrets,
                Arg.Any<IProgress<nUpdate.Administration.Core.Publishing.PipelineProgress>?>(),
                Arg.Any<CancellationToken>()).Returns(MigrationSample(legacyProject));
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
            new(nUpdate.Administration.Core.Migration.LegacyVersion.Parse(literal), literal)
            {
                UseStatistics = true,
                PackageUri = new Uri($"https://updates.example.com/aurora/{literal}/package.zip")
            };

        static nUpdate.Administration.Core.Migration.MigrationPackage Ready(string literal, int files, long size,
            params string[] warnings) =>
            nUpdate.Administration.Core.Migration.MigrationPackage.Ready(Entry(literal),
                $"https://updates.example.com/aurora/{literal}/package.zip", "package.zip", size, files, [],
                new nUpdate.Administration.Core.Migration.LegacyOperationConversion(
                    [new nUpdate.Operations.TerminateProcessOperation { ProcessName = "Aurora" }], warnings));

        return new(project.Id, true, null,
        [
            Ready("2.0.0.0", 14, 18_400_000),
            Ready("2.0.1.0", 3, 2_100_000),
            Ready("2.1.0.0", 9, 7_350_000,
                "Operation 2 (Scripts Create) has no counterpart in nUpdate 5 and is left out."),
        ]);
    }

    private static void CloseAll()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            foreach (var window in desktop.Windows.ToList())
                window.Close();
        }
    }

    private static UpdateProject SampleProject()
    {
        var project = AppTestContext.NewProject(statistics: true);
        project.Name = "Aurora";
        project.UpdateUrl = "https://updates.example.com/aurora/";
        project.Transfer = new TransferSettings
        {
            Protocol = TransferProtocol.Sftp,
            Port = 22,
            Host = "updates.example.com",
            Username = "deploy",
            Directory = "/updates/aurora"
        };
        project.AssemblyVersionPath = @"C:\Projects\Aurora\bin\Release\Aurora.exe";
        project.Packages.Add(new UpdatePackage
        {
            Version = new UpdateVersion("2.1.0"),
            Description = "Dark mode, faster start-up",
            Released = true,
            CreatedAt = new DateTimeOffset(2026, 10, 5, 14, 12, 0, TimeSpan.Zero)
        });
        project.Packages.Add(new UpdatePackage
        {
            Version = new UpdateVersion("2.0.1"),
            Description = "Fixes the crash when exporting reports",
            Released = true,
            CreatedAt = new DateTimeOffset(2026, 9, 21, 9, 40, 0, TimeSpan.Zero)
        });
        project.Packages.Add(new UpdatePackage
        {
            Version = new UpdateVersion("2.0.0"),
            Description = "New reporting module",
            Released = true,
            CreatedAt = new DateTimeOffset(2026, 9, 2, 17, 3, 0, TimeSpan.Zero)
        });
        project.Packages.Add(new UpdatePackage
        {
            Version = new UpdateVersion("2.2.0-beta.1"),
            Description = "Beta of the new importer",
            Released = false,
            CreatedAt = new DateTimeOffset(2026, 10, 6, 11, 0, 0, TimeSpan.Zero)
        });
        project.Log.Add(new LogEntry
        {
            Kind = LogEntryKind.Create,
            At = new DateTimeOffset(2026, 10, 5, 14, 10, 0, TimeSpan.Zero),
            Version = new UpdateVersion("2.1.0"),
            User = "dominic"
        });
        project.Log.Add(new LogEntry
        {
            Kind = LogEntryKind.Upload,
            At = new DateTimeOffset(2026, 10, 5, 14, 12, 0, TimeSpan.Zero),
            Version = new UpdateVersion("2.1.0"),
            User = "dominic"
        });
        return project;
    }

    [AvaloniaFact]
    public async Task InstallerAndUpdateDialogs_RenderAFrame()
    {
        var options = new nUpdate.Installer.InstallerOptions
        { Application = new nUpdate.Installer.ApplicationOptions { Name = "Aurora" } };
        var installer = new nUpdate.UpdateInstaller.UI.Avalonia.InstallerWindowViewModel(
            new nUpdate.UpdateInstaller.InstallerSession(options, "/tmp/nUpdate Installer/Aurora/install.log"));
        installer.Report(64, "Copying Aurora.exe...");
        var installerWindow = new nUpdate.UpdateInstaller.UI.Avalonia.InstallerWindow(installer);
        Capture("installer", installerWindow, frame: WindowFrame.Gnome);
        installer.AskAboutLockedFile("/opt/aurora/Aurora.dll", _ => { });
        Capture("installer-locked-file", installerWindow, frame: WindowFrame.Gnome);
        installer.ShowError(new UnauthorizedAccessException("Access to the path '/opt/aurora/Aurora.dll' is denied."),
            () => { });
        Capture("installer-error", installerWindow, frame: WindowFrame.Gnome);
        installer.Finish();

        var services = new nUpdate.Tests.Library.Support.TestServices();
        services.ApplicationInfo.ProductName.Returns("Aurora");
        var feed = new UpdateFeed();
        feed.Packages.Add(new PackageInfo
        {
            Version = new UpdateVersion("2.2.0"),
            Changelog =
            {
                ["en"] =
                    "- A new importer for CSV and Excel files\n- Dark mode follows the system\n- Starts twice as fast"
            },
            Files =
            [
                new PackageFile
                {
                    Path = "packages/2.2.0/any.zip", Size = 18_400_000, Sha512 = "x",
                    Signature = new PackageSignature { Value = "s" },
                    Touches = [nUpdate.Operations.OperationArea.Processes]
                }
            ],
        });
        services.Http.Text(HttpMethod.Get, "https://updates.example.com/aurora/nupdate.json",
            nUpdate.Serializer.Serialize(feed));
        using var manager = new UpdateManager(new Uri("https://updates.example.com/aurora/nupdate.json"),
            nUpdate.Tests.Support.TestKeys.PublicKey,
            currentVersion: new UpdateVersion("2.1.0"), services: services.Build());
        (await manager.CheckForUpdatesAsync()).ShouldBeTrue();
        var dialog =
            new nUpdate.UI.Avalonia.Views.UpdateDialog(
                new nUpdate.UI.Avalonia.ViewModels.NewUpdateDialogViewModel(manager));
        Capture("update-dialog", dialog, frame: WindowFrame.Gnome);
        dialog.Close();
    }

    /// <summary>How the screenshots draw the decorations that headless rendering leaves out.</summary>
    private enum WindowFrame
    {
        /// <summary>nUpdate Administration as it looks on Windows 11.</summary>
        Windows11,

        /// <summary>The installer and the update dialog as they look on Linux with GNOME.</summary>
        Gnome,
    }

    private static void Capture(string name, Window window, Action<Window>? arrange = null,
        WindowFrame frame = WindowFrame.Windows11)
    {
        if (!window.IsVisible)
            window.Show();
        arrange?.Invoke(window);
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
        var content = window.CaptureRenderedFrame();
        content.ShouldNotBeNull();
        if (OutputDirectory is { } directory)
        {
            Directory.CreateDirectory(directory);
            using var decorated = Render(frame == WindowFrame.Gnome
                ? GnomeFrame(window, content)
                : Windows11Frame(window, content));
#pragma warning disable CS0618 // the encoder-options overload adds nothing for a review PNG
            decorated.Save(System.IO.Path.Combine(directory, name + ".png"));
#pragma warning restore CS0618
        }
    }

    /// <summary>
    ///     The captured client area in a window frame as GNOME draws it: a header bar with the centred title and a round
    ///     close button, rounded corners and a shadow.
    /// </summary>
    private static Panel GnomeFrame(Window window, Bitmap content)
    {
        const double cornerRadius = 12;
        var ink = new SolidColorBrush(Color.Parse("#2E2E2E"));
        var title = new TextBlock
        {
            Text = window.Title,
            FontFamily = new FontFamily("Cantarell, Ubuntu, DejaVu Sans"),
            FontSize = 14.7,
            FontWeight = FontWeight.Bold,
            Foreground = ink,
            TextTrimming = TextTrimming.CharacterEllipsis,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(56, 0),
        };
        var close = new Border
        {
            Width = 24,
            Height = 24,
            CornerRadius = new CornerRadius(12),
            Background = new SolidColorBrush(Color.Parse("#DADADA")),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 11, 0),
            Child = Glyph("M 0,0 L 7,7 M 7,0 L 0,7", ink, 1.6),
        };
        var header = new Grid
        { Height = 46, Background = new SolidColorBrush(Color.Parse("#EBEBEB")), Children = { title, close } };
        return FramedWindow(header, content, cornerRadius, "#26000000", "0 4 18 0 #40000000");
    }

    /// <summary>
    ///     The captured client area in a window frame as Windows 11 draws it: a white (in dark mode a black) title bar with
    ///     the icon and the title, 46-pixel caption buttons (only Close for a window that cannot be resized), rounded
    ///     corners and a shadow.
    /// </summary>
    private static Panel Windows11Frame(Window window, Bitmap content)
    {
        var dark = window.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
        var ink = new SolidColorBrush(Color.Parse(dark ? "#FFFFFF" : "#1B1B1B"));
        var header = new DockPanel { Height = 32, Background = dark ? new SolidColorBrush(Color.Parse("#202020")) : Brushes.White };
        if (window.Icon is not null)
        {
            // Every window of nUpdate Administration shows nUpdate.ico; a WindowIcon cannot be read back as a bitmap.
            using var stream =
                Avalonia.Platform.AssetLoader.Open(new Uri("avares://nUpdate.Administration/Assets/nUpdate.ico"));
            var image = new Image
            { Source = new Bitmap(stream), Width = 16, Height = 16, Margin = new Thickness(12, 0, 10, 0) };
            DockPanel.SetDock(image, Dock.Left);
            header.Children.Add(image);
        }

        var buttons = new StackPanel { Orientation = Orientation.Horizontal };
        if (window.CanResize)
        {
            buttons.Children.Add(CaptionButton(Glyph("M 0,0.5 L 10,0.5", ink, 1)));
            buttons.Children.Add(CaptionButton(new Border
            {
                Width = 10,
                Height = 10,
                BorderBrush = ink,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(2)
            }));
        }

        buttons.Children.Add(CaptionButton(Glyph("M 0,0 L 8.5,8.5 M 8.5,0 L 0,8.5", ink, 1)));
        DockPanel.SetDock(buttons, Dock.Right);
        header.Children.Add(buttons);
        header.Children.Add(new TextBlock
        {
            Text = window.Title,
            FontFamily = new FontFamily("Segoe UI Variable Text, Segoe UI, fonts:Inter#Inter"),
            FontSize = 12,
            Foreground = ink,
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(window.Icon is null ? 12 : 0, 0, 0, 0),
        });
        return FramedWindow(header, content, 8, dark ? "#3A3A3A" : "#C9CCD1", "0 8 28 0 #38000000");
    }

    private static Border CaptionButton(Control glyph) =>
        new() { Width = 46, Height = 32, Child = glyph };

    private static Avalonia.Controls.Shapes.Path Glyph(string data, IBrush ink, double thickness) => new()
    {
        Data = Geometry.Parse(data),
        Stroke = ink,
        StrokeThickness = thickness,
        StrokeLineCap = PenLineCap.Round,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
    };

    /// <summary>A window of the header above the content, with rounded corners, an outline and a shadow, on a transparent margin.</summary>
    private static Panel FramedWindow(Control header, Bitmap content, double cornerRadius, string outline,
        string shadow)
    {
        DockPanel.SetDock(header, Dock.Top);
        var frame = new Border
        {
            Margin = new Thickness(28),
            CornerRadius = new CornerRadius(cornerRadius),
            Background = Brushes.White,
            BorderBrush = new SolidColorBrush(Color.Parse(outline)),
            BorderThickness = new Thickness(1),
            BoxShadow = BoxShadows.Parse(shadow),
            Child = new Border
            {
                CornerRadius = new CornerRadius(cornerRadius - 1),
                ClipToBounds = true,
                Child = new DockPanel
                {
                    Children =
                    {
                        header, new Image { Source = content, Width = content.Size.Width, Height = content.Size.Height }
                    }
                },
            },
        };
        return new Panel { Children = { frame } };
    }

    private static RenderTargetBitmap Render(Control root)
    {
        root.Measure(Size.Infinity);
        root.Arrange(new Rect(root.DesiredSize));
        var bitmap = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(root.DesiredSize.Width),
            (int)Math.Ceiling(root.DesiredSize.Height)));
        bitmap.Render(root);
        return bitmap;
    }
}
