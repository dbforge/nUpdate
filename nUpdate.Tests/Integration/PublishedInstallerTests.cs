using System.Diagnostics;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using nUpdate.Administration.Core.Packages;
using nUpdate.Installer;
using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Platform;
using nUpdate.Updating;

namespace nUpdate.Tests.Integration;

/// <summary>
///     Runs a published installer (the built-in one for the runtime identifier of the machine) the way the client starts
///     it: windowless, with real packages built by nUpdate Administration. CI sets <c>NUPDATE_INSTALLER</c> to the
///     executable on Linux, Windows and macOS; without it the tests are skipped.
/// </summary>
[Trait("Category", "PublishedInstaller")]
public sealed class PublishedInstallerTests : IDisposable
{
    private static readonly Guid ProjectId = Guid.Parse("6b1d3f20-58c4-4c0e-a6a1-1d2b3c4d5e6f");
    private readonly string _root = Directory.CreateTempSubdirectory("nupdate-published-").FullName;
    private readonly IFileSystem _fileSystem = new FileSystem();

    public void Dispose()
    {
        try
        {
            Directory.Delete(_root, recursive: true);
        }
        catch (IOException)
        {
            // A process the installer started may still hold a file; the temp folder is cleaned up eventually.
        }
    }

    private string Path(params string[] parts) => System.IO.Path.Combine([_root, .. parts]);

