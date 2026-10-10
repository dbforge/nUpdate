using nUpdate.Installer;

namespace nUpdate.UpdateInstaller.Reporting;

/// <summary>
///     Reports without a window, for systems without a display and for <c>ShowInstallerWindow = false</c>: a locked file
///     is retried two seconds later, until the engine's <see cref="InstallerServices.MaxLockedFileAttempts" /> aborts the
///     update; failures go to the error output and, on Windows, to the event log. Everything else is in <c>install.log</c>.
/// </summary>
internal sealed class WindowlessProgressReporter(InstallerServices services) : IProgressReporter, IDisposable
{
    /// <summary>The pause before a locked file is tried again.</summary>
    public static readonly TimeSpan LockedFileDelay = TimeSpan.FromSeconds(2);

    private readonly InstallerServices _services = services ?? throw new ArgumentNullException(nameof(services));
    private readonly ManualResetEventSlim _terminated = new();

    /// <summary>Blocks until <see cref="Terminate" />.</summary>
    public void Initialize() => _terminated.Wait();

    public void ReportUnpackingProgress(float progress, string currentFile)
    {
    }

    public void ReportOperationProgress(float progress, string currentOperation)
    {
    }

    public LockedFileDecision ReportLockedFile(string filePath, int attempt)
    {
        _services.Delay(LockedFileDelay);
        return LockedFileDecision.Retry;
    }

    public void Fail(Exception exception)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));
        var message = "nUpdate could not install the update: " + exception.ToString();
        try
        {
            _services.ErrorOutput.WriteLine(message);
        }
        catch (IOException)
        {
            // No error output to write to.
        }

        if (_services.EnvironmentInfo.IsWindows)
            _services.EventLog.WriteError(message);
    }

    public void Terminate() => _terminated.Set();

    public void Dispose() => _terminated.Dispose();
}
