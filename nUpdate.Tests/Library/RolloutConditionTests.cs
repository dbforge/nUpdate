using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class RolloutConditionTests
{
    [Fact]
    public void Constructor_SetsKeyValueAndNegated()
    {
        var empty = new RolloutCondition();
        empty.Key.ShouldBe("");
        empty.Value.ShouldBe("");
        empty.Negated.ShouldBeFalse();
        var negated = new RolloutCondition("k", "v", true);
        negated.Key.ShouldBe("k");
        negated.Value.ShouldBe("v");
        negated.Negated.ShouldBeTrue();
        Should.Throw<ArgumentNullException>(() => new RolloutCondition(null!, "v"));
        Should.Throw<ArgumentNullException>(() => new RolloutCondition("k", null!));
    }
}
