using nUpdate.Administration.Core.Migration;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public class MigrationPackageTests
{
    [Fact]
    public void MigrationPackage_ValidatesItsArguments()
    {
        var legacy = new LegacyFeedEntry(new UpdateVersion("1.0.0"), "1.0.0.0");
        var conversion = new LegacyOperationConversion([], []);
        Should.Throw<ArgumentNullException>(() => MigrationPackage.Migrated(null!));
        Should.Throw<ArgumentException>(() => MigrationPackage.Failed(legacy, null, ""));
        Should.Throw<ArgumentException>(() => MigrationPackage.Ready(legacy, "", "/p", 1, 1, [], conversion));
        Should.Throw<ArgumentException>(() => MigrationPackage.Ready(legacy, "s", "", 1, 1, [], conversion));
        Should.Throw<ArgumentNullException>(() => MigrationPackage.Ready(legacy, "s", "/p", 1, 1, null!, conversion));
        Should.Throw<ArgumentNullException>(() => MigrationPackage.Ready(legacy, "s", "/p", 1, 1, [], null!));
        var migrated = MigrationPackage.Migrated(legacy);
        migrated.Source.ShouldBeNull();
        migrated.SourcePath.ShouldBeNull();
        migrated.Operations.ShouldBeEmpty();
        migrated.Warnings.ShouldBeEmpty();
        migrated.SkippedEntries.ShouldBeEmpty();
        migrated.Problem.ShouldBeNull();
    }
}
