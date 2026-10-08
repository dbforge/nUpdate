namespace nUpdate.Exceptions;

/// <summary>Thrown when a downloaded package does not match what the feed announced, for example its size or hash.</summary>
public class InvalidPackageException : Exception
{
    public InvalidPackageException()
    {
    }

    public InvalidPackageException(string message)
        : base(message)
    {
    }

    public InvalidPackageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
