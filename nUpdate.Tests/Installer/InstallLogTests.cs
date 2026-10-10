using System.IO.Abstractions.TestingHelpers;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller.Reporting;

namespace nUpdate.Tests.Installer;

public class InstallLogTests
{
    private readonly TestInstallLogs _logs = new();

    [Fact]
    public void Write_AppendsTimestampedLinesAndNeverThrows()
    {
        _logs.FileSystem.AddDirectory("/tmp");
        var log = _logs.Log();
        log.Path.ShouldBe("/tmp/install.log");
        log.Write("first");
        log.Write("second");
        _logs.ReadLog("/tmp/install.log").ShouldBe([
            "2026-10-07 12:00:00.000 +02:00  first", "2026-10-07 12:00:00.000 +02:00  second"
        ]);
        log.Dispose();
        log.Write("after the end"); // ignored
        _logs.FileSystem.File.ReadAllLines("/tmp/install.log").Length.ShouldBe(2);
        log.Dispose();

        _logs.Log("/missing/install.log").Write("lost"); // no folder: best effort
        _logs.FileSystem.File.Exists("/missing/install.log").ShouldBeFalse();
        _logs.FileSystem.AddFile("/tmp/locked.log", new MockFileData("") { Attributes = FileAttributes.ReadOnly });
        _logs.Log("/tmp/locked.log").Write("lost");
        var none = _logs.Log(null);
        none.Write("nowhere");
        none.Path.ShouldBeNull();

        Should.Throw<ArgumentNullException>(() => log.Write(null!));
        Should.Throw<ArgumentNullException>(() => new InstallLog(null!, "/x", () => TestInstallLogs.Noon));
        Should.Throw<ArgumentNullException>(() => new InstallLog(_logs.FileSystem, "/x", null!));
    }
}
