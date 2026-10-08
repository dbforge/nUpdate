using nUpdate.Administration.Core.Migration;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public class MigrationPlanTests
{
    [Fact]
    public void MigrationPlan_ValidatesItsArguments()
    {
        Should.Throw<ArgumentNullException>(() => new MigrationPlan(Guid.Empty, false, null, null!));
        new MigrationPlan(Guid.Empty, false, null, []).IsComplete.ShouldBeFalse();
        new MigrationPlan(Guid.Empty, false, new UpdateFeed(), []).IsComplete.ShouldBeTrue();
        new MigrationPlan(Guid.Empty, false, null, []).Dispose();
    }
}
