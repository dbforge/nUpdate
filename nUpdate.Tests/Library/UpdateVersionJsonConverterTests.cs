using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class UpdateVersionJsonConverterTests
{
    [Fact]
    public void UpdateVersionJsonConverter_RoundTripsThroughTheCanonicalString()
    {
        var version = new UpdateVersion("1.2.0-beta.3");
        Serializer.Serialize(version).ShouldBe("\"1.2.0-beta.3\"");
        Serializer.Deserialize<UpdateVersion>("\"2.0.0-rc.1\"").ShouldBe(new UpdateVersion("2.0.0-rc.1"));
        Serializer.Deserialize<UpdateVersion>("null").ShouldBeNull();
        Serializer.Serialize(new Holder { Version = null }).ShouldBe("{\"version\":null}");
        Serializer.Deserialize<Holder>("{\"version\":\"1.0.0\"}")!.Version.ShouldBe(new UpdateVersion("1.0.0"));
        Should.Throw<Newtonsoft.Json.JsonSerializationException>(() => Serializer.Deserialize<UpdateVersion>("\"not a version\"")).Message.ShouldContain("not a version");
        // A feed written by an older tool is rejected with a hint at the expected form.
        Should.Throw<Newtonsoft.Json.JsonSerializationException>(() => Serializer.Deserialize<UpdateVersion>("\"1.0.0.0\"")).Message.ShouldContain("major.minor.patch");
        Should.Throw<Newtonsoft.Json.JsonSerializationException>(() => Serializer.Deserialize<UpdateVersion>("12")).Message.ShouldContain("string");
        var converter = new UpdateVersionJsonConverter();
        using var text = new StringWriter();
        using (var writer = new Newtonsoft.Json.JsonTextWriter(text))
            converter.WriteJson(writer, null, Newtonsoft.Json.JsonSerializer.CreateDefault());
        text.ToString().ShouldBe("null");
        Should.Throw<ArgumentNullException>(() => converter.WriteJson(null!, version, Newtonsoft.Json.JsonSerializer.CreateDefault()));
        Should.Throw<ArgumentNullException>(() => converter.ReadJson(null!, typeof(UpdateVersion), null, false, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    private sealed class Holder
    {
        public UpdateVersion? Version { get; set; }
    }
}
