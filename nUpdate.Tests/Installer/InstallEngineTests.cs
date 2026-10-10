using System.IO.Abstractions.TestingHelpers;
using nUpdate.Installer;
using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller;
using nUpdate.UpdateInstaller.Abstractions;
using nUpdate.Updating;

namespace nUpdate.Tests.Installer;

public class InstallEngineTests
{
    private readonly TestInstallerServices _services = new();

    private string AppFile(string name) => _services.FileSystem.Path.Combine(_services.AppDirectory, name);

    [Fact]
    public void Run_AppliesPackagesInVersionOrderWithOperationsAndRestartsHost()
    {
        var fs = _services.FileSystem;
        fs.AddFile(AppFile("app.exe"), new MockFileData("v1"));
        fs.AddFile(AppFile("obsolete.dll"), new MockFileData("x"));
        var second = _services.AddPackage("1.2.0",
            new Dictionary<string, string>
            { ["Program/app.exe"] = "v1.2", ["AppData/settings.json"] = "{}", ["Desktop/link.lnk"] = "l" },
            [new StartProcessOperation { Path = "%program%\\app.exe", Arguments = "--migrated" }]);
        var first = _services.AddPackage("1.1.0",
            new Dictionary<string, string>
            { ["Program/app.exe"] = "v1.1", ["Program/new.dll"] = "n", ["Temp/t.txt"] = "t" },
            [
                new TerminateProcessOperation { ProcessName = "helper", RunBeforeFileReplacement = true },
                new DeleteFilesOperation { Directory = "%program%", Files = ["obsolete.dll"] },
            ]);
        var options = _services.Options(second, first);
        options.Arguments =
        [
            new InstallerArgument("--updated", ArgumentCondition.Succeeded),
            new InstallerArgument("--failed", ArgumentCondition.Failed), new InstallerArgument("--always")
        ];

        var result = new InstallEngine(_services.Services).Run(options, _services.Reporter);

        result.Succeeded.ShouldBeTrue();
        fs.File.ReadAllText(AppFile("app.exe")).ShouldBe("v1.2");
        fs.File.ReadAllText(AppFile("new.dll")).ShouldBe("n");
        fs.File.Exists(AppFile("obsolete.dll")).ShouldBeFalse();
        fs.File.ReadAllText(fs.Path.Combine(_services.Root("appdata"), "settings.json")).ShouldBe("{}");
        fs.File.ReadAllText(fs.Path.Combine(_services.Root("temp"), "t.txt")).ShouldBe("t");
        fs.File.ReadAllText(fs.Path.Combine(_services.Root("desktop"), "link.lnk")).ShouldBe("l");
        fs.Directory.Exists(_services.PackagesDirectory).ShouldBeFalse();

        _services.ProcessService.Received().WaitForExit(77, TimeSpan.FromSeconds(1));
        _services.ProcessService.Received().Kill("helper");
        _services.ProcessService.Received().Start(fs.Path.Combine(_services.AppDirectory, "app.exe"), "--migrated");
        _services.ProcessService.Received().Start(options.Application.ExecutablePath, "--updated --always");
        _services.ProcessService.DidNotReceive().Start(options.Application.ExecutablePath,
            Arg.Is<string>(a => a.Contains("--failed")));

        _services.Reporter.Failures.ShouldBeEmpty();
        _services.Reporter.Terminated.ShouldBe(1);
        _services.Reporter.Operations.Select(o => o.Text).ShouldBe([
            "Waiting for App to close...", "Terminating process \"helper\"...", "Deleting file \"obsolete.dll\"...",
            "Starting process \"%program%\\app.exe\"..."
        ]);
        _services.Reporter.Unpacking.Count.ShouldBe(6);
        _services.Reporter.Operations.Last().Progress.ShouldBe(100f);
        _services.Reporter.Unpacking.Select(u => u.Progress)
            .ShouldBe(_services.Reporter.Unpacking.Select(u => u.Progress).OrderBy(p => p));
        // Operations and files together form the total; every report is below or at 100.
        _services.Reporter.Operations[0].Progress.ShouldBe(0f); // waiting for the host comes before any task
        _services.Reporter.Operations.Skip(1).Concat(_services.Reporter.Unpacking)
            .All(r => r.Progress is > 0 and <= 100).ShouldBeTrue();
    }

