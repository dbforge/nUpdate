using nUpdate.Administration.Core.Statistics;

namespace nUpdate.Tests.Administration.Core;

public class StatisticsDeployerTests
{
    [Fact]
    public void Constructor_ValidatesArguments()
    {
        var context = new Support.AdminTestContext();
        Should.Throw<ArgumentNullException>(() =>
            new StatisticsDeployer(null!, context.TransferFactory, context.Statistics));
        Should.Throw<ArgumentNullException>(() =>
            new StatisticsDeployer(context.FileSystem, null!, context.Statistics));
        Should.Throw<ArgumentNullException>(() =>
            new StatisticsDeployer(context.FileSystem, context.TransferFactory, null!));
        new StatisticsDeployer(context.FileSystem, context.TransferFactory, context.Statistics).ShouldNotBeNull();
    }
}
