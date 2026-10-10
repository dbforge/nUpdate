using System.Text;
using nUpdate.Installer;

namespace nUpdate.Tests.Library;

public class SerializerTests
{
    private sealed record Sample(string Name, int? Count, LockedFileDecision Decision = LockedFileDecision.Retry);

    [Fact]
    public void Serialize_UsesCamelCaseStringEnumsAndIncludesNulls()
    {
        Serializer.Serialize(new Sample("a", null, LockedFileDecision.Skip))
            .ShouldBe("""{"name":"a","count":null,"decision":"skip"}""");
        Serializer.Serialize(new Sample("a", 1), indented: true).ShouldContain(Environment.NewLine);
    }

    [Fact]
    public void Deserialize_FromStringAndStream_KeepsDatesAsStringsAndIgnoresCase()
    {
        Serializer.Deserialize<Sample>("""{"Name":"x","Count":3,"Decision":"ABORT"}""")
            .ShouldBe(new Sample("x", 3, LockedFileDecision.Abort));
        using var stream = new MemoryStream(Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes("""{"name":"2020-01-01T00:00:00Z","count":null}""")).ToArray());
        Serializer.Deserialize<Sample>(stream)!.Name.ShouldBe("2020-01-01T00:00:00Z");
        stream.CanRead.ShouldBeTrue();
    }
}
