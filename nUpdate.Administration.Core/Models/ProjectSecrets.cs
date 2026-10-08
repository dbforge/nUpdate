namespace nUpdate.Administration.Core.Models;

/// <summary>The secrets of an open project. In memory they are plain; in the project file they are encrypted under the project password.</summary>
public sealed class ProjectSecrets
{
    public string? TransferPassword { get; set; }

    public string? SftpKeyPassphrase { get; set; }

    public string? ProxyPassword { get; set; }

    public string? HttpAuthenticationPassword { get; set; }

    public string? StatisticsAdminSecret { get; set; }

    public string? StatisticsDatabasePassword { get; set; }

    /// <summary>The RSA private key as PEM.</summary>
    public string? PrivateKey { get; set; }

    /// <summary>A copy, for undoing edits.</summary>
    public ProjectSecrets Clone() => (ProjectSecrets)MemberwiseClone();

    /// <summary>Writes every secret into another instance.</summary>
    public void CopyTo(ProjectSecrets target)
    {
        ArgumentNullException.ThrowIfNull(target);
        target.TransferPassword = TransferPassword;
        target.SftpKeyPassphrase = SftpKeyPassphrase;
        target.ProxyPassword = ProxyPassword;
        target.HttpAuthenticationPassword = HttpAuthenticationPassword;
        target.StatisticsAdminSecret = StatisticsAdminSecret;
        target.StatisticsDatabasePassword = StatisticsDatabasePassword;
        target.PrivateKey = PrivateKey;
    }

    public TransferInterface.TransferCredentials ToTransferCredentials() => new()
    {
        Password = TransferPassword,
        SftpKeyPassphrase = SftpKeyPassphrase,
        ProxyPassword = ProxyPassword,
    };
}
