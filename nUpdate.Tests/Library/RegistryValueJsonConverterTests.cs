using Newtonsoft.Json;
using nUpdate.Operations;

namespace nUpdate.Tests.Library;

public class RegistryValueJsonConverterTests
{
    [Fact]
    public void RegistryValueJsonConverter_ReadsLenientlyAndReportsProblems()
    {
        Serializer.Deserialize<RegistryValue>("""{"name":"n","kind":"DWORD","value":"12"}""")!.Value.ShouldBe(12L);
        Serializer.Deserialize<RegistryValue>("""{"name":"n","kind":"string","value":5}""")!.Value.ShouldBe("5");
        Serializer.Deserialize<RegistryValue>("""{"name":"n","kind":"string"}""")!.Value.ShouldBeNull();
        Serializer.Deserialize<RegistryValue>("null").ShouldBeNull();
        Serializer.Serialize(new Holder()).ShouldBe("{\"value\":null}");
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<RegistryValue>("""{"kind":"string"}""")).Message.ShouldContain("name");
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<RegistryValue>("""{"name":"n"}""")).Message.ShouldContain("kind");
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<RegistryValue>("""{"name":"n","kind":"nope"}""")).Message.ShouldContain("nope");
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<RegistryValue>("""{"name":"n","kind":"dword"}""")).Message.ShouldContain("no value");
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<RegistryValue>("""{"name":"n","kind":"dword","value":"x"}""")).Message.ShouldContain("not a number");
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<RegistryValue>("""{"name":"n","kind":"qword","value":99999999999999999999999}""")).Message.ShouldContain("range");
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<RegistryValue>("""{"name":"n","kind":"dword","value":4294967296}""")).Message.ShouldContain("out of range");
        Serializer.Deserialize<RegistryValue>("""{"name":"n","kind":"dword","value":4294967295}""")!.Value.ShouldBe(4294967295L);
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<RegistryValue>("""{"name":"n","kind":"multiString","value":"x"}""")).Message.ShouldContain("array");
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<RegistryValue>("""{"name":"n","kind":"binary","value":"%%%"}""")).Message.ShouldContain("Base64");
        var converter = new RegistryValueJsonConverter();
        Should.Throw<ArgumentNullException>(() => converter.WriteJson(null!, null, JsonSerializer.CreateDefault()));
        Should.Throw<ArgumentNullException>(() => converter.ReadJson(null!, typeof(RegistryValue), null, false, JsonSerializer.CreateDefault()));
        using var text = new StringWriter();
        using (var writer = new JsonTextWriter(text))
            converter.WriteJson(writer, null, JsonSerializer.CreateDefault());
        text.ToString().ShouldBe("null");
    }

    private sealed class Holder
    {
        public RegistryValue? Value { get; set; }
    }
}
