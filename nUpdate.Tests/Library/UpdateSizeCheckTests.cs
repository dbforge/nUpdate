using System.IO.Abstractions.TestingHelpers;
using nUpdate.Ui;

namespace nUpdate.Tests.Library;

public class UpdateSizeCheckTests
{
    [Fact]
    public void HasEnoughSpace_RequiresTwiceThePackageSize()
    {
        var fileSystem = new MockFileSystem();
        var root = fileSystem.Path.GetPathRoot(fileSystem.Path.GetTempPath())!;
        fileSystem.AddDrive(root, new MockDriveData { AvailableFreeSpace = 1000 });

        UpdateSizeCheck.HasEnoughSpace(fileSystem, 500, out var bytesToFree).ShouldBeTrue();
        bytesToFree.ShouldBe(0);
        UpdateSizeCheck.HasEnoughSpace(fileSystem, 600, out bytesToFree).ShouldBeFalse();
        bytesToFree.ShouldBe(200);
        Should.Throw<ArgumentOutOfRangeException>(() => UpdateSizeCheck.HasEnoughSpace(fileSystem, -1, out _));
        Should.Throw<ArgumentNullException>(() => UpdateSizeCheck.HasEnoughSpace(null!, 1, out _));
        UpdateSizeCheck.RequiredMultiple.ShouldBe(2);

        var relative = Substitute.For<System.IO.Abstractions.IFileSystem>();
        relative.Path.GetTempPath().Returns("relative-temp");
        relative.Path.GetPathRoot("relative-temp").Returns("");
        Should.Throw<InvalidOperationException>(() => UpdateSizeCheck.HasEnoughSpace(relative, 1, out _));
    }
}
