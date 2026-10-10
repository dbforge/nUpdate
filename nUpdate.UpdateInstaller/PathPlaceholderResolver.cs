using System.IO.Abstractions;
using nUpdate.UpdateInstaller.Abstractions;

namespace nUpdate.UpdateInstaller;

/// <summary>
///     Resolves paths that start with <c>%program%</c>, <c>%appdata%</c>, <c>%temp%</c> or <c>%desktop%</c>.
///     Paths without a placeholder are returned unchanged.
/// </summary>
internal sealed class PathPlaceholderResolver
{
    private readonly IFileSystem _fileSystem;
    private readonly Dictionary<string, string> _roots;

    public PathPlaceholderResolver(IFileSystem fileSystem, string applicationDirectory, ISpecialFolders specialFolders)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        if (applicationDirectory is null)
            throw new ArgumentNullException(nameof(applicationDirectory));
        if (specialFolders is null)
            throw new ArgumentNullException(nameof(specialFolders));

        _roots = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["%program%"] = applicationDirectory,
            ["%appdata%"] = specialFolders.ApplicationData,
            ["%temp%"] = specialFolders.Temp,
            ["%desktop%"] = specialFolders.Desktop,
        };
    }

    public string Resolve(string path)
    {
        if (path is null)
            throw new ArgumentNullException(nameof(path));

        var parts = path.Split(['\\', '/'], StringSplitOptions.None);
        if (!_roots.TryGetValue(parts[0], out var root))
            return path;

        var rest = parts.Skip(1).Where(part => part.Length > 0).ToArray();
        return rest.Length == 0 ? root : _fileSystem.Path.Combine(root, _fileSystem.Path.Combine(rest));
    }
}
