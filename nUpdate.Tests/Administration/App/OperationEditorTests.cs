using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using nUpdate.Administration.ViewModels;
using nUpdate.Administration.Views.Controls;
using nUpdate.Operations;

namespace nUpdate.Tests.Administration.App;

/// <summary>The operation editor inside the package editor: one editor per operation kind and its validation.</summary>
public class OperationEditorTests
{
    private readonly AppTestContext _context = new();

    private static OperationKind Kind(string type) => OperationKind.FromType(type);

    [Fact]
    public void OperationKinds_MapToOperations()
    {
        OperationKind.All.Count.ShouldBe(10);
        OperationKind.All.Select(k => k.Type).ShouldBe(Operation.Types.Keys, ignoreOrder: true);
        Kind(StartServiceOperation.TypeName).Area.ShouldBe(OperationArea.Services);
        OperationKind.All.Where(k => k.RequiresWindows).Select(k => k.Area).Distinct()
            .ShouldBe([OperationArea.Registry, OperationArea.Services]);
        Should.Throw<ArgumentException>(() => OperationKind.FromType("nope"));
        Should.Throw<ArgumentException>(() => OperationKind.FromType("executeScript"));
        OperationKind.FromOperation(new StopServiceOperation()).Type.ShouldBe(StopServiceOperation.TypeName);
        Should.Throw<ArgumentNullException>(() => OperationKind.FromOperation(null!));
    }

    [Fact]
    public void OperationEditor_HasLabelsForEveryKind()
    {
        foreach (var kind in OperationKind.All)
        {
            var editor = new OperationEditorViewModel(kind);
            editor.ValueLabel.ShouldNotBeNullOrEmpty();
            editor.HasSecondValue.ShouldBe(kind.Type is RenameFileOperation.TypeName or StartProcessOperation.TypeName);
            editor.HasList.ShouldBe(kind.Type is DeleteFilesOperation.TypeName or CreateRegistryKeysOperation.TypeName
                or DeleteRegistryKeysOperation.TypeName or DeleteRegistryValuesOperation.TypeName
                or StartServiceOperation.TypeName);
            editor.HasRegistryValues.ShouldBe(kind.Type == SetRegistryValuesOperation.TypeName);
            editor.IsStartProcess.ShouldBe(kind.Type == StartProcessOperation.TypeName);
            editor.Validate().ShouldNotBeNull();
        }

        new OperationEditorViewModel(Kind(CreateRegistryKeysOperation.TypeName)) { Value = "HKCU\\x", ListText = "A" }
            .ToOperation().ShouldBeOfType<CreateRegistryKeysOperation>().SubKeys.ShouldBe(["A"]);
        new OperationEditorViewModel(Kind(DeleteRegistryKeysOperation.TypeName)) { Value = "HKCU\\x", ListText = "A" }
            .ToOperation().ShouldBeOfType<DeleteRegistryKeysOperation>().SubKeys.ShouldBe(["A"]);
        new OperationEditorViewModel(Kind(DeleteRegistryValuesOperation.TypeName)) { Value = "HKCU\\x", ListText = "V" }
            .ToOperation().ShouldBeOfType<DeleteRegistryValuesOperation>().Names.ShouldBe(["V"]);
        new OperationEditorViewModel(Kind(StopServiceOperation.TypeName)) { Value = "svc" }.ToOperation()
            .ShouldBeOfType<StopServiceOperation>().ServiceName.ShouldBe("svc");
        var roundTrip = OperationEditorViewModel.FromOperation(new StartServiceOperation
        { ServiceName = "svc", Arguments = ["-a"] });
        roundTrip.ListText.ShouldBe("-a");
    }

