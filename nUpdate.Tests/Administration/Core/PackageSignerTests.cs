using nUpdate.Administration.Core.Packages;
using nUpdate.Tests.Administration.Support;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Administration.Core;

public class PackageSignerTests
{
    private readonly AdminTestContext _context = new();

    [Fact]
    public void PackageSigner_SignsHashesAndVerifies()
    {
        var path = _context.AddSourceFile("pkg.zip", "package bytes");
        var signer = _context.Signer;
        var signature = signer.Sign(path, TestKeys.PrivateKey);
        signer.Verify(path, TestKeys.PublicKey, signature).ShouldBeTrue();
        signer.Hash(path).ShouldBe(TestKeys.Sha512("package bytes"u8.ToArray()));
        _context.FileSystem.File.WriteAllText(path, "tampered");
        signer.Verify(path, TestKeys.PublicKey, signature).ShouldBeFalse();
        Should.Throw<ArgumentException>(() => signer.Sign(" ", TestKeys.PrivateKey));
        Should.Throw<ArgumentException>(() => signer.Verify(" ", TestKeys.PublicKey, signature));
        Should.Throw<ArgumentException>(() => signer.Verify(path, TestKeys.PublicKey, " "));
        Should.Throw<ArgumentException>(() => signer.Hash(" "));
        Should.Throw<ArgumentNullException>(() => new PackageSigner(null!));
    }
}
