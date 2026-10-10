using System.IO.Abstractions.TestingHelpers;
using nUpdate.Installer;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller;

namespace nUpdate.Tests.Installer;

public class InstallerHostTests
{
    private readonly TestInstallerServices _services = new();

    private string LogPath => _services.FileSystem.Path.Combine(_services.PackagesDirectory, InstallerHost.LogFileName);

    private string Prepare(bool showWindow = true)
    {
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/app.dll"] = "new" });
        var options = _services.Options(package);
        options.Ui.ShowWindow = showWindow;
        return _services.WriteOptions(options);
    }

    private string AppFile(string name) => _services.FileSystem.Path.Combine(_services.AppDirectory, name);

    [Fact]
    public void Run_ShowsTheWindowAndInstalls()
    {
        var optionsPath = Prepare();
        InstallerSession? session = null;
        var window = new RecordingReporter();

        var exitCode = InstallerHost.Run([optionsPath], s =>
        {
            session = s;
            return window;
        }, _services.Services);

        exitCode.ShouldBe(InstallerHost.Succeeded);
        _services.FileSystem.File.ReadAllText(AppFile("app.dll")).ShouldBe("new");
        session!.Options.Application.Name.ShouldBe("App");
        session.LogFilePath.ShouldBe(LogPath);
        window.Initialized.ShouldBeTrue();
        window.Unpacking.Single().Text.ShouldBe("app.dll");
        window.Terminated.ShouldBe(1);
        // The engine deletes the package folder, the log lives on with the options next to it in the real temp folder.
        _services.ProcessService.Received().Start(_services.Options().Application.ExecutablePath, "");
    }

    [Fact]
    public void Run_WritesTheLogNextToTheOptions()
    {
        var optionsPath = Prepare(showWindow: false);
        var fs = _services.FileSystem;
        // Keep the folder: in production the options live in the installer's temp folder, not in the package folder.
        var folder = fs.Path.Combine(fs.Path.GetTempPath(), "installer");
        fs.Directory.CreateDirectory(folder);
        var kept = fs.Path.Combine(folder, "installer-options.json");
        fs.File.Copy(optionsPath, kept);

        InstallerHost.Run([kept], _ => new RecordingReporter(), _services.Services).ShouldBe(InstallerHost.Succeeded);

        var log = fs.File.ReadAllLines(fs.Path.Combine(folder, InstallerHost.LogFileName));
        log[0].ShouldContain("Updating App in"); // before anything the engine reports
        log[1].ShouldEndWith("Running without a window, as the options ask.");
        log.ShouldContain(l => l.EndsWith("Copied app.dll", StringComparison.Ordinal));
        log.ShouldContain(l => l.EndsWith("Finished.", StringComparison.Ordinal));
    }

    [Fact]
    public void Run_StaysWindowlessWhenAskedOrWithoutDisplay()
    {
        var created = 0;

        IProgressReporter Window(InstallerSession _)
        {
            created++;
            return new RecordingReporter();
        }

        InstallerHost.Run([Prepare(showWindow: false)], Window, _services.Services).ShouldBe(InstallerHost.Succeeded);
        _services.EnvironmentInfo.HasDisplay.Returns(false);
        InstallerHost.Run([Prepare()], Window, _services.Services).ShouldBe(InstallerHost.Succeeded);

        created.ShouldBe(0);
    }

    [Fact]
    public void Run_FallsBackWhenTheWindowCannotBeCreatedOrOpened()
    {
        InstallerHost.Run([Prepare()], _ => throw new InvalidOperationException("no theme"), _services.Services)
            .ShouldBe(InstallerHost.Succeeded);

        var window = Substitute.For<IProgressReporter>();
        window.When(w => w.Initialize()).Do(_ => throw new InvalidOperationException("XOpenDisplay failed"));
        InstallerHost.Run([Prepare()], _ => window, _services.Services).ShouldBe(InstallerHost.Succeeded);
        _services.FileSystem.File.ReadAllText(AppFile("app.dll")).ShouldBe("new");
    }

