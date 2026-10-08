using nUpdate.UpdateInstaller.Platform;

namespace nUpdate.Tests.Installer;

public class SystemSpecialFoldersTests
{
    [Fact]
    public void SystemSpecialFolders_ReadsTheEnvironment()
    {
        var folders = new SystemSpecialFolders();
        folders.ApplicationData.ShouldNotBeNullOrEmpty();
        folders.Temp.ShouldBe(Path.GetTempPath());
        folders.Desktop.ShouldNotBeNull();
    }
}
