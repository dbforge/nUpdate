namespace nUpdate.Administration.TransferInterface;

/// <summary>Where and how a project's files are uploaded. The secrets live in <see cref="TransferCredentials" />.</summary>
public sealed class TransferSettings
{
    public TransferProtocol Protocol { get; set; } = TransferProtocol.Sftp;

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 22;

    /// <summary>The remote directory that corresponds to the update URL.</summary>
    public string Directory { get; set; } = "/";

    public string Username { get; set; } = string.Empty;

    /// <summary>FTP only: passive mode (the client opens the data connection).</summary>
    public bool UsePassiveMode { get; set; } = true;

    /// <summary>FTPS only: the SHA-256 fingerprint of a server certificate the user chose to trust explicitly.</summary>
    public string? TrustedCertificateFingerprint { get; set; }

    /// <summary>SFTP only: path of a private key file used instead of or in addition to the password.</summary>
    public string? SftpPrivateKeyPath { get; set; }

    /// <summary>SFTP only: the SHA-256 fingerprint of the server's host key, learned on first connect.</summary>
    public string? TrustedHostKeyFingerprint { get; set; }

    /// <summary>Plugin only: the assembly providing the <see cref="ITransferProviderFactory" />.</summary>
    public string? PluginAssemblyPath { get; set; }

    public ProxySettings? Proxy { get; set; }
}
