using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class UpdateFilterTests
{
    private static PackageInfo Package(string version, bool necessary = false, string[]? platforms = null,
        string[]? unsupported = null, RolloutCondition[]? conditions = null) => new()
        {
            Version = new UpdateVersion(version),
            Necessary = necessary,
            Files = (platforms ?? [PackagePlatform.Any]).Select(p => new PackageFile { Platform = p, Path = $"packages/{version}/{p}.zip" }).ToList(),
            UnsupportedVersions = (unsupported ?? []).Select(v => new UpdateVersion(v)).ToList(),
            Rollout = new RolloutSettings { Mode = RolloutConditionMode.All, Conditions = (conditions ?? []).ToList() },
        };

    private static UpdateFilterOptions Options(string current = "1.0.0", Stability stability = Stability.Release, string platform = "win-x64", params (string Key, string Value)[] conditions) => new(new UpdateVersion(current))
    {
        MinimumStability = stability,
        Platform = platform,
        RolloutConditions = conditions.ToDictionary(c => c.Key, c => c.Value, StringComparer.Ordinal),
    };

    private static IEnumerable<string> Versions(IEnumerable<PackageInfo> result) => result.Select(c => c.Version.ToString());

    [Fact]
    public void Select_KeepsNewestAndNecessaryPackagesSortedAscending()
    {
        var packages = new[] { Package("1.0.0"), Package("1.1.0", necessary: true), Package("1.3.0"), Package("1.2.0"), Package("0.9.0", necessary: true) };
        Versions(UpdateFilter.Select(packages, Options())).ShouldBe(["1.1.0", "1.3.0"]);
    }

    [Fact]
    public void Select_ReturnsEmptyWhenNothingIsNewer()
    {
        UpdateFilter.Select([Package("1.0.0"), Package("0.5.0")], Options()).ShouldBeEmpty();
        UpdateFilter.Select([], Options()).ShouldBeEmpty();
    }

    [Fact]
    public void Select_FollowsTheMinimumStability()
    {
        var packages = new[] { Package("1.1.0-alpha.1", necessary: true), Package("1.1.0-beta.1", necessary: true), Package("1.1.0-rc.1", necessary: true), Package("1.1.0-nightly.3", necessary: true), Package("1.1.0", necessary: true) };
        Versions(UpdateFilter.Select(packages, Options())).ShouldBe(["1.1.0"]);
        Versions(UpdateFilter.Select(packages, Options(stability: Stability.ReleaseCandidate))).ShouldBe(["1.1.0-rc.1", "1.1.0"]);
        Versions(UpdateFilter.Select(packages, Options(stability: Stability.Beta))).ShouldBe(["1.1.0-beta.1", "1.1.0-rc.1", "1.1.0"]);
        Versions(UpdateFilter.Select(packages, Options(stability: Stability.Any))).ShouldBe(["1.1.0-alpha.1", "1.1.0-beta.1", "1.1.0-nightly.3", "1.1.0-rc.1", "1.1.0"]);

        var options = Options();
        options.AcceptedPreReleaseLabels = ["NIGHTLY"];
        Versions(UpdateFilter.Select(packages, options)).ShouldBe(["1.1.0-nightly.3", "1.1.0"]);

        var preview = new[] { Package("1.2.0-preview.1"), Package("1.2.0") };
        Versions(UpdateFilter.Select(preview, Options(stability: Stability.Any))).ShouldBe(["1.2.0"]); // the release is newer than its preview
        Versions(UpdateFilter.Select([Package("1.2.0-preview.1")], Options(stability: Stability.Beta))).ShouldBeEmpty();
    }

    [Fact]
    public void Select_LeavesOutPackagesWithoutAFileForThePlatform()
    {
        var packages = new[]
        {
            Package("1.1.0", necessary: true, platforms: ["win-x86"]),
            Package("1.2.0", necessary: true, platforms: ["win-x64", "linux"]),
            Package("1.3.0", necessary: true, platforms: ["osx"]),
            Package("1.4.0", necessary: true),
        };
        Versions(UpdateFilter.Select(packages, Options(platform: "win-x64"))).ShouldBe(["1.2.0", "1.4.0"]);
        Versions(UpdateFilter.Select(packages, Options(platform: "win-x86"))).ShouldBe(["1.1.0", "1.4.0"]);
        Versions(UpdateFilter.Select(packages, Options(platform: "linux-arm64"))).ShouldBe(["1.2.0", "1.4.0"]);
        Versions(UpdateFilter.Select(packages, Options(platform: "osx-arm64"))).ShouldBe(["1.3.0", "1.4.0"]);
        Versions(UpdateFilter.Select([Package("2.0.0", platforms: ["linux-x64"])], Options(platform: "linux-arm64"))).ShouldBeEmpty();
    }

    [Fact]
    public void Select_SkipsUnsupportedCurrentVersionsComparedByRelease()
    {
        var packages = new[] { Package("1.1.0", necessary: true, unsupported: ["1.0.0"]), Package("1.2.0", necessary: true, unsupported: ["0.9.0"]), Package("1.3.0", unsupported: []) };
        Versions(UpdateFilter.Select(packages, Options(current: "1.0.0-beta.2"))).ShouldBe(["1.2.0", "1.3.0"]);
    }

    [Fact]
    public void Select_AppliesRolloutConditions()
    {
        var packages = new[] { Package("1.1.0", necessary: true, conditions: [new RolloutCondition("R", "east")]), Package("1.2.0", necessary: true) };
        Versions(UpdateFilter.Select(packages, Options(conditions: ("R", "west")))).ShouldBe(["1.2.0"]);
        Versions(UpdateFilter.Select(packages, Options(conditions: ("R", "east")))).ShouldBe(["1.1.0", "1.2.0"]);
    }

    [Fact]
    public void Select_RejectsNullArguments()
    {
        Should.Throw<ArgumentNullException>(() => UpdateFilter.Select(null!, Options()));
        Should.Throw<ArgumentNullException>(() => UpdateFilter.Select([], null!));
        Should.Throw<ArgumentNullException>(() => new UpdateFilterOptions(null!));
    }
}
