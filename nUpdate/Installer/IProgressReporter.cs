namespace nUpdate.Installer;

/// <summary>
///     The user interface of the installer. The engine runs on a background thread and calls the report methods from
///     there; implementations marshal to their UI thread as needed.
/// </summary>
/// <remarks>
///     <c>nUpdate.UpdateInstaller.InstallerHost</c> falls back to a windowless reporter when a window fails, so a window
///     must fail loudly: when <see cref="Initialize" /> cannot show it, or once it is gone, the methods that wait for the
///     user (<see cref="ReportLockedFile" />, <see cref="Fail" />) throw instead of blocking.
/// </remarks>
public interface IProgressReporter
{
    /// <summary>
    ///     Shows the UI and blocks until <see cref="Terminate" /> is called. Called on the main thread after the engine
    ///     has been started. Throws when the UI cannot be shown.
    /// </summary>
    void Initialize();

    /// <summary>Reports a copied file. <paramref name="progress" /> is 0 to 100.</summary>
    void ReportUnpackingProgress(float progress, string currentFile);

    /// <summary>Reports an executed operation or what the installer is doing. <paramref name="progress" /> is 0 to 100.</summary>
    void ReportOperationProgress(float progress, string currentOperation);

    /// <summary>
    ///     A file to replace is locked by another process. Returns what the engine should do; it is asked again on
    ///     <see cref="LockedFileDecision.Retry" /> with an incremented attempt until the engine's limit is reached.
    /// </summary>
    LockedFileDecision ReportLockedFile(string filePath, int attempt);

    /// <summary>
    ///     Something went wrong: the update failed, or it is in place but the application could not be restarted. May
    ///     block until the user has read the message. The engine cleans up after this call.
    /// </summary>
    void Fail(Exception exception);

    /// <summary>Unblocks <see cref="Initialize" />.</summary>
    void Terminate();
}
