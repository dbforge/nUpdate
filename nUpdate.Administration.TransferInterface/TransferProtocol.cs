namespace nUpdate.Administration.TransferInterface;

/// <summary>The protocol a project uploads its files with.</summary>
public enum TransferProtocol
{
    /// <summary>Plain FTP.</summary>
    Ftp,

    /// <summary>FTP upgraded to TLS after connecting (AUTH TLS, usually port 21).</summary>
    FtpsExplicit,

    /// <summary>FTP over TLS from the first byte (usually port 990).</summary>
    FtpsImplicit,

    /// <summary>SFTP over SSH (usually port 22).</summary>
    Sftp,

    /// <summary>A provider from a plugin assembly.</summary>
    Plugin,
}
