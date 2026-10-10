using System.Reflection;

namespace nUpdate.Administration;

/// <summary>The version of nUpdate Administration as it was released, for example <c>5.0.0-rc.1</c>.</summary>
public static class AdministrationVersion
{
    public static string Text { get; } = Read(typeof(AdministrationVersion).Assembly);

    /// <summary>The informational version without the build metadata after <c>+</c> (the commit SourceLink appends).</summary>
    internal static string Read(Assembly assembly)
    {
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                      ?? assembly.GetName().Version?.ToString(3) ?? "0.0.0";
        var metadata = version.IndexOf('+', StringComparison.Ordinal);
        return metadata < 0 ? version : version[..metadata];
    }
}
