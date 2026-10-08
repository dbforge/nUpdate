using nUpdate.Administration.Core.Models;
using nUpdate.Administration.ViewModels;

namespace nUpdate.Tests.Administration.App;

/// <summary>The statistics settings editor used by the wizard and the settings dialog.</summary>
public class StatisticsSettingsEditorTests
{
    private readonly AppTestContext _context = new();

    [Fact]
    public void StatisticsEditor_RoundTripsAndValidates()
    {
        var editor = _context.Factory.Create<StatisticsSettingsEditorViewModel>();
        editor.Validate().ShouldBeNull();
        editor.Enabled = true;
        editor.Validate().ShouldNotBeNull();
        editor.DatabaseName = "db";
        editor.DatabaseUsername = "u";
        editor.Validate().ShouldBeNull();
        editor.EndpointUrl = "not a url";
        editor.Validate()!.ShouldContain("absolute HTTP(S) URL");
        editor.DatabasePassword = "pw";
        editor.EndpointUrl = " https://s/x.php ";
        editor.Validate().ShouldBeNull();

        var settings = editor.ToSettings();
        settings.Enabled.ShouldBeTrue();
        settings.EndpointUrl.ShouldBe("https://s/x.php");
        settings.Database!.Name.ShouldBe("db");
        settings.Database.Username.ShouldBe("u");
        var secrets = new ProjectSecrets();
        editor.ApplySecrets(secrets);
        secrets.StatisticsDatabasePassword.ShouldBe("pw");

        var reloaded = _context.Factory.Create<StatisticsSettingsEditorViewModel>();
        reloaded.Load(settings, secrets);
        reloaded.DatabaseName.ShouldBe("db");
        reloaded.DatabasePassword.ShouldBe("pw");
        reloaded.Load(new StatisticsSettings(), new ProjectSecrets());
        reloaded.DatabaseHost.ShouldBe("localhost");
        reloaded.DatabaseName.ShouldBe("");

        editor.Enabled = false;
        editor.EndpointUrl = " ";
        editor.ToSettings().Database.ShouldBeNull();
        editor.ToSettings().EndpointUrl.ShouldBeNull();
        editor.ApplySecrets(secrets);
        secrets.StatisticsDatabasePassword.ShouldBeNull();
        Should.Throw<ArgumentNullException>(() => editor.Load(null!, secrets));
        Should.Throw<ArgumentNullException>(() => editor.Load(settings, null!));
        Should.Throw<ArgumentNullException>(() => editor.ApplySecrets(null!));
    }
}
