namespace nUpdate.UpdateInstaller.Abstractions;

/// <summary>Extracts a package archive into a directory.</summary>
public interface IPackageExtractor
{
    void Extract(string packagePath, string targetDirectory);
}
