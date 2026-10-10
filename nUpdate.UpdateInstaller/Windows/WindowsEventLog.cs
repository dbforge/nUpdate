using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using nUpdate.UpdateInstaller.Abstractions;

namespace nUpdate.UpdateInstaller.Windows;

/// <summary>
///     Writes errors to the Windows Application log under the source <c>nUpdate</c> through the Win32 API, which needs
///     no package and no administrator rights. Only for Windows.
/// </summary>
[ExcludeFromCodeCoverage] // Writes to the real event log; only reached on Windows, by the default-services test of InstallerHost.
internal sealed class WindowsEventLog : IEventLog
{
    private const string Source = "nUpdate";
    private const ushort ErrorType = 1;

    /// <summary>The event log rejects strings above 31 839 characters.</summary>
    private const int MaxMessageLength = 30_000;

    public void WriteError(string message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        if (message.Length > MaxMessageLength)
            message = message.Substring(0, MaxMessageLength);

        var handle = NativeMethods.RegisterEventSourceW(null, Source);
        if (handle == IntPtr.Zero)
            return;
        try
        {
            NativeMethods.ReportEventW(handle, ErrorType, 0, 1, IntPtr.Zero, 1, 0, [message], IntPtr.Zero);
        }
        finally
        {
            NativeMethods.DeregisterEventSource(handle);
        }
    }

    private static class NativeMethods
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr RegisterEventSourceW(string? uncServerName, string sourceName);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReportEventW(IntPtr eventLog, ushort type, ushort category, uint eventId,
            IntPtr userSid, ushort numStrings, uint dataSize,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)]
            string[] strings, IntPtr rawData);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeregisterEventSource(IntPtr eventLog);
    }
}
