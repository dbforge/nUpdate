using Newtonsoft.Json;
using nUpdate.Operations;

namespace nUpdate.Tests.Library;

public class OperationJsonConverterTests
{
    [Fact]
    public void ReadJson_NeedsAKnownType()
    {
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<Operation>("""{"directory":"x"}"""))
            .Message.ShouldContain("type");
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<Operation>("""{"type":"formatDisk"}"""))
            .Message.ShouldContain("formatDisk");
        Should.Throw<JsonSerializationException>(() => Serializer.Deserialize<Operation>("""{"type":7}""")).Message
            .ShouldContain("type");
        Serializer.Deserialize<Operation>("null").ShouldBeNull();
        Serializer.Deserialize<List<Operation>>(
                """[{"type":"stopService","serviceName":"s"},{"type":"deleteFiles","directory":"d","files":[]}]""")!
            .Select(o => o.Type).ShouldBe(["stopService", "deleteFiles"]);
    }

    [Fact]
    public void OperationJsonConverter_GuardsItsArguments()
    {
        var converter = new OperationJsonConverter();
        converter.CanWrite.ShouldBeFalse();
        converter.CanConvert(typeof(DeleteFilesOperation)).ShouldBeTrue();
        converter.CanConvert(typeof(string)).ShouldBeFalse();
        Should.Throw<ArgumentNullException>(() =>
            converter.ReadJson(null!, typeof(Operation), null, JsonSerializer.CreateDefault()));
        Should.Throw<ArgumentNullException>(() =>
            converter.ReadJson(new JsonTextReader(new StringReader("{}")), typeof(Operation), null, null!));
        Should.Throw<NotSupportedException>(() =>
            converter.WriteJson(new JsonTextWriter(new StringWriter()), null, JsonSerializer.CreateDefault()));
    }
}
