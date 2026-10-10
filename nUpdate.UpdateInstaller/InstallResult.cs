namespace nUpdate.UpdateInstaller;

/// <summary>The outcome of an installer run.</summary>
internal sealed class InstallResult
{
    private InstallResult(bool succeeded, Exception? error)
    {
        Succeeded = succeeded;
        Error = error;
    }

    public bool Succeeded { get; }

    public Exception? Error { get; }

    public static InstallResult Success { get; } = new(true, null);

    public static InstallResult Failure(Exception error) =>
        new(false, error ?? throw new ArgumentNullException(nameof(error)));
}
