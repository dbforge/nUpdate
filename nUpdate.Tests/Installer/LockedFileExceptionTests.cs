using nUpdate.UpdateInstaller;

namespace nUpdate.Tests.Installer;

public class LockedFileExceptionTests
{
    [Fact]
    public void LockedFileException_HasAllConstructors()
    {
        var inner = new InvalidOperationException();
        new LockedFileException().ShouldNotBeNull();
        new LockedFileException("m").Message.ShouldBe("m");
        new LockedFileException("m", inner).InnerException.ShouldBe(inner);
        var locked = new LockedFileException("m", "/f", inner);
        locked.FilePath.ShouldBe("/f");
        locked.InnerException.ShouldBe(inner);
    }
}