    [Fact]
    public void Run_SkipsDesktopInServiceContextButKeepsProgressConsistent()
    {
        _services.EnvironmentInfo.IsServiceContext.Returns(true);
        var package = _services.AddPackage("1.1.0",
            new Dictionary<string, string> { ["Program/a.txt"] = "a", ["Desktop/l.lnk"] = "l" });
        var options = _services.Options(package);
        options.Host.AfterInstall = AfterInstall.Close;
        options.Host.ProcessId = null;

        new InstallEngine(_services.Services).Run(options, _services.Reporter).Succeeded.ShouldBeTrue();

        _services.FileSystem.Directory.Exists(_services.Root("desktop")).ShouldBeFalse();
        _services.Reporter.Unpacking.Single().Text.ShouldBe("a.txt");
        _services.ProcessService.DidNotReceive().WaitForExit(Arg.Any<int>(), Arg.Any<TimeSpan>());
        _services.ProcessService.DidNotReceive().Start(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public void Run_IgnoresEmptyRootFolders()
    {
        _services.SpecialFolders.ApplicationData.Returns(string.Empty);
        var package = _services.AddPackage("1.1.0",
            new Dictionary<string, string> { ["Program/a.txt"] = "a", ["AppData/"] = "", ["Temp/"] = "" });
        var options = _services.Options(package);
        options.Host.AfterInstall = AfterInstall.KeepRunning;
        new InstallEngine(_services.Services).Run(options, _services.Reporter).Succeeded.ShouldBeTrue();
        _services.FileSystem.Directory.Exists(_services.Root("temp")).ShouldBeFalse();
    }

    [Fact]
    public void Run_PackageWithoutOperationsWorks()
    {
        var package = _services.AddPackage("2.0.0", new Dictionary<string, string> { ["Program/x.txt"] = "x" });
        var options = _services.Options(package);
        options.Host.AfterInstall = AfterInstall.KeepRunning;
        new InstallEngine(_services.Services).Run(options, _services.Reporter).Succeeded.ShouldBeTrue();
        _services.FileSystem.File.ReadAllText(AppFile("x.txt")).ShouldBe("x");
    }

    [Fact]
    public void Run_OrdersPackagesByManifestVersionNotFileName()
    {
        var fs = _services.FileSystem;
        var newer = fs.Path.Combine(_services.PackagesDirectory, "a.zip");
        var older = fs.Path.Combine(_services.PackagesDirectory, "b.zip");
        fs.AddFile(newer,
            new MockFileData(TestInstallerServices.BuildZip(new Dictionary<string, string> { ["Program/x.txt"] = "2" },
                TestInstallerServices.Manifest("2.0.0"))));
        fs.AddFile(older,
            new MockFileData(TestInstallerServices.BuildZip(new Dictionary<string, string> { ["Program/x.txt"] = "1" },
                TestInstallerServices.Manifest("1.5.0-beta.1"))));
        var options = _services.Options(newer, older);
        options.Host.AfterInstall = AfterInstall.KeepRunning;

        new InstallEngine(_services.Services).Run(options, _services.Reporter).Succeeded.ShouldBeTrue();

        fs.File.ReadAllText(AppFile("x.txt")).ShouldBe("2");
        _services.Reporter.Unpacking.Select(u => u.Progress).ShouldBe([50f, 100f]);
    }

    [Fact]
    public void Run_FailsCleanlyWhenAPackageIsMissing()
    {
        var options = _services.Options(_services.FileSystem.Path.Combine(_services.PackagesDirectory, "9.0.0.0.zip"));
        options.Arguments = [new InstallerArgument("--failed", ArgumentCondition.Failed)];

        var result = new InstallEngine(_services.Services).Run(options, _services.Reporter);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldBeOfType<FileNotFoundException>();
        _services.Reporter.Failures.Single().ShouldBe(result.Error);
        _services.Reporter.Terminated.ShouldBe(1);
        _services.ProcessService.Received().Start(options.Application.ExecutablePath, "--failed");
    }

    [Fact]
    public void Run_StopsAfterAFailedOperationAndDoesNotRestartWithoutRestart()
    {
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/a.txt"] = "a" },
            [new StartServiceOperation { ServiceName = "svc", RunBeforeFileReplacement = true }]);
        _services.ServiceController.When(s => s.StartService("svc", Arg.Any<string[]>()))
            .Do(_ => throw new InvalidOperationException("no such service"));
        var options = _services.Options(package);
        options.Host.AfterInstall = AfterInstall.Close;

        var result = new InstallEngine(_services.Services).Run(options, _services.Reporter);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Message.ShouldBe("no such service");
        _services.FileSystem.File.Exists(AppFile("a.txt")).ShouldBeFalse();
        _services.FileSystem.Directory.Exists(_services.PackagesDirectory).ShouldBeFalse();
        _services.ProcessService.DidNotReceive().Start(Arg.Any<string>(), Arg.Any<string>());
        _services.Reporter.Terminated.ShouldBe(1);
    }

    [Theory]
    [InlineData("null", "is empty")]
    [InlineData("{broken", "is not valid")]
    [InlineData("""{"format":1,"operations":[{"type":"teleport"}]}""", "is not valid")]
    public void Run_RejectsPackagesWithAnUnreadableManifest(string manifest, string expectedMessage)
    {
        var path = _services.FileSystem.Path.Combine(_services.PackagesDirectory, "1.1.0.0.zip");
        _services.FileSystem.AddFile(path,
            new MockFileData(TestInstallerServices.BuildZip(new Dictionary<string, string>
            { ["Program/a.txt"] = "a", [PackageLayout.ManifestFileName] = manifest })));
        var result = new InstallEngine(_services.Services).Run(_services.Options(path), _services.Reporter);
        result.Error.ShouldBeOfType<InvalidDataException>().Message.ShouldContain(expectedMessage);
        _services.FileSystem.File.Exists(AppFile("a.txt")).ShouldBeFalse();
    }

    [Fact]
    public void Run_RejectsPackagesWithoutAManifest()
    {
        var path = _services.FileSystem.Path.Combine(_services.PackagesDirectory, "1.1.0.0.zip");
        _services.FileSystem.AddFile(path,
            new MockFileData(TestInstallerServices.BuildZip(new Dictionary<string, string>
            { ["Program/a.txt"] = "a" })));
        var result = new InstallEngine(_services.Services).Run(_services.Options(path), _services.Reporter);
        result.Error.ShouldBeOfType<InvalidDataException>().Message.ShouldContain("manifest.json");
    }

    [Fact]
    public void Run_RejectsPackagesWithAnotherManifestFormat()
    {
        var path = _services.FileSystem.Path.Combine(_services.PackagesDirectory, "1.1.0.0.zip");
        var manifest = TestInstallerServices.Manifest("1.1.0");
        manifest.Format = 99;
        _services.FileSystem.AddFile(path,
            new MockFileData(TestInstallerServices.BuildZip(new Dictionary<string, string>(), manifest)));
        var result = new InstallEngine(_services.Services).Run(_services.Options(path), _services.Reporter);
        result.Error.ShouldBeOfType<nUpdate.Exceptions.UnsupportedFormatException>().Message
            .ShouldContain("package manifest");
        _services.Reporter.Failures.Single().ShouldBe(result.Error);
    }

    [Fact]
    public void Run_FailsWhenThePackageDirectoryCannotBeDetermined()
    {
        var result = new InstallEngine(_services.Services).Run(_services.Options("/"), _services.Reporter);
        result.Error.ShouldBeOfType<InvalidDataException>();
        _services.Reporter.Terminated.ShouldBe(1);
    }

    [Fact]
    public void Run_WarnsAboutCleanupFailuresWithoutChangingTheOutcome()
    {
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/a.txt"] = "a" });
        var locked = _services.FileSystem.Path.Combine(_services.PackagesDirectory, "locked.tmp");
        _services.FileSystem.AddFile(locked, new MockFileData("x") { Attributes = FileAttributes.ReadOnly });
        var options = _services.Options(package);
        options.Host.AfterInstall = AfterInstall.KeepRunning;

        var result = new InstallEngine(_services.Services).Run(options, _services.Reporter);

        result.Succeeded.ShouldBeTrue();
        _services.FileSystem.File.ReadAllText(AppFile("a.txt")).ShouldBe("a");
        _services.Reporter.Failures.ShouldBeEmpty(); // no error for the user: the update is in place
        _services.Reporter.Operations.Last().ShouldBe((100f, _services.Reporter.Operations.Last().Text));
        _services.Reporter.Operations.Last().Text.ShouldStartWith("Warning: ");
        _services.Reporter.Terminated.ShouldBe(1);
    }

    [Fact]
    public void Run_SucceedsWhenTheReporterFailsWithAWarning()
    {
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/a.txt"] = "a" });
        _services.FileSystem.AddFile(_services.FileSystem.Path.Combine(_services.PackagesDirectory, "locked.tmp"),
            new MockFileData("x") { Attributes = FileAttributes.ReadOnly });
        var options = _services.Options(package);
        options.Host = new HostOptions { ProcessId = null, AfterInstall = AfterInstall.KeepRunning };
        var reporter = Substitute.For<IProgressReporter>();
        reporter.When(r => r.ReportOperationProgress(Arg.Any<float>(), Arg.Any<string>()))
            .Do(_ => throw new InvalidOperationException("window gone"));

        new InstallEngine(_services.Services).Run(options, reporter).Succeeded.ShouldBeTrue();
    }

    [Fact]
    public void Run_AbortsOnLockedFilesWhenTheUiSaysSo()
    {
        var fs = _services.FileSystem;
        fs.AddFile(AppFile("app.exe"), new MockFileData("old") { AllowedFileShare = FileShare.None });
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/app.exe"] = "new" });
        var options = _services.Options(package);
        options.Host.AfterInstall = AfterInstall.KeepRunning;

        var result = new InstallEngine(_services.Services).Run(options, _services.Reporter);

        result.Error.ShouldBeOfType<LockedFileException>();
        _services.Reporter.LockedFiles.Single().Path.ShouldBe(AppFile("app.exe"));
        fs.GetFile(AppFile("app.exe")).TextContents.ShouldBe("old");
    }

    [Fact]
    public void Run_ValidatesArguments()
    {
        var engine = new InstallEngine(_services.Services);
        Should.Throw<ArgumentNullException>(() => engine.Run(null!, _services.Reporter));
        Should.Throw<ArgumentNullException>(() => engine.Run(_services.Options("/p/1.0.0.0.zip"), null!));
        new InstallEngine().ShouldNotBeNull();
    }

    [Fact]
    public void Run_UsesCustomExtractor()
    {
        var extractor = Substitute.For<IPackageExtractor>();
        var fs = _services.FileSystem;
        extractor.When(e => e.Extract(Arg.Any<string>(), Arg.Any<string>())).Do(call =>
            fs.AddFile(fs.Path.Combine(call.ArgAt<string>(1), PackageLayout.ManifestFileName),
                new MockFileData(Serializer.Serialize(TestInstallerServices.Manifest("1.1.0")))));
        _services.Services.PackageExtractor = extractor;
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string>());
        var options = _services.Options(package);
        options.Host.AfterInstall = AfterInstall.KeepRunning;
        new InstallEngine(_services.Services).Run(options, _services.Reporter).Succeeded.ShouldBeTrue();
        extractor.Received().Extract(package, fs.Path.Combine(_services.PackagesDirectory, "1.1.0"));
    }

