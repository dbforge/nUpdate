using System.IO.Abstractions;
using System.IO.Compression;
using nUpdate.Packaging;

namespace nUpdate.Administration.Core.Packages;

/// <summary>Reads what an existing package contains.</summary>
public interface IPackageContentReader
{
    Task<PackageContent> ReadAsync(string packagePath, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Writes every package file below <paramref name="targetDirectory" /> under its entry name, for example
    ///     <c>Program/bin/app.dll</c>, and overwrites files that are already there. On Linux and macOS each file gets the
    ///     Unix permissions stored in its entry, so a package built from the files again keeps them.
    /// </summary>
    /// <returns>The content, with <see cref="PackageContentEntry.ExtractedPath" /> set for every entry.</returns>
    Task<PackageContent> ExtractAsync(string packagePath, string targetDirectory,
        CancellationToken cancellationToken = default);
}

/// <summary>The entries and the manifest of a package.</summary>
public sealed class PackageContent(IReadOnlyList<PackageContentEntry> entries, PackageManifest? manifest)
{
    public IReadOnlyList<PackageContentEntry> Entries { get; } =
        entries ?? throw new ArgumentNullException(nameof(entries));

    /// <summary>The manifest, or <c>null</c> for a package without one (written by nUpdate Administration 3.x or 4.x).</summary>
    public PackageManifest? Manifest { get; } = manifest;
}

public sealed class PackageContentEntry(PackageRoot root, string relativePath, long size, string? extractedPath = null,
    int mode = 0)
{
    public PackageRoot Root { get; } = root;

    public string RelativePath { get; } = relativePath ?? throw new ArgumentNullException(nameof(relativePath));

    public long Size { get; } = size;

    /// <summary>The file <see cref="IPackageContentReader.ExtractAsync" /> wrote, or <c>null</c> when the package was only read.</summary>
    public string? ExtractedPath { get; } = extractedPath;

    /// <summary>The Unix permissions (rwxrwxrwx bits) the zip stores for the file, or 0 when it stores none.</summary>
    public int Mode { get; } = mode;
}

public sealed class PackageContentReader : IPackageContentReader
{
    private readonly IFileSystem _fileSystem;
    private readonly bool _isWindows;

    public PackageContentReader(IFileSystem fileSystem)
        : this(fileSystem, OperatingSystem.IsWindows())
    {
    }

    /// <param name="fileSystem">The file system the package is read from and the files are extracted to.</param>
    /// <param name="isWindows">Whether files have no Unix permissions, so the modes stored in the entries are not restored.</param>
    public PackageContentReader(IFileSystem fileSystem, bool isWindows)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _isWindows = isWindows;
    }

    public Task<PackageContent> ReadAsync(string packagePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packagePath);
        return ReadEntriesAsync(packagePath, null, cancellationToken);
    }

    public Task<PackageContent> ExtractAsync(string packagePath, string targetDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packagePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetDirectory);
        return ReadEntriesAsync(packagePath, targetDirectory, cancellationToken);
    }

    /// <param name="packagePath">The package zip.</param>
    /// <param name="targetDirectory">Where the package files are extracted to, or <c>null</c> to only list them.</param>
    /// <param name="cancellationToken">Cancels between entries.</param>
    private async Task<PackageContent> ReadEntriesAsync(string packagePath, string? targetDirectory,
        CancellationToken cancellationToken)
    {
        var entries = new List<PackageContentEntry>();
        PackageManifest? manifest = null;

        using var stream = _fileSystem.File.OpenRead(packagePath);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        foreach (var entry in archive.Entries)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (entry.FullName.EndsWith('/'))
                continue;
            if (string.Equals(entry.FullName, PackageLayout.ManifestFileName, StringComparison.OrdinalIgnoreCase))
            {
                using var reader = new StreamReader(entry.Open());
                manifest = Serializer.Deserialize<PackageManifest>(await reader.ReadToEndAsync(cancellationToken)
                    .ConfigureAwait(false));
                continue;
            }

            if (!PackageFileEntry.TryParseEntryName(entry.FullName, out var root, out var relativePath))
                continue;
            var mode = (entry.ExternalAttributes >> 16) & 0x1FF; // the rwxrwxrwx bits; 0 when the zip stores none
            var extractedPath = targetDirectory is null
                ? null
                : await ExtractEntryAsync(entry, mode, targetDirectory, cancellationToken).ConfigureAwait(false);
            entries.Add(new PackageContentEntry(root, relativePath, entry.Length, extractedPath, mode));
        }

        return new PackageContent(entries, manifest);
    }

    /// <summary>
    ///     Writes a package file, whose name <see cref="PackageFileEntry.TryParseEntryName" /> accepted and so cannot
    ///     leave <paramref name="targetDirectory" />, and restores its Unix permissions the way the installer does.
    /// </summary>
    private async Task<string> ExtractEntryAsync(ZipArchiveEntry entry, int mode, string targetDirectory,
        CancellationToken cancellationToken)
    {
        var path = _fileSystem.Path.Combine(targetDirectory,
            entry.FullName.Replace('/', _fileSystem.Path.DirectorySeparatorChar));
        _fileSystem.Directory.CreateDirectory(_fileSystem.Path.GetDirectoryName(path)!);
        using (var input = entry.Open())
        using (var output = _fileSystem.File.Create(path))
            await input.CopyToAsync(output, cancellationToken).ConfigureAwait(false);

        if (!_isWindows && mode != 0)
        {
#pragma warning disable CA1416 // Only called off Windows, as the flag says.
            _fileSystem.File.SetUnixFileMode(path, (UnixFileMode)mode);
#pragma warning restore CA1416
        }

        return path;
    }
}
