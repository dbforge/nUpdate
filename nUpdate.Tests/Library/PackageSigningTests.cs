using System.Security.Cryptography;
using System.Text;
using nUpdate.Security;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Library;

public class PackageSigningTests
{
    [Fact]
    public void Verify_WorksForBytesAndStreams()
    {
        var data = new byte[1000];
        new Random(1).NextBytes(data);

        using var signer = PackageSigning.FromPrivateKey(TestKeys.PrivateKey);
        using var verifier = PackageSigning.FromPublicKey(TestKeys.PublicKey);
        signer.KeySize.ShouldBe(2048);

        var byteSignature = signer.Sign(data);
        verifier.Verify(data, byteSignature).ShouldBeTrue();
        using (var stream = new MemoryStream(data))
            verifier.Verify(stream, byteSignature).ShouldBeTrue();

        byte[] streamSignature;
        using (var stream = new MemoryStream(data))
            streamSignature = signer.Sign(stream);
        verifier.Verify(data, streamSignature).ShouldBeTrue();

        data[0] ^= 0xFF;
        verifier.Verify(data, byteSignature).ShouldBeFalse();
    }

    [Fact]
    public void Sign_UsesPssPadding()
    {
        var data = Encoding.UTF8.GetBytes("payload");
        using var signer = PackageSigning.FromPrivateKey(TestKeys.PrivateKey);
        var signature = signer.Sign(data);
        using var rsa = RSA.Create();
        rsa.ImportParameters(RsaKeyPem.DecodePublic(TestKeys.PublicKey));
        rsa.VerifyData(data, signature, HashAlgorithmName.SHA512, RSASignaturePadding.Pss).ShouldBeTrue();
        rsa.VerifyData(data, signature, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1).ShouldBeFalse();
    }

    [Fact]
    public void PackageSigning_KeysRoundTripThroughPem()
    {
        TestKeys.PublicKey.ShouldStartWith("-----BEGIN PUBLIC KEY-----\n");
        TestKeys.PublicKey.ShouldEndWith("-----END PUBLIC KEY-----");
        TestKeys.PrivateKey.ShouldStartWith("-----BEGIN PRIVATE KEY-----\n");
        TestKeys.PublicKey.Split('\n').Skip(1).SkipLast(1).ShouldAllBe(line => line.Length <= 64);
        RsaKeyPem.IsPublicKey(TestKeys.PublicKey).ShouldBeTrue();
        RsaKeyPem.IsPublicKey(TestKeys.PrivateKey).ShouldBeFalse();
        RsaKeyPem.IsPublicKey(null).ShouldBeFalse();

        using var fromPrivate = PackageSigning.FromPrivateKey(TestKeys.PrivateKey);
        fromPrivate.PublicKeyPem.ShouldBe(TestKeys.PublicKey);
        fromPrivate.PrivateKeyPem.ShouldBe(TestKeys.PrivateKey);
        using var fromPublic = PackageSigning.FromPublicKey(TestKeys.PublicKey);
        fromPublic.PublicKeyPem.ShouldBe(TestKeys.PublicKey);
        Should.Throw<CryptographicException>(() => fromPublic.PrivateKeyPem);
    }

    [Fact]
    public void FromXml_ReadsTheKeysOfEarlierVersions()
    {
        using var rsa = RSA.Create(2048);
        using var fromXml = PackageSigning.FromXml(rsa.ToXmlString(true));
        fromXml.PublicKeyPem.ShouldBe(RsaKeyPem.EncodePublic(rsa.ExportParameters(false)));
        fromXml.PrivateKeyPem.ShouldBe(RsaKeyPem.EncodePrivate(rsa.ExportParameters(true)));
        using var publicOnly = PackageSigning.FromXml(rsa.ToXmlString(false));
        publicOnly.PublicKeyPem.ShouldBe(fromXml.PublicKeyPem);
        Should.Throw<ArgumentNullException>(() => PackageSigning.FromXml(" "));
        Should.Throw<ArgumentException>(() =>
            PackageSigning.FromXml("<RSAKeyValue><Modulus>AQ==</Modulus></RSAKeyValue>"));
        Should.Throw<ArgumentException>(() => PackageSigning.FromXml("not xml"));
    }

    [Fact]
    public void Generate_RejectsTinyKeys()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => PackageSigning.Generate(256));
        PackageSigning.DefaultKeySize.ShouldBe(8192);
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var signing = PackageSigning.FromPublicKey(TestKeys.PublicKey);
        signing.Dispose();
        signing.Dispose();
    }
}
