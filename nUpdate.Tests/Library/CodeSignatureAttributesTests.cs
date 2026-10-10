using nUpdate.Platform;

namespace nUpdate.Tests.Library;

/// <summary>The macOS code signature attributes; off macOS there are none, and only <c>com.apple.cs.*</c> is ever written.</summary>
public class CodeSignatureAttributesTests
{
    private readonly CodeSignatureAttributes _offMacOS = new(isMacOS: false);

    [Fact]
    public void Read_FindsNoneOffMacOS()
    {
        _offMacOS.Read("/any/file").ShouldBeEmpty();
        Should.Throw<ArgumentNullException>(() => _offMacOS.Read(null!));
    }

    [Fact]
    public void Write_AcceptsOnlyCodeSignatureAttributesAndDoesNothingOffMacOS()
    {
        _offMacOS.Write("/any/file", "com.apple.cs.CodeDirectory", [1, 2, 3]);
        Should.Throw<ArgumentException>(() => _offMacOS.Write("/any/file", "com.apple.quarantine", [1]))
            .ParamName.ShouldBe("name");
        Should.Throw<ArgumentNullException>(() => _offMacOS.Write(null!, "com.apple.cs.CodeDirectory", [1]));
        Should.Throw<ArgumentNullException>(() => _offMacOS.Write("/any/file", null!, [1]));
        Should.Throw<ArgumentNullException>(() => _offMacOS.Write("/any/file", "com.apple.cs.CodeDirectory", null!));
        CodeSignatureAttributes.Prefix.ShouldBe("com.apple.cs.");
        new CodeSignatureAttributes().ShouldNotBeNull();
    }

    [Fact]
    public void OnMacOS_ReadsAndWritesThroughTheFileSystem()
    {
        new CodeSignatureAttributes(isMacOS: true).ShouldNotBeNull();
        var written = new List<(string Path, string Name, byte[] Value)>();
        var attributes = new CodeSignatureAttributes(
            path => new Dictionary<string, byte[]> { [CodeSignatureAttributes.Prefix + "CodeDirectory"] = [(byte)path.Length] },
            (path, name, value) => written.Add((path, name, value)));

        attributes.Read("/app")[CodeSignatureAttributes.Prefix + "CodeDirectory"].ShouldBe([(byte)4]);
        attributes.Write("/app", CodeSignatureAttributes.Prefix + "CodeSignature", [9]);

        var write = written.ShouldHaveSingleItem();
        write.Path.ShouldBe("/app");
        write.Name.ShouldBe(CodeSignatureAttributes.Prefix + "CodeSignature");
        write.Value.ShouldBe([(byte)9]);
    }
}
