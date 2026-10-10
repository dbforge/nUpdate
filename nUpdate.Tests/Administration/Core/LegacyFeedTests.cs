using nUpdate.Administration.Core.Migration;
using nUpdate.Updating;
using static nUpdate.Tests.Administration.Support.LegacyTestData;

namespace nUpdate.Tests.Administration.Core;

public class LegacyFeedTests
{
    [Fact]
    public void Parse_IsLenient()
    {
        var entries = LegacyFeed.Parse(LegacyFeedJson());
        entries.Count.ShouldBe(2);
        entries[0].Version.ShouldBe(new UpdateVersion("1.0.0"));
        entries[0].LiteralVersion.ShouldBe("1.0.0.0");
        entries[0].RemoteDirectory.ShouldBe("1.0.0.0");
        entries[0].Platform.ShouldBe("win");
        entries[0].Changelog["de-DE"].ShouldBe("Erste");
        entries[0].Rollout.Mode.ShouldBe(RolloutConditionMode.Any);
        entries[0].Rollout.Conditions.ShouldBeEmpty();
        entries[0].UnsupportedVersions.ShouldBeEmpty();
        entries[0].Operations.ShouldBeNull();
        entries[1].Version.ToString().ShouldBe("1.1.0-beta.2");
        entries[1].LiteralVersion.ShouldBe("1.1.0.0b2");
        entries[1].Platform.ShouldBe("win-x64");
        entries[1].Necessary.ShouldBeTrue();
        entries[1].Rollout.Mode.ShouldBe(RolloutConditionMode.All);
        entries[1].Rollout.Conditions.Single().Negated.ShouldBeTrue();
        entries[1].UnsupportedVersions.ShouldBe([new UpdateVersion("0.9.0")]);
        entries[1].PackageUri!.ToString().ShouldEndWith("1.1.0.0b2/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee.zip");
        entries[1].Operations!.Count.ShouldBe(1);

        LegacyFeed.Parse(" ").ShouldBeEmpty();
        LegacyFeed.Parse(
                """[{"LiteralVersion":"2.0","Architecture":"x86","RolloutConditionMode":"All","Changelog":null,"UpdatePackageUri":"relative"}]""")
            .Single().Platform.ShouldBe("win-x86");
        LegacyFeed.Parse("""[{"LiteralVersion":"2.0","Architecture":"nope","RolloutConditionMode":"AtLeastOne"}]""")
            .Single().Rollout.Mode.ShouldBe(RolloutConditionMode.Any);
        LegacyFeed.Parse("""[{"LiteralVersion":"2.0","Architecture":"x64"}]""").Single().Platform.ShouldBe("win-x64");
        LegacyFeed.Parse("""[{"LiteralVersion":"2.0","Architecture":"Independent"}]""").Single().Platform
            .ShouldBe("win");
        LegacyFeed.Parse("""[{"LiteralVersion":"2.0","Architecture":0,"RolloutConditionMode":0}]""").Single().Platform
            .ShouldBe("win-x86");
        LegacyFeed.Parse("""[{"LiteralVersion":"2.0","Architecture":1}]""").Single().Platform.ShouldBe("win-x64");
        var sparse = LegacyFeed
            .Parse(
                """[{"LiteralVersion":"2.0","Architecture":null,"Changelog":{"en":null},"RolloutConditions":[{"Key":"k","Value":null}]}]""")
            .Single();
        sparse.Platform.ShouldBe("win");
        sparse.Changelog["en"].ShouldBe("");
        sparse.Rollout.Conditions.Single().Value.ShouldBe("");
        Should.Throw<InvalidDataException>(() => LegacyFeed.Parse("""[{"LiteralVersion":null}]"""));
        Should.Throw<InvalidDataException>(() => LegacyFeed.Parse("{broken"));
        Should.Throw<InvalidDataException>(() => LegacyFeed.Parse("{}"));
        Should.Throw<InvalidDataException>(() => LegacyFeed.Parse("""[{"LiteralVersion":"not a version"}]"""));
        Should.Throw<ArgumentNullException>(() => LegacyFeed.Parse(null!));
        Should.Throw<ArgumentNullException>(() => new LegacyFeedEntry(null!, "1"));
        Should.Throw<ArgumentNullException>(() => new LegacyFeedEntry(new UpdateVersion("1.0.0"), null!));
    }

    [Fact]
    public void Parse_LeavesOutWhatItCannotRead()
    {
        var unreadable = new List<string>();
        var entries =
            LegacyFeed.Parse(
                """[{"LiteralVersion":"x"},{"LiteralVersion":"1.0","NecessaryUpdate":"true","UseStatistics":"maybe","Signature":7,"RolloutConditions":[{"Key":"k","Value":"v","IsNegativeCondition":"no"}]}]""",
                unreadable);
        unreadable.ShouldBe(["x"]);
        entries.Single().Necessary.ShouldBeTrue();
        entries.Single().UseStatistics.ShouldBeFalse();
        entries.Single().Signature.ShouldBeNull();
        entries.Single().Rollout.Conditions.Single().Negated.ShouldBeFalse();
        Should.Throw<InvalidDataException>(() => LegacyFeed.Parse("""[{"LiteralVersion":"x"}]"""));
    }
}
