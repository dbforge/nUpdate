using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Services;

namespace nUpdate.Administration.ViewModels;

/// <summary>Edits the settings of an existing project.</summary>
public partial class ProjectSettingsViewModel : DialogViewModel
{
    private readonly IProjectService _projects;
    private readonly IProjectPasswordStore _passwords;
    private readonly ViewModelFactory _factory;
    private readonly IDialogService _dialogs;

    [ObservableProperty] private string _name;

    [ObservableProperty] private string _updateUrl;

    [ObservableProperty] private bool _useHttpAuthentication;

    [ObservableProperty] private string _httpUsername;

    [ObservableProperty] private string _httpPassword;

    [ObservableProperty] private bool _saveCredentials;

    /// <summary>A new project password; empty keeps the remembered one.</summary>
    [ObservableProperty] private string _projectPassword = string.Empty;

    [ObservableProperty] private string _projectPasswordConfirmation = string.Empty;

    [ObservableProperty] private string? _assemblyVersionPath;

    public ProjectSettingsViewModel(IProjectService projects, IProjectPasswordStore passwords, ViewModelFactory factory,
        IDialogService dialogs,
        TransferSettingsEditorViewModel transfer, StatisticsSettingsEditorViewModel statistics, UpdateProject project,
        ProjectSecrets secrets)
    {
        _projects = projects ?? throw new ArgumentNullException(nameof(projects));
        _passwords = passwords ?? throw new ArgumentNullException(nameof(passwords));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
        Transfer = transfer ?? throw new ArgumentNullException(nameof(transfer));
        Statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        Project = project ?? throw new ArgumentNullException(nameof(project));
        Secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));
        Title = $"Settings of {project.Name}";
        _name = project.Name;
        _updateUrl = project.UpdateUrl;
        _useHttpAuthentication = project.HttpAuthentication is not null;
        _httpUsername = project.HttpAuthentication?.Username ?? string.Empty;
        _httpPassword = secrets.HttpAuthenticationPassword ?? string.Empty;
        _saveCredentials = !string.IsNullOrEmpty(project.Secrets);
        _assemblyVersionPath = project.AssemblyVersionPath;
        Transfer.Load(project.Transfer, secrets);
        Statistics.Load(project.Statistics, secrets);
    }

    public TransferSettingsEditorViewModel Transfer { get; }

    public StatisticsSettingsEditorViewModel Statistics { get; }

    public UpdateProject Project { get; }

    public ProjectSecrets Secrets { get; }

    public string Folder => Project.Folder;

    /// <summary>The password fields are only needed when the file holds no secrets yet or the password is to be changed.</summary>
    public string PasswordHint => string.IsNullOrEmpty(Project.Secrets)
        ? "Choose a project password."
        : "Leave the password empty to keep the current one.";

    public string? Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return "Enter a project name.";
        if (!UpdateProject.IsValidUpdateUrl(UpdateUrl))
            return "Enter the absolute HTTP(S) URL under which the updates are served.";
        if (UseHttpAuthentication && string.IsNullOrWhiteSpace(HttpUsername))
            return "Enter the user name for the HTTP authentication.";
        if (SaveCredentials && (ProjectPassword.Length > 0 || string.IsNullOrEmpty(Project.Secrets)))
            return ProjectPasswordViewModel.Validate(true, ProjectPassword, ProjectPasswordConfirmation) ??
                   Transfer.Validate() ?? Statistics.Validate();
        return Transfer.Validate() ?? Statistics.Validate();
    }

    /// <summary>Applies the edits to the project, saves and renames it; when anything fails the project is restored.</summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = Validate();
        if (ErrorMessage is not null)
            return;

        var snapshot = new Snapshot(Project, Secrets);
        var statisticsWereEnabled = Project.Statistics.Enabled;
        var ok = await RunBusyAsync("Saving...", async _ =>
        {
            var password = await ResolvePasswordAsync();
            Project.UpdateUrl = UpdateProject.NormalizeUpdateUrl(UpdateUrl);
            Project.Transfer = Transfer.ToSettings();
            Project.HttpAuthentication = UseHttpAuthentication
                ? new HttpAuthenticationSettings { Username = HttpUsername.Trim() }
                : null;
            Project.Statistics = Statistics.ToSettings();
            Project.AssemblyVersionPath =
                string.IsNullOrWhiteSpace(AssemblyVersionPath) ? null : AssemblyVersionPath.Trim();
            Transfer.ApplySecrets(Secrets);
            Statistics.ApplySecrets(Secrets);
            Secrets.HttpAuthenticationPassword =
                UseHttpAuthentication && !string.IsNullOrEmpty(HttpPassword) ? HttpPassword : null;
            try
            {
                // The rename goes first: it is checked against the project list and only touches the name, so a
                // failure further down leaves a consistently named project behind.
                if (!string.Equals(Name.Trim(), Project.Name, StringComparison.Ordinal))
                    await _projects.RenameAsync(Project, Name.Trim());
                if (Project.Statistics.Enabled && !statisticsWereEnabled)
                    await _projects.SetupStatisticsAsync(Project, Secrets);
                await _projects.SaveAsync(Project, Secrets, password);
            }
            catch (Exception)
            {
                snapshot.Restore(Project, Secrets);
                throw;
            }
        });
        if (ok)
            Close(true);
    }

    /// <summary>The password the secrets are saved under: the new one, else the remembered one; <c>null</c> when credentials are not saved.</summary>
    private async Task<string?> ResolvePasswordAsync()
    {
        if (!SaveCredentials)
            return null;
        if (ProjectPassword.Length > 0)
            return ProjectPassword;
        return await _passwords.GetAsync(Project.Id) ??
               throw new InvalidOperationException(
                   "The project password is not known on this machine. Enter a new project password.");
    }

    /// <summary>Opens the migration assistant from the settings, for projects where it was postponed.</summary>
    [RelayCommand]
    private async Task MigrateAsync()
    {
        var assistant = _factory.Create<MigrationViewModel>(Project, Secrets);
        await _dialogs.ShowDialogAsync(assistant);
        Migrated |= assistant.Migrated;
    }

    /// <summary>The editable state of a project and its secrets, so a failed save can be undone in memory.</summary>
    private sealed class Snapshot(UpdateProject project, ProjectSecrets secrets)
    {
        private readonly string _updateUrl = project.UpdateUrl;
        private readonly TransferInterface.TransferSettings _transfer = project.Transfer;
        private readonly HttpAuthenticationSettings? _httpAuthentication = project.HttpAuthentication;
        private readonly StatisticsSettings _statistics = project.Statistics;
        private readonly string? _secretsBlob = project.Secrets;
        private readonly string? _assemblyVersionPath = project.AssemblyVersionPath;
        private readonly ProjectSecrets _secrets = secrets.Clone();

        public void Restore(UpdateProject project, ProjectSecrets secrets)
        {
            project.UpdateUrl = _updateUrl;
            project.Transfer = _transfer;
            project.HttpAuthentication = _httpAuthentication;
            project.Statistics = _statistics;
            project.Secrets = _secretsBlob;
            project.AssemblyVersionPath = _assemblyVersionPath;
            _secrets.CopyTo(secrets);
        }
    }

    [RelayCommand]
    private async Task DeleteProjectAsync()
    {
        if (!await _dialogs.ConfirmAsync("Delete project",
                $"Delete the project \"{Project.Name}\"? This removes it from the list and deletes its folder.",
                "Delete"))
            return;
        var deleteServerFiles = await _dialogs.ConfirmAsync("Delete server files",
            "Also delete the published packages, the feed, the statistics script and any legacy files from the server? Clients will no longer find updates.",
            "Delete on server", "Keep");
        var ok = await RunBusyAsync("Deleting...",
            _ => _projects.DeleteAsync(Project, Secrets, deleteLocalFiles: true, deleteServerFiles: deleteServerFiles));
        if (ok)
        {
            Deleted = true;
            Close(true);
        }
    }

    public bool Deleted { get; private set; }

    /// <summary>True when the legacy feed was migrated from this dialog, so the project window re-checks the server.</summary>
    public bool Migrated { get; private set; }
}
