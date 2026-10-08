using System.Security.Cryptography;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Services;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.ViewModels;

/// <summary>What the credentials dialog asks for.</summary>
public enum CredentialsMode
{
    /// <summary>The project password that decrypts the secrets stored in the project file.</summary>
    ProjectPassword,

    /// <summary>The secrets themselves, for a file that holds none or one whose secrets are incomplete.</summary>
    Secrets,
}

/// <summary>
///     Unlocks the secrets of a project: with the project password when the file stores them encrypted, or by asking
///     for each secret when the file holds none (or not all of them).
/// </summary>
public partial class CredentialsViewModel : DialogViewModel
{
    private readonly IFilePickerService _files;

    [ObservableProperty]
    private string _projectPassword = string.Empty;

    [ObservableProperty]
    private bool _rememberPassword = true;

    [ObservableProperty]
    private string _transferPassword = string.Empty;

    [ObservableProperty]
    private string _sftpKeyPassphrase = string.Empty;

    [ObservableProperty]
    private string _proxyPassword = string.Empty;

    [ObservableProperty]
    private string _httpPassword = string.Empty;

    [ObservableProperty]
    private string _statisticsAdminSecret = string.Empty;

    [ObservableProperty]
    private string _privateKey = string.Empty;

    public CredentialsViewModel(IFilePickerService files, UpdateProject project, ProjectSecrets secrets, CredentialsMode mode)
    {
        _files = files ?? throw new ArgumentNullException(nameof(files));
        Project = project ?? throw new ArgumentNullException(nameof(project));
        Secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));
        if (mode == CredentialsMode.ProjectPassword && string.IsNullOrEmpty(project.Secrets))
            throw new ArgumentException("The project file holds no encrypted secrets, so there is no project password to ask for.", nameof(mode));
        Mode = mode;
        Title = AsksForProjectPassword ? $"Unlock {project.Name}" : $"Credentials for {project.Name}";
        TransferPassword = secrets.TransferPassword ?? string.Empty;
        SftpKeyPassphrase = secrets.SftpKeyPassphrase ?? string.Empty;
        ProxyPassword = secrets.ProxyPassword ?? string.Empty;
        HttpPassword = secrets.HttpAuthenticationPassword ?? string.Empty;
        StatisticsAdminSecret = secrets.StatisticsAdminSecret ?? string.Empty;
        PrivateKey = secrets.PrivateKey ?? string.Empty;
    }

    public UpdateProject Project { get; }

    public ProjectSecrets Secrets { get; }

    public CredentialsMode Mode { get; }

    /// <summary>The file holds encrypted secrets, so only the project password is needed.</summary>
    public bool AsksForProjectPassword => Mode == CredentialsMode.ProjectPassword;

    public bool AsksForSecrets => !AsksForProjectPassword;

    /// <summary>The project password that unlocked the secrets, so the caller can remember it.</summary>
    public string? EnteredPassword { get; private set; }

    public bool NeedsSftpPassphrase => Project.Transfer.Protocol == TransferProtocol.Sftp && !string.IsNullOrEmpty(Project.Transfer.SftpPrivateKeyPath);

    public bool NeedsProxyPassword => Project.Transfer.Proxy is not null;

    public bool NeedsHttpPassword => Project.HttpAuthentication is not null;

    public bool NeedsStatisticsSecret => Project.Statistics.Enabled;

    [RelayCommand]
    private async Task LoadPrivateKeyAsync()
    {
        var path = await _files.PickFileAsync("Choose the private key file (PEM)", new FileTypeFilter("Key files", "*.pem", "*.key", "*.txt"), FileTypeFilter.All);
        if (path is null)
            return;
        try
        {
            PrivateKey = await File.ReadAllTextAsync(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task AcceptAsync()
    {
        if (AsksForProjectPassword)
        {
            if (ProjectPassword.Length == 0)
            {
                ErrorMessage = "Enter the project password.";
                return;
            }

            try
            {
                // The key derivation takes a moment by design; it must not freeze the dialog.
                var blob = Project.Secrets!;
                var password = ProjectPassword;
                var unlocked = await Task.Run(() => ProjectSecretsProtection.Unprotect(blob, password));
                unlocked.CopyTo(Secrets);
            }
            catch (Exception ex) when (ex is CryptographicException or InvalidDataException)
            {
                ErrorMessage = "The project password is wrong.";
                return;
            }

            EnteredPassword = ProjectPassword;
            Close(true);
            return;
        }

        // Passwords are taken as typed; only the pasted key is trimmed.
        Secrets.TransferPassword = Null(TransferPassword);
        Secrets.SftpKeyPassphrase = Null(SftpKeyPassphrase);
        Secrets.ProxyPassword = Null(ProxyPassword);
        Secrets.HttpAuthenticationPassword = Null(HttpPassword);
        Secrets.StatisticsAdminSecret = Null(StatisticsAdminSecret.Trim());
        Secrets.PrivateKey = Null(PrivateKey.Trim());
        if (!ProjectSecretsProtection.IsComplete(Project, Secrets))
        {
            ErrorMessage = "The transfer password (or SFTP key), the private key and, if statistics are enabled, the admin secret are required.";
            return;
        }

        Close(true);
    }

    private static string? Null(string value) => string.IsNullOrEmpty(value) ? null : value;
}
