namespace nUpdate.Exceptions;

/// <summary>Thrown when the update feed cannot be parsed or is inconsistent.</summary>
public class InvalidFeedException : Exception
{
    public InvalidFeedException()
    {
    }

    public InvalidFeedException(string message)
        : base(message)
    {
    }

    public InvalidFeedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
