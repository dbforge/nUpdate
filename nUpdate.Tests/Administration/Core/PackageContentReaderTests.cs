using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Text;
using nUpdate.Administration.Core.Packages;
using nUpdate.Operations;
using nUpdate.Packaging;
using nUpdate.Tests.Administration.Support;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Administration.Core;

public class PackageContentReaderTests
{
    private readonly AdminTestContext _context = new();

    /// <summary>A package with an executable (0750), a file without stored permissions, a folder entry, an entry outside the roots and a manifest.</summary>
    private void AddPackage(string path)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            Write(archive, "Program/bin/app", "binary", (0x8000 | 0x1E8) << 16);
            Write(archive, "AppData/cfg.json", "{}", 0);
            archive.CreateEntry("Program/");
            Write(archive, "Unknown/x.txt", "x", 0);
            var manifest = new PackageManifest { Operations = [new TerminateProcessOperation { ProcessName = "app" }] };
            manifest.CodeSignatures["Program/bin/app"] =
                new Dictionary<string, string> { ["com.apple.cs.CodeDirectory"] = "AQ==" };
            Write(archive, PackageLayout.ManifestFileName, Serializer.Serialize(manifest), 0);
        }

        _context.FileSystem.AddFile(path, new MockFileData(stream.ToArray()));
    }

    private static void Write(ZipArchive archive, string name, string content, int externalAttributes)
    {
        var entry = archive.CreateEntry(name);
        entry.ExternalAttributes = externalAttributes;
        using var output = entry.Open();
        output.Write(Encoding.UTF8.GetBytes(content));
    }

    [Fact]
    public async Task Read_SkipsUnknownRootsAndReportsMissingManifest()
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            archive.CreateEntry("Unknown/x.txt");
            archive.CreateEntry("rootfile.txt");
            archive.CreateEntry("Temp/t.txt");
            archive.CreateEntry("Program/");
        }

        _context.FileSystem.AddFile("/p/odd.zip", new MockFileData(stream.ToArray()));
        var content = await new PackageContentReader(_context.FileSystem).ReadAsync("/p/odd.zip");
        content.Entries.Single().Root.ShouldBe(PackageRoot.Temp);
        content.Entries.Single().ExtractedPath.ShouldBeNull();
        content.Manifest.ShouldBeNull();
        await Should.ThrowAsync<ArgumentException>(() => new PackageContentReader(_context.FileSystem).ReadAsync(" "));
        Should.Throw<ArgumentNullException>(() => new PackageContentReader(null!));
        Should.Throw<ArgumentNullException>(() => new PackageContentReader(null!, isWindows: false));
        Should.Throw<ArgumentNullException>(() => new PackageContent(null!, null));
        Should.Throw<ArgumentNullException>(() => new PackageContentEntry(PackageRoot.Temp, null!, 0));
    }

    [UnixFact]
    public async Task Extract_WritesThePackageFilesWithTheModesStoredInTheZip()
    {
        AddPackage("/p/linux.zip");
        _context.FileSystem.AddFile("/work/Program/bin/app", new MockFileData("old"));
        var reader = new PackageContentReader(_context.FileSystem, isWindows: false);

        var content = await reader.ExtractAsync("/p/linux.zip", "/work");

        var app = _context.FileSystem.Path.Combine("/work", "Program", "bin", "app");
        var settings = _context.FileSystem.Path.Combine("/work", "AppData", "cfg.json");
        content.Entries.Select(e => (e.Root, e.RelativePath, e.ExtractedPath))
            .ShouldBe([(PackageRoot.Program, "bin/app", app), (PackageRoot.AppData, "cfg.json", settings)]);
        _context.FileSystem.File.ReadAllText(app).ShouldBe("binary"); // overwritten
        _context.FileSystem.File.ReadAllText(settings).ShouldBe("{}");
        _context.FileSystem.File.GetUnixFileMode(app).ShouldBe((UnixFileMode)0x1E8);
        _context.FileSystem.File.GetUnixFileMode(settings).ShouldBe(
            UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead | UnixFileMode.OtherRead);
        _context.FileSystem.Directory.Exists("/work/Unknown").ShouldBeFalse();
        content.Manifest!.Operations.Single().ShouldBeOfType<TerminateProcessOperation>().ProcessName.ShouldBe("app");
    }

    [Fact]
    public async Task Extract_LeavesTheModesAloneOnWindows()
    {
        AddPackage("/p/win.zip");

        var content = await new PackageContentReader(_context.FileSystem, isWindows: true)
            .ExtractAsync("/p/win.zip", "/work");

        content.Entries.Count.ShouldBe(2);
        content.Entries[0].CodeSignature!["com.apple.cs.CodeDirectory"].ShouldBe("AQ=="); // the manifest's, by entry name
        content.Entries[1].CodeSignature.ShouldBeNull();
        _context.FileSystem.File.ReadAllText(content.Entries[0].ExtractedPath!).ShouldBe("binary");
        content.Entries[0].Mode.ShouldBe(0x1E8); // reported, so a rebuild on Windows can store it again
        new PackageContentEntry(PackageRoot.Temp, "a", 1).Mode.ShouldBe(0);
        await Should.ThrowAsync<ArgumentException>(() => _context.ContentReader.ExtractAsync(" ", "/work"));
        await Should.ThrowAsync<ArgumentException>(() => _context.ContentReader.ExtractAsync("/p/win.zip", ""));
    }
}
