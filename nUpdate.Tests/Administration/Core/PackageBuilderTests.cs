using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using nUpdate.Administration.Core.Packages;
using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Platform;
using nUpdate.Tests.Administration.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public class PackageBuilderTests
{
    private readonly AdminTestContext _context = new();

    [Fact]
    public async Task Build_WritesFilesAndManifestWithoutEmptyRoots()
    {
        var version = new UpdateVersion("1.0.0-beta.1");
        var definition = new PlatformPackage("linux-x64");
        definition.Files.Add(new PackageFileEntry(PackageRoot.Program, "app.dll",
            _context.AddSourceFile("app.dll", "binary")));
        definition.Files.Add(new PackageFileEntry(PackageRoot.AppData, "cfg/settings.json",
            _context.AddSourceFile("settings.json", "{}")));
        definition.Operations.Add(
            new TerminateProcessOperation { ProcessName = "app", RunBeforeFileReplacement = true });
        var project = _context.NewProject();
        var packagePath = project.PackageFilePath(version, definition.Platform);

        var manifest = await _context.Builder.BuildAsync(definition, version, project.Id, packagePath);

        manifest.ProjectId.ShouldBe(project.Id);
        manifest.Version.ShouldBe(version);
        manifest.Platform.ShouldBe("linux-x64");
        manifest.CreatedAt.ShouldBe(AdminTestContext.Now);
        manifest.Touches.ShouldBe([OperationArea.Processes]);
        using var archive = new ZipArchive(new MemoryStream(_context.FileSystem.File.ReadAllBytes(packagePath)),
            ZipArchiveMode.Read);
        archive.Entries.Select(e => e.FullName).OrderBy(n => n, StringComparer.Ordinal)
            .ShouldBe(["AppData/cfg/settings.json", "Program/app.dll", "manifest.json"]);
        using (var reader = new StreamReader(archive.GetEntry("Program/app.dll")!.Open()))
            (await reader.ReadToEndAsync()).ShouldBe("binary");
        archive.GetEntry("Program/app.dll")!.ExternalAttributes
            .ShouldBe(unchecked((int)0x81A40000)); // a regular file with 0644
        var manifestFile =
            _context.FileSystem.Path.Combine(project.PlatformDirectory(version, "linux-x64"), "manifest.json");
        var written = Serializer.Deserialize<PackageManifest>(_context.FileSystem.File.ReadAllText(manifestFile))!;
        written.Operations.Single().ShouldBeOfType<TerminateProcessOperation>().RunBeforeFileReplacement.ShouldBeTrue();
        _context.FileSystem.File.ReadAllText(manifestFile).ShouldContain("\"type\": \"terminateProcess\"");

        var content = await new PackageContentReader(_context.FileSystem).ReadAsync(packagePath);
        content.Entries.Select(e => $"{e.Root}:{e.RelativePath}:{e.Size}")
            .ShouldBe(["Program:app.dll:6", "AppData:cfg/settings.json:2"]);
        content.Manifest!.Operations.Single().ShouldBeOfType<TerminateProcessOperation>().ProcessName.ShouldBe("app");
    }

    [Fact]
    public async Task Build_StoresDetectedModesOnWindows()
    {
        var builder = new PackageBuilder(_context.FileSystem, () => AdminTestContext.Now, isWindows: true);
        var package = new PlatformPackage("osx-arm64");
        _context.FileSystem.AddFile("/src/App", new MockFileData([0xCF, 0xFA, 0xED, 0xFE, 7]));
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "Contents/MacOS/App", "/src/App"));
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "Contents/Info.plist",
            _context.AddSourceFile("Info.plist", "<plist/>")));

        await builder.BuildAsync(package, new UpdateVersion("1.0.0"), Guid.Empty, "/out/osx.zip");

        using var archive = new ZipArchive(new MemoryStream(_context.FileSystem.File.ReadAllBytes("/out/osx.zip")),
            ZipArchiveMode.Read);
        ((archive.GetEntry("Program/Contents/MacOS/App")!.ExternalAttributes >> 16) & 0xFFFF).ShouldBe(0x81ED);
        ((archive.GetEntry("Program/Contents/Info.plist")!.ExternalAttributes >> 16) & 0xFFFF).ShouldBe(0x81A4);
    }

    [Fact]
    public async Task Build_StoresTheCodeSignatureAttributesOfTheFiles()
    {
        // macOS keeps the signature of a file in Contents/MacOS that is no Mach-O in extended attributes, which a zip loses.
        var signatures = Substitute.For<ICodeSignatureAttributes>();
        var dll = _context.AddSourceFile("App.dll", "assembly");
        signatures.Read(dll).Returns(new Dictionary<string, byte[]> { ["com.apple.cs.CodeDirectory"] = [1, 2] });
        signatures.Read(Arg.Is<string>(p => p != dll)).Returns(new Dictionary<string, byte[]>());
        var builder = new PackageBuilder(_context.FileSystem, () => AdminTestContext.Now, false, signatures);
        var package = new PlatformPackage("osx-arm64");
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "Contents/MacOS/App.dll", dll));
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "Contents/MacOS/Kept.dll",
                _context.AddSourceFile("Kept.dll", "kept"))
        { CodeSignature = new Dictionary<string, string> { ["com.apple.cs.CodeSignature"] = "Aw==" } });
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "Contents/Info.plist",
            _context.AddSourceFile("Info.plist", "<plist/>")));

        var manifest = await builder.BuildAsync(package, new UpdateVersion("1.0.0"), Guid.Empty, "/out/osx.zip");

        manifest.CodeSignatures.Keys.ShouldBe(["Program/Contents/MacOS/App.dll", "Program/Contents/MacOS/Kept.dll"],
            ignoreOrder: true);
        manifest.CodeSignatures["Program/Contents/MacOS/App.dll"]["com.apple.cs.CodeDirectory"].ShouldBe("AQI=");
        manifest.CodeSignatures["Program/Contents/MacOS/Kept.dll"]["com.apple.cs.CodeSignature"].ShouldBe("Aw==");
        signatures.DidNotReceive().Read(Arg.Is<string>(p => p.EndsWith("Kept.dll", StringComparison.Ordinal)));
    }

    [Fact]
    public async Task Build_StoresTheGivenModeInsteadOfDetectingOne()
    {
        var builder = new PackageBuilder(_context.FileSystem, () => AdminTestContext.Now, isWindows: true);
        var package = new PlatformPackage("linux-x64");
        package.Files.Add(new PackageFileEntry(PackageRoot.Program, "run", _context.AddSourceFile("run", "no shebang"))
        { UnixMode = 0x1ED });

        await builder.BuildAsync(package, new UpdateVersion("1.0.0"), Guid.Empty, "/out/linux.zip");

        using var archive = new ZipArchive(new MemoryStream(_context.FileSystem.File.ReadAllBytes("/out/linux.zip")),
            ZipArchiveMode.Read);
        ((archive.GetEntry("Program/run")!.ExternalAttributes >> 16) & 0xFFFF).ShouldBe(0x81ED);
    }

    [Fact]
    public async Task Build_RejectsDuplicatesMissingFilesLinksWindowsOnlyOperationsAndBadArguments()
    {
        var builder = new PackageBuilder(_context.FileSystem);
        var version = new UpdateVersion("1.0.0");
        var definition = new PlatformPackage("any");
        var source = _context.AddSourceFile("a.txt", "a");
        definition.Files.Add(new PackageFileEntry(PackageRoot.Program, "a.txt", source));
        definition.Files.Add(new PackageFileEntry(PackageRoot.Program, "A.TXT", source));
        (await Should.ThrowAsync<InvalidOperationException>(() =>
                builder.BuildAsync(definition, version, Guid.Empty, "/out/pkg.zip"))).Message
            .ShouldContain("more than once");

        var missing = new PlatformPackage("any");
        missing.Files.Add(new PackageFileEntry(PackageRoot.Program, "b.txt", "/nope/b.txt"));
        await Should.ThrowAsync<FileNotFoundException>(() =>
            builder.BuildAsync(missing, version, Guid.Empty, "/out/pkg.zip"));

        var linked = new PlatformPackage("any");
        _context.FileSystem.AddDirectory("/src");
        _context.FileSystem.File.CreateSymbolicLink("/src/link.txt", source);
        linked.Files.Add(new PackageFileEntry(PackageRoot.Program, "link.txt", "/src/link.txt"));
        (await Should.ThrowAsync<InvalidOperationException>(() =>
            builder.BuildAsync(linked, version, Guid.Empty, "/out/pkg.zip"))).Message.ShouldContain("symbolic link");

        var windowsRegistry = new PlatformPackage("win");
        windowsRegistry.Operations.Add(new DeleteRegistryValuesOperation { Key = "HKCU\\x", Names = ["v"] });
        (await builder.BuildAsync(windowsRegistry, version, Guid.Empty, "/out/win.zip")).Touches.ShouldBe([
            OperationArea.Registry
        ]);

        await Should.ThrowAsync<ArgumentNullException>(() =>
            builder.BuildAsync(null!, version, Guid.Empty, "/out/pkg.zip"));
        await Should.ThrowAsync<ArgumentNullException>(() =>
            builder.BuildAsync(missing, null!, Guid.Empty, "/out/pkg.zip"));
        await Should.ThrowAsync<ArgumentException>(() => builder.BuildAsync(missing, version, Guid.Empty, " "));
        Should.Throw<ArgumentNullException>(() => new PackageBuilder(null!));
        Should.Throw<ArgumentNullException>(() => new PackageBuilder(_context.FileSystem, null!, false));
        Should.Throw<ArgumentNullException>(() =>
            new PackageBuilder(_context.FileSystem, () => AdminTestContext.Now, false, null!));

        (await builder.BuildAsync(new PlatformPackage("any"), new UpdateVersion("2.0.0"), Guid.Empty, "pkg-in-cwd.zip"))
            .CreatedAt.ShouldBeGreaterThan(AdminTestContext.Now);
        _context.FileSystem.File.Exists("pkg-in-cwd.zip").ShouldBeTrue();

        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var one = new PlatformPackage("any");
        one.Files.Add(new PackageFileEntry(PackageRoot.Program, "a.txt", source));
        await Should.ThrowAsync<OperationCanceledException>(() =>
            builder.BuildAsync(one, new UpdateVersion("3.0.0"), Guid.Empty, "/out/c.zip", cts.Token));
    }
}
