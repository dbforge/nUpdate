using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Packages;

/// <summary>Everything needed to build the package files of a version: one per platform.</summary>
public sealed class PackageDefinition(UpdateVersion version)
{
    private readonly List<PlatformPackage> _platforms = [];

    public UpdateVersion Version { get; } = version ?? throw new ArgumentNullException(nameof(version));

    /// <summary>The package file of each platform, see <see cref="PackagePlatform" />; each platform once.</summary>
    public IReadOnlyList<PlatformPackage> Platforms => _platforms;

    /// <summary>The package of the platform, added when it is not there yet.</summary>
    public PlatformPackage GetOrAddPlatform(string platform)
    {
        var existing = _platforms.FirstOrDefault(p => p.Platform == platform);
        if (existing is not null)
            return existing;
        var package = new PlatformPackage(platform);
        _platforms.Add(package);
        return package;
    }
}

/// <summary>The files and operations of the package file of one platform.</summary>
public sealed class PlatformPackage
{
    public PlatformPackage(string platform)
    {
        if (!PackagePlatform.IsKnown(platform))
            throw new ArgumentException(
                $"\"{platform}\" is not a platform nUpdate knows. Use one of {string.Join(", ", PackagePlatform.All)}.",
                nameof(platform));
        Platform = platform;
    }

    /// <summary>For example <c>win-x64</c>, <c>linux</c> or <c>any</c>.</summary>
    public string Platform { get; }

    /// <summary>The files to ship, keyed by the package root they belong to.</summary>
    public List<PackageFileEntry> Files { get; } = [];

    public List<Operation> Operations { get; } = [];
}

/// <summary>A file that goes into the package.</summary>
public sealed class PackageFileEntry
{
    /// <param name="root">The folder on the client the file goes to.</param>
    /// <param name="relativePath">The path below the root, with either separator.</param>
    /// <param name="sourcePath">The local file to copy into the package.</param>
    public PackageFileEntry(PackageRoot root, string relativePath, string sourcePath)
    {
        Root = root;
        RelativePath = (relativePath ?? throw new ArgumentNullException(nameof(relativePath))).Replace('\\', '/')
            .TrimStart('/');
        SourcePath = sourcePath ?? throw new ArgumentNullException(nameof(sourcePath));
        if (RelativePath.Length == 0)
            throw new ArgumentException("The relative path is empty.", nameof(relativePath));
        if (RelativePath.Split('/').Any(part => part is "." or ".."))
            throw new ArgumentException("The relative path must not contain '.' or '..' segments.",
                nameof(relativePath));
    }

    public PackageRoot Root { get; }

    /// <summary>The path inside the root, with <c>/</c> separators.</summary>
    public string RelativePath { get; }

    /// <summary>The local file to copy into the package.</summary>
    public string SourcePath { get; }

    /// <summary>
    ///     The Unix permissions to store for the file, or <c>null</c> to take them from the file itself (see
    ///     <see cref="UnixModeDetector" />). An unchanged file of an existing package keeps the ones it was stored with,
    ///     which Windows cannot tell from the extracted file.
    /// </summary>
    public int? UnixMode { get; init; }

    /// <summary>The entry name inside the zip, for example <c>Program/bin/app.dll</c>.</summary>
    public string EntryName => $"{PackageLayout.FolderName(Root)}/{RelativePath}";

    /// <summary>
    ///     Splits a zip entry name into its root and relative path. Only entries directly below one of the four roots
    ///     with plain segments count; anything else (other folders, <c>..</c>, backslashes, root entries) is not a
    ///     package file and must not end up in a signed package.
    /// </summary>
    public static bool TryParseEntryName(string entryName, out PackageRoot root, out string relativePath)
    {
        root = default;
        relativePath = string.Empty;
        if (string.IsNullOrEmpty(entryName) || entryName.EndsWith('/'))
            return false;
        var segments = entryName.Split('/');
        if (segments.Length < 2 ||
            segments.Any(segment => segment is "" or "." or ".." || segment.IndexOfAny(['\\', ':']) >= 0))
            return false;
        var match = PackageLayout.Roots.FirstOrDefault(r =>
            string.Equals(PackageLayout.FolderName(r), segments[0], StringComparison.Ordinal));
        if (!string.Equals(PackageLayout.FolderName(match), segments[0], StringComparison.Ordinal))
            return false;
        root = match;
        relativePath = string.Join("/", segments.Skip(1));
        return true;
    }
}
