using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using nUpdate.UpdateInstaller.Abstractions;

namespace nUpdate.UpdateInstaller.Platform;

/// <summary>Process control through <see cref="Process" />.</summary>
[ExcludeFromCodeCoverage] // Starts, waits for and kills real processes; verified by the published-installer runs on every system.
public sealed class SystemProcessService : IProcessService
{
    private const int ErrorCancelled = 1223;

    public bool WaitForExit(int processId, TimeSpan timeout)
    {
        try
        {
            using var process = Process.GetProcessById(processId);
            return process.WaitForExit((int)timeout.TotalMilliseconds);
        }
        catch (ArgumentException)
        {
            return true; // no such process
        }
        catch (InvalidOperationException)
        {
            return true; // exited meanwhile
        }
    }

    /// <remarks>
    ///     Through the shell, so a document opens in its application. On Linux and macOS an executable file is run
    ///     directly; anything else is opened with <c>xdg-open</c> or <c>open</c>.
    /// </remarks>
    public bool Start(string fileName, string arguments)
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            { FileName = fileName, Arguments = arguments, UseShellExecute = true });
            return true;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == ErrorCancelled)
        {
            return false;
        }
    }

    public int Run(string fileName, string arguments)
    {
        using var process = Process.Start(new ProcessStartInfo
        { FileName = fileName, Arguments = arguments, UseShellExecute = true });
        if (process is null)
            return 0; // the shell handed the file to an application that was already running: nothing to wait for
        process.WaitForExit();
        return process.ExitCode;
    }

    public void Kill(string processName)
    {
        foreach (var process in Process.GetProcessesByName(processName))
        {
            using (process)
            {
                process.Kill();
                process.WaitForExit(10_000);
            }
        }
    }
}
