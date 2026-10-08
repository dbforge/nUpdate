using nUpdate.Administration.Core.Models;

namespace nUpdate.Tests.Administration.Core;

public class UpdatePackageTests
{
    [Fact]
    public void Released_DefaultsToFalse()
    {
        new UpdatePackage().Released.ShouldBeFalse();
    }
}
