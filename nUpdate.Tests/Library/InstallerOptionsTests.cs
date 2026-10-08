using nUpdate.Installer;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class InstallerOptionsTests
{
    [Fact]
    public void InstallerOptions_RoundTrip_PreservesEverything()
    {
        var options = new InstallerOptions
        {
            Packages = [new InstallerPackage { Path = "/tmp/1.0.0.zip" }],
            Application = new ApplicationOptions { Name = "App", Directory = "/apps/App.app/Contents/MacOS", ExecutablePath = "/apps/App.app/Contents/MacOS/App", Bundle = "/apps/App.app" },
            Host = new HostOptions { ProcessId = 42, AfterInstall = AfterInstall.Close },
            Arguments = [new InstallerArgument("--updated", ArgumentCondition.Succeeded)],
            Ui = new InstallerUiOptions { ShowWindow = false, IconPath = "/tmp/icon.png", AccentColor = "#336699" },
            Texts = { [nameof(InstallerText.Copying)] = "Kopiere {0}..." },
        };

        var json = Serializer.Serialize(options);
        json.ShouldStartWith("{\"format\":2,");
        json.ShouldContain("\"afterInstall\":\"close\"");
        json.ShouldContain("\"when\":\"succeeded\"");
        json.ShouldContain("\"ui\":{\"showWindow\":false,\"iconPath\":\"/tmp/icon.png\",\"accentColor\":\"#336699\"}");
        json.ShouldNotContain("programDirectory");
        var restored = Serializer.Deserialize<InstallerOptions>(json)!;

        restored.Format.ShouldBe(InstallerOptions.CurrentFormat);
        restored.Packages.Single().Path.ShouldBe("/tmp/1.0.0.zip");
        restored.Application.Directory.ShouldBe("/apps/App.app/Contents/MacOS");
        restored.Application.Bundle.ShouldBe("/apps/App.app");
        restored.Application.ProgramDirectory.ShouldBe("/apps/App.app");
        restored.Host.ProcessId.ShouldBe(42);
        restored.Host.AfterInstall.ShouldBe(AfterInstall.Close);
        restored.Arguments.Single().Value.ShouldBe("--updated");
        restored.Arguments.Single().When.ShouldBe(ArgumentCondition.Succeeded);
        restored.Ui.ShowWindow.ShouldBeFalse();
        restored.Ui.IconPath.ShouldBe("/tmp/icon.png");
        restored.Ui.AccentColor.ShouldBe("#336699");
        restored.Text(InstallerText.Copying).ShouldBe("Kopiere {0}...");
    }

    [Fact]
    public void Text_FallsBackToEnglishDefaults()
    {
        var options = new InstallerOptions();
        foreach (var key in Enum.GetValues<InstallerText>())
            options.Text(key).ShouldBe(InstallerTexts.Default(key));
        Should.Throw<ArgumentOutOfRangeException>(() => InstallerTexts.Default((InstallerText)999));
        options.Host.ProcessId.ShouldBeNull();
        options.Host.AfterInstall.ShouldBe(AfterInstall.Restart);
        options.Ui.ShowWindow.ShouldBeTrue();
        options.Ui.IconPath.ShouldBeNull();
        options.Ui.AccentColor.ShouldBeNull();
        options.Application.Bundle.ShouldBeNull();
        new ApplicationOptions { Directory = "/app" }.ProgramDirectory.ShouldBe("/app");
        new ApplicationOptions { Directory = "/app", Bundle = "" }.ProgramDirectory.ShouldBe("/app");
    }
}
