using System.IO.Abstractions.TestingHelpers;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller;

namespace nUpdate.Tests.Installer;

public class ZipPackageExtractorTests
{
    private readonly TestInstallerServices _services = new();

    [Fact]
    public void Extract_WritesEntriesAndDirectoriesThroughFileSystem()
    {
        var zip = TestInstallerServices.BuildZip(new Dictionary<string, string> { ["Program/app.dll"] = "new", ["Program/sub/x.txt"] = "x", ["empty/"] = "" });
        var fs = _services.FileSystem;
        fs.AddFile("/pkg/1.0.zip", new MockFileData(zip));
        new ZipPackageExtractor(fs, _services.FilePermissions).Extract("/pkg/1.0.zip", "/out/1.0/");
        fs.File.ReadAllText(fs.Path.Combine("/out/1.0", "Program", "app.dll")).ShouldBe("new");
        fs.File.ReadAllText(fs.Path.Combine("/out/1.0", "Program", "sub", "x.txt")).ShouldBe("x");
        fs.Directory.Exists(fs.Path.Combine("/out/1.0", "empty")).ShouldBeTrue();
    }

    [Fact]
    public void Extract_RestoresTheStoredUnixModes()
    {
        var files = new Dictionary<string, string> { ["Program/app"] = "elf", ["Program/readme.txt"] = "text", ["Program/legacy.dll"] = "dll" };
        var modes = new Dictionary<string, int> { ["Program/app"] = 0x81ED /* regular file 0755 */, ["Program/readme.txt"] = 0x1A4 /* 0644 */ };
        var fs = _services.FileSystem;
        fs.AddFile("/pkg/1.0.zip", new MockFileData(TestInstallerServices.BuildZip(files, modes: modes)));
        new ZipPackageExtractor(fs, _services.FilePermissions).Extract("/pkg/1.0.zip", "/out");

        _services.FilePermissions.Received(1).SetMode(fs.Path.Combine(fs.Path.GetFullPath("/out"), "Program", "app"), 0x1ED);
        _services.FilePermissions.Received(1).SetMode(fs.Path.Combine(fs.Path.GetFullPath("/out"), "Program", "readme.txt"), 0x1A4);
        _services.FilePermissions.ReceivedCalls().Count().ShouldBe(2); // an entry without a mode keeps the default
    }

    [Theory]
    [InlineData("../evil.txt")]
    [InlineData("a/../../evil.txt")]
    [InlineData("/abs/evil.txt")]
    [InlineData("../1.0-evil/x.txt")]
    public void Extract_RejectsPathTraversal(string entry)
    {
        var zip = TestInstallerServices.BuildZip(new Dictionary<string, string> { [entry] = "x" });
        var fs = _services.FileSystem;
        fs.AddFile("/pkg/bad.zip", new MockFileData(zip));
        Should.Throw<InvalidDataException>(() => new ZipPackageExtractor(fs, _services.FilePermissions).Extract("/pkg/bad.zip", "/out/1.0"));
        fs.AllFiles.Count(f => f.Contains("evil", StringComparison.Ordinal)).ShouldBe(0);
    }

    [Fact]
    public void ZipPackageExtractor_ValidatesArguments()
    {
        var extractor = new ZipPackageExtractor(_services.FileSystem, _services.FilePermissions);
        Should.Throw<ArgumentNullException>(() => new ZipPackageExtractor(null!, _services.FilePermissions));
        Should.Throw<ArgumentNullException>(() => new ZipPackageExtractor(_services.FileSystem, null!));
        Should.Throw<ArgumentNullException>(() => extractor.Extract(null!, "/x"));
        Should.Throw<ArgumentNullException>(() => extractor.Extract("/x", null!));
    }
}
