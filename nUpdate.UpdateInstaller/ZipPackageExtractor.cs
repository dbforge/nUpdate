using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Reflection;
using nUpdate.Platform;
using nUpdate.UpdateInstaller.Abstractions;

namespace nUpdate.UpdateInstaller;

/// <summary>
///     Extracts zip packages through the file-system abstraction, refuses entries that escape the target and restores
///     the Unix permissions nUpdate Administration stored in each entry.
/// </summary>
public sealed class ZipPackageExtractor(IFileSystem fileSystem, IFilePermissions permissions) : IPackageExtractor
{
    private readonly IFileSystem _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    private readonly IFilePermissions
        _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));

    /// <summary>
    ///     <see cref="ZipArchiveEntry" />'s <c>ExternalAttributes</c>, which .NET Standard 2.0 does not declare but every
    ///     .NET (Core) runtime has. Only .NET Framework lacks it, and it only runs on Windows, where modes do not apply.
    /// </summary>
    private static readonly PropertyInfo?
        ExternalAttributes = typeof(ZipArchiveEntry).GetProperty("ExternalAttributes");

    /// <summary>The permission bits (<c>rwxrwxrwx</c>) of an entry, or 0 when the zip stores none.</summary>
    private static int UnixMode(ZipArchiveEntry entry) => (ReadExternalAttributes(entry) >> 16) & 0x1FF;

    [ExcludeFromCodeCoverage] // The property is missing only on .NET Framework.
    private static int ReadExternalAttributes(ZipArchiveEntry entry) =>
        ExternalAttributes?.GetValue(entry) is int attributes ? attributes : 0;

    public void Extract(string packagePath, string targetDirectory)
    {
        if (packagePath is null)
            throw new ArgumentNullException(nameof(packagePath));
        if (targetDirectory is null)
            throw new ArgumentNullException(nameof(targetDirectory));

        var root = _fileSystem.Path.GetFullPath(targetDirectory).TrimEnd(_fileSystem.Path.DirectorySeparatorChar,
                       _fileSystem.Path.AltDirectorySeparatorChar)
                   + _fileSystem.Path.DirectorySeparatorChar;
        _fileSystem.Directory.CreateDirectory(root);

        using var stream = _fileSystem.File.OpenRead(packagePath);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        foreach (var entry in archive.Entries)
        {
            var relative = entry.FullName.Replace('/', _fileSystem.Path.DirectorySeparatorChar)
                .Replace('\\', _fileSystem.Path.DirectorySeparatorChar);
            var destination = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(root, relative));
            if (!destination.StartsWith(root, StringComparison.Ordinal))
                throw new InvalidDataException(
                    $"The package entry \"{entry.FullName}\" points outside the target directory.");

            if (entry.FullName.EndsWith("/", StringComparison.Ordinal) ||
                entry.FullName.EndsWith("\\", StringComparison.Ordinal))
            {
                _fileSystem.Directory.CreateDirectory(destination);
                continue;
            }

            _fileSystem.Directory.CreateDirectory(_fileSystem.Path.GetDirectoryName(destination)!);
            using (var input = entry.Open())
            using (var output = _fileSystem.File.Create(destination))
                input.CopyTo(output);

            var mode = UnixMode(entry);
            if (mode != 0)
                _permissions.SetMode(destination, mode);
        }
    }
}
