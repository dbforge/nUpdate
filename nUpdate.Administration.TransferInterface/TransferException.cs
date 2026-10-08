namespace nUpdate.Administration.TransferInterface;

/// <summary>A transfer failed. The message is suitable for showing to the user.</summary>
public class TransferException : Exception
{
    public TransferException()
    {
    }

    public TransferException(string message)
        : base(message)
    {
    }

    public TransferException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>The server presented a certificate or host key the user has not trusted yet.</summary>
public class UntrustedServerException : TransferException
{
    public UntrustedServerException()
    {
    }

    public UntrustedServerException(string message)
        : base(message)
    {
    }

    public UntrustedServerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public UntrustedServerException(string message, string fingerprint, string subject)
        : base(message)
    {
        Fingerprint = fingerprint;
        Subject = subject;
    }

    /// <summary>The SHA-256 fingerprint the user can choose to trust.</summary>
    public string? Fingerprint { get; }

    /// <summary>A description of the certificate or key, for the confirmation dialog.</summary>
    public string? Subject { get; }
}
