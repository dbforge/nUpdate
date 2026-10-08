using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class RolloutConditionEvaluatorTests
{
    private static (string Version, RolloutSettings Rollout) Package(string version, RolloutConditionMode mode, params RolloutCondition[] conditions) =>
        (version, new RolloutSettings { Mode = mode, Conditions = conditions.ToList() });

    private static Dictionary<string, string> Client(params (string Key, string Value)[] pairs) =>
        pairs.ToDictionary(p => p.Key, p => p.Value, StringComparer.Ordinal);

    private static IEnumerable<string> Matching(IEnumerable<(string Version, RolloutSettings Rollout)> packages, Dictionary<string, string> client) =>
        packages.Where(p => RolloutConditionEvaluator.Matches(p.Rollout, client)).Select(p => p.Version);

    private static (string, RolloutSettings)[] AnyFirst() =>
    [
        Package("1.2", RolloutConditionMode.Any, new RolloutCondition("R", "east"), new RolloutCondition("CNR", "23654", true)),
        Package("1.3", RolloutConditionMode.Any, new RolloutCondition("R", "east"), new RolloutCondition("R", "west"), new RolloutCondition("CNR", "36587", true), new RolloutCondition("CNR", "32578", true)),
    ];

    private static (string, RolloutSettings)[] AnySecond() =>
    [
        Package("1.2", RolloutConditionMode.Any, new RolloutCondition("CC", "3"), new RolloutCondition("CNR", "23654", true)),
        Package("1.3", RolloutConditionMode.Any, new RolloutCondition("CC", "1"), new RolloutCondition("CC", "3"), new RolloutCondition("CC", "4", true)),
    ];

    private static (string, RolloutSettings)[] AnyThird() =>
    [
        Package("1.2", RolloutConditionMode.Any, new RolloutCondition("P", "secure")),
        Package("1.3", RolloutConditionMode.Any, new RolloutCondition("P", "secure"), new RolloutCondition("P", "special")),
    ];

    private static (string, RolloutSettings)[] AllFirst() =>
    [
        Package("1.2", RolloutConditionMode.All, new RolloutCondition("R", "east"), new RolloutCondition("CNR", "23654")),
        Package("1.3", RolloutConditionMode.All, new RolloutCondition("R", "east"), new RolloutCondition("CNR", "36448")),
    ];

    private static (string, RolloutSettings)[] AllSecond() =>
    [
        Package("1.2", RolloutConditionMode.All, new RolloutCondition("CC", "3"), new RolloutCondition("CNR", "23654", true)),
        Package("1.3", RolloutConditionMode.All, new RolloutCondition("CC", "3"), new RolloutCondition("CC", "4", true)),
    ];

    public static TheoryData<string, string, string[]> Scenarios => new()
    {
        { "AnyFirst", "R=east;CNR=23654", ["1.3"] },
        { "AnyFirst", "R=west;CNR=23654", ["1.3"] },
        { "AnyFirst", "R=east;CNR=56478", ["1.2", "1.3"] },
        { "AnyFirst", "R=west;CNR=10394", ["1.3"] },
        { "AnySecond", "CC=3;CNR=23654", ["1.3"] },
        { "AnySecond", "CC=4;CNR=23654", [] },
        { "AnySecond", "CC=3;CNR=56478", ["1.2", "1.3"] },
        { "AnySecond", "CC=1;CNR=10394", ["1.3"] },
        { "AnyThird", "P=special", ["1.3"] },
        { "AnyThird", "P=test", [] },
        { "AnyThird", "P=secure", ["1.2", "1.3"] },
        { "AllFirst", "R=east;CNR=23654", ["1.2"] },
        { "AllFirst", "R=east;CNR=36448", ["1.3"] },
        { "AllFirst", "R=east;CNR=36447", [] },
        { "AllSecond", "CC=3", ["1.2", "1.3"] },
        { "AllSecond", "CC=3;CNR=23654", ["1.3"] },
        { "AllSecond", "CC=1", [] },
        { "AllSecond", "CC=4", [] },
    };

    [Theory]
    [MemberData(nameof(Scenarios))]
    public void Matches_FollowsScenario(string scenario, string clientSpec, string[] expected)
    {
        var packages = scenario switch
        {
            "AnyFirst" => AnyFirst(),
            "AnySecond" => AnySecond(),
            "AnyThird" => AnyThird(),
            "AllFirst" => AllFirst(),
            _ => AllSecond(),
        };
        var client = clientSpec.Split(';').Select(part => part.Split('=')).ToDictionary(kv => kv[0], kv => kv[1], StringComparer.Ordinal);
        Matching(packages, client).ShouldBe(expected);
    }

    [Fact]
    public void Matches_WithoutClientConditions_OnlyAllowsPackagesWithoutPositiveConditions()
    {
        var packages = new[]
        {
            Package("1.2", RolloutConditionMode.Any, new RolloutCondition("R", "east")),
            Package("1.3", RolloutConditionMode.Any, new RolloutCondition("CNR", "1", true)),
            Package("1.4", RolloutConditionMode.All),
        };
        Matching(packages, Client()).ShouldBe(["1.3", "1.4"]);
    }

    [Fact]
    public void Matches_WithoutPackageConditions_AlwaysTrue()
    {
        var packages = new[] { Package("1.2", RolloutConditionMode.All), Package("1.3", RolloutConditionMode.Any) };
        Matching(packages, Client(("R", "east"))).ShouldBe(["1.2", "1.3"]);
        Matching(packages, Client()).ShouldBe(["1.2", "1.3"]);
    }

    [Fact]
    public void Matches_ComparesValuesCaseInsensitivelyAndKeysCaseSensitively()
    {
        var rollout = Package("1.2", RolloutConditionMode.All, new RolloutCondition("R", "East")).Rollout;
        RolloutConditionEvaluator.Matches(rollout, Client(("R", "eAsT"))).ShouldBeTrue();
        RolloutConditionEvaluator.Matches(rollout, Client(("r", "east"))).ShouldBeFalse();
        var negated = Package("1.2", RolloutConditionMode.All, new RolloutCondition("R", "east", true)).Rollout;
        RolloutConditionEvaluator.Matches(negated, Client(("R", "EAST"))).ShouldBeFalse();
        RolloutConditionEvaluator.Matches(negated, Client(("R", "west"))).ShouldBeTrue();
    }

    [Fact]
    public void Matches_OnlyNegatedConditionsInAnyMode_AllowsNonMatchingClients()
    {
        var rollout = Package("1.2", RolloutConditionMode.Any, new RolloutCondition("R", "east", true)).Rollout;
        RolloutConditionEvaluator.Matches(rollout, Client(("R", "west"))).ShouldBeTrue();
    }

    [Fact]
    public void Matches_RejectsInvalidInput()
    {
        Should.Throw<ArgumentNullException>(() => RolloutConditionEvaluator.Matches(null!, Client()));
        Should.Throw<ArgumentNullException>(() => RolloutConditionEvaluator.Matches(new RolloutSettings(), null!));
        var invalid = Package("1.0", (RolloutConditionMode)9, new RolloutCondition("R", "east")).Rollout;
        Should.Throw<ArgumentOutOfRangeException>(() => RolloutConditionEvaluator.Matches(invalid, Client(("R", "east"))));
    }
}
