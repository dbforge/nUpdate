using System.Diagnostics.CodeAnalysis;
using nUpdate.Updating;

namespace nUpdate.Platform;

/// <summary>Reads the system information from the environment.</summary>
internal sealed class SystemInformation : ISystemInformation
{
    [ExcludeFromCodeCoverage] // Thin wrapper over Environment; the mapping is tested through OperatingSystemNames.
    public string OperatingSystemName =>
        OperatingSystemNames.FromVersion(Environment.OSVersion.Platform, Environment.OSVersion.Version);

    public string RuntimeIdentifier => PackagePlatform.Current;
}