    [Fact]
    public void OperationEditor_BuildsEveryKind()
    {
        var delete = new OperationEditorViewModel(Kind(DeleteFilesOperation.TypeName))
        { Value = "%program%", ListText = "a.dll\r\n\r\nb.dll", RunBeforeFileReplacement = true };
        delete.HasList.ShouldBeTrue();
        delete.HasSecondValue.ShouldBeFalse();
        delete.Validate().ShouldBeNull();
        var deleteOperation = delete.ToOperation().ShouldBeOfType<DeleteFilesOperation>();
        deleteOperation.Files.ShouldBe(["a.dll", "b.dll"]);
        deleteOperation.RunBeforeFileReplacement.ShouldBeTrue();

        var rename = new OperationEditorViewModel(Kind(RenameFileOperation.TypeName))
        { Value = "%program%\\a", SecondValue = " b " };
        rename.HasSecondValue.ShouldBeTrue();
        rename.ToOperation().ShouldBeOfType<RenameFileOperation>().NewName.ShouldBe("b");

        var process = new OperationEditorViewModel(Kind(StartProcessOperation.TypeName))
        { Value = "tool.exe", SecondValue = "--x", FailOnError = true };
        var started = process.ToOperation().ShouldBeOfType<StartProcessOperation>();
        started.Arguments.ShouldBe("--x");
        started.WaitForExit.ShouldBeFalse();
        started.FailOnError.ShouldBeFalse(); // failing on an exit code needs waiting for it
        process.WaitForExit = true;
        started = process.ToOperation().ShouldBeOfType<StartProcessOperation>();
        started.WaitForExit.ShouldBeTrue();
        started.FailOnError.ShouldBeTrue();
        new OperationEditorViewModel(Kind(TerminateProcessOperation.TypeName)) { Value = "tool" }.ToOperation()
            .ShouldBeOfType<TerminateProcessOperation>().ProcessName.ShouldBe("tool");

        var service = new OperationEditorViewModel(Kind(StartServiceOperation.TypeName)) { Value = "svc" };
        service.Validate().ShouldBeNull();
        service.ToOperation().ShouldBeOfType<StartServiceOperation>().Arguments.ShouldBeEmpty();

        var registry = new OperationEditorViewModel(Kind(SetRegistryValuesOperation.TypeName))
        { Value = "HKEY_CURRENT_USER\\x" };
        registry.HasRegistryValues.ShouldBeTrue();
        registry.Validate()!.ShouldContain("at least one value");
        registry.AddRegistryValueCommand.Execute(null);
        registry.Validate()!.ShouldContain("needs a name");
        registry.RegistryValues.Single().Name = "N";
        registry.RegistryValues.Single().Value = "x";
        registry.RegistryValues.Single().Kind = RegistryValueKind.DWord;
        registry.Validate()!.ShouldContain("not valid for DWord");
        registry.RegistryValues.Single().Value = "99999999999";
        registry.Validate()!.ShouldContain("out of range");
        registry.RegistryValues.Single().Value = "1";
        registry.Validate().ShouldBeNull();
        var set = registry.ToOperation().ShouldBeOfType<SetRegistryValuesOperation>();
        set.Values.Single().Kind.ShouldBe(RegistryValueKind.DWord);
        set.Values.Single().Value.ShouldBe(1L);
        registry.RemoveRegistryValueCommand.Execute(registry.RegistryValues.Single());
        registry.RemoveRegistryValueCommand.Execute(null);
        registry.RegistryValues.ShouldBeEmpty();
        RegistryValueEditorViewModel.Kinds.Count.ShouldBe(6);
    }

