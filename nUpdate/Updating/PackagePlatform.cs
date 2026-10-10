using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace nUpdate.Updating;

/// <summary>
///     The platforms a package file is built for: .NET runtime identifiers such as <c>win-x64</c>, operating systems
///     (<c>win</c>, <c>linux</c>, <c>osx</c>) and <c>any</c>. A client takes the file of its own runtime identifier, else
///     the one of its operating system, else the one for any platform.
/// </summary>
internal static class PackagePlatform
{
    public const string Any = "any";

    public const string Windows = "win";

    public const string Linux = "linux";

    public const string MacOS = "osx";

    /// <summary>Every platform a package can name, each operating system followed by its runtime identifiers.</summary>
    public static IReadOnlyList<string> All { get; } =
    [
        Any,
        Windows, "win-x64", "win-x86", "win-arm64",
        Linux, "linux-x64", "linux-arm64",
        MacOS, "osx-x64", "osx-arm64",
    ];

    /// <summary>
    ///     The runtime identifier of the running process, for example <c>linux-arm64</c>. The architecture is the
    ///     process's own, so an x64 build running under emulation keeps receiving x64 packages.
    /// </summary>
    public static string Current { get; } = Identify(CurrentOperatingSystem(), RuntimeInformation.ProcessArchitecture);

    /// <summary>Whether the name is one of <see cref="All" />.</summary>
    public static bool IsKnown(string? platform) =>
        platform is not null && All.Contains(platform, StringComparer.Ordinal);

    /// <summary>The operating system part of a platform: <c>win</c> for <c>win-x64</c> and for <c>win</c>.</summary>
    public static string OperatingSystemOf(string platform)
    {
        if (platform is null)
            throw new ArgumentNullException(nameof(platform));
        var dash = platform.IndexOf('-');
        return dash < 0 ? platform : platform.Substring(0, dash);
    }

    /// <summary>Whether the platform is Windows or a Windows runtime identifier, where registry and service operations exist.</summary>
    public static bool IsWindows(string platform) =>
        string.Equals(OperatingSystemOf(platform), Windows, StringComparison.OrdinalIgnoreCase);

    /// <summary>The runtime identifier of an operating system name and a process architecture.</summary>
    internal static string Identify(string operatingSystem, Architecture architecture) =>
        operatingSystem + "-" + architecture.ToString().ToLowerInvariant();

    [ExcludeFromCodeCoverage] // Each branch only runs on its own operating system.
    private static string CurrentOperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return MacOS;
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Linux : "unix";
    }
}
