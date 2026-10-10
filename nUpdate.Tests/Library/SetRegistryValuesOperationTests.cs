using nUpdate.Operations;
using nUpdate.Tests.Library.Support;

namespace nUpdate.Tests.Library;

public class SetRegistryValuesOperationTests
{
    [Fact]
    public void Values_AreTypedByKind()
    {
        var operation = new SetRegistryValuesOperation
        {
            Key = "HKEY_CURRENT_USER\\Software\\App",
            Values =
            [
                RegistryValue.String("Name", "Value"),
                RegistryValue.ExpandString("Path", "%ProgramFiles%\\App"),
                RegistryValue.DWord("Mode", 1),
                RegistryValue.QWord("Big", 5_000_000_000),
                RegistryValue.MultiString("List", "a", "b"),
                RegistryValue.Binary("Blob", [1, 2, 3]),
                new RegistryValue("Empty", RegistryValueKind.String, null),
            ],
        };

        var json = Serializer.Serialize(operation);
        json.ShouldContain("""{"name":"Mode","kind":"dword","value":1}""");
        json.ShouldContain("""{"name":"Big","kind":"qword","value":5000000000}""");
        json.ShouldContain("""{"name":"List","kind":"multiString","value":["a","b"]}""");
        json.ShouldContain("""{"name":"Blob","kind":"binary","value":"AQID"}""");
        json.ShouldContain("""{"name":"Path","kind":"expandString","value":"%ProgramFiles%\\App"}""");
        json.ShouldContain("""{"name":"Empty","kind":"string","value":null}""");

        var restored = OperationJson.RoundTrip(operation).ShouldBeOfType<SetRegistryValuesOperation>();
        restored.Values.Select(v => v.Kind).ShouldBe([
            RegistryValueKind.String, RegistryValueKind.ExpandString, RegistryValueKind.DWord, RegistryValueKind.QWord,
            RegistryValueKind.MultiString, RegistryValueKind.Binary, RegistryValueKind.String
        ]);
        restored.Values[2].Value.ShouldBe(1L);
        restored.Values[3].Value.ShouldBe(5_000_000_000L);
        restored.Values[4].Value.ShouldBe(new[] { "a", "b" });
        restored.Values[5].Value.ShouldBe(new byte[] { 1, 2, 3 });
        restored.Values[6].Value.ShouldBeNull();
    }
}
