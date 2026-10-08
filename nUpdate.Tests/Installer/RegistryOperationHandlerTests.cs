using nUpdate.Operations;
using nUpdate.Tests.Installer.Support;
using nUpdate.UpdateInstaller.Operations;

namespace nUpdate.Tests.Installer;

public class RegistryOperationHandlerTests
{
    private readonly TestInstallerServices _services = new();

    [Fact]
    public void Execute_DelegatesEveryMethod()
    {
        var handler = new RegistryOperationHandler();
        handler.Area.ShouldBe(OperationArea.Registry);
        const string key = "HKEY_CURRENT_USER\\Software\\App";
        var context = _services.Context();

        var create = new CreateRegistryKeysOperation { Key = key, SubKeys = ["A", "B"] };
        handler.CountTasks(create).ShouldBe(2);
        handler.Execute(create, context);
        _services.Registry.Received().CreateSubKey(key, "A");
        _services.Registry.Received().CreateSubKey(key, "B");

        var delete = new DeleteRegistryKeysOperation { Key = key, SubKeys = ["A"] };
        handler.CountTasks(delete).ShouldBe(1);
        handler.Execute(delete, context);
        _services.Registry.Received().DeleteSubKey(key, "A");

        var set = new SetRegistryValuesOperation
        {
            Key = key,
            Values =
            [
                RegistryValue.DWord("N", 1), new RegistryValue("M", RegistryValueKind.String, null), RegistryValue.MultiString("L", ["a", "b"]),
                RegistryValue.Binary("B", [1, 2]), RegistryValue.ExpandString("E", "%TEMP%"),
            ],
        };
        handler.CountTasks(set).ShouldBe(5);
        handler.Execute(set, context);
        _services.Registry.Received(5).SetValue(key, Arg.Any<RegistryValue>());

        var deleteValues = new DeleteRegistryValuesOperation { Key = key, Names = ["N"] };
        handler.CountTasks(deleteValues).ShouldBe(1);
        handler.Execute(deleteValues, context);
        _services.Registry.Received().DeleteValue(key, "N");

        _services.Reporter.Operations.Select(o => o.Text).ShouldBe([
            "Creating registry subkey \"A\"...", "Creating registry subkey \"B\"...", "Deleting registry subkey \"A\"...",
            "Setting value of \"N\" in the registry to \"1\"...", "Setting value of \"M\" in the registry to \"\"...", "Setting value of \"L\" in the registry to \"a, b\"...",
            "Setting value of \"B\" in the registry to \"AQI=\"...", "Setting value of \"E\" in the registry to \"%TEMP%\"...", "Deleting name-value-pair \"N\"...",
        ]);
        handler.CountTasks(new StartProcessOperation { Path = "x" }).ShouldBe(1);
        Should.Throw<NotSupportedException>(() => handler.Execute(new StartProcessOperation { Path = "x" }, context));
    }

    [Fact]
    public void RegistryOperationHandler_RejectsNullArguments()
    {
        var context = _services.Context();
        var operation = new DeleteFilesOperation { Directory = "x" };
        var handler = new RegistryOperationHandler();
        Should.Throw<ArgumentNullException>(() => handler.CountTasks(null!));
        Should.Throw<ArgumentNullException>(() => handler.Execute(null!, context));
        Should.Throw<ArgumentNullException>(() => handler.Execute(operation, null!));
    }
}