    [Fact]
    public void RegistryValueEditor_ParsesAndFormatsEveryKind()
    {
        new RegistryValueEditorViewModel { Name = "n", Kind = RegistryValueKind.QWord, Value = " 7 " }.ToRegistryValue()
            .Value.ShouldBe(7L);
        new RegistryValueEditorViewModel { Name = "n", Kind = RegistryValueKind.MultiString, Value = "a\r\n\r\n b " }
            .ToRegistryValue().Value.ShouldBe(new[] { "a", "b" });
        new RegistryValueEditorViewModel { Name = "n", Kind = RegistryValueKind.Binary, Value = "1, 2,255" }
            .ToRegistryValue().Value.ShouldBe(new byte[] { 1, 2, 255 });
        new RegistryValueEditorViewModel { Name = "n", Kind = RegistryValueKind.Binary, Value = "AQID" }
            .ToRegistryValue().Value.ShouldBe(new byte[] { 1, 2, 3 });
        new RegistryValueEditorViewModel { Name = "n", Kind = RegistryValueKind.Binary, Value = "" }.ToRegistryValue()
            .Value.ShouldBe(Array.Empty<byte>());
        new RegistryValueEditorViewModel { Name = "n", Kind = RegistryValueKind.ExpandString, Value = "%TEMP%" }
            .ToRegistryValue().Kind.ShouldBe(RegistryValueKind.ExpandString);
        new RegistryValueEditorViewModel { Name = "n", Kind = RegistryValueKind.String, Value = "plain" }
            .ToRegistryValue().Value.ShouldBe("plain");
        new RegistryValueEditorViewModel { Name = "n", Kind = RegistryValueKind.Binary, Value = "nope!" }.Validate()!
            .ShouldContain("not valid");

        RegistryValueEditorViewModel.FromValue(RegistryValue.DWord("d", 5)).Value.ShouldBe("5");
        RegistryValueEditorViewModel.FromValue(RegistryValue.MultiString("m", "a", "b")).Value
            .ShouldBe("a" + Environment.NewLine + "b");
        RegistryValueEditorViewModel.FromValue(RegistryValue.Binary("b", [1, 2])).Value.ShouldBe("1,2");
        RegistryValueEditorViewModel.FromValue(RegistryValue.String("s", "text")).Value.ShouldBe("text");
        RegistryValueEditorViewModel.FromValue(new RegistryValue("e", RegistryValueKind.String, null)).Value
            .ShouldBe("");
        Should.Throw<ArgumentNullException>(() => RegistryValueEditorViewModel.FromValue(null!));
    }

    [Fact]
    public void OperationEditor_ValidatesAndRoundTripsExistingOperations()
    {
        new OperationEditorViewModel(Kind(DeleteFilesOperation.TypeName)).Validate()!.ShouldContain(
            "enter the directory");
        new OperationEditorViewModel(Kind(DeleteFilesOperation.TypeName)) { Value = "x" }.Validate()!.ShouldContain(
            "at least one entry");
        new OperationEditorViewModel(Kind(RenameFileOperation.TypeName)) { Value = "x" }.Validate()!.ShouldContain(
            "new file name");
        Should.Throw<ArgumentNullException>(() => new OperationEditorViewModel(null!));

        var fromDelete = OperationEditorViewModel.FromOperation(new DeleteFilesOperation
        { Directory = "%temp%", Files = ["a", "b"], RunBeforeFileReplacement = true });
        fromDelete.Value.ShouldBe("%temp%");
        fromDelete.ListText.ShouldBe("a" + Environment.NewLine + "b");
        fromDelete.RunBeforeFileReplacement.ShouldBeTrue();
        var fromRename = OperationEditorViewModel.FromOperation(new RenameFileOperation { Path = "a", NewName = "b" });
        fromRename.SecondValue.ShouldBe("b");
        OperationEditorViewModel.FromOperation(new CreateRegistryKeysOperation { Key = "k", SubKeys = ["s"] }).ListText
            .ShouldBe("s");
        OperationEditorViewModel.FromOperation(new DeleteRegistryKeysOperation { Key = "k", SubKeys = ["s"] }).Value
            .ShouldBe("k");
        OperationEditorViewModel.FromOperation(new DeleteRegistryValuesOperation { Key = "k", Names = ["v"] }).ListText
            .ShouldBe("v");
        var fromProcess = OperationEditorViewModel.FromOperation(new StartProcessOperation
        { Path = "p", Arguments = "--a", WaitForExit = true, FailOnError = true });
        fromProcess.SecondValue.ShouldBe("--a");
        fromProcess.WaitForExit.ShouldBeTrue();
        fromProcess.FailOnError.ShouldBeTrue();
        OperationEditorViewModel.FromOperation(new TerminateProcessOperation { ProcessName = "p" }).Value.ShouldBe("p");
        OperationEditorViewModel.FromOperation(new StopServiceOperation { ServiceName = "s" }).Value.ShouldBe("s");
        var fromRegistry = OperationEditorViewModel.FromOperation(new SetRegistryValuesOperation
        {
            Key = "k",
            Values = [RegistryValue.DWord("n", 5), new RegistryValue("m", RegistryValueKind.String, null)]
        });
        fromRegistry.RegistryValues.Count.ShouldBe(2);
        fromRegistry.RegistryValues[0].Value.ShouldBe("5");
        fromRegistry.RegistryValues[1].Value.ShouldBe("");
        Should.Throw<ArgumentNullException>(() => OperationEditorViewModel.FromOperation(null!));
    }

