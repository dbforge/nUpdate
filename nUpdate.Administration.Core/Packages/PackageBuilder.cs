using System.IO.Abstractions;
using System.IO.Compression;
using System.Text;
using nUpdate.Packaging;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Packages;

/// <summary>Builds packages with <see cref="ZipArchive" /> through the file-system abstraction.</summary>
public sealed class PackageBuilder : IPackageBuilder
{
    /// <summary>The Unix file type bits of a regular file, stored with the permissions as zip tools expect.</summary>
    private const int RegularFileType = 0x8000;

    private readonly IFileSystem _fileSystem;
    private readonly Func<DateTimeOffset> _now;
    private readonly bool _isWindows;

    public PackageBuilder(IFileSystem fileSystem)
        : this(fileSystem, () => DateTimeOffset.UtcNow, OperatingSystem.IsWindows())
    {
    }

    /// <param name="fileSystem">The file system the files are read from and the package is written to.</param>
    /// <param name="now">The creation time written into the manifest.</param>
    /// <param name="isWindows">Whether files lack Unix permissions, so executables are recognized by content.</param>
    public PackageBuilder(IFileSystem fileSystem, Func<DateTimeOffset> now, bool isWindows)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _now = now ?? throw new ArgumentNullException(nameof(now));
        _isWindows = isWindows;
    }

    public async Task<PackageManifest> BuildAsync(PlatformPackage package, UpdateVersion version, Guid projectId,
        string packagePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(packagePath);

        var duplicates = package.Files.GroupBy(f => f.EntryName, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicates.Count > 0)
            throw new InvalidOperationException(
                $"The {package.Platform} package contains the entry \"{duplicates[0]}\" more than once.");
        var entries = new List<(PackageFileEntry File, int Mode)>();
        foreach (var file in package.Files)
        {
            if (!_fileSystem.File.Exists(file.SourcePath))
                throw new FileNotFoundException(
                    $"The file \"{file.SourcePath}\" for package entry \"{file.EntryName}\" does not exist.",
                    file.SourcePath);
            if (_fileSystem.FileInfo.New(file.SourcePath).LinkTarget is not null)
                throw new InvalidOperationException(
                    $"\"{file.SourcePath}\" is a symbolic link. Packages cannot contain links; add the file it points to instead.");
            entries.Add((file, file.UnixMode ?? UnixModeDetector.Detect(_fileSystem, file.SourcePath, _isWindows)));
        }

        var manifest = new PackageManifest
        {
            ProjectId = projectId,
            Version = version,
            Platform = package.Platform,
            CreatedAt = _now(),
            Operations = package.Operations.ToList()
        };
        var manifestJson = Serializer.Serialize(manifest, indented: true);

        var directory = _fileSystem.Path.GetDirectoryName(packagePath);
        if (!string.IsNullOrEmpty(directory))
            _fileSystem.Directory.CreateDirectory(directory);

        using (var output = _fileSystem.File.Create(packagePath))
        using (var archive = new ZipArchive(output, ZipArchiveMode.Create))
        {
            foreach (var (file, mode) in entries)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var entry = archive.CreateEntry(file.EntryName, CompressionLevel.Optimal);
                entry.ExternalAttributes = (RegularFileType | mode) << 16;
                using var input = _fileSystem.File.OpenRead(file.SourcePath);
                using var target = entry.Open();
                await input.CopyToAsync(target, cancellationToken).ConfigureAwait(false);
            }

            var manifestEntry = archive.CreateEntry(PackageLayout.ManifestFileName, CompressionLevel.Optimal);
            using var manifestStream = manifestEntry.Open();
            await manifestStream.WriteAsync(Encoding.UTF8.GetBytes(manifestJson), cancellationToken)
                .ConfigureAwait(false);
        }

        if (!string.IsNullOrEmpty(directory))
            await _fileSystem.File
                .WriteAllTextAsync(_fileSystem.Path.Combine(directory, PackageLayout.ManifestFileName), manifestJson,
                    cancellationToken).ConfigureAwait(false);
        return manifest;
    }
}
