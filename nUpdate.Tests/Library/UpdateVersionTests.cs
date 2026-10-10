using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class UpdateVersionTests
{
    [Theory]
    [InlineData("0.0.0")]
    [InlineData("1.0.0")]
    [InlineData("1.10.0")]
    [InlineData("1.2.3.4")]
    [InlineData("2.1.0-beta.1")]
    [InlineData("2.1.0-alpha")]
    [InlineData("1.2.0.7-preview.3")]
    [InlineData("1.2.0-rc.2+build.7")]
    [InlineData("3.1.4+only.metadata")]
    [InlineData("1.0.0-0.3.7")]
    [InlineData("1.0.0-x-y-z.--")]
    [InlineData("1.0.0-a99999999999")]
    [InlineData("1.0.0-0alpha")]
    [InlineData("1.0.0+001.build")]
    public void Constructor_AcceptsTheCanonicalFormAndKeepsIt(string input)
    {
        var version = new UpdateVersion(input);
        version.ToString().ShouldBe(input);
        UpdateVersion.IsValid(input).ShouldBeTrue();
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1.0")]
    [InlineData("1.0.0.0")]
    [InlineData("1.2.0.0b3")]
    [InlineData("1.2b3")]
    [InlineData("1a")]
    [InlineData("1.0 b3")]
    [InlineData("1.0.0 beta")]
    [InlineData("1.0.0.0-beta.2")]
    [InlineData("1.0.0beta")]
    [InlineData("01.0.0")]
    [InlineData("1.00.0")]
    [InlineData("1.0.00")]
    [InlineData("1.0.0.01")]
    [InlineData("1.0.0-beta.01")]
    [InlineData("1.0.0.")]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("1.2.3.4.5")]
    [InlineData("1.0.0-")]
    [InlineData("-1.0.0")]
    [InlineData("99999999999.0.0")]
    [InlineData("1.0.0+")]
    [InlineData("1.0.0-beta..1")]
    [InlineData("1.0.0-beta_1")]
    [InlineData(" 1.0.0")]
    [InlineData("v1.0.0")]
    public void Constructor_RejectsEverythingButTheCanonicalForm(string input)
    {
        Should.Throw<ArgumentException>(() => new UpdateVersion(input)).Message.ShouldContain("major.minor.patch");
        UpdateVersion.TryParse(input, out var result).ShouldBeFalse();
        result.ShouldBeNull();
        UpdateVersion.IsValid(input).ShouldBeFalse();
    }

    [Fact]
    public void Constructor_RejectsNull()
    {
        Should.Throw<ArgumentNullException>(() => new UpdateVersion(null!));
        UpdateVersion.TryParse(null, out _).ShouldBeFalse();
        UpdateVersion.IsValid(null).ShouldBeFalse();
    }

    [Fact]
    public void TryParse_ReturnsVersionForValidInput()
    {
        UpdateVersion.TryParse("2.1.0-beta.4", out var version).ShouldBeTrue();
        version.ShouldNotBeNull();
        version.Major.ShouldBe(2);
        version.Minor.ShouldBe(1);
        version.Build.ShouldBe(0);
        version.Revision.ShouldBe(0);
        version.PreRelease.ShouldBe("beta.4");
        version.IsPreRelease.ShouldBeTrue();
        version.Stage.ShouldBe(PreReleaseStage.Beta);
    }

    [Fact]
    public void Constructor_FillsEveryPart()
    {
        var version = new UpdateVersion("1.5.2.7");
        version.Major.ShouldBe(1);
        version.Minor.ShouldBe(5);
        version.Build.ShouldBe(2);
        version.Revision.ShouldBe(7);
        new UpdateVersion(1, 5, 2, 0).ToString().ShouldBe("1.5.2");
    }

    [Fact]
    public void Constructor_WithoutArguments_IsZero()
    {
        var version = new UpdateVersion();
        version.ToString().ShouldBe("0.0.0");
        version.IsPreRelease.ShouldBeFalse();
        version.Stage.ShouldBe(PreReleaseStage.None);
    }

    [Theory]
    [InlineData(-1, 0, 0, 0)]
    [InlineData(0, -1, 0, 0)]
    [InlineData(0, 0, -1, 0)]
    [InlineData(0, 0, 0, -1)]
    public void Constructor_RejectsNegativeParts(int major, int minor, int build, int revision)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new UpdateVersion(major, minor, build, revision));
    }

    [Theory]
    [InlineData("1.2.0-beta.1", "beta.1", "Beta")]
    [InlineData("1.2.0-alpha", "alpha", "Alpha")]
    [InlineData("1.2.0-rc.2+build.7", "rc.2", "ReleaseCandidate")]
    [InlineData("1.2.0-preview.3", "preview.3", "Other")]
    [InlineData("2.0.0-nightly-20261006", "nightly-20261006", "Other")]
    [InlineData("1.0.0-RC", "RC", "ReleaseCandidate")]
    [InlineData("1.0.0-Beta.2", "Beta.2", "Beta")]
    [InlineData("3.1.4+only.metadata", null, "None")]
    public void Stage_ClassifiesLabelsByTheirFirstIdentifier(string input, string? preRelease, string stage)
    {
        var version = new UpdateVersion(input);
        version.PreRelease.ShouldBe(preRelease);
        version.Stage.ToString().ShouldBe(stage);
        version.IsPreRelease.ShouldBe(preRelease is not null);
    }

    [Fact]
    public void Release_DropsLabelAndMetadata()
    {
        var version = new UpdateVersion("1.2.3.4-beta.1+exp");
        version.Release.ToString().ShouldBe("1.2.3.4");
        version.Release.IsPreRelease.ShouldBeFalse();
        var release = new UpdateVersion("2.0.0");
        release.Release.ShouldBeSameAs(release);
        (new UpdateVersion("1.0.0-beta.2").Release == new UpdateVersion("1.0.0").Release).ShouldBeTrue();
    }

    [Fact]
    public void CompareTo_OrdersByPartsThenLabel()
    {
        (new UpdateVersion("1.2.0") < new UpdateVersion("1.3.0")).ShouldBeTrue();
        (new UpdateVersion("1.3.0-alpha.1") > new UpdateVersion("1.3.0-alpha")).ShouldBeTrue();

        var ordered = new[] { "1.4.0-beta.1", "1.4.0-rc", "1.4.0-rc.1", "1.4.0" }
            .Select(v => new UpdateVersion(v)).ToArray();
        for (var i = 0; i < ordered.Length - 1; i++)
        {
            (ordered[i] < ordered[i + 1]).ShouldBeTrue();
            (ordered[i] <= ordered[i + 1]).ShouldBeTrue();
            (ordered[i + 1] > ordered[i]).ShouldBeTrue();
            (ordered[i + 1] >= ordered[i]).ShouldBeTrue();
            ordered[i].CompareTo(ordered[i + 1]).ShouldBeLessThan(0);
            ordered[i + 1].CompareTo(ordered[i]).ShouldBeGreaterThan(0);
        }

        (new UpdateVersion("1.0.0-alpha") < new UpdateVersion("1.0.0-beta")).ShouldBeTrue();
        (new UpdateVersion("1.0.0-beta") < new UpdateVersion("1.0.0-rc")).ShouldBeTrue();
        (new UpdateVersion("1.0.0.1") > new UpdateVersion("1.0.0")).ShouldBeTrue();
        (new UpdateVersion("1.0.1") > new UpdateVersion("1.0.0.9")).ShouldBeTrue();
        (new UpdateVersion("1.0.0") < new UpdateVersion("1.0.0.1")).ShouldBeTrue();
        (new UpdateVersion("1.0.1") < new UpdateVersion("1.1.0")).ShouldBeTrue();
        (new UpdateVersion("2.0.0") > new UpdateVersion("1.9.9.9")).ShouldBeTrue();
        (new UpdateVersion("1.0.0") > new UpdateVersion("1.0.0.1")).ShouldBeFalse();
        (new UpdateVersion("1.1.0") > new UpdateVersion("1.2.0")).ShouldBeFalse();
        (new UpdateVersion("1.0.1") > new UpdateVersion("1.0.2")).ShouldBeFalse();
    }

    [Fact]
    public void CompareTo_FollowsSemVerPrecedenceForLabels()
    {
        var ordered = new[]
            {
                "1.0.0-alpha", "1.0.0-alpha.1", "1.0.0-alpha.beta", "1.0.0-beta", "1.0.0-beta.2", "1.0.0-beta.11",
                "1.0.0-rc.1", "1.0.0"
            }
            .Select(v => new UpdateVersion(v)).ToList();
        for (var i = 0; i < ordered.Count - 1; i++)
            (ordered[i] < ordered[i + 1]).ShouldBeTrue($"{ordered[i]} < {ordered[i + 1]}");
        ordered.OrderByDescending(v => v).First().ToString().ShouldBe("1.0.0");
        (new UpdateVersion("1.0.0-1") < new UpdateVersion("1.0.0-a")).ShouldBeTrue();
        (new UpdateVersion("1.0.0-a") > new UpdateVersion("1.0.0-1")).ShouldBeTrue();
        (new UpdateVersion("1.0.0-99999999999999999999") < new UpdateVersion("1.0.0-a")).ShouldBeTrue();
        new UpdateVersion("1.0.0-beta.1").CompareTo(new UpdateVersion("1.0.0-beta.1")).ShouldBe(0);
    }

    [Fact]
    public void Equals_IsConsistentWithHashCodeAndOperators()
    {
        var a = new UpdateVersion("1.2.0-beta.3");
        var b = new UpdateVersion(1, 2, 0, 0, "beta.3", null);
        var c = new UpdateVersion("1.2.0-beta.4");

        a.ShouldBe(b);
        a.Equals((object)b).ShouldBeTrue();
        a.GetHashCode().ShouldBe(b.GetHashCode());
        (a == b).ShouldBeTrue();
        (a != b).ShouldBeFalse();
        (a <= b).ShouldBeTrue();
        (a >= b).ShouldBeTrue();
        a.CompareTo(b).ShouldBe(0);

        a.ShouldNotBe(c);
        (a == c).ShouldBeFalse();
        (a != c).ShouldBeTrue();
        a.GetHashCode().ShouldNotBe(c.GetHashCode());
        a.Equals("1.2.0-beta.3").ShouldBeFalse();
        a.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void BuildMetadata_IsKeptButIgnoredForOrderingAndEquality()
    {
        var plain = new UpdateVersion("1.2.0-beta.1");
        var tagged = new UpdateVersion("1.2.0-beta.1+exp.sha.5114f85");
        tagged.BuildMetadata.ShouldBe("exp.sha.5114f85");
        plain.BuildMetadata.ShouldBeNull();
        tagged.ShouldBe(plain);
        tagged.GetHashCode().ShouldBe(plain.GetHashCode());
        tagged.CompareTo(plain).ShouldBe(0);
        tagged.ToString().ShouldBe("1.2.0-beta.1+exp.sha.5114f85");
    }

    [Fact]
    public void UpdateVersion_OperatorsHandleNull()
    {
        UpdateVersion? none = null;
        var some = new UpdateVersion("1.0.0");

        (none == null).ShouldBeTrue();
        (none != null).ShouldBeFalse();
        (none == some).ShouldBeFalse();
        (some == none).ShouldBeFalse();
        (none < some).ShouldBeTrue();
        (none <= some).ShouldBeTrue();
        (none > some).ShouldBeFalse();
        (none >= some).ShouldBeFalse();
        (some > none).ShouldBeTrue();
        (some >= none).ShouldBeTrue();
        (some < none).ShouldBeFalse();
        (some <= none).ShouldBeFalse();
        (none >= null).ShouldBeTrue();
        (none < null).ShouldBeFalse();
        some.CompareTo((UpdateVersion?)null).ShouldBe(1);
        some.CompareTo((object?)null).ShouldBe(1);
        some.CompareTo((object)new UpdateVersion("1.0.0")).ShouldBe(0);
        Should.Throw<ArgumentException>(() => some.CompareTo("1.0.0"));
    }

    [Fact]
    public void Max_FindsTheHighestAndMinTheLowestVersion()
    {
        var versions = new[]
            {
                "1.0.0", "1.1.0", "1.2.0-alpha.1", "1.3.0-beta.1", "1.2.0-beta.3", "1.3.0-beta.3", "1.3.0.1-beta.97",
                "1.1.1"
            }
            .Select(v => new UpdateVersion(v)).ToList();

        UpdateVersion.Max(versions).ToString().ShouldBe("1.3.0.1-beta.97");
        UpdateVersion.Min(versions).ToString().ShouldBe("1.0.0");
        UpdateVersion.Max([]).ShouldBe(new UpdateVersion());
        UpdateVersion.Min([]).ShouldBe(new UpdateVersion());
        Should.Throw<ArgumentNullException>(() => UpdateVersion.Max(null!));
        Should.Throw<ArgumentNullException>(() => UpdateVersion.Min(null!));
    }

    [Fact]
    public void CompareTo_IsUsedForSorting()
    {
        var sorted = new[] { "2.0.0", "1.0.0-beta", "1.0.0", "1.0.0-alpha" }.Select(v => new UpdateVersion(v))
            .OrderBy(v => v).ToList();
        sorted.Select(v => v.ToString()).ShouldBe(["1.0.0-alpha", "1.0.0-beta", "1.0.0", "2.0.0"]);
    }

    [Fact]
    public void Constructor_WithLabelsValidatesThem()
    {
        var version = new UpdateVersion(1, 2, 3, 4, "beta.1", "build.9");
        version.ToString().ShouldBe("1.2.3.4-beta.1+build.9");
        version.BuildMetadata.ShouldBe("build.9");
        // Labels are taken as they are; the classic shortcuts are no longer expanded.
        new UpdateVersion(1, 2, 3, 4, "b3", null).ToString().ShouldBe("1.2.3.4-b3");
        new UpdateVersion(1, 2, 3, 4, null, null).IsPreRelease.ShouldBeFalse();
        new UpdateVersion(1, 2, 3).ToString().ShouldBe("1.2.3");
        new UpdateVersion(1, 0, 0, 0, null, "007").BuildMetadata.ShouldBe("007");
        Should.Throw<ArgumentException>(() => new UpdateVersion(1, 0, 0, 0, "bad label", null)).Message
            .ShouldContain("pre-release label");
        Should.Throw<ArgumentException>(() => new UpdateVersion(1, 0, 0, 0, "beta.01", null)).Message
            .ShouldContain("leading zeros");
        Should.Throw<ArgumentException>(() => new UpdateVersion(1, 0, 0, 0, null, "bad meta")).Message
            .ShouldContain("build metadata");
    }
}
