using System.IO.Abstractions.TestingHelpers;
using nUpdate.Administration.Core.Packages;
using nUpdate.Platform;
using nUpdate.Tests.Administration.Support;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Administration.Core;

public class UnixModeDetectorTests
{
    private readonly AdminTestContext _context = new();

    [Fact]
    public void Detect_RecognizesExecutablesByContentOnWindows()
    {
        var fs = _context.FileSystem;
        var cases = new Dictionary<string, byte[]>
        {
            ["/src/elf"] = [0x7F, 0x45, 0x4C, 0x46, 2, 1],
            ["/src/macho32"] = [0xFE, 0xED, 0xFA, 0xCE],
            ["/src/macho32le"] = [0xCE, 0xFA, 0xED, 0xFE],
            ["/src/macho64"] = [0xFE, 0xED, 0xFA, 0xCF],
            ["/src/macho64le"] = [0xCF, 0xFA, 0xED, 0xFE],
            ["/src/universal"] = [0xCA, 0xFE, 0xBA, 0xBE, 0, 0, 0, 2],
            ["/src/universalle"] = [0xBE, 0xBA, 0xFE, 0xCA],
            ["/src/script.sh"] = "#!/bin/sh\necho"u8.ToArray(),
            ["/src/shebang"] = "#!"u8.ToArray(),
        };
        foreach (var (path, bytes) in cases)
        {
            fs.AddFile(path, new MockFileData(bytes));
            UnixModeDetector.Detect(fs, path, isWindows: true).ShouldBe(FilePermissions.ExecutableMode, path);
        }

        foreach (var (path, bytes) in new Dictionary<string, byte[]> { ["/src/readme.txt"] = "hello"u8.ToArray(), ["/src/empty"] = [], ["/src/one"] = [0x23], ["/src/app.dll"] = [0x4D, 0x5A, 0x90, 0] })
        {
            fs.AddFile(path, new MockFileData(bytes));
            UnixModeDetector.Detect(fs, path, isWindows: true).ShouldBe(FilePermissions.RegularMode, path);
        }

        Should.Throw<ArgumentNullException>(() => UnixModeDetector.Detect(null!, "/x", true));
        Should.Throw<ArgumentNullException>(() => UnixModeDetector.Detect(fs, null!, true));
    }

    [UnixFact]
    public void Detect_TakesTheSourceFilesModeOnUnix()
    {
        var fs = _context.FileSystem;
        fs.AddFile("/src/tool", new MockFileData("plain text") { UnixMode = (UnixFileMode)Convert.ToInt32("750", 8) });
        fs.AddFile("/src/secret", new MockFileData([0x7F, 0x45, 0x4C, 0x46]) { UnixMode = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.SetUser });
        UnixModeDetector.Detect(fs, "/src/tool", isWindows: false).ShouldBe(Convert.ToInt32("750", 8));
        UnixModeDetector.Detect(fs, "/src/secret", isWindows: false).ShouldBe(Convert.ToInt32("600", 8)); // only the permission bits
        fs.AddFile("/src/mounted", new MockFileData("x") { UnixMode = (UnixFileMode)Convert.ToInt32("777", 8) });
        UnixModeDetector.Detect(fs, "/src/mounted", isWindows: false).ShouldBe(Convert.ToInt32("755", 8)); // never writable by others
    }
}
