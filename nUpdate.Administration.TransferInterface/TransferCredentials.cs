namespace nUpdate.Administration.TransferInterface;

/// <summary>The unprotected secrets a provider needs at runtime. Never persisted.</summary>
public sealed class TransferCredentials
{
    public string? Password { get; set; }

    public string? SftpKeyPassphrase { get; set; }

    public string? ProxyPassword { get; set; }
}
