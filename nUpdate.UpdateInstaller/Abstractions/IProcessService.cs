namespace nUpdate.UpdateInstaller.Abstractions;

/// <summary>Process control for the host application and for process operations.</summary>
public interface IProcessService
{
    /// <summary>Waits for the process to exit. Returns <c>false</c> when the timeout elapsed first. A process that no longer exists counts as exited.</summary>
    bool WaitForExit(int processId, TimeSpan timeout);

    /// <summary>Starts a process. Returns <c>false</c> when the user declined an elevation prompt.</summary>
    bool Start(string fileName, string arguments);

    /// <summary>
    ///     Starts a process, waits until it has exited and returns its exit code; 0 when the shell handed the file to an
    ///     application that was already running, so there is no process to wait for.
    /// </summary>
    int Run(string fileName, string arguments);

    /// <summary>Terminates every process with the given name (without extension).</summary>
    void Kill(string processName);
}
