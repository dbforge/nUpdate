using nUpdate.Operations;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller.Operations;

namespace nUpdate.Tests.Installer;

public class OperationDispatcherTests
{
    private readonly TestInstallerServices _services = new();

    [Fact]
    public void OperationDispatcher_RoutesByAreaAndRejectsDuplicatesAndUnknownAreas()
    {
        var fileHandler = Substitute.For<IOperationHandler>();
        fileHandler.Area.Returns(OperationArea.Files);
        fileHandler.CountTasks(Arg.Any<Operation>()).Returns(3);
        var dispatcher = new OperationDispatcher([fileHandler]);
        var context = _services.Context();
        var operations = new Operation[]
            { new DeleteFilesOperation { Directory = "x" }, new RenameFileOperation { Path = "y" } };

        dispatcher.CountTasks(operations).ShouldBe(6);
        dispatcher.Execute(operations, context);
        fileHandler.Received(2).Execute(Arg.Any<Operation>(), context);

        Should.Throw<NotSupportedException>(() =>
            dispatcher.CountTasks([new CreateRegistryKeysOperation { Key = "k" }]));
        Should.Throw<ArgumentException>(() => new OperationDispatcher([fileHandler, fileHandler]));
        Should.Throw<ArgumentNullException>(() => new OperationDispatcher(null!));
        Should.Throw<ArgumentNullException>(() => dispatcher.CountTasks(null!));
        Should.Throw<ArgumentNullException>(() => dispatcher.Execute(null!, context));
        Should.Throw<ArgumentNullException>(() => dispatcher.Execute(operations, null!));
    }
}
