namespace nUpdate.UpdateInstaller;

/// <summary>Thrown when a file could not be replaced because another process keeps it open.</summary>
public class LockedFileException : IOException
{
    public LockedFileException()
    {
    }

    public LockedFileException(string message)
        : base(message)
    {
    }

    public LockedFileException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public LockedFileException(string message, string filePath, Exception? innerException)
        : base(message, innerException)
    {
        FilePath = filePath;
    }

    public string? FilePath { get; }
}
