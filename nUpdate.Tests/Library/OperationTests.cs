using nUpdate.Operations;
using nUpdate.Tests.Library.Support;

namespace nUpdate.Tests.Library;

public class OperationTests
{
    [Fact]
    public void Types_ListsEveryOperationAndEachRoundTrips()
    {
        Operation.Types.Count.ShouldBe(10);
        foreach (var (type, clrType) in Operation.Types)
        {
            var operation = (Operation)Activator.CreateInstance(clrType)!;
            operation.Type.ShouldBe(type);
            var json = Serializer.Serialize(operation);
            json.ShouldStartWith($"{{\"type\":\"{type}\"");
            json.ShouldContain("\"runBeforeFileReplacement\":false");
            OperationJson.RoundTrip(operation).ShouldBeOfType(clrType);
        }
    }

    [Fact]
    public void Operation_Serialization_KeepsEveryField()
    {
        var delete = OperationJson.RoundTrip(new DeleteFilesOperation { Directory = "%program%", Files = ["a.txt", "b.txt"], RunBeforeFileReplacement = true }).ShouldBeOfType<DeleteFilesOperation>();
        delete.Directory.ShouldBe("%program%");
        delete.Files.ShouldBe(["a.txt", "b.txt"]);
        delete.RunBeforeFileReplacement.ShouldBeTrue();
        delete.Area.ShouldBe(OperationArea.Files);

        var rename = OperationJson.RoundTrip(new RenameFileOperation { Path = "%program%/a.dll", NewName = "b.dll" }).ShouldBeOfType<RenameFileOperation>();
        rename.Path.ShouldBe("%program%/a.dll");
        rename.NewName.ShouldBe("b.dll");

        OperationJson.RoundTrip(new CreateRegistryKeysOperation { Key = "HKCU\\x", SubKeys = ["a"] }).ShouldBeOfType<CreateRegistryKeysOperation>().SubKeys.ShouldBe(["a"]);
        OperationJson.RoundTrip(new DeleteRegistryKeysOperation { Key = "HKCU\\x", SubKeys = ["b"] }).ShouldBeOfType<DeleteRegistryKeysOperation>().Area.ShouldBe(OperationArea.Registry);
        OperationJson.RoundTrip(new DeleteRegistryValuesOperation { Key = "HKCU\\x", Names = ["Mode"] }).ShouldBeOfType<DeleteRegistryValuesOperation>().Names.ShouldBe(["Mode"]);

        var process = OperationJson.RoundTrip(new StartProcessOperation { Path = "tool.exe", Arguments = "--quiet", WaitForExit = true, FailOnError = true }).ShouldBeOfType<StartProcessOperation>();
        process.Arguments.ShouldBe("--quiet");
        process.WaitForExit.ShouldBeTrue();
        process.FailOnError.ShouldBeTrue();
        process.Area.ShouldBe(OperationArea.Processes);
        process.RequiresWindows.ShouldBeFalse();
        Serializer.Serialize(new StartProcessOperation()).ShouldContain("\"waitForExit\":false,\"failOnError\":false");
        Serializer.Serialize(new StartProcessOperation()).ShouldNotContain("requiresWindows");
        OperationJson.RoundTrip(new TerminateProcessOperation { ProcessName = "tool" }).ShouldBeOfType<TerminateProcessOperation>().ProcessName.ShouldBe("tool");

        var service = OperationJson.RoundTrip(new StartServiceOperation { ServiceName = "svc", Arguments = ["-a"] }).ShouldBeOfType<StartServiceOperation>();
        service.Arguments.ShouldBe(["-a"]);
        service.Area.ShouldBe(OperationArea.Services);
        OperationJson.RoundTrip(new StopServiceOperation { ServiceName = "svc" }).ShouldBeOfType<StopServiceOperation>().ServiceName.ShouldBe("svc");
        service.RequiresWindows.ShouldBeTrue();
        new SetRegistryValuesOperation().RequiresWindows.ShouldBeTrue();
        new DeleteFilesOperation().RequiresWindows.ShouldBeFalse();
        Enum.GetValues<OperationArea>().Where(Operation.IsWindowsOnly).ShouldBe([OperationArea.Registry, OperationArea.Services]);
    }

    [Fact]
    public void Area_IsKnownForEveryOperation()
    {
        new Dictionary<string, OperationArea>
        {
            ["deleteFiles"] = OperationArea.Files,
            ["renameFile"] = OperationArea.Files,
            ["createRegistryKeys"] = OperationArea.Registry,
            ["deleteRegistryKeys"] = OperationArea.Registry,
            ["setRegistryValues"] = OperationArea.Registry,
            ["deleteRegistryValues"] = OperationArea.Registry,
            ["startProcess"] = OperationArea.Processes,
            ["terminateProcess"] = OperationArea.Processes,
            ["startService"] = OperationArea.Services,
            ["stopService"] = OperationArea.Services,
        }.ShouldAllBe(pair => ((Operation)Activator.CreateInstance(Operation.Types[pair.Key])!).Area == pair.Value);
    }
}