    [AvaloniaFact]
    public void OperationEditor_ShowsTheProcessOptionsOnlyForStartProcess()
    {
        var process = new OperationEditorViewModel(Kind(StartProcessOperation.TypeName))
        { Value = "%program%/migrate" };
        var editor = new OperationEditor { DataContext = process };
        var host = new Window { Content = editor };
        host.Show();
        editor.ValueBox.Text.ShouldBe("%program%/migrate");
        editor.WaitForExitBox.IsEffectivelyVisible.ShouldBeTrue();
        editor.FailOnErrorBox.IsEnabled.ShouldBeFalse();
        process.WaitForExit = true;
        editor.FailOnErrorBox.IsEnabled.ShouldBeTrue();
        editor.DataContext = new OperationEditorViewModel(Kind(DeleteFilesOperation.TypeName)) { Value = "%program%" };
        editor.WaitForExitBox.IsEffectivelyVisible.ShouldBeFalse();
        host.Close();
    }

    [Fact]
    public void OperationEditor_SummarisesItsCard()
    {
        var start = new OperationEditorViewModel(OperationKind.FromType(StartProcessOperation.TypeName));
        start.Summary.ShouldBe("Not filled in yet");
        start.Phase.ShouldBe("After files");
        start.UsesPaths.ShouldBeTrue();
        start.Value = " %program%/migrate ";
        start.Summary.ShouldBe("%program%/migrate");
        start.SecondValue = "--quiet";
        start.WaitForExit = true;
        start.Summary.ShouldBe("%program%/migrate --quiet · waits");
        start.FailOnError = true;
        start.Summary.ShouldBe("%program%/migrate --quiet · waits, fails on error");
        start.RunBeforeFileReplacement = true;
        start.Phase.ShouldBe("Before files");

        var terminate = new OperationEditorViewModel(OperationKind.FromType(TerminateProcessOperation.TypeName))
        { Value = "Tray", WaitForExit = true };
        terminate.Summary.ShouldBe("Tray");
        terminate.UsesPaths.ShouldBeFalse();
        new OperationEditorViewModel(OperationKind.FromType(DeleteFilesOperation.TypeName)).UsesPaths.ShouldBeTrue();

        var set = new OperationEditorViewModel(OperationKind.FromType(SetRegistryValuesOperation.TypeName));
        var before = set.Signature;
        set.AddRegistryValueCommand.Execute(null);
        set.Signature.ShouldNotBe(before);
        OperationKind.All.Select(k => k.AreaTitle).Distinct()
            .ShouldBe(["Files", "Processes", "Registry · Windows", "Services · Windows"]);
        Should.Throw<ArgumentNullException>(() => start.InsertPlaceholder(null!));
    }
}
