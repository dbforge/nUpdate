using System.Globalization;
using nUpdate.Operations;

namespace nUpdate.Updating;

/// <summary>The <c>nupdate.json</c> document: every package a project has published, newest or oldest in any order.</summary>
public sealed class UpdateFeed
{
    public const string FileName = "nupdate.json";

    public const int CurrentFormat = 1;

    public int Format { get; set; } = CurrentFormat;

    public Guid ProjectId { get; set; }

    public List<PackageInfo> Packages { get; set; } = [];
}

/// <summary>One published package as the feed describes it.</summary>
public sealed class PackageInfo
{
    public UpdateVersion Version { get; set; } = new();

    public DateTimeOffset PublishedAt { get; set; }

    /// <summary>When <c>true</c> the package is installed even if a newer one exists.</summary>
    public bool Necessary { get; set; }

    /// <summary>
    ///     Whether the application starts again after this package: <c>Restart</c> or <c>Close</c>, overriding
    ///     <see cref="UpdateManager.DefaultAfterInstall" />, or <c>null</c> to leave it to the application.
    /// </summary>
    public AfterInstall? AfterInstall { get; set; }

    /// <summary>The changelog per culture name (<c>en</c>, <c>de-DE</c>).</summary>
    public Dictionary<string, string> Changelog { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Client versions that must not install this package, compared by release (without the pre-release label).</summary>
    public List<UpdateVersion> UnsupportedVersions { get; set; } = [];

    public RolloutSettings Rollout { get; set; } = new();

    /// <summary>One package file per platform; see <see cref="PackagePlatform" />.</summary>
    public List<PackageFile> Files { get; set; } = [];

    /// <summary>Where downloads are reported, or <c>null</c> when the project has no statistics.</summary>
    public PackageStatistics? Statistics { get; set; }

    /// <summary>
    ///     The file for a client with the runtime identifier: the one built for exactly that identifier, else the one for
    ///     its operating system, else the one for any platform; <c>null</c> when none fits.
    /// </summary>
    public PackageFile? FindFile(string runtimeIdentifier)
    {
        if (runtimeIdentifier is null)
            throw new ArgumentNullException(nameof(runtimeIdentifier));

        return FileFor(runtimeIdentifier) ?? FileFor(PackagePlatform.OperatingSystemOf(runtimeIdentifier)) ?? FileFor(PackagePlatform.Any);
    }

    private PackageFile? FileFor(string platform) => Files.FirstOrDefault(f => string.Equals(f.Platform, platform, StringComparison.OrdinalIgnoreCase));

    /// <summary>The changelog of the culture, falling back to its parent, then to English, then to the first entry, then to an empty string.</summary>
    public string GetChangelog(CultureInfo culture)
    {
        if (culture is null)
            throw new ArgumentNullException(nameof(culture));

        for (var current = culture; !string.IsNullOrEmpty(current.Name); current = current.Parent)
        {
            if (Changelog.TryGetValue(current.Name, out var text))
                return text;
        }

        if (Changelog.TryGetValue("en", out var english))
            return english;
        return Changelog.Count > 0 ? Changelog.First().Value : string.Empty;
    }
}

/// <summary>The package file of one platform: where it is and what to expect of it.</summary>
public sealed class PackageFile
{
    /// <summary>The platform the file is built for, for example <c>win-x64</c>, <c>linux</c> or <c>any</c>.</summary>
    public string Platform { get; set; } = PackagePlatform.Any;

    /// <summary>The path relative to the feed, or an absolute URL for a mirror.</summary>
    public string Path { get; set; } = string.Empty;

    public long Size { get; set; }

    /// <summary>The Base64 SHA-512 of the file.</summary>
    public string Sha512 { get; set; } = string.Empty;

    public PackageSignature Signature { get; set; } = new();

    /// <summary>The parts of the system the file's operations touch, so a client can tell the user before installing.</summary>
    public List<OperationArea> Touches { get; set; } = [];
}

/// <summary>The signature of the package file.</summary>
public sealed class PackageSignature
{
    public const string RsaPssSha512 = "rsa-pss-sha512";

    public string Algorithm { get; set; } = RsaPssSha512;

    /// <summary>The Base64 signature.</summary>
    public string Value { get; set; } = string.Empty;
}

/// <summary>The statistics endpoint of the package's project.</summary>
public sealed class PackageStatistics
{
    /// <summary>The endpoint's base URL, relative to the feed or absolute.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>Whether downloads of this package are counted.</summary>
    public bool Enabled { get; set; } = true;
}