    private string Write(string relativePath, string text)
    {
        var path = Path(relativePath);
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text);
        return path;
    }

    private async Task<string> BuildPackageAsync(PlatformPackage package, string version)
    {
        var path = Path("downloads", $"{version}.zip");
        await new PackageBuilder(_fileSystem).BuildAsync(package, new UpdateVersion(version), ProjectId, path);
        return path;
    }

    /// <summary>Copies the installer into a folder of its own, writes the options next to it and runs it like UpdateManager does.</summary>
    private (int ExitCode, string Log) RunInstaller(ApplicationOptions application, params string[] packages) =>
        RunInstaller(application, packages, showWindow: false);

    private (int ExitCode, string Log) RunInstaller(ApplicationOptions application, string[] packages, bool showWindow,
        Action<ProcessStartInfo>? configure = null)
    {
        var source = Environment.GetEnvironmentVariable(PublishedInstallerFactAttribute.Variable)!;
        var folder = Path("nUpdate Installer");
        Directory.CreateDirectory(folder);
        var installer = System.IO.Path.Combine(folder, System.IO.Path.GetFileName(source));
        File.Copy(source, installer, overwrite: true);
        if (!OperatingSystem.IsWindows())
            File.SetUnixFileMode(installer, (UnixFileMode)Convert.ToInt32("755", 8));

        var options = new InstallerOptions
        {
            Packages = packages.Select(p => new InstallerPackage { Path = p }).ToList(),
            Application = application,
            Host = new HostOptions { ProcessId = null, AfterInstall = AfterInstall.KeepRunning },
            Ui = new InstallerUiOptions { ShowWindow = showWindow },
        };
        var optionsPath = System.IO.Path.Combine(folder, "installer-options.json");
        File.WriteAllText(optionsPath, Serializer.Serialize(options, indented: true));

        var startInfo = new ProcessStartInfo(installer, $"\"{optionsPath}\"")
        { UseShellExecute = false, RedirectStandardError = true };
        configure?.Invoke(startInfo);
        using var process = Process.Start(startInfo)!;
        var errors = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(120_000))
        {
            process.Kill();
            throw new TimeoutException("The installer did not finish within two minutes.");
        }

        var logPath = System.IO.Path.Combine(folder, "install.log");
        var log = File.Exists(logPath) ? File.ReadAllText(logPath) : "(no install.log)";
        return (process.ExitCode, log + errors.Result);
    }

    [PublishedInstallerFact]
    public async Task Installs_files_restores_their_modes_runs_operations_and_logs()
    {
        var application = Path("app");
        Write("app/app.txt", "version 1");
        Write("app/obsolete.txt", "old");
        var package = new PlatformPackage(PackagePlatform.Any);
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "app.txt", Write("build/app.txt", "version 2")));
        var tool = Write("build/tool", "#!/bin/sh\necho tool\n");
        if (!OperatingSystem.IsWindows())
            File.SetUnixFileMode(tool,
                (UnixFileMode)FilePermissions.ExecutableMode); // the builder takes the mode of the source file
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "bin/tool", tool));
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "data/settings.json",
            Write("build/settings.json", "{}")));
        package.Operations.Add(new DeleteFilesOperation
        { Directory = "%program%", Files = ["obsolete.txt"], RunBeforeFileReplacement = true });

        var (exitCode, log) = RunInstaller(
            new ApplicationOptions
            {
                Name = "Published",
                Directory = application,
                ExecutablePath = System.IO.Path.Combine(application, "app")
            },
            await BuildPackageAsync(package, "1.1.0"));

        exitCode.ShouldBe(0, log);
        File.ReadAllText(Path("app", "app.txt")).ShouldBe("version 2");
        File.Exists(Path("app", "obsolete.txt")).ShouldBeFalse();
        File.ReadAllText(Path("app", "data", "settings.json")).ShouldBe("{}");
        if (!OperatingSystem.IsWindows())
        {
            ((int)File.GetUnixFileMode(Path("app", "bin", "tool"))).ShouldBe(FilePermissions.ExecutableMode);
            ((int)File.GetUnixFileMode(Path("app", "data", "settings.json"))).ShouldBe(FilePermissions.RegularMode);
        }

        log.ShouldContain("Running without a window, as the options ask.");
        log.ShouldContain("Copied app.txt");
        log.ShouldContain("Finished.");
        Directory.Exists(Path("downloads")).ShouldBeFalse(); // the installer cleans up the downloaded packages
    }

    [PublishedInstallerFact]
    public async Task Fails_without_touching_anything_when_a_package_is_broken()
    {
        var application = Path("app");
        Write("app/app.txt", "version 1");
        var package = new PlatformPackage(PackagePlatform.Any);
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "app.txt", Write("build/app.txt", "version 2")));
        var good = await BuildPackageAsync(package, "1.1.0");
        var broken = Write("downloads/1.2.0.zip", "not a zip");

        var (exitCode, log) =
            RunInstaller(
                new ApplicationOptions
                {
                    Name = "Published",
                    Directory = application,
                    ExecutablePath = System.IO.Path.Combine(application, "app")
                }, good, broken);

        exitCode.ShouldBe(1, log);
        File.ReadAllText(Path("app", "app.txt")).ShouldBe("version 1");
        log.ShouldContain("Failed:");
    }

    private async Task<(ApplicationOptions Application, string Package)> PrepareSimpleUpdateAsync()
    {
        var application = Path("app");
        Write("app/app.txt", "version 1");
        var package = new PlatformPackage(PackagePlatform.Any);
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "app.txt", Write("build/app.txt", "version 2")));
        return (
            new ApplicationOptions
            {
                Name = "Published",
                Directory = application,
                ExecutablePath = System.IO.Path.Combine(application, "app")
            }, await BuildPackageAsync(package, "1.1.0"));
    }

    [PublishedInstallerFact(NeedsDisplay = true)]
    public async Task Shows_its_window_when_there_is_a_display()
    {
        var (application, package) = await PrepareSimpleUpdateAsync();

        var (exitCode, log) = RunInstaller(application, [package], showWindow: true);

        exitCode.ShouldBe(0, log);
        File.ReadAllText(Path("app", "app.txt")).ShouldBe("version 2");
        log.ShouldNotContain("The installer window failed");
        log.ShouldNotContain("could not be created");
        if (!OperatingSystem.IsWindows()) // a Windows runner may have no interactive desktop
            log.ShouldNotContain("Running without a window");
    }

    [PublishedInstallerFact(LinuxOnly = true)]
    public async Task Falls_back_to_no_window_when_the_display_cannot_be_opened()
    {
        var (application, package) = await PrepareSimpleUpdateAsync();

        var (exitCode, log) = RunInstaller(application, [package], showWindow: true, startInfo =>
        {
            startInfo.Environment["DISPLAY"] = ":4242"; // no X server listens there
            startInfo.Environment.Remove("WAYLAND_DISPLAY");
        });

        exitCode.ShouldBe(0, log);
        File.ReadAllText(Path("app", "app.txt")).ShouldBe("version 2");
        log.ShouldContain("The installer window failed; continuing without it");
    }

    [PublishedInstallerFact(MacOsOnly = true)]
    public async Task Swaps_a_signed_macOS_bundle_that_still_verifies_and_starts()
    {
        // The installed bundle, version 1.0, with an ad-hoc signed echo as its executable.
        var bundle = Path("Applications", "Demo.app");
        CreateBundle(bundle, "1.0");
        Write("Applications/Demo.app/Contents/Resources/old.txt", "only in 1.0");

        // The new bundle, signed as a whole the way the developer ships it, goes into Program.
        var staged = Path("build", "Demo.app");
        CreateBundle(staged, "2.0");
        var package = new PlatformPackage("osx-arm64");
        foreach (var file in Directory.GetFiles(staged, "*", SearchOption.AllDirectories))
            package.Files.Add(new PackageFileEntry(PackageRoot.Program, System.IO.Path.GetRelativePath(staged, file),
                file));

        var executable = System.IO.Path.Combine(bundle, "Contents", "MacOS", "Demo");
        var (exitCode, log) = RunInstaller(new ApplicationOptions
        {
            Name = "Demo",
            Directory = System.IO.Path.GetDirectoryName(executable)!,
            ExecutablePath = executable,
            Bundle = bundle,
        }, await BuildPackageAsync(package, "2.0.0"));

        exitCode.ShouldBe(0, log);
        File.ReadAllText(System.IO.Path.Combine(bundle, "Contents", "Info.plist"))
            .ShouldContain("<string>2.0</string>");
        File.Exists(System.IO.Path.Combine(bundle, "Contents", "Resources", "old.txt")).ShouldBeFalse();
        Directory.Exists(bundle + ".new").ShouldBeFalse();
        Run("codesign", $"--verify --deep --strict \"{bundle}\"").ExitCode.ShouldBe(0);
        var started = Run(executable, "still starts");
        started.ExitCode.ShouldBe(0);
        started.Output.Trim().ShouldBe("still starts");
    }

    private static void CreateBundle(string bundle, string version)
    {
        var macOs = System.IO.Path.Combine(bundle, "Contents", "MacOS");
        Directory.CreateDirectory(macOs);
        File.WriteAllText(System.IO.Path.Combine(bundle, "Contents", "Info.plist"), $$"""
              <?xml version="1.0" encoding="UTF-8"?>
              <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
              <plist version="1.0">
              <dict>
                <key>CFBundleExecutable</key><string>Demo</string>
                <key>CFBundleIdentifier</key><string>net.nupdate.demo</string>
                <key>CFBundleName</key><string>Demo</string>
                <key>CFBundlePackageType</key><string>APPL</string>
                <key>CFBundleShortVersionString</key><string>{{version}}</string>
              </dict>
              </plist>
              """);
        // A Mach-O of its own, like echo: Apple's arm64e system binaries would not run once re-signed ad hoc.
        var source = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(bundle)!, $"demo-{version}.c");
        File.WriteAllText(source,
            "#include <stdio.h>\nint main(int argc, char **argv) { for (int i = 1; i < argc; i++) printf(i > 1 ? \" %s\" : \"%s\", argv[i]); printf(\"\\n\"); return 0; }\n");
        Run("cc", $"-o \"{System.IO.Path.Combine(macOs, "Demo")}\" \"{source}\"").ExitCode.ShouldBe(0);
        File.Delete(source);
        Run("codesign", $"--force --deep --sign - \"{bundle}\"").ExitCode.ShouldBe(0);
    }

    private static (int ExitCode, string Output) Run(string fileName, string arguments)
    {
        using var process = Process.Start(new ProcessStartInfo(fileName, arguments)
        { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true })!;
        var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
        process.WaitForExit();
        return (process.ExitCode, output);
    }
}

