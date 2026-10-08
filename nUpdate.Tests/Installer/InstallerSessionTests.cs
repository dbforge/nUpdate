using nUpdate.UpdateInstaller;

namespace nUpdate.Tests.Installer;

public class InstallerSessionTests
{
    [Fact]
    public void Constructor_ValidatesArguments()
    {
        Should.Throw<ArgumentNullException>(() => new InstallerSession(null!, null));
    }
}
