using System.Runtime.InteropServices;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class PackagePlatformTests
{
    [Fact]
    public void All_ListsEveryOperatingSystemWithItsRuntimeIdentifiers()
    {
        PackagePlatform.All.ShouldBe([
            "any", "win", "win-x64", "win-x86", "win-arm64", "linux", "linux-x64", "linux-arm64", "osx", "osx-x64",
            "osx-arm64"
        ]);
        PackagePlatform.IsKnown("any").ShouldBeTrue();
        PackagePlatform.IsKnown("WIN").ShouldBeFalse();
        PackagePlatform.IsKnown("freebsd-x64").ShouldBeFalse();
        PackagePlatform.IsKnown(null).ShouldBeFalse();
    }

    [Fact]
    public void Current_IsTheRuntimeIdentifierOfThisProcess()
    {
        PackagePlatform.Current.ShouldBe(RuntimeInformation.RuntimeIdentifier);
    }

    [Theory]
    [InlineData("win", Architecture.X86, "win-x86")]
    [InlineData("win", Architecture.X64, "win-x64")]
    [InlineData("linux", Architecture.Arm, "linux-arm")]
    [InlineData("osx", Architecture.Arm64, "osx-arm64")]
    [InlineData("linux", Architecture.S390x, "linux-s390x")]
    public void Identify_CombinesOperatingSystemAndArchitecture(string operatingSystem, Architecture architecture,
        string expected)
    {
        PackagePlatform.Identify(operatingSystem, architecture).ShouldBe(expected);
    }

    [Fact]
    public void OperatingSystemOf_SplitsAndClassifiesPlatforms()
    {
        PackagePlatform.OperatingSystemOf("linux-arm64").ShouldBe("linux");
        PackagePlatform.OperatingSystemOf("osx").ShouldBe("osx");
        PackagePlatform.IsWindows("win").ShouldBeTrue();
        PackagePlatform.IsWindows("Win-arm64").ShouldBeTrue();
        PackagePlatform.IsWindows("any").ShouldBeFalse();
        PackagePlatform.IsWindows("linux-x64").ShouldBeFalse();
        Should.Throw<ArgumentNullException>(() => PackagePlatform.OperatingSystemOf(null!));
    }
}
