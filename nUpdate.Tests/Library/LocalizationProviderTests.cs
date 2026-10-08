using System.Globalization;
using System.IO.Abstractions.TestingHelpers;
using nUpdate.Localization;

namespace nUpdate.Tests.Library;

public class LocalizationProviderTests
{
    [Theory]
    [InlineData("en", "Cancel")]
    [InlineData("de-DE", "Abbrechen")]
    [InlineData("de-AT", "Abbrechen")]
    [InlineData("de-CH", "Abbrechen")]
    [InlineData("es-ES", "Cancelar")]
    [InlineData("it-IT", "Annulla")]
    [InlineData("zh-CN", "取消")]
    [InlineData("DE-de", "Abbrechen")]
    public void Load_ReadsIntegratedCultures(string culture, string expectedCancel)
    {
        var texts = new LocalizationProvider().Load(new CultureInfo(culture));
        texts.Cancel.ShouldBe(expectedCancel);
        texts.InstallerExtracting.ShouldNotStartWith("\"");
        texts.RenamingFile.ShouldNotStartWith("\"");
    }

    [Fact]
    public void IntegratedCultures_AreListedAndDetected()
    {
        LocalizationProvider.IntegratedCultures.Select(c => c.Name).ShouldBe(["de-AT", "de-CH", "de-DE", "en", "es-ES", "it-IT", "zh-CN"]);
        LocalizationProvider.IsIntegratedCulture(new CultureInfo("en")).ShouldBeTrue();
        LocalizationProvider.IsIntegratedCulture(new CultureInfo("fr-FR")).ShouldBeFalse();
        LocalizationProvider.IsIntegratedCulture(null!).ShouldBeFalse();
        LocalizationProvider.DefaultCulture.Name.ShouldBe("en");
        LocalizationProvider.IsAvailable(new CultureInfo("fr-FR"), null).ShouldBeFalse();
        Should.Throw<ArgumentNullException>(() => LocalizationProvider.Resolve(null!, null));
        LocalizationProvider.Resolve(new CultureInfo("de-DE"), null).Name.ShouldBe("de-DE");
        LocalizationProvider.Resolve(new CultureInfo("de-LI"), null).Name.ShouldBe("de-DE");
        LocalizationProvider.Resolve(new CultureInfo("it"), null).Name.ShouldBe("it-IT");
        LocalizationProvider.Resolve(new CultureInfo("zh"), null).Name.ShouldBe("zh-CN");
        LocalizationProvider.Resolve(new CultureInfo("es-MX"), null).Name.ShouldBe("es-ES");
        LocalizationProvider.Resolve(new CultureInfo("pt-BR"), null).Name.ShouldBe("en");
        LocalizationProvider.Resolve(CultureInfo.InvariantCulture, null).Name.ShouldBe("en");
        var custom = new Dictionary<CultureInfo, string> { [new CultureInfo("fr-CA")] = "/fr.json" };
        LocalizationProvider.Resolve(new CultureInfo("fr-BE"), custom).Name.ShouldBe("fr-CA");
        LocalizationProvider.Resolve(new CultureInfo("fr"), custom).Name.ShouldBe("fr-CA");
        LocalizationProvider.IsAvailable(new CultureInfo("fr-FR"), new Dictionary<CultureInfo, string> { [new CultureInfo("fr-FR")] = "/x.json" }).ShouldBeTrue();
        LocalizationProvider.IsAvailable(null!, new Dictionary<CultureInfo, string>()).ShouldBeFalse();
    }

    [Fact]
    public void Load_UsesCustomFileAndFallsBackToDefaultsForMissingKeys()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/loc/fr.json", new MockFileData("""{"cancel":"Annuler"}"""));
        var custom = new Dictionary<CultureInfo, string> { [new CultureInfo("fr-FR")] = "/loc/fr.json" };

        var texts = new LocalizationProvider(fileSystem).Load(new CultureInfo("fr-FR"), custom);
        texts.Cancel.ShouldBe("Annuler");
        texts.Install.ShouldBe("Install");
    }

    [Fact]
    public void Load_CustomFileOverridesIntegratedCulture()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/loc/en.json", new MockFileData("""{"Cancel":"Nope"}"""));
        var custom = new Dictionary<CultureInfo, string> { [new CultureInfo("en")] = "/loc/en.json" };
        new LocalizationProvider(fileSystem).Load(new CultureInfo("en"), custom).Cancel.ShouldBe("Nope");
    }

    [Fact]
    public void Load_ReportsProblems()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/loc/bad.json", new MockFileData("{not json"));
        fileSystem.AddFile("/loc/null.json", new MockFileData("null"));
        var provider = new LocalizationProvider(fileSystem);
        var bad = new Dictionary<CultureInfo, string> { [new CultureInfo("fr-FR")] = "/loc/bad.json" };
        var missing = new Dictionary<CultureInfo, string> { [new CultureInfo("fr-FR")] = "/loc/missing.json" };
        var empty = new Dictionary<CultureInfo, string> { [new CultureInfo("fr-FR")] = "/loc/null.json" };

        Should.Throw<InvalidDataException>(() => provider.Load(new CultureInfo("fr-FR"), bad));
        Should.Throw<FileNotFoundException>(() => provider.Load(new CultureInfo("fr-FR"), missing));
        Should.Throw<InvalidDataException>(() => provider.Load(new CultureInfo("fr-FR"), empty));
        Should.Throw<ArgumentException>(() => provider.Load(new CultureInfo("fr-FR")));
        Should.Throw<ArgumentNullException>(() => provider.Load(null!));
        Should.Throw<ArgumentNullException>(() => new LocalizationProvider(null!));
    }
}
