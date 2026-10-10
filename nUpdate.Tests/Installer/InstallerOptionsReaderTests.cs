using System.IO.Abstractions.TestingHelpers;
using nUpdate.Exceptions;
using nUpdate.Installer;
using nUpdate.UpdateInstaller;
using nUpdate.Updating;

namespace nUpdate.Tests.Installer;

public class InstallerOptionsReaderTests
{
    private static string Valid(Action<InstallerOptions>? mutate = null)
    {
        var options = new InstallerOptions
        {
            Packages = [new InstallerPackage { Path = "/p/1.0.0.zip" }],
            Application = new ApplicationOptions { Name = "App", Directory = "/app", ExecutablePath = "/app/a.exe" },
        };
        mutate?.Invoke(options);
        return Serializer.Serialize(options);
    }

    [Fact]
    public void Read_LoadsValidFile()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("/opts.json", new MockFileData(Valid()));
        var options = InstallerOptionsReader.Read(fileSystem, "/opts.json");
        options.Packages.Select(p => p.Path).ShouldBe(["/p/1.0.0.zip"]);
        options.Application.Directory.ShouldBe("/app");
        options.Host.AfterInstall.ShouldBe(AfterInstall.Restart);
    }

    [Fact]
    public void Read_ValidatesArguments()
    {
        var fileSystem = new MockFileSystem();
        Should.Throw<ArgumentNullException>(() => InstallerOptionsReader.Read(null!, "/x"));
        Should.Throw<ArgumentException>(() => InstallerOptionsReader.Read(fileSystem, " "));
        Should.Throw<FileNotFoundException>(() => InstallerOptionsReader.Read(fileSystem, "/missing.json"));
    }

    [Theory]
    [InlineData("{broken")]
    [InlineData("null")]
    public void Parse_RejectsInvalidJson(string json)
    {
        Should.Throw<InvalidDataException>(() => InstallerOptionsReader.Parse(json));
    }

    [Theory]
    [InlineData(1, "older")]
    [InlineData(99, "newer")]
    public void Parse_RejectsOtherFormats(int format, string hint)
    {
        Should.Throw<UnsupportedFormatException>(() => InstallerOptionsReader.Parse(Valid(o => o.Format = format)))
            .Message.ShouldContain(hint);
    }

    [Fact]
    public void Parse_RejectsIncompleteOptions()
    {
        Should.Throw<InvalidDataException>(() => InstallerOptionsReader.Parse(Valid(o => o.Packages.Clear()))).Message
            .ShouldContain("no packages");
        Should.Throw<InvalidDataException>(() => InstallerOptionsReader.Parse(Valid(o => o.Packages[0].Path = " ")))
            .Message.ShouldContain("without a path");
        Should.Throw<InvalidDataException>(() => InstallerOptionsReader.Parse(Valid(o => o.Application.Directory = "")))
            .Message.ShouldContain("application directory");
        Should.Throw<InvalidDataException>(() =>
                InstallerOptionsReader.Parse(Valid(o => o.Application.ExecutablePath = ""))).Message
            .ShouldContain("executable");
        Should.Throw<InvalidDataException>(() => InstallerOptionsReader.Parse(Valid(o =>
        {
            o.Application.ExecutablePath = "";
            o.Host.AfterInstall = AfterInstall.Close;
        })));
        InstallerOptionsReader.Parse(Valid(o =>
        {
            o.Application.ExecutablePath = "";
            o.Host.AfterInstall = AfterInstall.KeepRunning;
        })).Host.AfterInstall.ShouldBe(AfterInstall.KeepRunning);
    }

    [Fact]
    public void Parse_TreatsExplicitNullSectionsAsDefaults()
    {
        var options = InstallerOptionsReader.Parse("""
                                                   {"format":2,"packages":[{"path":"/p/1.0.0.zip"}],"application":{"directory":"/app","executablePath":"/app/a.exe"},
                                                    "host":null,"arguments":null,"ui":null,"texts":null}
                                                   """);
        options.Host.AfterInstall.ShouldBe(AfterInstall.Restart);
        options.Host.ProcessId.ShouldBeNull();
        options.Arguments.ShouldBeEmpty();
        options.Ui.ShowWindow.ShouldBeTrue();
        options.Texts.ShouldBeEmpty();
        options.Text(InstallerText.Copying).ShouldBe("Copying {0}...");

        Should.Throw<InvalidDataException>(() =>
                InstallerOptionsReader.Parse("""{"format":2,"packages":null,"application":null}"""))
            .Message.ShouldContain("no packages");
        Should.Throw<InvalidDataException>(() =>
                InstallerOptionsReader.Parse("""{"format":2,"packages":[null],"application":null}"""))
            .Message.ShouldContain("without a path");
        Should.Throw<InvalidDataException>(() =>
                InstallerOptionsReader.Parse("""{"format":2,"packages":[{"path":"/p/1.zip"}],"application":null}"""))
            .Message.ShouldContain("application directory");
    }

    [Fact]
    public void Parse_ReadsCamelCaseEnumsAndArguments()
    {
        var options = InstallerOptionsReader.Parse("""
                                                   {"format":2,"packages":[{"path":"/p/1.0.0.zip"}],"application":{"name":"App","directory":"/app","executablePath":""},
                                                    "host":{"processId":12,"afterInstall":"keepRunning"},"arguments":[{"value":"--a","when":"failed"},{"value":"--b","when":"always"}],
                                                    "ui":{"showWindow":false,"accentColor":"#123456"},"texts":{"Copying":"Kopiere {0}..."}}
                                                   """);
        options.Host.ProcessId.ShouldBe(12);
        options.Host.AfterInstall.ShouldBe(AfterInstall.KeepRunning);
        options.Arguments.Select(a => (a.Value, a.When))
            .ShouldBe([("--a", ArgumentCondition.Failed), ("--b", ArgumentCondition.Always)]);
        options.Ui.ShowWindow.ShouldBeFalse();
        options.Ui.AccentColor.ShouldBe("#123456");
        options.Text(InstallerText.Copying).ShouldBe("Kopiere {0}...");
    }
}
