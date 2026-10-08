using nUpdate.Installer;
using nUpdate.Tests.Installer.Support;

namespace nUpdate.Tests.Installer;

public class RecordingReporterTests
{
    private readonly TestInstallerServices _services = new();

    [Fact]
    public void Initialize_SetsInitialized()
    {
        _services.Reporter.Initialize();
        _services.Reporter.Initialized.ShouldBeTrue();
        Enum.GetValues<LockedFileDecision>().Length.ShouldBe(3);
    }
}
