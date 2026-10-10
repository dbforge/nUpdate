using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Services;
using nUpdate.Security;

namespace nUpdate.Administration.ViewModels;

/// <summary>Collects everything for a new project in five wizard steps and creates it.</summary>
public partial class NewProjectViewModel : DialogViewModel
{
    private readonly IProjectService _projects;
    private readonly AdministrationPaths _paths;
    private readonly IFilePickerService _files;
    private bool _folderEdited;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGeneralStep))]
    [NotifyPropertyChangedFor(nameof(IsAuthenticationStep))]
    [NotifyPropertyChangedFor(nameof(IsTransferStep))]
    [NotifyPropertyChangedFor(nameof(IsStatisticsStep))]
    [NotifyPropertyChangedFor(nameof(IsSecurityStep))]
    [NotifyPropertyChangedFor(nameof(IsLastStep))]
    [NotifyPropertyChangedFor(nameof(ContinueText))]
    [NotifyCanExecuteChangedFor(nameof(BackCommand))]
    private int _step;

    [ObservableProperty] private string _name = string.Empty;

    /// <summary>The folder the project file and the packages are written to; follows the name until the user edits it.</summary>
    [ObservableProperty] private string _folder = string.Empty;

    [ObservableProperty] private string _updateUrl = "https://";

    [ObservableProperty] private bool _useHttpAuthentication;

    [ObservableProperty] private string _httpUsername = string.Empty;

    [ObservableProperty] private string _httpPassword = string.Empty;

    [ObservableProperty] private bool _saveCredentials = true;

    [ObservableProperty] private string _projectPassword = string.Empty;

    [ObservableProperty] private string _projectPasswordConfirmation = string.Empty;

    [ObservableProperty] private int _keySize = PackageSigning.DefaultKeySize;

    [ObservableProperty] private bool _testConnectionFirst = true;

    public NewProjectViewModel(IProjectService projects, AdministrationPaths paths, IFilePickerService files,
        TransferSettingsEditorViewModel transfer, StatisticsSettingsEditorViewModel statistics)
    {
        _projects = projects ?? throw new ArgumentNullException(nameof(projects));
        _paths = paths ?? throw new ArgumentNullException(nameof(paths));
        _files = files ?? throw new ArgumentNullException(nameof(files));
        Transfer = transfer ?? throw new ArgumentNullException(nameof(transfer));
        Statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        Title = "New project";
        Steps =
        [
            new WizardStep(1, "General"), new WizardStep(2, "Authentication"), new WizardStep(3, "Transfer"),
            new WizardStep(4, "Statistics"), new WizardStep(5, "Security")
        ];
        Steps[0].IsCurrent = true;
        Folder = paths.DefaultProjectsDirectory;
    }

    public IReadOnlyList<WizardStep> Steps { get; }

    public bool IsGeneralStep => Step == 0;

    public bool IsAuthenticationStep => Step == 1;

    public bool IsTransferStep => Step == 2;

    public bool IsStatisticsStep => Step == 3;

    public bool IsSecurityStep => Step == 4;

    public bool IsLastStep => Step == Steps.Count - 1;

    public string ContinueText => IsLastStep ? "Create project" : "Continue";

    public TransferSettingsEditorViewModel Transfer { get; }

    public StatisticsSettingsEditorViewModel Statistics { get; }

    public IReadOnlyList<int> KeySizes { get; } = [2048, 4096, 8192];

    /// <summary>The created project, set when the dialog was accepted.</summary>
    public ProjectLoadResult? Result { get; private set; }

    public string? Validate() =>
        ValidateStep(0) ?? ValidateStep(1) ?? ValidateStep(2) ?? ValidateStep(3) ?? ValidateStep(4);

    /// <summary>Validates one wizard step so the user is told about a problem before moving on.</summary>
    public string? ValidateStep(int step)
    {
        if (step == 0)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return "Enter a project name.";
            if (Name.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return "The project name must be usable as a folder name.";
            if (string.IsNullOrWhiteSpace(Folder))
                return "Choose the folder of the project.";
            return UpdateProject.IsValidUpdateUrl(UpdateUrl)
                ? null
                : "Enter the absolute HTTP(S) URL under which the updates are served.";
        }

        if (step == 1)
            return UseHttpAuthentication && string.IsNullOrWhiteSpace(HttpUsername)
                ? "Enter the user name for the HTTP authentication."
                : null;
        if (step == 2)
            return Transfer.Validate();
        if (step == 3)
            return Statistics.Validate();
        return step == 4
            ? ProjectPasswordViewModel.Validate(SaveCredentials, ProjectPassword, ProjectPasswordConfirmation)
            : null;
    }

    partial void OnNameChanged(string value)
    {
        if (!_folderEdited)
            Folder = _paths.SuggestedProjectFolder(value.Trim());
    }

    /// <summary>Once the user has chosen a folder it no longer follows the name.</summary>
    partial void OnFolderChanged(string value)
    {
        if (!string.Equals(value, _paths.SuggestedProjectFolder(Name.Trim()), StringComparison.Ordinal) &&
            !string.Equals(value, _paths.DefaultProjectsDirectory, StringComparison.Ordinal))
            _folderEdited = true;
    }

    partial void OnStepChanged(int value)
    {
        ErrorMessage = null;
        for (var i = 0; i < Steps.Count; i++)
        {
            Steps[i].IsCurrent = i == value;
            Steps[i].IsDone = i < value;
        }
    }

    [RelayCommand]
    private async Task BrowseFolderAsync()
    {
        var folder = await _files.PickFolderAsync("Choose the project folder");
        if (folder is not null)
        {
            Folder = folder;
            _folderEdited = true;
        }
    }

    private bool CanGoBack() => Step > 0;

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void Back() => Step--;

    /// <summary>Moves to the next step after validating the current one; on the last step it creates the project.</summary>
    [RelayCommand]
    private async Task ContinueAsync()
    {
        if (IsLastStep)
        {
            await CreateAsync();
            return;
        }

        ErrorMessage = ValidateStep(Step);
        if (ErrorMessage is null)
            Step++;
    }

    [RelayCommand]
    private async Task CreateAsync()
    {
        ErrorMessage = Validate();
        if (ErrorMessage is not null)
            return;

        if (TestConnectionFirst && !await Transfer.TestConnectionAsync())
        {
            ErrorMessage = Transfer.TestResult;
            return;
        }

        var secrets = new ProjectSecrets
        {
            HttpAuthenticationPassword =
                UseHttpAuthentication && !string.IsNullOrEmpty(HttpPassword) ? HttpPassword : null
        };
        Transfer.ApplySecrets(secrets);
        Statistics.ApplySecrets(secrets);
        var request = new NewProjectRequest
        {
            Name = Name.Trim(),
            Folder = Folder.Trim(),
            UpdateUrl = UpdateUrl.Trim(),
            Transfer = Transfer.ToSettings(),
            Secrets = secrets,
            HttpAuthentication = UseHttpAuthentication
                ? new HttpAuthenticationSettings { Username = HttpUsername.Trim() }
                : null,
            Statistics = Statistics.ToSettings(),
            ProjectPassword = SaveCredentials ? ProjectPassword : null,
            KeySize = KeySize,
            TestConnection = false,
        };

        if (await RunBusyAsync("Creating the project...",
                async progress => Result = await _projects.CreateAsync(request, progress)))
            Close(true);
    }
}
