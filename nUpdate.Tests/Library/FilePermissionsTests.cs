using nUpdate.Platform;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Library;

public sealed class FilePermissionsTests : IDisposable
{
    private readonly string _directory = Directory.CreateTempSubdirectory("nupdate-permissions-").FullName;

    public void Dispose()
    {
        if (!OperatingSystem.IsWindows())
            File.SetUnixFileMode(_directory, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        Directory.Delete(_directory, recursive: true);
    }

    [Fact]
    public void CanWrite_ProbesTheFolderAndLeavesNothingBehind()
    {
        var permissions = new FilePermissions();
        permissions.CanWrite(_directory).ShouldBeTrue();
        Directory.GetFiles(_directory).ShouldBeEmpty();
        permissions.CanWrite(Path.Combine(_directory, "missing")).ShouldBeFalse();
        Should.Throw<ArgumentNullException>(() => permissions.CanWrite(null!));
    }

    [UnixFact]
    public void CanWrite_IsFalseForAReadOnlyFolder()
    {
        if (Environment.UserName == "root")
            return; // root may write everywhere
        File.SetUnixFileMode(_directory, UnixFileMode.UserRead | UnixFileMode.UserExecute);
        new FilePermissions().CanWrite(_directory).ShouldBeFalse();
    }

    [UnixFact]
    public void SetMode_ChangesThePermissionBits()
    {
        var file = Path.Combine(_directory, "tool ä.sh");
        File.WriteAllText(file, "#!/bin/sh");
        var permissions = new FilePermissions();
        permissions.SetMode(file, FilePermissions.ExecutableMode);
        File.GetUnixFileMode(file).ShouldBe((UnixFileMode)FilePermissions.ExecutableMode);
        permissions.SetMode(file, FilePermissions.RegularMode);
        File.GetUnixFileMode(file).ShouldBe((UnixFileMode)FilePermissions.RegularMode);
        Should.Throw<IOException>(() => permissions.SetMode(Path.Combine(_directory, "missing"), FilePermissions.RegularMode)).Message.ShouldContain("missing");
    }

    [Fact]
    public void SetMode_ChecksItsArgumentsAndDoesNothingOnWindows()
    {
        var windows = new FilePermissions(isWindows: true);
        windows.SetMode(Path.Combine(_directory, "missing"), FilePermissions.ExecutableMode);
        Should.Throw<ArgumentNullException>(() => windows.SetMode(null!, 0));
        Should.Throw<ArgumentOutOfRangeException>(() => windows.SetMode("x", -1));
        Should.Throw<ArgumentOutOfRangeException>(() => windows.SetMode("x", 0x1000));
        FilePermissions.ExecutableMode.ShouldBe(Convert.ToInt32("755", 8));
        FilePermissions.RegularMode.ShouldBe(Convert.ToInt32("644", 8));
    }
}
