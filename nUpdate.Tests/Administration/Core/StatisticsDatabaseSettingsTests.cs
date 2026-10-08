using nUpdate.Administration.Core.Models;

namespace nUpdate.Tests.Administration.Core;

public class StatisticsDatabaseSettingsTests
{
    [Fact]
    public void Host_DefaultsToLocalhost()
    {
        new StatisticsDatabaseSettings().Host.ShouldBe("localhost");
    }
}
