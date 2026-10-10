using nUpdate.Packaging;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Packages;

/// <summary>Builds package zip files.</summary>
public interface IPackageBuilder
{
    /// <summary>
    ///     Writes the package file of one platform to <paramref name="packagePath" />: the files below their root folders,
    ///     each with its Unix permissions, plus <c>manifest.json</c>. The manifest is also written next to the package for
    ///     inspection.
    /// </summary>
    /// <returns>The manifest the package carries.</returns>
    /// <exception cref="InvalidOperationException">An entry is listed twice or a file is a symbolic link.</exception>
    /// <exception cref="FileNotFoundException">A file does not exist.</exception>
    Task<PackageManifest> BuildAsync(PlatformPackage package, UpdateVersion version, Guid projectId, string packagePath,
        CancellationToken cancellationToken = default);
}
