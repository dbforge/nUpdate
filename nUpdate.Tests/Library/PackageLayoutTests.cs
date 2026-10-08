using nUpdate.Packaging;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class PackageLayoutTests
{
    [Fact]
    public void PackageLayout_NamesTheFilesAndFolders()
    {
        PackageLayout.ManifestFileName.ShouldBe("manifest.json");
        PackageLayout.Roots.ShouldBe([PackageRoot.Program, PackageRoot.AppData, PackageRoot.Temp, PackageRoot.Desktop]);
        PackageLayout.FolderName(PackageRoot.AppData).ShouldBe("AppData");
        Should.Throw<ArgumentOutOfRangeException>(() => PackageLayout.FolderName((PackageRoot)9));
        PackageLayout.PackageFileName("win-x64").ShouldBe("win-x64.zip");
        PackageLayout.RemoteVersionDirectory(new UpdateVersion("2.1.0-beta.1")).ShouldBe("packages/2.1.0-beta.1");
        PackageLayout.RemotePackagePath(new UpdateVersion("2.1.0"), "linux").ShouldBe("packages/2.1.0/linux.zip");
        Should.Throw<ArgumentNullException>(() => PackageLayout.PackageFileName(null!));
        Should.Throw<ArgumentNullException>(() => PackageLayout.RemoteVersionDirectory(null!));
    }
}
