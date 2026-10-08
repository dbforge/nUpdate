using nUpdate.Administration.Core.Migration;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public sealed class LegacyVersionTests
{
    [Theory]
    [InlineData("1", "1.0.0")]
    [InlineData("1.2", "1.2.0")]
    [InlineData("1.0.0.0", "1.0.0")]
    [InlineData("1.2.3.4", "1.2.3.4")]
    [InlineData("01.002.3", "1.2.3")]
    [InlineData("1.2.0.0b3", "1.2.0-beta.3")]
    [InlineData("1.2b3", "1.2.0-beta.3")]
    [InlineData("1a", "1.0.0-alpha")]
    [InlineData("1a1", "1.0.0-alpha.1")]
    [InlineData("1.2rc2", "1.2.0-rc.2")]
    [InlineData("1.0.0.0RC1", "1.0.0-rc.1")]
    [InlineData("1.0-a", "1.0.0-alpha")]
    [InlineData("1.0-a.2", "1.0.0-alpha.2")]
    [InlineData("1.0a.2", "1.0.0-alpha.2")]
    [InlineData("1.0 b3", "1.0.0-beta.3")]
    [InlineData("1.0b03", "1.0.0-beta.3")]
    [InlineData("1.0b0", "1.0.0-beta")]
    [InlineData("1.0.0.0 Beta 2", "1.0.0-beta.2")]
    [InlineData("1.0.0.0 Alpha 0", "1.0.0-alpha")]
    [InlineData("2.1.0.0 ReleaseCandidate 1", "2.1.0-rc.1")]
    [InlineData("1.1.0.0-beta.2", "1.1.0-beta.2")]
    [InlineData("1.0-beta.01", "1.0.0-beta.1")]
    [InlineData("1.0-nightly.000", "1.0.0-nightly.0")]
    [InlineData("1.2.0.7-preview.3+build.7", "1.2.0.7-preview.3+build.7")]
    [InlineData("2.1.0-beta.1", "2.1.0-beta.1")]
    [InlineData("  1.0.0.0  ", "1.0.0")]
    [InlineData("1.0 Beta", "1.0.0-beta")] // nUpdate 4 leaves the number out of the long form when it is 0
    [InlineData("1.0.0.0 ReleaseCandidate", "1.0.0-rc")]
    [InlineData("1.0 Preview", "1.0.0-Preview")] // any other word is a label after a space
    public void TryParse_ConvertsEveryEarlierSpellingToTheCanonicalVersion(string legacy, string expected)
    {
        LegacyVersion.TryParse(legacy, out var version).ShouldBeTrue();
        version.ToString().ShouldBe(expected);
        LegacyVersion.Parse(legacy).ShouldBe(new UpdateVersion(expected));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("1.2.3.4.5")]
    [InlineData("1.0-")]
    [InlineData("99999999999")]
    [InlineData("1.0b99999999999")]
    [InlineData("1.0-beta_1")]
    public void TryParse_RejectsWhatNoNUpdateWrote(string? text)
    {
        LegacyVersion.TryParse(text, out var version).ShouldBeFalse();
        version.ShouldBeNull();
        if (text is not null)
            Should.Throw<InvalidDataException>(() => LegacyVersion.Parse(text)).Message.ShouldContain(text);
    }

    [Fact]
    public void Parse_RejectsNull() => Should.Throw<ArgumentNullException>(() => LegacyVersion.Parse(null!));
}
