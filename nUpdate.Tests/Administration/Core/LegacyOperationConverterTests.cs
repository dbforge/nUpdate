using Newtonsoft.Json.Linq;
using nUpdate.Administration.Core.Migration;
using nUpdate.Operations;
using static nUpdate.Tests.Administration.Support.LegacyTestData;

namespace nUpdate.Tests.Administration.Core;

public class LegacyOperationConverterTests
{
    [Fact]
    public void Convert_HandlesEveryShape()
    {
        var conversion = LegacyOperationConverter.Convert((JArray)JToken.Parse(LegacyOperations));
        var operations = conversion.Operations;
        conversion.Warnings.ShouldBe([
            "Operation 5: the registry value \"Bad\" cannot be read as DWord and is left out.",
            "Operation 5: a registry value without a name is left out.",
            "Operation 11 (Scripts Execute) has no counterpart in nUpdate 5 and is left out.",
            "Operation 12 (Scripts Create) has no counterpart in nUpdate 5 and is left out.",
            "Operation 13 (9 Create) has no counterpart in nUpdate 5 and is left out.",
            "Operation 14 cannot be read and is left out.",
        ]);
        operations.Select(o => o.Type).ShouldBe([
            "deleteFiles", "renameFile", "createRegistryKeys", "deleteRegistryKeys", "setRegistryValues",
            "deleteRegistryValues",
            "startProcess", "terminateProcess", "startService", "stopService",
        ]);
        operations[0].ShouldBeOfType<DeleteFilesOperation>().Files.ShouldBe(["old.dll"]);
        operations[1].ShouldBeOfType<RenameFileOperation>().NewName.ShouldBe("kept.txt");
        operations[2].ShouldBeOfType<CreateRegistryKeysOperation>().SubKeys.ShouldBe(["Sub"]);
        operations[3].ShouldBeOfType<DeleteRegistryKeysOperation>().SubKeys.ShouldBe(["Old"]);
        var values = operations[4].ShouldBeOfType<SetRegistryValuesOperation>().Values;
        values.Select(v => $"{v.Name}:{v.Kind}").ShouldBe([
            "Installed:DWord", "Big:QWord", "Text:String", "Expand:ExpandString", "Multi:MultiString",
            "MultiArray:MultiString", "Bytes:Binary", "Base64:Binary", "ByteArray:Binary", "Default:String"
        ]);
        values[0].Value.ShouldBe(42L);
        values[1].Value.ShouldBe(7L);
        values[2].Value.ShouldBe("hello");
        values[4].Value.ShouldBe(new[] { "a", "b" });
        values[5].Value.ShouldBe(new[] { "x" });
        values[6].Value.ShouldBe(new byte[] { 1, 2, 3 });
        values[7].Value.ShouldBe(new byte[] { 1, 2, 3 });
        values[8].Value.ShouldBe(new byte[] { 9 });
        values[9].Value.ShouldBe("");
        operations[5].ShouldBeOfType<DeleteRegistryValuesOperation>().Names.ShouldBe(["Gone"]);
        operations[6].ShouldBeOfType<StartProcessOperation>().Arguments.ShouldBe("--flag");
        var terminate = operations[7].ShouldBeOfType<TerminateProcessOperation>();
        terminate.ProcessName.ShouldBe("helper");
        terminate.RunBeforeFileReplacement.ShouldBeTrue();
        operations[8].ShouldBeOfType<StartServiceOperation>().Arguments.ShouldBe(["-a", "-b"]);
        operations[9].ShouldBeOfType<StopServiceOperation>().ServiceName.ShouldBe("svc");

        LegacyOperationConverter.Convert((JArray?)null).Operations.ShouldBeEmpty();
        LegacyOperationConverter.Convert((JArray)JToken.Parse("""[{"Area":"Gone","Method":"Away"},{}]""")).Warnings
            .ShouldBe([
                "Operation 1 (Gone Away) has no counterpart in nUpdate 5 and is left out.",
                "Operation 2 (? ?) has no counterpart in nUpdate 5 and is left out."
            ]);
        Should.Throw<ArgumentNullException>(() => new LegacyOperationConversion(null!, []));
        Should.Throw<ArgumentNullException>(() => new LegacyOperationConversion([], null!));
        LegacyOperationConverter.Convert(JObject.Parse("""{"Area":2,"Method":5,"Value":"x","Value2":{"odd":1}}"""))
            .ShouldBeOfType<StartProcessOperation>().Arguments.ShouldBe("""{"odd":1}""");
        LegacyOperationConverter.Convert(JObject.Parse("""{"Area":0,"Method":1,"Value":"d","Value2":"single"}"""))
            .ShouldBeOfType<DeleteFilesOperation>().Files.ShouldBe(["single"]);
        LegacyOperationConverter.Convert(JObject.Parse("""{"Area":0,"Method":1,"Value":"d","Value2":null}"""))
            .ShouldBeOfType<DeleteFilesOperation>().Files.ShouldBeEmpty();
        LegacyOperationConverter.Convert(JObject.Parse("""{"Area":1,"Method":3,"Value":"k","Value2":"not an array"}"""))
            .ShouldBeOfType<SetRegistryValuesOperation>().Values.ShouldBeEmpty();
        LegacyOperationConverter
            .Convert(JObject.Parse(
                """{"Area":1,"Method":3,"Value":"k","Value2":[{"Item1":"n","Item2":{"o":1},"Item3":1}]}"""))
            .ShouldBeOfType<SetRegistryValuesOperation>().Values.Single().Value.ShouldBe("""{"o":1}""");
        LegacyOperationConverter
            .Convert(JObject.Parse(
                """{"Area":1,"Method":3,"Value":"k","Value2":[{"Item1":"n","Item2":"nope!","Item3":3}]}"""))
            .ShouldBeOfType<SetRegistryValuesOperation>().Values.ShouldBeEmpty();
        LegacyOperationConverter.Convert(JObject.Parse("""{"Area":"Nope","Method":0}""")).ShouldBeNull();
        LegacyOperationConverter.Convert(JObject.Parse("""{"Area":0,"Method":1}"""))
            .ShouldBeOfType<DeleteFilesOperation>().Directory.ShouldBe("");
        LegacyOperationConverter.Convert(JObject.Parse("""{"Area":0,"Method":1,"Value":"d","Value2":""}"""))
            .ShouldBeOfType<DeleteFilesOperation>().Files.ShouldBeEmpty();
        LegacyOperationConverter.Convert(JObject.Parse("""{"Area":0,"Method":1,"Value":"d","Value2":7}"""))
            .ShouldBeOfType<DeleteFilesOperation>().Files.ShouldBe(["7"]);
        LegacyOperationConverter.Convert(JObject.Parse("""{"Area":0,"Method":2,"Value":"p","Value2":null}"""))
            .ShouldBeOfType<RenameFileOperation>().NewName.ShouldBe("");
        LegacyOperationConverter
            .Convert(JObject.Parse(
                """{"Area":1,"Method":3,"Value":"k","Value2":[{"Item1":"n","Item2":1,"Item3":null}]}"""))
            .ShouldBeOfType<SetRegistryValuesOperation>().Values.Single().Kind.ShouldBe(RegistryValueKind.String);
        foreach (var (area, method) in new[]
                     { (0, 0), (0, 3), (1, 2), (1, 5), (2, 0), (2, 7), (3, 1), (3, 7), (4, 5), (4, 1), (0, 9), (9, 1) })
            LegacyOperationConverter.Convert(JObject.Parse($$"""{"Area":{{area}},"Method":{{method}},"Value":"v"}"""))
                .ShouldBeNull();
        Should.Throw<ArgumentNullException>(() => LegacyOperationConverter.Convert((JObject)null!));
    }

    [Fact]
    public void Convert_LeavesOutWhatItCannotRead()
    {
        var conversion = LegacyOperationConverter.Convert((JArray)JToken.Parse(
            """[{"Area":0,"Method":1,"Value":"d","ExecuteBeforeReplacingFiles":"perhaps"},{"Area":99999999999,"Method":1},{"Area":1,"Method":3,"Value":"k","Value2":[{"Item1":"n","Item2":"300,1","Item3":3}]}]"""));
        conversion.Operations.Single().ShouldBeOfType<SetRegistryValuesOperation>().Values.ShouldBeEmpty();
        conversion.Warnings[0].ShouldStartWith("Operation 1 cannot be read and is left out:");
        conversion.Warnings[1].ShouldStartWith("Operation 2 cannot be read and is left out:");
        conversion.Warnings[2].ShouldContain("cannot be read as Binary");
    }
}
