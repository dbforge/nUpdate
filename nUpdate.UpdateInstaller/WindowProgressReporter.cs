using nUpdate.Installer;

namespace nUpdate.UpdateInstaller;

/// <summary>
///     The base of an installer window. The engine reports from its own thread; reports are posted to the UI thread. The
///     latest progress before the window is open is shown once it is, questions and errors wait for it. When the window
///     cannot open, or is gone, the waiting calls throw, so that <see cref="InstallerHost" /> continues without a window.
/// </summary>
/// <remarks>
///     A window implements <see cref="RunWindow" /> and <see cref="Post" />; the other abstract members show something
///     and are only called on the UI thread.
/// </remarks>
public abstract class WindowProgressReporter(InstallerSession session) : IProgressReporter, IDisposable
{
    private readonly ManualResetEventSlim _ready = new();
    private readonly ManualResetEventSlim _closed = new();
    private readonly object _gate = new();
    private bool _shown;
    private bool _terminated;
    private (float Progress, string Text)? _pending;

    /// <summary>The options and the log of this run.</summary>
    public InstallerSession Session { get; } = session ?? throw new ArgumentNullException(nameof(session));

    /// <summary>Shows the window and returns when it has closed.</summary>
    public void Initialize()
    {
        try
        {
            RunWindow(Shown);
        }
        finally
        {
            // A window that never opened, or that is gone, must not keep the engine waiting.
            _closed.Set();
            _ready.Set();
        }
    }

    public void ReportUnpackingProgress(float progress, string currentFile) =>
        Report(progress, Session.Options.Text(InstallerText.Copying, currentFile));

    public void ReportOperationProgress(float progress, string currentOperation) => Report(progress, currentOperation);

    public LockedFileDecision ReportLockedFile(string filePath, int attempt)
    {
        WaitForWindow();
        using var answered = new ManualResetEventSlim();
        var decision = LockedFileDecision.Abort;
        Post(() => AskAboutLockedFile(filePath, d =>
        {
            decision = d;
            answered.Set();
        }));
        WaitForUser(answered);
        return decision;
    }

    /// <summary>Shows the error and returns once the user has closed it.</summary>
    public void Fail(Exception exception)
    {
        WaitForWindow();
        using var acknowledged = new ManualResetEventSlim();
        Post(() => ShowError(exception, acknowledged.Set));
        WaitForUser(acknowledged);
    }

    /// <remarks>Posting happens under the lock, so the window sees the reports and the end in the order they came.</remarks>
    public void Terminate()
    {
        lock (_gate)
        {
            _terminated = true;
            if (_shown)
                Post(Finish); // otherwise Shown closes the window as soon as it is open
        }
    }

    public void Dispose()
    {
        _ready.Dispose();
        _closed.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Shows the window, calls <paramref name="shown" /> once it is open and returns when it has closed. Throws when the
    ///     window cannot be shown. Called on the installer's main thread.
    /// </summary>
    protected abstract void RunWindow(Action shown);

    /// <summary>Runs the action on the UI thread without waiting for it.</summary>
    protected abstract void Post(Action action);

    /// <summary>Shows the progress (0 to 100) and what the installer is doing.</summary>
    protected abstract void ShowProgress(float progress, string text);

    /// <summary>Asks what to do with a file another process holds open, and passes the user's answer on.</summary>
    protected abstract void AskAboutLockedFile(string filePath, Action<LockedFileDecision> answer);

    /// <summary>Shows the error, with <see cref="InstallerSession.LogFilePath" /> when there is one, and calls <paramref name="closed" /> once the user has read it.</summary>
    protected abstract void ShowError(Exception exception, Action closed);

    /// <summary>Closes the window: the installer is done.</summary>
    protected abstract void Finish();

    private void Shown()
    {
        lock (_gate)
        {
            _shown = true;
            if (_pending is { } report)
                Post(() => ShowProgress(report.Progress, report.Text));
            if (_terminated)
                Post(Finish);
        }

        _ready.Set();
    }

    private void Report(float progress, string text)
    {
        lock (_gate)
        {
            if (_shown)
                Post(() => ShowProgress(progress, text));
            else
                _pending = (progress, text); // shown once the window is open, such as the wait for the application
        }
    }

    private void WaitForWindow()
    {
        _ready.Wait();
        if (_closed.IsSet)
            throw new InvalidOperationException("The installer window is not open.");
    }

    /// <summary>Waits for the user's answer, unless the window goes away first.</summary>
    private void WaitForUser(ManualResetEventSlim answer)
    {
        if (WaitHandle.WaitAny([answer.WaitHandle, _closed.WaitHandle]) == 1)
            throw new InvalidOperationException("The installer window was closed.");
    }
}
