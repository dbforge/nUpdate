using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Services;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.ViewModels;

/// <summary>Edits <see cref="TransferSettings" /> plus the matching secrets; shared by the new-project and settings dialogs.</summary>
public partial class TransferSettingsEditorViewModel : ViewModelBase
{
    private readonly IProjectService _projects;
    private readonly IDialogService _dialogs;
    private readonly IFilePickerService _files;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFtp))]
    [NotifyPropertyChangedFor(nameof(IsSftp))]
    [NotifyPropertyChangedFor(nameof(IsPlugin))]
    private TransferProtocol _protocol = TransferProtocol.Sftp;

    [ObservableProperty]
    private string _host = string.Empty;

    [ObservableProperty]
    private int _port = 22;

    [ObservableProperty]
    private string _directory = "/";

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _usePassiveMode = true;

    [ObservableProperty]
    private string? _trustedCertificateFingerprint;

    [ObservableProperty]
    private string _sftpPrivateKeyPath = string.Empty;

    [ObservableProperty]
    private string _sftpKeyPassphrase = string.Empty;

    [ObservableProperty]
    private string? _trustedHostKeyFingerprint;

    [ObservableProperty]
    private string _pluginAssemblyPath = string.Empty;

    [ObservableProperty]
    private bool _useProxy;

    [ObservableProperty]
    private string _proxyAddress = string.Empty;

    [ObservableProperty]
    private string _proxyUsername = string.Empty;

    [ObservableProperty]
    private string _proxyPassword = string.Empty;

    [ObservableProperty]
    private string? _testResult;

    [ObservableProperty]
    private bool _isTesting;

    public TransferSettingsEditorViewModel(IProjectService projects, IDialogService dialogs, IFilePickerService files)
    {
        _projects = projects ?? throw new ArgumentNullException(nameof(projects));
        _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
        _files = files ?? throw new ArgumentNullException(nameof(files));
    }

    /// <summary>SFTP first: it is the recommended protocol and the default for new projects.</summary>
    public IReadOnlyList<TransferProtocol> Protocols { get; } =
        [TransferProtocol.Sftp, TransferProtocol.FtpsExplicit, TransferProtocol.FtpsImplicit, TransferProtocol.Ftp, TransferProtocol.Plugin];

    public bool IsFtp => Protocol is TransferProtocol.Ftp or TransferProtocol.FtpsExplicit or TransferProtocol.FtpsImplicit;

    public bool IsSftp => Protocol == TransferProtocol.Sftp;

    public bool IsPlugin => Protocol == TransferProtocol.Plugin;

    partial void OnProtocolChanged(TransferProtocol value)
    {
        Port = value switch
        {
            TransferProtocol.Sftp when Port is 21 or 990 => 22,
            TransferProtocol.FtpsImplicit when Port is 21 or 22 => 990,
            TransferProtocol.Ftp or TransferProtocol.FtpsExplicit when Port is 22 or 990 => 21,
            _ => Port,
        };
    }

    public void Load(TransferSettings settings, ProjectSecrets secrets)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(secrets);
        Protocol = settings.Protocol;
        Host = settings.Host;
        Port = settings.Port;
        Directory = settings.Directory;
        Username = settings.Username;
        Password = secrets.TransferPassword ?? string.Empty;
        UsePassiveMode = settings.UsePassiveMode;
        TrustedCertificateFingerprint = settings.TrustedCertificateFingerprint;
        SftpPrivateKeyPath = settings.SftpPrivateKeyPath ?? string.Empty;
        SftpKeyPassphrase = secrets.SftpKeyPassphrase ?? string.Empty;
        TrustedHostKeyFingerprint = settings.TrustedHostKeyFingerprint;
        PluginAssemblyPath = settings.PluginAssemblyPath ?? string.Empty;
        UseProxy = settings.Proxy is not null;
        ProxyAddress = settings.Proxy?.Address ?? string.Empty;
        ProxyUsername = settings.Proxy?.Username ?? string.Empty;
        ProxyPassword = secrets.ProxyPassword ?? string.Empty;
    }

    public TransferSettings ToSettings() => new()
    {
        Protocol = Protocol,
        Host = Host.Trim(),
        Port = Port,
        Directory = string.IsNullOrWhiteSpace(Directory) ? "/" : Directory.Trim(),
        Username = Username.Trim(),
        UsePassiveMode = UsePassiveMode,
        TrustedCertificateFingerprint = TrustedCertificateFingerprint,
        SftpPrivateKeyPath = string.IsNullOrWhiteSpace(SftpPrivateKeyPath) ? null : SftpPrivateKeyPath.Trim(),
        TrustedHostKeyFingerprint = TrustedHostKeyFingerprint,
        PluginAssemblyPath = string.IsNullOrWhiteSpace(PluginAssemblyPath) ? null : PluginAssemblyPath.Trim(),
        Proxy = UseProxy && !string.IsNullOrWhiteSpace(ProxyAddress)
            ? new ProxySettings { Address = ProxyAddress.Trim(), Username = string.IsNullOrWhiteSpace(ProxyUsername) ? null : ProxyUsername.Trim() }
            : null,
    };

    /// <summary>Writes the secret fields into the given secrets.</summary>
    public void ApplySecrets(ProjectSecrets secrets)
    {
        ArgumentNullException.ThrowIfNull(secrets);
        secrets.TransferPassword = string.IsNullOrEmpty(Password) ? null : Password;
        secrets.SftpKeyPassphrase = string.IsNullOrEmpty(SftpKeyPassphrase) ? null : SftpKeyPassphrase;
        secrets.ProxyPassword = string.IsNullOrEmpty(ProxyPassword) ? null : ProxyPassword;
    }

    /// <summary>A validation message, or <c>null</c> when the settings are complete.</summary>
    public string? Validate()
    {
        if (IsPlugin)
            return string.IsNullOrWhiteSpace(PluginAssemblyPath) ? "Choose the plugin assembly." : null;
        if (string.IsNullOrWhiteSpace(Host))
            return "Enter the server host name.";
        if (Port is < 1 or > 65535)
            return "The port must be between 1 and 65535.";
        if (string.IsNullOrWhiteSpace(Username))
            return "Enter the user name.";
        if (IsSftp && string.IsNullOrEmpty(Password) && string.IsNullOrWhiteSpace(SftpPrivateKeyPath))
            return "Enter a password or choose a private key file.";
        if (!IsSftp && string.IsNullOrEmpty(Password))
            return "Enter the password.";
        return null;
    }

    [RelayCommand]
    private async Task BrowseKeyFileAsync()
    {
        var path = await _files.PickFileAsync("Choose the SSH private key", FileTypeFilter.All);
        if (path is not null)
            SftpPrivateKeyPath = path;
    }

    [RelayCommand]
    private async Task BrowsePluginAsync()
    {
        var path = await _files.PickFileAsync("Choose the transfer plugin", new FileTypeFilter("Assemblies", "*.dll", "*.exe"), FileTypeFilter.All);
        if (path is not null)
            PluginAssemblyPath = path;
    }

    /// <summary>Tests the connection, offering to trust an unknown certificate or host key.</summary>
    [RelayCommand]
    public async Task<bool> TestConnectionAsync()
    {
        var problem = Validate();
        if (problem is not null)
        {
            TestResult = problem;
            return false;
        }

        IsTesting = true;
        TestResult = "Connecting...";
        try
        {
            while (true)
            {
                var settings = ToSettings();
                var secrets = new ProjectSecrets();
                ApplySecrets(secrets);
                try
                {
                    await _projects.TestConnectionAsync(settings, secrets.ToTransferCredentials());
                    TestResult = "Connection successful.";
                    return true;
                }
                catch (UntrustedServerException ex) when (ex.Fingerprint is not null)
                {
                    var what = IsSftp ? "host key" : "certificate";
                    var trust = await _dialogs.ConfirmAsync($"Unknown {what}",
                        $"{ex.Message}{Environment.NewLine}{Environment.NewLine}{ex.Subject}{Environment.NewLine}SHA-256: {ex.Fingerprint}{Environment.NewLine}{Environment.NewLine}Trust this {what}?", "Trust", "Cancel");
                    if (!trust)
                    {
                        TestResult = $"The {what} was not trusted.";
                        return false;
                    }

                    if (IsSftp)
                        TrustedHostKeyFingerprint = ex.Fingerprint;
                    else
                        TrustedCertificateFingerprint = ex.Fingerprint;
                }
                catch (Exception ex) when (ex is TransferException or InvalidOperationException or FileNotFoundException or NotSupportedException or ArgumentException or UriFormatException)
                {
                    TestResult = ex.Message;
                    return false;
                }
            }
        }
        finally
        {
            IsTesting = false;
        }
    }
}
