using nUpdate.Installer;

namespace nUpdate.UpdateInstaller.Reporting;

/// <summary>
///     Reports to a window and switches to a windowless reporter for good as soon as the window fails: when it cannot
///     be opened (no display server after all) or when one of its methods throws. The update itself never notices.
/// </summary>
public sealed class FallbackProgressReporter : IProgressReporter
{
    private readonly IProgressReporter _window;
    private readonly IProgressReporter _fallback;
    private readonly InstallLog _log;
    private volatile bool _failed;

    public FallbackProgressReporter(IProgressReporter window, IProgressReporter fallback, InstallLog log)
    {
        _window = window ?? throw new ArgumentNullException(nameof(window));
        _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>Blocks in the window until it is closed, or in the windowless reporter once the window failed.</summary>
    public void Initialize()
    {
        if (!_failed)
        {
            try
            {
                _window.Initialize();
                if (!_failed)
                    return;
            }
            catch (Exception ex)
            {
                SwitchToFallback(ex);
            }
        }

        _fallback.Initialize();
    }

    public void ReportUnpackingProgress(float progress, string currentFile) => Call(r => r.ReportUnpackingProgress(progress, currentFile));

    public void ReportOperationProgress(float progress, string currentOperation) => Call(r => r.ReportOperationProgress(progress, currentOperation));

    public LockedFileDecision ReportLockedFile(string filePath, int attempt)
    {
        var decision = LockedFileDecision.Abort;
        Call(r => decision = r.ReportLockedFile(filePath, attempt));
        return decision;
    }

    public void Fail(Exception exception) => Call(r => r.Fail(exception));

    /// <summary>Unblocks both, so <see cref="Initialize" /> returns whichever it is waiting in.</summary>
    public void Terminate()
    {
        try
        {
            _window.Terminate();
        }
        catch (Exception ex)
        {
            _log.Write("The installer window could not be closed: " + ex.ToString());
        }

        _fallback.Terminate();
    }

    private void Call(Action<IProgressReporter> action)
    {
        if (!_failed)
        {
            try
            {
                action(_window);
                return;
            }
            catch (Exception ex)
            {
                SwitchToFallback(ex);
            }
        }

        action(_fallback);
    }

    private void SwitchToFallback(Exception exception)
    {
        _failed = true;
        _log.Write("The installer window failed; continuing without it: " + exception.ToString());
    }
}
