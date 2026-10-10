namespace nUpdate.UpdateInstaller.Abstractions;

/// <summary>Extracts a package archive into a directory.</summary>
internal interface IPackageExtractor
{
    void Extract(string packagePath, string targetDirectory);
}
