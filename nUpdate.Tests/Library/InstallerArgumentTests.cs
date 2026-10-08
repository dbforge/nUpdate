using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class InstallerArgumentTests
{
    [Fact]
    public void InstallerArgument_RequiresAValueAndDefaultsToAlways()
    {
        Should.Throw<ArgumentNullException>(() => new InstallerArgument(null!));
        new InstallerArgument("--x").When.ShouldBe(ArgumentCondition.Always);
    }
}
