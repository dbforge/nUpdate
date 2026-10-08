using System.Globalization;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class PackageInfoTests
{
    [Fact]
    public void GetChangelog_FallsBackToParentThenEnglishThenFirstThenEmpty()
    {
        var package = new PackageInfo { Changelog = { ["de"] = "Deutsch", ["en"] = "English" } };
        package.GetChangelog(new CultureInfo("de-DE")).ShouldBe("Deutsch");
        package.GetChangelog(new CultureInfo("DE")).ShouldBe("Deutsch");
        package.GetChangelog(new CultureInfo("fr-FR")).ShouldBe("English");

        package.Changelog.Remove("en");
        package.GetChangelog(new CultureInfo("fr-FR")).ShouldBe("Deutsch");

        package.Changelog.Clear();
        package.GetChangelog(new CultureInfo("fr-FR")).ShouldBe("");
        package.GetChangelog(CultureInfo.InvariantCulture).ShouldBe("");
        Should.Throw<ArgumentNullException>(() => package.GetChangelog(null!));
    }

    [Fact]
    public void FindFile_PrefersTheRuntimeIdentifierThenTheOperatingSystemThenAny()
    {
        var package = new PackageInfo
        {
            Files =
            [
                new PackageFile { Platform = "any" },
                new PackageFile { Platform = "linux" },
                new PackageFile { Platform = "linux-arm64" },
                new PackageFile { Platform = "win-x64" },
            ],
        };
        package.FindFile("linux-arm64")!.Platform.ShouldBe("linux-arm64");
        package.FindFile("LINUX-X64")!.Platform.ShouldBe("linux");
        package.FindFile("win-x64")!.Platform.ShouldBe("win-x64");
        package.FindFile("win-arm64")!.Platform.ShouldBe("any");
        package.FindFile("osx-arm64")!.Platform.ShouldBe("any");
        new PackageInfo { Files = [new PackageFile { Platform = "win" }] }.FindFile("osx-x64").ShouldBeNull();
        Should.Throw<ArgumentNullException>(() => package.FindFile(null!));
    }
}