    [Fact]
    public void BuildArguments_FiltersByOptionAndQuotesSpaces()
    {
        var arguments = new[]
        {
            new InstallerArgument("--a", ArgumentCondition.Succeeded),
            new InstallerArgument("with space", ArgumentCondition.Succeeded),
            new InstallerArgument("\"already quoted\"", ArgumentCondition.Succeeded),
            new InstallerArgument("--f", ArgumentCondition.Failed),
            new InstallerArgument("--always"),
        };
        InstallEngine.BuildArguments(arguments, ArgumentCondition.Succeeded)
            .ShouldBe("--a \"with space\" \"already quoted\" --always");
        InstallEngine.BuildArguments(arguments, ArgumentCondition.Failed).ShouldBe("--f --always");
        InstallEngine.BuildArguments([], ArgumentCondition.Failed).ShouldBe("");
        Should.Throw<ArgumentNullException>(() => InstallEngine.BuildArguments(null!, ArgumentCondition.Failed));
    }

    [Theory]
    [InlineData("", "\"\"")]
    [InlineData("say \"hi\"", "\"say \\\"hi\\\"\"")]
    [InlineData("tab\there", "\"tab\there\"")]
    [InlineData("C:\\dir with space\\", "\"C:\\dir with space\\\\\"")]
    [InlineData("back\\\"slash", "\"back\\\\\\\"slash\"")]
    [InlineData("C:\\plain\\path", "C:\\plain\\path")]
    public void BuildArguments_EscapesLikeTheWindowsCommandLineParser(string argument, string expected)
    {
        InstallEngine
            .BuildArguments([new InstallerArgument(argument, ArgumentCondition.Succeeded)], ArgumentCondition.Succeeded)
            .ShouldBe(expected);
    }