/// <summary>Runs only when <c>NUPDATE_INSTALLER</c> names a published installer executable.</summary>
public sealed class PublishedInstallerFactAttribute : FactAttribute
{
    public const string Variable = "NUPDATE_INSTALLER";

    public PublishedInstallerFactAttribute([CallerFilePath] string? sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = -1)
        : base(sourceFilePath, sourceLineNumber)
    {
        var path = Environment.GetEnvironmentVariable(Variable);
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            Skip = $"Set {Variable} to a published installer to run this test.";
    }

    /// <summary>Skips the test on systems other than macOS.</summary>
    public bool MacOsOnly
    {
        get => false;
        init
        {
            if (value && !OperatingSystem.IsMacOS())
                Skip ??= "This test swaps a macOS application bundle.";
        }
    }

    /// <summary>Skips the test on systems other than Linux.</summary>
    public bool LinuxOnly
    {
        get => false;
        init
        {
            if (value && !OperatingSystem.IsLinux())
                Skip ??= "This test points the installer at a missing X server.";
        }
    }

    /// <summary>Skips the test on Linux without an X11 or Wayland display (CI runs it under xvfb-run).</summary>
    public bool NeedsDisplay
    {
        get => false;
        init
        {
            if (value && OperatingSystem.IsLinux() &&
                string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")) &&
                string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")))
                Skip ??= "This test needs a display.";
        }
    }
}
