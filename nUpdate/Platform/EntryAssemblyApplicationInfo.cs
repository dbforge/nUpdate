using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace nUpdate.Platform;

/// <summary>Derives the application information from the entry assembly and the running process.</summary>
public sealed class EntryAssemblyApplicationInfo : IApplicationInfo
{
    private const string DotnetHostName = "dotnet";
    private readonly Assembly? _assembly;
    private readonly string? _mainModulePath;
    private readonly bool _isWindows;

    public EntryAssemblyApplicationInfo()
        : this(Assembly.GetEntryAssembly(), GetMainModulePath(), RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
    }

    internal EntryAssemblyApplicationInfo(Assembly? assembly, string? mainModulePath, bool isWindows)
    {
        _assembly = assembly;
        _mainModulePath = mainModulePath;
        _isWindows = isWindows;
    }

    public string ProductName =>
        _assembly?.GetName().Name ?? (_mainModulePath is null ? "Application" : FileNameWithoutExtension(_mainModulePath));

    /// <summary>
    ///     The application's executable: the process image, unless the application was started through the dotnet host,
    ///     in which case the apphost next to the entry assembly is assumed (<c>App.exe</c> on Windows, <c>App</c> elsewhere).
    /// </summary>
    public string? ExecutablePath
    {
        get
        {
            if (_mainModulePath is not null && !IsDotnetHost(_mainModulePath))
                return _mainModulePath;

            var location = _assembly?.Location;
            if (string.IsNullOrEmpty(location))
                return _mainModulePath;
            if (location!.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                return location;
            return _isWindows ? Path.ChangeExtension(location, ".exe") : Path.ChangeExtension(location, null);
        }
    }

    public string? DeclaredVersion =>
        _assembly?.GetCustomAttributes(false).OfType<ApplicationVersionAttribute>().SingleOrDefault()?.Version;

    public string UserAgentProduct
    {
        get
        {
            var name = _assembly?.GetName();
            return name is null ? "nUpdate/5.0" : $"{name.Name}/{name.Version}";
        }
    }

    public int CurrentProcessId => Process.GetCurrentProcess().Id;

    private static bool IsDotnetHost(string path) =>
        string.Equals(FileNameWithoutExtension(path), DotnetHostName, StringComparison.OrdinalIgnoreCase);

    /// <summary>Like <see cref="Path.GetFileNameWithoutExtension(string)" />, but accepts both separators on every OS.</summary>
    private static string FileNameWithoutExtension(string path)
    {
        var separator = path.LastIndexOfAny(['\\', '/']);
        return Path.GetFileNameWithoutExtension(separator < 0 ? path : path.Substring(separator + 1));
    }

    [ExcludeFromCodeCoverage] // Reads the live process; its failure modes cannot be provoked in a test.
    private static string? GetMainModulePath()
    {
        try
        {
            return Process.GetCurrentProcess().MainModule?.FileName;
        }
        catch (Exception exception) when (exception is Win32Exception or InvalidOperationException or NotSupportedException or PlatformNotSupportedException)
        {
            return null;
        }
    }
}
