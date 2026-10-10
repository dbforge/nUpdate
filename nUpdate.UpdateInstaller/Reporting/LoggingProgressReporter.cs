using System.Globalization;
using nUpdate.Installer;

namespace nUpdate.UpdateInstaller.Reporting;

/// <summary>Writes everything the engine reports to <see cref="InstallLog" /> and passes it on.</summary>
public sealed class LoggingProgressReporter(IProgressReporter inner, InstallLog log) : IProgressReporter
{
    private readonly IProgressReporter _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private readonly InstallLog _log = log ?? throw new ArgumentNullException(nameof(log));

    public void Initialize() => _inner.Initialize();

    public void ReportUnpackingProgress(float progress, string currentFile)
    {
        _log.Write(Percent(progress) + " Copied " + currentFile);
        _inner.ReportUnpackingProgress(progress, currentFile);
    }

    public void ReportOperationProgress(float progress, string currentOperation)
    {
        _log.Write(Percent(progress) + " " + currentOperation);
        _inner.ReportOperationProgress(progress, currentOperation);
    }

    public LockedFileDecision ReportLockedFile(string filePath, int attempt)
    {
        var decision = _inner.ReportLockedFile(filePath, attempt);
        _log.Write($"\"{filePath}\" is in use (attempt {attempt}): {decision}");
        return decision;
    }

    public void Fail(Exception exception)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));
        _log.Write("Failed: " + exception.ToString());
        _inner.Fail(exception);
    }

    public void Terminate()
    {
        _log.Write("Finished.");
        _inner.Terminate();
    }

    private static string Percent(float progress) => $"[{progress.ToString("0.0", CultureInfo.InvariantCulture),5}%]";
}
