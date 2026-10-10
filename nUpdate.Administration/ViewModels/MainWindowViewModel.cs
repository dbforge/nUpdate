using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Services;
using nUpdate.Administration.TransferInterface;
using nUpdate.Exceptions;

namespace nUpdate.Administration.ViewModels;

/// <summary>A known project as the start window shows it: its registration and what its project file says.</summary>
public sealed partial class ProjectCardViewModel(ProjectRegistration registration) : ViewModelBase
{
    public ProjectRegistration Registration { get; } = registration ?? throw new ArgumentNullException(nameof(registration));

    public Guid Id => Registration.Id;

    public string Name => Registration.Name;

    public string Path => Registration.Path;

    /// <summary>Up to two letters for the project's tile.</summary>
    public string Initials => InitialsOf(Name);

    /// <summary>The first letters of the first two words of a project name, the stand-in for a project icon.</summary>
    public static string InitialsOf(string name) =>
        string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(word => char.ToUpperInvariant(word[0])));

    [ObservableProperty] private string _updateUrl = string.Empty;

    /// <summary>The newest released version, or what stands in for it ("Not published yet", "No packages yet").</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowsUnreleased))]
    private string _latest = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowsUnreleased))]
    private bool _hasRelease;

    /// <summary>Whether the label for a project without a release shows ("Not published yet", "No packages yet").</summary>
    public bool ShowsUnreleased => !HasRelease && Latest.Length > 0;

    [ObservableProperty] private string _packages = string.Empty;

    [ObservableProperty] private string _protocol = string.Empty;

    /// <summary>Why the project file could not be read, or <c>null</c>.</summary>
    [ObservableProperty] private string? _problem;

    /// <summary>Fills the details from the project file; a file that cannot be read becomes <see cref="Problem" />.</summary>
    public void Describe(UpdateProject? project, string? problem = null)
    {
        Problem = problem;
        if (project is null)
            return;
        UpdateUrl = project.UpdateUrl;
        var released = project.Packages.Where(p => p.Released).Select(p => p.Version).OrderByDescending(v => v).FirstOrDefault();
        HasRelease = released is not null;
        Latest = released is not null ? $"{released} released" : project.Packages.Count > 0 ? "Not published yet" : "No packages yet";
        Packages = project.Packages.Count == 1 ? "1 package" : $"{project.Packages.Count} packages";
        Protocol = project.Transfer.Protocol switch
        {
            TransferProtocol.Ftp => "FTP",
            TransferProtocol.FtpsExplicit => "FTPS",
            TransferProtocol.FtpsImplicit => "FTPS (implicit)",
            TransferProtocol.Sftp => "SFTP",
            _ => "Plugin",
        };
    }
}