    [Fact]
    public void Run_FailsWhenNoPackagesAreListed()
    {
        var options = _services.Options();

        var result = new InstallEngine(_services.Services).Run(options, _services.Reporter);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldBeOfType<InvalidDataException>();
        _services.Reporter.Terminated.ShouldBe(1);
    }

    [Fact]
    public void Run_SurvivesAReporterWhoseFailThrows()
    {
        var reporter = Substitute.For<IProgressReporter>();
        reporter.When(r => r.Fail(Arg.Any<Exception>())).Do(_ => throw new InvalidOperationException("ui is gone"));
        var options = _services.Options(_services.FileSystem.Path.Combine(_services.PackagesDirectory, "9.0.0.0.zip"));

        var result = new InstallEngine(_services.Services).Run(options, reporter);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldBeOfType<FileNotFoundException>();
        reporter.Received(1).Terminate();
    }

    [Fact]
    public void Run_ReportsARestartThatFailsWithoutChangingTheOutcome()
    {
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/a.txt"] = "a" });
        _services.ProcessService.Start(Arg.Any<string>(), Arg.Any<string>())
            .Returns(_ => throw new System.ComponentModel.Win32Exception(2));
        var options = _services.Options(package);

        var result = new InstallEngine(_services.Services).Run(options, _services.Reporter);

        result.Succeeded.ShouldBeTrue();
        _services.FileSystem.File.ReadAllText(AppFile("a.txt")).ShouldBe("a");
        _services.Reporter.Failures.Single().ShouldBeOfType<System.ComponentModel.Win32Exception>();
        _services.Reporter.Terminated.ShouldBe(1);

        _services.Reporter.Failures.Clear();
        var failing = _services.Options(_services.FileSystem.Path.Combine(_services.PackagesDirectory, "9.0.0.0.zip"));
        new InstallEngine(_services.Services).Run(failing, _services.Reporter).Succeeded.ShouldBeFalse();
        _services.Reporter.Failures.Select(f => f.GetType()).ShouldBe([
            typeof(FileNotFoundException), typeof(System.ComponentModel.Win32Exception)
        ]);
    }