    [Fact]
    public void Run_ReportsAFailedUpdate()
    {
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/app.dll"] = "new" });
        var options = _services.Options(package,
            _services.FileSystem.Path.Combine(_services.PackagesDirectory, "missing.zip"));
        var window = new RecordingReporter();

        InstallerHost.Run([_services.WriteOptions(options)], _ => window, _services.Services)
            .ShouldBe(InstallerHost.Failed);

        window.Failures.ShouldHaveSingleItem().ShouldBeOfType<FileNotFoundException>();
    }

    [Fact]
    public void Run_ShowsWhyItCannotStartInTheWindow()
    {
        var window = new RecordingReporter();
        var missing = _services.FileSystem.Path.Combine(_services.PackagesDirectory, "missing.json");

        InstallerSession? session = null;
        InstallerHost.Run([missing], s =>
        {
            session = s;
            return window;
        }, _services.Services).ShouldBe(InstallerHost.CouldNotStart);

        window.Failures.ShouldHaveSingleItem().ShouldBeOfType<FileNotFoundException>();
        window.Terminated.ShouldBe(1);
        window.Initialized.ShouldBeTrue();
        session!.LogFilePath.ShouldBe(LogPath);
        session.Options.Text(InstallerText.WindowTitle).ShouldBe("nUpdate");
        session.Options.Text(InstallerText.UpdatingErrorCaption).ShouldBe("Error while initializing the installer.");
        _services.ErrorOutput.ToString().ShouldBeEmpty();

        // Without a display the error goes to the error output instead.
        _services.EnvironmentInfo.HasDisplay.Returns(false);
        InstallerHost.Run([missing], _ => window, _services.Services).ShouldBe(InstallerHost.CouldNotStart);
        window.Failures.Count.ShouldBe(1);
        _services.ErrorOutput.ToString().ShouldContain("missing.json");
    }

    [Fact]
    public void Run_CannotStartWithoutReadableOptions()
    {
        var fs = _services.FileSystem;
        _services.EnvironmentInfo.HasDisplay.Returns(false); // the errors go to the error output and the event log
        static IProgressReporter Window(InstallerSession _) => new RecordingReporter();
        InstallerHost.Run([], Window, _services.Services).ShouldBe(InstallerHost.CouldNotStart);
        InstallerHost.Run(["a", "b"], Window, _services.Services).ShouldBe(InstallerHost.CouldNotStart);
        InstallerHost.Run([" "], Window, _services.Services).ShouldBe(InstallerHost.CouldNotStart);
        _services.ErrorOutput.ToString().ShouldContain("Expected exactly one argument");

        var missing = fs.Path.Combine(_services.PackagesDirectory, "missing.json");
        InstallerHost.Run([missing], Window, _services.Services).ShouldBe(InstallerHost.CouldNotStart);
        fs.File.ReadAllText(LogPath).ShouldContain("Error while initializing the installer.");
        fs.File.ReadAllText(LogPath).ShouldContain("FileNotFoundException");

        InstallerHost.Run([fs.Path.Combine(_services.Root("nowhere"), "options.json")], Window, _services.Services)
            .ShouldBe(InstallerHost.CouldNotStart);
        fs.File.Exists(fs.Path.Combine(_services.Root("nowhere"), InstallerHost.LogFileName)).ShouldBeFalse();

        fs.AddFile(fs.Path.Combine(_services.PackagesDirectory, "broken.json"), new MockFileData("{"));
        InstallerHost.Run([fs.Path.Combine(_services.PackagesDirectory, "broken.json")], Window, _services.Services)
            .ShouldBe(InstallerHost.CouldNotStart);
        _services.EventLog.Received().WriteError(Arg.Is<string>(m => m.Contains("not valid JSON")));

        Should.Throw<ArgumentNullException>(() => InstallerHost.Run(null!, Window, _services.Services));
        Should.Throw<ArgumentNullException>(() => InstallerHost.Run([], null!, _services.Services));
    }

    [Fact]
    public void Run_UsesTheProductionServicesByDefault()
    {
        var folder = Directory.CreateTempSubdirectory("nupdate-host-").FullName;
        try
        {
            var missing = Path.Combine(folder, "missing.json");
            // A window that cannot be created leaves the windowless reporter, whether or not this machine has a display.
            InstallerHost.Run([missing], _ => throw new InvalidOperationException("no window"))
                .ShouldBe(InstallerHost.CouldNotStart);
            File.ReadAllText(Path.Combine(folder, InstallerHost.LogFileName)).ShouldContain("does not exist");
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }
}