/// <summary>The project list: open, create and forget projects.</summary>
public partial class MainWindowViewModel(
    IProjectStore store,
    IProjectService projects,
    IProjectPasswordStore passwords,
    IDialogService dialogs,
    IFilePickerService files,
    ViewModelFactory factory)
    : ViewModelBase
{
    private readonly IProjectStore _store = store ?? throw new ArgumentNullException(nameof(store));
    private readonly IProjectService _projects = projects ?? throw new ArgumentNullException(nameof(projects));
    private readonly IProjectPasswordStore _passwords = passwords ?? throw new ArgumentNullException(nameof(passwords));
    private readonly IDialogService _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
    private readonly IFilePickerService _files = files ?? throw new ArgumentNullException(nameof(files));
    private readonly ViewModelFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <summary>The files of the projects whose windows are open, so that a project is not opened twice.</summary>
    private readonly HashSet<string> _openProjectFiles = new(StringComparer.Ordinal);

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenSelectedCommand))]
    [NotifyCanExecuteChangedFor(nameof(ForgetSelectedCommand))]
    private ProjectCardViewModel? _selectedProject;

    [ObservableProperty] private string _status = string.Empty;

    /// <summary>Filters <see cref="VisibleProjects" /> by name, folder and update URL.</summary>
    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<ProjectCardViewModel> Projects { get; } = [];

    /// <summary>The projects that match <see cref="SearchText" />.</summary>
    public ObservableCollection<ProjectCardViewModel> VisibleProjects { get; } = [];

    public bool HasProjects => Projects.Count > 0;

    public bool HasNoProjects => Projects.Count == 0;

    partial void OnSearchTextChanged(string value) => Filter();

    public string Version =>
        $"nUpdate Administration {typeof(MainWindowViewModel).Assembly.GetName().Version?.ToString(3)}";

    public async Task InitializeAsync(string? projectPath = null)
    {
        await RefreshAsync();
        if (projectPath is not null)
            await OpenProjectAsync(projectPath);
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        try
        {
            var entries = await _store.ListAsync();
            Projects.Clear();
            foreach (var entry in entries)
                Projects.Add(new ProjectCardViewModel(entry));

            Status = Projects.Count == 1 ? "1 project" : $"{Projects.Count} projects";
            OnPropertyChanged(nameof(HasProjects));
            OnPropertyChanged(nameof(HasNoProjects));
            Filter();
            // The cards show at once; what each project contains follows as its file is read.
            foreach (var card in Projects.ToList())
                await DescribeAsync(card);
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnsupportedFormatException
                                       or Newtonsoft.Json.JsonException)
        {
            await _dialogs.ShowErrorAsync("Error while reading the project list", ex.Message);
        }
    }

    [RelayCommand]
    private async Task NewProjectAsync()
    {
        var viewModel = _factory.Create<NewProjectViewModel>();
        if (await _dialogs.ShowDialogAsync(viewModel) && viewModel.Result is { } result)
        {
            await RefreshAsync();
            OpenProjectWindow(result, result.Project.Path);
        }
    }

    [RelayCommand]
    private async Task OpenProjectFileAsync()
    {
        var path = await _files.PickFileAsync("Open project", FileTypeFilter.Project, FileTypeFilter.All);
        if (path is not null)
            await OpenProjectAsync(path);
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private Task OpenSelectedAsync() => OpenProjectAsync(SelectedProject!.Path);

    /// <summary>The Open button of a project card.</summary>
    [RelayCommand]
    private Task OpenProjectCardAsync(ProjectCardViewModel card)
    {
        SelectedProject = card;
        return OpenProjectAsync(card.Path);
    }

    /// <summary>"Remove from list" in the menu of a project card.</summary>
    [RelayCommand]
    private Task ForgetProjectCardAsync(ProjectCardViewModel card)
    {
        SelectedProject = card;
        return ForgetSelectedAsync();
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task ForgetSelectedAsync()
    {
        var selected = SelectedProject!;
        if (!await _dialogs.ConfirmAsync("Remove from list",
                $"Remove \"{selected.Name}\" from the list? The project folder stays where it is.", "Remove"))
            return;
        try
        {
            if (selected.Id == Guid.Empty)
            {
                // A file of an earlier version that was never opened with this one is only known by its path.
                await _store.UnregisterPathAsync(selected.Path);
            }
            else
            {
                await _store.UnregisterAsync(selected.Id);
                await _passwords.RemoveAsync(selected.Id);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            await _dialogs.ShowErrorAsync("Error while updating the project list", ex.Message);
        }

        await RefreshAsync();
    }

    /// <summary>
    ///     Loads a project file, unlocks its secrets with the remembered or entered project password (or asks for the
    ///     secrets themselves when the file holds none), converts files of earlier versions and shows the project window.
    /// </summary>
    public async Task OpenProjectAsync(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (_openProjectFiles.Contains(FullPath(path)))
        {
            await _dialogs.ShowInfoAsync("Project already open", "This project is already open in another window.");
            return;
        }

        ProjectLoadResult? loaded;
        try
        {
            loaded = await LoadAsync(path);
        }
        catch (Exception ex) when (IsLoadError(ex))
        {
            await _dialogs.ShowErrorAsync("Error while opening the project", ex.Message);
            return;
        }

        if (loaded is null)
            return;

        if (loaded.SecretsState == SecretsState.Unreadable && loaded.Migrated)
        {
            await _dialogs.ShowInfoAsync("Cannot read the project's credentials",
                "The stored credentials were protected by another user or machine, or with another password. Enter them again.");
        }

        if (!ProjectSecretsProtection.IsComplete(loaded.Project, loaded.Secrets))
        {
            var credentials =
                _factory.Create<CredentialsViewModel>(loaded.Project, loaded.Secrets, CredentialsMode.Secrets);
            if (!await _dialogs.ShowDialogAsync(credentials))
                return;
        }

        if (loaded.Migrated)
        {
            await _dialogs.ShowInfoAsync("Project converted",
                "The project was created with an earlier version of nUpdate Administration and has been converted to the new format. Choose how its credentials are stored from now on.");
            var password = _factory.Create<ProjectPasswordViewModel>();
            if (!await _dialogs.ShowDialogAsync(password))
                return;
            try
            {
                await _projects.SaveMigratedAsync(loaded.Project, loaded.Secrets, password.Result);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                await _dialogs.ShowErrorAsync("Error while saving the converted project", ex.Message);
                return;
            }
        }
        else
        {
            await _store.RegisterAsync(new ProjectRegistration(loaded.Project.Id, loaded.Project.Name,
                loaded.Project.Path));
        }

        await RefreshAsync();
        OpenProjectWindow(loaded, path);
    }

    /// <summary>Loads the file; for a project with encrypted secrets, tries the remembered password and then asks for it. <c>null</c> when the user cancelled.</summary>
    private async Task<ProjectLoadResult?> LoadAsync(string path)
    {
        var loaded = await _store.LoadAsync(path);
        if (loaded.SecretsState != SecretsState.PasswordRequired)
            return loaded;

        var remembered = await _passwords.GetAsync(loaded.Project.Id);
        if (remembered is not null)
        {
            var unlocked = await _store.LoadAsync(path, remembered);
            if (unlocked.SecretsState == SecretsState.Loaded)
                return unlocked;
        }

        var credentials =
            _factory.Create<CredentialsViewModel>(loaded.Project, loaded.Secrets, CredentialsMode.ProjectPassword);
        if (!await _dialogs.ShowDialogAsync(credentials))
            return null;
        if (credentials.EnteredPassword is { } password && credentials.RememberPassword)
            await _passwords.SetAsync(loaded.Project.Id, password);
        return new ProjectLoadResult(loaded.Project, loaded.Secrets, migrated: false, SecretsState.Loaded);
    }

    private bool HasSelection() => SelectedProject is not null;

    /// <summary>Reads the project file for the card's details; the card stays without them when it cannot be read.</summary>
    private async Task DescribeAsync(ProjectCardViewModel card)
    {
        try
        {
            card.Describe((await _store.LoadAsync(card.Path))?.Project);
        }
        catch (Exception ex) when (IsLoadError(ex))
        {
            card.Describe(null, ex.Message);
        }
    }

    private void Filter()
    {
        var words = SearchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        VisibleProjects.Clear();
        foreach (var card in Projects.Where(card => words.All(word => Matches(card, word))))
            VisibleProjects.Add(card);
    }

    private static bool Matches(ProjectCardViewModel card, string word) =>
        card.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || card.Path.Contains(word, StringComparison.OrdinalIgnoreCase)
        || card.UpdateUrl.Contains(word, StringComparison.OrdinalIgnoreCase);

    private void OpenProjectWindow(ProjectLoadResult loaded, string openedPath) =>
        _ = ShowProjectWindowAsync(loaded, openedPath);

    /// <summary>Shows the project window and refreshes the list when the project is renamed and once the window closes, since it may have been deleted.</summary>
    private async Task ShowProjectWindowAsync(ProjectLoadResult loaded, string openedPath)
    {
        // A converted project is known by the file that was opened and by the file it was written to.
        var files = new[] { FullPath(openedPath), FullPath(loaded.Project.Path) };
        foreach (var file in files)
            _openProjectFiles.Add(file);
        try
        {
            await ShowProjectWindowCoreAsync(loaded);
        }
        finally
        {
            foreach (var file in files)
                _openProjectFiles.Remove(file);
        }
    }

    private async Task ShowProjectWindowCoreAsync(ProjectLoadResult loaded)
    {
        var viewModel = _factory.Create<ProjectViewModel>(loaded.Project, loaded.Secrets);
        var name = viewModel.Name;
        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ProjectViewModel.Name) && viewModel.Name != name)
            {
                name = viewModel.Name;
                _ = RefreshAsync();
            }
        };
        await _dialogs.ShowWindowAsync(viewModel);
        await RefreshAsync();
    }

    private static string FullPath(string path)
    {
        try
        {
            return Path.GetFullPath(path);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return path;
        }
    }

    private static bool IsLoadError(Exception ex) =>
        ex is IOException or UnauthorizedAccessException or InvalidDataException or UnsupportedFormatException
            or FormatException or Newtonsoft.Json.JsonException;
}