    [Fact]
    public void Run_ClearsAnExtractionFolderLeftByAnEarlierRun()
    {
        var fs = _services.FileSystem;
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/a.txt"] = "a" });
        var stale = fs.Path.Combine(_services.PackagesDirectory, "1.1.0", "Program", "stale.dll");
        fs.AddFile(stale, new MockFileData("left over"));
        var options = _services.Options(package);
        options.Host.AfterInstall = AfterInstall.KeepRunning;

        new InstallEngine(_services.Services).Run(options, _services.Reporter).Succeeded.ShouldBeTrue();

        fs.File.ReadAllText(AppFile("a.txt")).ShouldBe("a");
        fs.File.Exists(AppFile("stale.dll")).ShouldBeFalse();
    }

    [Fact]
    public void Run_ContinuesWhenTheHostDoesNotExitInTime()
    {
        _services.ProcessService.WaitForExit(Arg.Any<int>(), Arg.Any<TimeSpan>()).Returns(false);
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/a.txt"] = "a" });
        var options = _services.Options(package);
        options.Host.AfterInstall = AfterInstall.KeepRunning;

        new InstallEngine(_services.Services).Run(options, _services.Reporter).Succeeded.ShouldBeTrue();
        _services.FileSystem.File.ReadAllText(AppFile("a.txt")).ShouldBe("a");
    }

    [Fact]
    public void Run_AppliesEveryTypedOperationFromTheManifest()
    {
        var fs = _services.FileSystem;
        fs.AddFile(AppFile("old.dll"), new MockFileData("x"));
        fs.AddFile(AppFile("keep.txt"), new MockFileData("k"));
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/a.txt"] = "a" },
        [
            new DeleteFilesOperation { Directory = "%program%", Files = ["old.dll"] },
            new RenameFileOperation { Path = "%program%\\keep.txt", NewName = "kept.txt" },
            new SetRegistryValuesOperation
                { Key = "HKEY_CURRENT_USER\\Software\\App", Values = [RegistryValue.DWord("Installed", 42)] },
            new StartServiceOperation { ServiceName = "svc", Arguments = ["-a", "-b"] },
            new TerminateProcessOperation { ProcessName = "helper", RunBeforeFileReplacement = true },
        ]);
        var options = _services.Options(package);
        options.Host.AfterInstall = AfterInstall.KeepRunning;

