using System.Reflection;

namespace nUpdate.Administration.Core.Packages;

/// <summary>Reads the assembly version of a managed assembly without loading it; used to suggest the next package version.</summary>
public static class AssemblyVersionReader
{
    /// <returns>The version, or <c>null</c> when the file is missing or not a managed assembly.</returns>
    public static Version? TryRead(string? assemblyPath)
    {
        if (string.IsNullOrWhiteSpace(assemblyPath))
            return null;
        try
        {
            return AssemblyName.GetAssemblyName(assemblyPath).Version;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or BadImageFormatException
                                       or ArgumentException or System.Security.SecurityException)
        {
            return null;
        }
    }
}
