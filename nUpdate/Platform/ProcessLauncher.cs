using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace nUpdate.Platform;

/// <summary>Starts processes with <see cref="Process.Start(ProcessStartInfo)" />, on Windows optionally elevated via UAC.</summary>
[ExcludeFromCodeCoverage] // Starts a real process; verified through the Windows end-to-end run.
internal sealed class ProcessLauncher : IProcessLauncher
{
    private const int ErrorCancelled = 1223;

    public bool Start(string fileName, string arguments, bool elevated)
    {
        // Only Windows elevates through the shell; elsewhere the executable is started directly.
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = isWindows,
        };
        if (elevated)
            startInfo.Verb = "runas"; // UpdateManager only asks for it on Windows

        try
        {
            using var process = Process.Start(startInfo);
            return true;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == ErrorCancelled)
        {
            return false;
        }
    }
}