        var result = new InstallEngine(_services.Services).Run(options, _services.Reporter);

        result.Error?.ToString().ShouldBeNull();
        fs.File.Exists(AppFile("old.dll")).ShouldBeFalse();
        fs.File.ReadAllText(AppFile("kept.txt")).ShouldBe("k");
        fs.File.ReadAllText(AppFile("a.txt")).ShouldBe("a");
        _services.Registry.Received().SetValue(@"HKEY_CURRENT_USER\Software\App",
            Arg.Is<RegistryValue>(v =>
                v.Name == "Installed" && Equals(v.Value, 42L) && v.Kind == RegistryValueKind.DWord));
        _services.ServiceController.Received()
            .StartService("svc", Arg.Is<string[]>(a => a.SequenceEqual(new[] { "-a", "-b" })));
        _services.ProcessService.Received().Kill("helper");
        _services.Reporter.Operations[1].Text.ShouldBe("Terminating process \"helper\"...");
        _services.Reporter.Operations.Select(o => o.Progress)
            .ShouldBe(_services.Reporter.Operations.Select(o => o.Progress).OrderBy(p => p));
    }

    [Fact]
    public void Run_RefusesRegistryAndServiceOperationsOffWindowsBeforeTouchingAnything()
    {
        _services.EnvironmentInfo.IsWindows.Returns(false);
        var fs = _services.FileSystem;
        fs.AddFile(AppFile("app"), new MockFileData("v1"));
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/app"] = "v2" },
        [
            new DeleteFilesOperation { Directory = "%program%", Files = ["app"], RunBeforeFileReplacement = true },
            new StopServiceOperation { ServiceName = "svc" }
        ]);

        var result = new InstallEngine(_services.Services).Run(_services.Options(package), _services.Reporter);

        result.Error.ShouldBeOfType<InvalidDataException>().Message
            .ShouldBe("The package 1.1.0 contains registry or service operations, which only exist on Windows.");
        fs.File.ReadAllText(AppFile("app")).ShouldBe("v1");
        _services.ServiceController.ReceivedCalls().ShouldBeEmpty();
    }

    [Fact]
    public void Run_OffWindows_ReplacesFilesWithoutProbingForLocks()
    {
        _services.EnvironmentInfo.IsWindows.Returns(false);
        var fs = _services.FileSystem;
        fs.AddFile(AppFile("app"), new MockFileData("v1"));
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string> { ["Program/app"] = "v2" });

        new InstallEngine(_services.Services).Run(_services.Options(package), _services.Reporter).Succeeded
            .ShouldBeTrue();

        fs.File.ReadAllText(AppFile("app")).ShouldBe("v2");
    }

    private (InstallerOptions Options, string Bundle) BundleOptions(params string[] packages)
    {
        var bundle = _services.Root("Applications") + "/App.app";
        var options = _services.Options(packages);
        options.Application.Bundle = bundle;
        options.Application.Directory = bundle + "/Contents/MacOS";
        options.Application.ExecutablePath = bundle + "/Contents/MacOS/App";
        return (options, bundle);
    }

    [Fact]
    public void Run_ReplacesAMacOSBundleAsAWhole()
    {
        _services.EnvironmentInfo.IsWindows.Returns(false);
        _services.Services.DirectorySwap =
            new nUpdate.UpdateInstaller.Platform.DirectorySwap(_services.FileSystem, atomic: false);
        var fs = _services.FileSystem;
        var package = _services.AddPackage("1.1.0", new Dictionary<string, string>
        {
            ["Program/Contents/Info.plist"] = "plist v2",
            ["Program/Contents/MacOS/App"] = "binary v2",
            ["AppData/App/settings.json"] = "{}",
        },
            [
                new DeleteFilesOperation
                {
                    Directory = "%program%/Contents/Resources", Files = ["cache.bin"], RunBeforeFileReplacement = true
                }
            ]);
        var (options, bundle) = BundleOptions(package);
        fs.AddFile(bundle + "/Contents/Info.plist", new MockFileData("plist v1"));
        fs.AddFile(bundle + "/Contents/MacOS/App", new MockFileData("binary v1"));
        fs.AddFile(bundle + "/Contents/Resources/cache.bin", new MockFileData("cache"));
        fs.AddFile(bundle + "/Contents/Resources/old.png", new MockFileData("old"));
        fs.AddFile(bundle + ".new/stale.txt", new MockFileData("stale"));

        var result = new InstallEngine(_services.Services).Run(options, _services.Reporter);

        result.Error?.ToString().ShouldBeNull();
        fs.File.ReadAllText(bundle + "/Contents/Info.plist").ShouldBe("plist v2");
        fs.File.ReadAllText(bundle + "/Contents/MacOS/App").ShouldBe("binary v2");
        fs.Directory.Exists(bundle + "/Contents/Resources")
            .ShouldBeFalse(); // the bundle is the package's, nothing of the old one survives
        fs.Directory.Exists(bundle + ".new").ShouldBeFalse();
        fs.File.ReadAllText(fs.Path.Combine(_services.Root("appdata"), "App", "settings.json")).ShouldBe("{}");
        _services.Reporter.Operations[1].Text.ShouldBe("Deleting file \"cache.bin\"...");
        _services.ProcessService.Received().Start(options.Application.ExecutablePath, "");
    }

    [Fact]
    public void Run_RefusesABundlePackageWithoutInfoPlistAndKeepsPackagesWithoutProgramFiles()
    {
        var fs = _services.FileSystem;
        var partial = _services.AddPackage("1.1.0",
            new Dictionary<string, string> { ["Program/Contents/MacOS/App"] = "binary v2" });
        var (options, bundle) = BundleOptions(partial);
        fs.AddFile(bundle + "/Contents/MacOS/App", new MockFileData("binary v1"));

        new InstallEngine(_services.Services).Run(options, _services.Reporter).Error
            .ShouldBeOfType<InvalidDataException>().Message.ShouldContain("Contents/Info.plist is missing");
        fs.File.ReadAllText(bundle + "/Contents/MacOS/App").ShouldBe("binary v1");

        var settingsOnly = _services.AddPackage("1.2.0",
            new Dictionary<string, string> { ["AppData/App/settings.json"] = "{}" });
        (options, _) = BundleOptions(settingsOnly);
        new InstallEngine(_services.Services).Run(options, _services.Reporter).Succeeded.ShouldBeTrue();
        _services.DirectorySwap.ReceivedCalls().ShouldBeEmpty();
    }

    [Fact]
    public void Run_WarnsAboutAnOldBundleThatCannotBeDeleted()
    {
        var fs = _services.FileSystem;
        var package = _services.AddPackage("1.1.0",
            new Dictionary<string, string> { ["Program/Contents/Info.plist"] = "plist v2" });
        var (options, bundle) = BundleOptions(package);
        _services.DirectorySwap.When(s => s.Swap(bundle, bundle + ".new")).Do(_ =>
            fs.AddFile(bundle + ".new/Contents/locked.bin",
                new MockFileData("x") { Attributes = FileAttributes.ReadOnly }));

        var result = new InstallEngine(_services.Services).Run(options, _services.Reporter);

        result.Succeeded.ShouldBeTrue();
        _services.Reporter.Failures.ShouldBeEmpty();
        _services.Reporter.Operations.ShouldContain(o =>
            o.Text == $"Warning: The previous bundle could not be deleted from \"{bundle}.new\".");
    }

    [Fact]
    public void Run_RemovesAHalfBuiltBundleAndKeepsTheInstalledOne()
    {
        var fs = _services.FileSystem;
        var package = _services.AddPackage("1.1.0",
            new Dictionary<string, string>
            { ["Program/Contents/Info.plist"] = "plist v2", ["Program/Contents/MacOS/App"] = "binary v2" });
        var (options, bundle) = BundleOptions(package);
        fs.AddFile(bundle + "/Contents/Info.plist", new MockFileData("plist v1"));
        // The copy into the new bundle stops after its first file, the way a full disk would stop it.
        var reporter = Substitute.For<IProgressReporter>();
        reporter.When(r => r.ReportUnpackingProgress(Arg.Any<float>(), Arg.Any<string>()))
            .Do(_ => throw new IOException("disk full"));

        var result = new InstallEngine(_services.Services).Run(options, reporter);

        result.Error.ShouldBeOfType<IOException>().Message.ShouldBe("disk full");
        fs.Directory.Exists(bundle + ".new").ShouldBeFalse();
        fs.File.ReadAllText(bundle + "/Contents/Info.plist").ShouldBe("plist v1");
        _services.DirectorySwap.ReceivedCalls().ShouldBeEmpty();
    }
}
