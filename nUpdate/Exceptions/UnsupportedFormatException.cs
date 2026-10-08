namespace nUpdate.Exceptions;

/// <summary>Thrown when a feed, package or options document has a format version this nUpdate does not read.</summary>
public class UnsupportedFormatException : Exception
{
    public UnsupportedFormatException()
    {
    }

    public UnsupportedFormatException(string message)
        : base(message)
    {
    }

    public UnsupportedFormatException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
