using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class VersionRangeFormatterTests
{
    [Fact]
    public void Format_ListsUpToTwoAndRangesMore()
    {
        VersionRangeFormatter.Format([]).ShouldBe("");
        VersionRangeFormatter.Format([new UpdateVersion("1.0.0")]).ShouldBe("1.0.0");
        VersionRangeFormatter.Format([new UpdateVersion("1.1.0"), new UpdateVersion("1.0.0-beta.1")])
            .ShouldBe("1.0.0-beta.1, 1.1.0");
        VersionRangeFormatter
            .Format([new UpdateVersion("1.2.0"), new UpdateVersion("1.0.0"), new UpdateVersion("1.1.0")])
            .ShouldBe("1.0.0 - 1.2.0");
        Should.Throw<ArgumentNullException>(() => VersionRangeFormatter.Format(null!));
    }
}
