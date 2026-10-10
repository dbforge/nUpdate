using nUpdate.Operations;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller;
using nUpdate.UpdateInstaller.Operations;

namespace nUpdate.Tests.Installer;

public class ProcessOperationHandlerTests
{
    private readonly TestInstallerServices _services = new();

    [Fact]
    public void Execute_StartsAndKills()
    {
        var handler = new ProcessOperationHandler();
        handler.Area.ShouldBe(OperationArea.Processes);
        var context = _services.Context();
        var start = new StartProcessOperation { Path = "%program%\\tool.exe", Arguments = "--flag" };
        handler.CountTasks(start).ShouldBe(1);
        handler.Execute(start, context);
        _services.ProcessService.Received()
            .Start(_services.FileSystem.Path.Combine(_services.AppDirectory, "tool.exe"), "--flag");

        handler.Execute(new StartProcessOperation { Path = "notepad.exe" }, context);
        _services.ProcessService.Received().Start("notepad.exe", "");

        handler.Execute(new TerminateProcessOperation { ProcessName = "tool" }, context);
        _services.ProcessService.Received().Kill("tool");
        _services.Reporter.Operations.Select(o => o.Text).ShouldBe([
            "Starting process \"%program%\\tool.exe\"...", "Starting process \"notepad.exe\"...",
            "Terminating process \"tool\"..."
        ]);
        Should.Throw<NotSupportedException>(() =>
            handler.Execute(new StopServiceOperation { ServiceName = "x" }, context));
    }

    [Fact]
    public void Execute_WaitsForTheProcessAndFailsOnAnErrorExitCodeWhenAsked()
    {
        var handler = new ProcessOperationHandler();
        var progress = new ProgressTracker();
        progress.AddTasks(4);
        var context = _services.Context(progress: progress);
        var tool = _services.FileSystem.Path.Combine(_services.AppDirectory, "migrate.sh");
        _services.ProcessService.Run(tool, "--db").Returns(0, 3, 3);

        handler.Execute(
            new StartProcessOperation
            { Path = "%program%/migrate.sh", Arguments = "--db", WaitForExit = true, FailOnError = true }, context);
        handler.Execute(
            new StartProcessOperation { Path = "%program%/migrate.sh", Arguments = "--db", WaitForExit = true },
            context);
        var failure = Should.Throw<InvalidOperationException>(() =>
            handler.Execute(
                new StartProcessOperation
                { Path = "%program%/migrate.sh", Arguments = "--db", WaitForExit = true, FailOnError = true },
                context));

        failure.Message.ShouldBe("\"%program%/migrate.sh\" exited with code 3.");
        _services.ProcessService.Received(3).Run(tool, "--db");
        _services.ProcessService.DidNotReceive().Start(Arg.Any<string>(), Arg.Any<string>());
        _services.Reporter.Operations.Select(o => (o.Progress, o.Text)).ShouldBe(
        [
            (0f, "Waiting for \"%program%/migrate.sh\" to exit..."),
            (25f, "Starting process \"%program%/migrate.sh\"..."),
            (25f, "Waiting for \"%program%/migrate.sh\" to exit..."),
            (50f, "Starting process \"%program%/migrate.sh\"..."),
            (50f, "Waiting for \"%program%/migrate.sh\" to exit..."),
        ]);
    }

    [Fact]
    public void ProcessOperationHandler_RejectsNullArguments()
    {
        var context = _services.Context();
        var operation = new DeleteFilesOperation { Directory = "x" };
        var handler = new ProcessOperationHandler();
        Should.Throw<ArgumentNullException>(() => handler.CountTasks(null!));
        Should.Throw<ArgumentNullException>(() => handler.Execute(null!, context));
        Should.Throw<ArgumentNullException>(() => handler.Execute(operation, null!));
    }
}
