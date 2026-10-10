using System.IO.Abstractions.TestingHelpers;
using nUpdate.Installer;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller;

namespace nUpdate.Tests.Installer;

public class DirectoryCopierTests
{
    private readonly TestInstallerServices _services = new();

    [Fact]
    public void Copy_CopiesRecursivelyAndReportsProgress()
    {
        var fs = _services.FileSystem;
        fs.AddFile("/src/a.txt", new MockFileData("a"));
        fs.AddFile("/src/sub/b.txt", new MockFileData("b"));
        fs.AddFile("/dst/a.txt", new MockFileData("old"));
        var copier = new DirectoryCopier(fs, 3);
        copier.CountFiles("/src").ShouldBe(2);
        copier.CountFiles("/nope").ShouldBe(0);
        var progress = new ProgressTracker();
        progress.AddTasks(2);

        copier.Copy("/src", "/dst", _services.Context(progress: progress));

        fs.File.ReadAllText("/dst/a.txt").ShouldBe("a");
        fs.File.ReadAllText(fs.Path.Combine("/dst", "sub", "b.txt")).ShouldBe("b");
        _services.Reporter.Unpacking.ShouldBe([(50f, "a.txt"), (100f, "b.txt")]);
    }

    [Fact]
    public void Copy_RetriesSkipsOrAbortsLockedFiles()
    {
        var fs = _services.FileSystem;
        fs.AddFile("/src/locked.txt", new MockFileData("new"));
        fs.AddFile("/dst/locked.txt", new MockFileData("old") { AllowedFileShare = FileShare.None });
        var copier = new DirectoryCopier(fs, 3);

        _services.Reporter.LockedFileDecision =
            (_, attempt) => attempt == 1 ? LockedFileDecision.Retry : LockedFileDecision.Skip;
        copier.Copy("/src", "/dst", _services.Context());
        fs.GetFile("/dst/locked.txt").TextContents.ShouldBe("old");
        _services.Reporter.LockedFiles.Select(l => l.Attempt).ShouldBe([1, 2]);

        _services.Reporter.LockedFiles.Clear();
        _services.Reporter.LockedFileDecision = (_, _) => LockedFileDecision.Retry;
        var ex = Should.Throw<LockedFileException>(() => copier.Copy("/src", "/dst", _services.Context()));
        var locked = fs.Path.Combine("/dst", "locked.txt");
        ex.FilePath.ShouldBe(locked);
        ex.Message.ShouldContain(locked);
        ex.InnerException.ShouldBeOfType<IOException>();
        // The UI is asked on every attempt, including the last one, where Retry turns into Abort.
        _services.Reporter.LockedFiles.Select(l => l.Attempt).ShouldBe([1, 2, 3]);
        fs.File.Exists(locked + DirectoryCopier.TempSuffix).ShouldBeFalse();

        _services.Reporter.LockedFiles.Clear();
        _services.Reporter.LockedFileDecision =
            (_, attempt) => attempt == 3 ? LockedFileDecision.Skip : LockedFileDecision.Retry;
        copier.Copy("/src", "/dst", _services.Context());
        _services.Reporter.LockedFiles.Count.ShouldBe(3);

        _services.Reporter.LockedFileDecision = (_, _) => LockedFileDecision.Abort;
        Should.Throw<LockedFileException>(() => copier.Copy("/src", "/dst", _services.Context()));

        fs.GetFile("/dst/locked.txt").AllowedFileShare = FileShare.ReadWrite | FileShare.Delete;
        _services.Reporter.LockedFileDecision = (_, _) => LockedFileDecision.Retry;
        copier.Copy("/src", "/dst", _services.Context());
        fs.File.ReadAllText("/dst/locked.txt").ShouldBe("new");
    }

    [Fact]
    public void Copy_PropagatesOtherIoErrors()
    {
        var fs = Substitute.For<System.IO.Abstractions.IFileSystem>();
        fs.Directory.GetFiles("/src").Returns(["/src/a.txt"]);
        fs.Directory.GetDirectories("/src").Returns([]);
        fs.Path.GetFileName("/src/a.txt").Returns("a.txt");
        fs.Path.Combine("/dst", "a.txt").Returns("/dst/a.txt");
        fs.File.When(f => f.Copy("/src/a.txt", "/dst/a.txt" + DirectoryCopier.TempSuffix, true)).Do(_ =>
            throw new IOException("disk full") { HResult = unchecked((int)0x80070070) });
        var copier = new DirectoryCopier(fs, 3);
        Should.Throw<IOException>(() => copier.Copy("/src", "/dst", _services.Context())).Message.ShouldBe("disk full");
    }

    [Fact]
    public void Copy_ReplacesFilesAtomicallyAndKeepsNoStagedCopyBehind()
    {
        var fs = _services.FileSystem;
        fs.AddFile("/src/a.txt", new MockFileData("new"));
        fs.AddFile("/dst/a.txt", new MockFileData("old"));
        new DirectoryCopier(fs, 1).Copy("/src", "/dst", _services.Context());
        fs.File.ReadAllText("/dst/a.txt").ShouldBe("new");
        fs.AllFiles.Count(f => f.EndsWith(DirectoryCopier.TempSuffix, StringComparison.Ordinal)).ShouldBe(0);

        // A failure after staging removes the staged copy even when the file system refuses to delete it.
        var failing = Substitute.For<System.IO.Abstractions.IFileSystem>();
        failing.Directory.GetFiles("/src").Returns(["/src/a.txt"]);
        failing.Directory.GetDirectories("/src").Returns([]);
        failing.Path.GetFileName("/src/a.txt").Returns("a.txt");
        failing.Path.Combine("/dst", "a.txt").Returns("/dst/a.txt");
        failing.File.Exists("/dst/a.txt").Returns(false);
        failing.File.When(f => f.Move("/dst/a.txt" + DirectoryCopier.TempSuffix, "/dst/a.txt"))
            .Do(_ => throw new UnauthorizedAccessException("denied"));
        failing.File.When(f => f.Delete("/dst/a.txt" + DirectoryCopier.TempSuffix))
            .Do(_ => throw new IOException("busy"));
        Should.Throw<UnauthorizedAccessException>(() =>
            new DirectoryCopier(failing, 1).Copy("/src", "/dst", _services.Context()));
    }

    [Fact]
    public void DirectoryCopier_ValidatesArguments()
    {
        var fs = _services.FileSystem;
        Should.Throw<ArgumentNullException>(() => new DirectoryCopier(null!, 1));
        Should.Throw<ArgumentOutOfRangeException>(() => new DirectoryCopier(fs, 0));
        var copier = new DirectoryCopier(fs, 1);
        Should.Throw<ArgumentNullException>(() => copier.Copy(null!, "/d", _services.Context()));
        Should.Throw<ArgumentNullException>(() => copier.Copy("/s", null!, _services.Context()));
        Should.Throw<ArgumentNullException>(() => copier.Copy("/s", "/d", null!));
        Should.Throw<ArgumentNullException>(() => DirectoryCopier.IsLockedFileError(null!));
        DirectoryCopier.IsLockedFileError(new IOException("x") { HResult = unchecked((int)0x80070020) }).ShouldBeTrue();
        DirectoryCopier.IsLockedFileError(new IOException("x") { HResult = unchecked((int)0x80070021) }).ShouldBeTrue();
        DirectoryCopier.IsLockedFileError(new IOException("x") { HResult = unchecked((int)0x80070002) })
            .ShouldBeFalse();
    }
}
