using System.Globalization;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class ChangelogFormatterTests
{
    [Fact]
    public void Format_OrdersByVersionAndUsesCulture()
    {
        var packages = new[]
        {
            new PackageInfo { Version = new UpdateVersion("1.2.0"), Changelog = { ["en"] = "Two" } },
            new PackageInfo
                { Version = new UpdateVersion("1.1.0"), Changelog = { ["de-DE"] = "Eins", ["en"] = "One" } },
        };
        ChangelogFormatter.Format(packages, new CultureInfo("de-DE")).ShouldBe("1.1.0:\nEins\n\n1.2.0:\nTwo");
        ChangelogFormatter.Format(packages, new CultureInfo("en"), "\r\n")
            .ShouldBe("1.1.0:\r\nOne\r\n\r\n1.2.0:\r\nTwo");
        ChangelogFormatter.Format([], new CultureInfo("en")).ShouldBe("");
        Should.Throw<ArgumentNullException>(() => ChangelogFormatter.Format(null!, new CultureInfo("en")));
        Should.Throw<ArgumentNullException>(() => ChangelogFormatter.Format(packages, null!));
    }
}
