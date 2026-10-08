using nUpdate.Operations;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller;
using nUpdate.UpdateInstaller.Operations;

namespace nUpdate.Tests.Installer;

public class FileOperationHandlerTests
{
    private readonly TestInstallerServices _services = new();

    [Fact]
    public void Execute_DeletesListedFilesAndCountsThem()
    {
        var handler = new FileOperationHandler();
        handler.Area.ShouldBe(OperationArea.Files);
        var fs = _services.FileSystem;
        fs.AddFile(fs.Path.Combine(_services.AppDirectory, "old.dll"), new MockFileDataText("x"));
        fs.AddFile(fs.Path.Combine(_services.AppDirectory, "keep.dll"), new MockFileDataText("x"));
        var operation = new DeleteFilesOperation { Directory = "%program%", Files = ["old.dll", "missing.dll"] };
        var progress = new ProgressTracker();
        progress.AddTasks(handler.CountTasks(operation));
        var context = _services.Context(progress: progress);

        handler.Execute(operation, context);

        fs.File.Exists(fs.Path.Combine(_services.AppDirectory, "old.dll")).ShouldBeFalse();
        fs.File.Exists(fs.Path.Combine(_services.AppDirectory, "keep.dll")).ShouldBeTrue();
        _services.Reporter.Operations.Select(o => o.Text).ShouldBe(["Deleting file \"old.dll\"...", "Deleting file \"missing.dll\"..."]);
        _services.Reporter.Operations.Last().Progress.ShouldBe(100f);
    }

    [Fact]
    public void Execute_RenamesFilesReplacingExistingTargets()
    {
        var handler = new FileOperationHandler();
        var fs = _services.FileSystem;
        fs.AddFile(fs.Path.Combine(_services.AppDirectory, "a.txt"), new MockFileDataText("A"));
        fs.AddFile(fs.Path.Combine(_services.AppDirectory, "b.txt"), new MockFileDataText("B"));
        var operation = new RenameFileOperation { Path = "%program%\\a.txt", NewName = "b.txt" };
        handler.CountTasks(operation).ShouldBe(1);

        handler.Execute(operation, _services.Context());
        fs.File.Exists(fs.Path.Combine(_services.AppDirectory, "a.txt")).ShouldBeFalse();
        fs.File.ReadAllText(fs.Path.Combine(_services.AppDirectory, "b.txt")).ShouldBe("A");
        _services.Reporter.Operations.Single().Text.ShouldBe("Renaming file \"a.txt\" to \"b.txt\"...");

        handler.Execute(new RenameFileOperation { Path = "%program%\\missing.txt", NewName = "c.txt" }, _services.Context());
        fs.File.Exists(fs.Path.Combine(_services.AppDirectory, "c.txt")).ShouldBeFalse();
        Should.Throw<InvalidOperationException>(() => handler.Execute(new RenameFileOperation { Path = "%program%\\x" }, _services.Context()));
        Should.Throw<NotSupportedException>(() => handler.Execute(new StartProcessOperation { Path = "x" }, _services.Context()));
    }

    [Fact]
    public void FileOperationHandler_RejectsNullArguments()
    {
        var context = _services.Context();
        var operation = new DeleteFilesOperation { Directory = "x" };
        var handler = new FileOperationHandler();
        Should.Throw<ArgumentNullException>(() => handler.CountTasks(null!));
        Should.Throw<ArgumentNullException>(() => handler.Execute(null!, context));
        Should.Throw<ArgumentNullException>(() => handler.Execute(operation, null!));
    }

    private sealed class MockFileDataText(string text) : System.IO.Abstractions.TestingHelpers.MockFileData(text);
}
