namespace nUpdate.Platform;

/// <summary>Maps an OS platform and version to the display name used in the statistics.</summary>
internal static class OperatingSystemNames
{
    public static string FromVersion(PlatformID platform, Version version)
    {
        if (version is null)
            throw new ArgumentNullException(nameof(version));

        if (platform != PlatformID.Win32NT)
        {
            return platform switch
            {
                PlatformID.Unix => "Linux",
                PlatformID.MacOSX => "macOS",
                _ => "Unknown",
            };
        }

        return (version.Major, version.Minor) switch
        {
            (6, 0) => "Windows Vista",
            (6, 1) => "Windows 7",
            (6, 2) => "Windows 8",
            (6, 3) => "Windows 8.1",
            (10, 0) when version.Build >= 22000 => "Windows 11",
            (10, 0) => "Windows 10",
            (var major, _) when major > 10 => $"Windows {major}",
            _ => "Windows",
        };
    }
}
