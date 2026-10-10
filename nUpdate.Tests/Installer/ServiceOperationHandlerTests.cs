using nUpdate.Operations;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller.Operations;

namespace nUpdate.Tests.Installer;

public class ServiceOperationHandlerTests
{
    private readonly TestInstallerServices _services = new();

    [Fact]
    public void Execute_StartsAndStops()
    {
        var handler = new ServiceOperationHandler();
        handler.Area.ShouldBe(OperationArea.Services);
        var context = _services.Context();
        var start = new StartServiceOperation { ServiceName = "svc", Arguments = ["-a"] };
        handler.CountTasks(start).ShouldBe(1);
        handler.Execute(start, context);
        _services.ServiceController.Received().StartService("svc", Arg.Is<string[]>(a => a.Single() == "-a"));
        handler.Execute(new StopServiceOperation { ServiceName = "svc" }, context);
        _services.ServiceController.Received().StopService("svc");
        _services.Reporter.Operations.Select(o => o.Text)
            .ShouldBe(["Starting service \"svc\"...", "Stopping service \"svc\"..."]);
        Should.Throw<NotSupportedException>(() =>
            handler.Execute(new TerminateProcessOperation { ProcessName = "x" }, context));
    }

    [Fact]
    public void ServiceOperationHandler_RejectsNullArguments()
    {
        var context = _services.Context();
        var operation = new DeleteFilesOperation { Directory = "x" };
        var handler = new ServiceOperationHandler();
        Should.Throw<ArgumentNullException>(() => handler.CountTasks(null!));
        Should.Throw<ArgumentNullException>(() => handler.Execute(null!, context));
        Should.Throw<ArgumentNullException>(() => handler.Execute(operation, null!));
    }
}
