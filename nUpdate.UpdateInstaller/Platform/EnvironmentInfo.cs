using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using nUpdate.UpdateInstaller.Abstractions;

namespace nUpdate.UpdateInstaller.Platform;

/// <summary>Environment facts from <see cref="Environment" /> and <see cref="RuntimeInformation" />.</summary>
public sealed class EnvironmentInfo : IEnvironmentInfo
{
    [ExcludeFromCodeCoverage] // Thin wrapper over Environment.
    public bool IsServiceContext => !Environment.UserInteractive;

    [ExcludeFromCodeCoverage] // Thin wrapper over RuntimeInformation.
    public bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    [ExcludeFromCodeCoverage] // Thin wrapper over RuntimeInformation.
    public bool IsMacOs => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public bool HasDisplay => DetectDisplay(IsWindows, IsMacOs, IsServiceContext, Environment.GetEnvironmentVariable);

    /// <summary>Windows needs an interactive session, macOS always has its window server, Linux needs <c>DISPLAY</c> or <c>WAYLAND_DISPLAY</c>.</summary>
    internal static bool DetectDisplay(bool isWindows, bool isMacOs, bool isServiceContext,
        Func<string, string?> variable)
    {
        if (isWindows)
            return !isServiceContext;
        if (isMacOs)
            return true;
        return !string.IsNullOrEmpty(variable("DISPLAY")) || !string.IsNullOrEmpty(variable("WAYLAND_DISPLAY"));
    }
}
