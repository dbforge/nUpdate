using nUpdate.Platform;

namespace nUpdate.Tests.Library;

public class OperatingSystemNamesTests
{
    [Theory]
    [InlineData(PlatformID.Win32NT, "6.0.6000", "Windows Vista")]
    [InlineData(PlatformID.Win32NT, "6.1.7601", "Windows 7")]
    [InlineData(PlatformID.Win32NT, "6.2.9200", "Windows 8")]
    [InlineData(PlatformID.Win32NT, "6.3.9600", "Windows 8.1")]
    [InlineData(PlatformID.Win32NT, "10.0.19045", "Windows 10")]
    [InlineData(PlatformID.Win32NT, "10.0.22631", "Windows 11")]
    [InlineData(PlatformID.Win32NT, "12.0.1", "Windows 12")]
    [InlineData(PlatformID.Win32NT, "5.1.2600", "Windows")]
    [InlineData(PlatformID.Win32NT, "6.4.0", "Windows")]
    [InlineData(PlatformID.Win32NT, "10.1.0", "Windows")]
    [InlineData(PlatformID.Unix, "6.8.0", "Linux")]
    [InlineData(PlatformID.MacOSX, "14.0", "macOS")]
    [InlineData(PlatformID.Xbox, "1.0", "Unknown")]
    public void FromVersion_MapsPlatformAndVersion(PlatformID platform, string version, string expected)
    {
        OperatingSystemNames.FromVersion(platform, Version.Parse(version)).ShouldBe(expected);
    }

    [Fact]
    public void FromVersion_RejectsNull()
    {
        Should.Throw<ArgumentNullException>(() => OperatingSystemNames.FromVersion(PlatformID.Win32NT, null!));
    }
}
