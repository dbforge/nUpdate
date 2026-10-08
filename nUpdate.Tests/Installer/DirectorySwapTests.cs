using System.IO.Abstractions.TestingHelpers;
using nUpdate.UpdateInstaller.Platform;

namespace nUpdate.Tests.Installer;

public class DirectorySwapTests
{
    private readonly MockFileSystem _fileSystem = new();

    [Fact]
    public void Swap_ExchangesTheDirectoriesWithRenames()
    {
        _fileSystem.AddFile("/apps/App.app/Contents/old.txt", new MockFileData("old"));
        _fileSystem.AddFile("/apps/App.app.new/Contents/new.txt", new MockFileData("new"));
        _fileSystem.AddFile("/apps/App.app" + DirectorySwap.ParkedSuffix + "/stale.txt", new MockFileData("stale"));

        new DirectorySwap(_fileSystem, atomic: false).Swap("/apps/App.app", "/apps/App.app.new");

        _fileSystem.File.ReadAllText("/apps/App.app/Contents/new.txt").ShouldBe("new");
        _fileSystem.File.Exists("/apps/App.app/Contents/old.txt").ShouldBeFalse();
        _fileSystem.File.ReadAllText("/apps/App.app.new/Contents/old.txt").ShouldBe("old");
        _fileSystem.Directory.Exists("/apps/App.app" + DirectorySwap.ParkedSuffix).ShouldBeFalse();
    }

    [Fact]
    public void Swap_PutsTheCurrentDirectoryBackWhenTheReplacementCannotMove()
    {
        _fileSystem.AddFile("/apps/App.app/Contents/old.txt", new MockFileData("old"));

        Should.Throw<DirectoryNotFoundException>(() => new DirectorySwap(_fileSystem, atomic: false).Swap("/apps/App.app", "/apps/missing.new"));

        _fileSystem.File.ReadAllText("/apps/App.app/Contents/old.txt").ShouldBe("old");
        _fileSystem.Directory.Exists("/apps/App.app" + DirectorySwap.ParkedSuffix).ShouldBeFalse();
    }

    [Fact]
    public void Swap_KeepsTheNewDirectoryWhenTheOldOneCannotBeMovedOn()
    {
        var fileSystem = Substitute.For<System.IO.Abstractions.IFileSystem>();
        var directory = Substitute.For<System.IO.Abstractions.IDirectory>();
        fileSystem.Directory.Returns(directory);
        var parked = "/apps/App.app" + DirectorySwap.ParkedSuffix;
        directory.When(d => d.Move(parked, "/apps/App.app.new")).Do(_ => throw new IOException("in use"));

        new DirectorySwap(fileSystem, atomic: false).Swap("/apps/App.app", "/apps/App.app.new");

        directory.Received(1).Move("/apps/App.app", parked);
        directory.Received(1).Move("/apps/App.app.new", "/apps/App.app"); // the new one is in place; the old one stays parked
    }

    [Fact]
    public void Swap_UsesTheAtomicSwapWhenItSucceeds()
    {
        _fileSystem.AddFile("/apps/App.app/a.txt", new MockFileData("a"));
        var calls = new List<(string, string)>();
        new DirectorySwap(_fileSystem, (current, replacement) =>
        {
            calls.Add((current, replacement));
            return true;
        }).Swap("/apps/App.app", "/apps/App.app.new");

        calls.ShouldBe([("/apps/App.app", "/apps/App.app.new")]);
        _fileSystem.File.Exists("/apps/App.app/a.txt").ShouldBeTrue(); // the renames did not run
        new DirectorySwap(_fileSystem, atomic: true).ShouldNotBeNull();
    }

    [Fact]
    public void DirectorySwap_ChecksItsArguments()
    {
        Should.Throw<ArgumentNullException>(() => new DirectorySwap(null!, atomic: false));
        var swap = new DirectorySwap(_fileSystem, atomic: false);
        Should.Throw<ArgumentNullException>(() => swap.Swap(null!, "/b"));
        Should.Throw<ArgumentNullException>(() => swap.Swap("/a", null!));
    }
}
