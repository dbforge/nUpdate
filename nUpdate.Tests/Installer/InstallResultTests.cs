using nUpdate.UpdateInstaller;

namespace nUpdate.Tests.Installer;

public class InstallResultTests
{
    [Fact]
    public void InstallResult_CarriesOutcome()
    {
        InstallResult.Success.Succeeded.ShouldBeTrue();
        InstallResult.Success.Error.ShouldBeNull();
        var error = new InvalidOperationException();
        InstallResult.Failure(error).Succeeded.ShouldBeFalse();
        InstallResult.Failure(error).Error.ShouldBe(error);
        Should.Throw<ArgumentNullException>(() => InstallResult.Failure(null!));
    }
}
