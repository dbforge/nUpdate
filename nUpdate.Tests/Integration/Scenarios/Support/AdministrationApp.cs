using System.Globalization;
using System.IO.Abstractions;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using nUpdate.Administration;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Services;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views;
using nUpdate.Packaging;
using nUpdate.Tests.Integration.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Integration.Scenarios.Support;

/// <summary>
///     nUpdate Administration as the user runs it: the real composition root with the real services against the
///     container servers, a data folder of its own, and the real windows, shown headlessly. Only the native file
///     dialogs and the clipboard are scripted.
/// </summary>
#pragma warning disable CA1822 // the wait helpers read like actions on the app
public sealed class AdministrationApp : IDisposable
{
    /// <summary>The project password of every project the scenarios create with saved credentials.</summary>
    public const string ProjectPassword = "scenario-password";

    private readonly IntegrationContext _context;

    public AdministrationApp(IntegrationContext context)
    {
        _context = context;
        Paths = new AdministrationPaths(new FileSystem(), Path.Combine(context.Root, "app"),
            Path.Combine(context.Root, "app-projects"));
        Provider = AppServices.Build(Paths, services =>
        {
            services.AddSingleton<IDialogService>(Dialogs);
            services.AddSingleton<IFilePickerService>(Picker);
            services.AddSingleton<IClipboardService>(Clipboard);
        });
        MainViewModel = Provider.GetRequiredService<MainWindowViewModel>();
        Main = new MainWindow { DataContext = MainViewModel };
        Dialogs.Owner = Main;
    }

    public AdministrationPaths Paths { get; }

    public ServiceProvider Provider { get; }

    public HeadlessDialogService Dialogs { get; } = new();

    public ScriptedFilePicker Picker { get; } = new();

    public MemoryClipboard Clipboard { get; } = new();

    public MainWindow Main { get; }

    public MainWindowViewModel MainViewModel { get; }

    public IProjectStore Store => Provider.GetRequiredService<IProjectStore>();

    public IProjectService Projects => Provider.GetRequiredService<IProjectService>();

    public IPublishService Publisher => Provider.GetRequiredService<IPublishService>();

    public IFeedStore Feeds => Provider.GetRequiredService<IFeedStore>();

    public IProjectPasswordStore Passwords => Provider.GetRequiredService<IProjectPasswordStore>();

    public ILegacyFeedMigrator Migrator => Provider.GetRequiredService<ILegacyFeedMigrator>();

    /// <summary>The folder a project of the name is created in by the wizard's default and by <see cref="ExistingProjectAsync" />.</summary>
    public string ProjectFolder(string name) => Paths.SuggestedProjectFolder(name);

    public string ProjectFile(string name) => Path.Combine(ProjectFolder(name), UpdateProject.FileName);

    /// <summary>Loads a project of the scenarios with its password.</summary>
    public Task<ProjectLoadResult> SavedAsync(string name) => Store.LoadAsync(ProjectFile(name), ProjectPassword);

    /// <summary>Shows the main window and lets it load the project list, as the application does on start.</summary>
    public async Task StartAsync()
    {
        Main.Show();
        await MainViewModel.InitializeAsync();
        User.Pump();
    }

    /// <summary>Waits for a window of the given type to open and returns it.</summary>
    public async Task<T> WindowAsync<T>() where T : Window
    {
        await User.WaitUntil(() => Dialogs.Open.OfType<T>().Any(w => w.IsVisible), $"a {typeof(T).Name} to open");
        var window = Dialogs.Open.OfType<T>().Last(w => w.IsVisible);
        window.UpdateLayout();
        return window;
    }

    /// <summary>Waits for a message box to open.</summary>
    public async Task<Popup> PopupAsync()
    {
        await User.WaitUntil(() => Dialogs.Open.OfType<MessageWindow>().Any(w => w.IsVisible), "a message box to open");
        return new Popup(Dialogs.Open.OfType<MessageWindow>().Last(w => w.IsVisible));
    }

    public Task ClosedAsync(Window window) =>
        User.WaitUntil(() => !window.IsVisible, $"the {window.GetType().Name} to close");

    public Task IdleAsync(DialogViewModel viewModel) =>
        User.WaitUntil(() => !viewModel.IsBusy, $"{viewModel.GetType().Name} to finish");

    /// <summary>The names in the main window's project list, as shown.</summary>
    public IReadOnlyList<string> ListedProjects =>
        Main.ProjectList.Items.Cast<ProjectCardViewModel>().Select(p => p.Name).ToList();

