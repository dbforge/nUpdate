using nUpdate.Installer;
using nUpdate.Localization;

namespace nUpdate.Tests.Library;

public class InstallerTextMapperTests
{
    [Fact]
    public void ToInstallerTexts_MapsEveryInstallerText()
    {
        var texts = new UpdateTexts { InstallerCopying = "Kopiere {0}" };
        var mapped = InstallerTextMapper.ToInstallerTexts(texts);
        mapped.Keys.OrderBy(k => k, StringComparer.Ordinal).ShouldBe(Enum.GetNames<InstallerText>().OrderBy(k => k, StringComparer.Ordinal));
        mapped[nameof(InstallerText.Copying)].ShouldBe("Kopiere {0}");
        Should.Throw<ArgumentNullException>(() => InstallerTextMapper.ToInstallerTexts(null!));
    }
}