    /// <summary>Transfer settings for the container servers, already trusting their host key or certificate.</summary>
    public async Task<TransferSettings> TrustedTransferAsync(TransferProtocol protocol)
    {
        var settings = protocol == TransferProtocol.Sftp ? _context.SftpSettings() : _context.FtpSettings(protocol);
        await using var learn = await _context.ConnectTrustedAsync(settings, Credentials(protocol));
        await learn.ListAsync(string.Empty, false);
        return settings;
    }

    public static TransferCredentials Credentials(TransferProtocol protocol) =>
        protocol == TransferProtocol.Sftp ? IntegrationContext.SftpCredentials : IntegrationContext.FtpCredentials;

    public static string Password(TransferProtocol protocol) => protocol == TransferProtocol.Sftp
        ? ServerFixture.SftpPassword
        : ServerFixture.FtpPassword;

    /// <summary>Creates a project with the real services, as a user did in an earlier session.</summary>
    public async Task<ProjectLoadResult> ExistingProjectAsync(string name,
        TransferProtocol protocol = TransferProtocol.Sftp, bool statistics = false, bool saveCredentials = true)
    {
        var request = new NewProjectRequest
        {
            Name = name,
            Folder = ProjectFolder(name),
            UpdateUrl = _context.Server.HttpBaseUrl,
            Transfer = await TrustedTransferAsync(protocol),
            Secrets = new ProjectSecrets
            {
                TransferPassword = Password(protocol),
                StatisticsDatabasePassword = statistics ? ServerFixture.DbPassword : null
            },
            Statistics = statistics
                ? new StatisticsSettings
                {
                    Enabled = true,
                    Database = new StatisticsDatabaseSettings
                    { Host = "mysql", Name = ServerFixture.DbName, Username = ServerFixture.DbUser }
                }
                : new StatisticsSettings(),
            ProjectPassword = saveCredentials ? ProjectPassword : null,
            KeySize = 2048,
            TestConnection = false,
        };
        var created = await Projects.CreateAsync(request);
        await MainViewModel.RefreshAsync();
        return created;
    }

    /// <summary>Adds a package to a project with the real services; <paramref name="publish" /> uploads it as well.</summary>
    public async Task<UpdatePackage> ExistingPackageAsync(ProjectLoadResult project, string version, string description,
        bool publish)
    {
        var definition = new PackageDefinition(new UpdateVersion(version));
        definition.GetOrAddPlatform("any").Files.Add(new PackageFileEntry(PackageRoot.Program, "app.exe",
            _context.WriteFile($"{version}/app.exe", $"version {version}")));
        var request = new PublishRequest(project.Project, project.Secrets, definition)
        { Description = description, Publish = publish };
        request.Changelog[new CultureInfo("en")] = $"Changes in {version}.";
        return await Publisher.CreatePackageAsync(request);
    }

    /// <summary>Opens a listed project through the main window and returns its window.</summary>
    public async Task<ProjectWindow> OpenListedProjectAsync(string name)
    {
        ClickOpen(ListedProjects.ToList().IndexOf(name));
        return await WindowAsync<ProjectWindow>();
    }

    /// <summary>The card of a listed project in the start window.</summary>
    public ProjectCardViewModel Card(int index) => ((MainWindowViewModel)Main.DataContext!).VisibleProjects[index];

    /// <summary>Clicks Open on the card of a listed project.</summary>
    public void ClickOpen(int index) => User.Click(User.ButtonFor(Main.ProjectList, Card(index), "Open"));

    /// <summary>Chooses "Remove from list" in the menu of a project card.</summary>
    public void ClickRemove(int index)
    {
        var card = Card(index);
        User.Pump();
        var more = Main.ProjectList.GetVisualDescendants().OfType<Button>()
            .Single(b => ReferenceEquals(b.DataContext, card) && Equals(ToolTip.GetTip(b), "More actions"));
        User.Click(more);
        User.Click(((MenuFlyout)more.Flyout!).Items.OfType<MenuItem>().Single(m => Equals(m.Header, "Remove from list")));
    }

    public void Dispose()
    {
        foreach (var window in Dialogs.Open.ToList())
            window.Close();
        Main.Close();
        User.Pump();
        Provider.Dispose();
    }
}
#pragma warning restore CA1822

/// <summary>A message box as the user sees it: a title, a message and buttons.</summary>
public sealed class Popup(MessageWindow window)
{
    public MessageWindow Window { get; } = window;

    public string Title => Window.Title ?? string.Empty;

    public string Message => Window.Message;

    public IReadOnlyList<string> Buttons => Window.GetVisualDescendants().OfType<Button>()
        .Select(b => b.Content?.ToString() ?? string.Empty).ToList();

    public async Task ClickAsync(string button)
    {
        User.Click(User.ButtonWithText(Window, button));
        await User.WaitUntil(() => !Window.IsVisible, $"the message box \"{Title}\" to close");
    }
}
