using System.Security.Cryptography;
using nUpdate.Security;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Library;

public class RsaKeyPemTests
{
    [Fact]
    public void RsaKeyPem_MatchesTheRuntimeEncoder()
    {
        // The hand-written DER must produce exactly what .NET itself produces for the same key.
        using var rsa = RSA.Create(2048);
        RsaKeyPem.EncodePublic(rsa.ExportParameters(false)).ShouldBe(rsa.ExportSubjectPublicKeyInfoPem());
        RsaKeyPem.EncodePrivate(rsa.ExportParameters(true)).ShouldBe(rsa.ExportPkcs8PrivateKeyPem());

        using var imported = RSA.Create();
        imported.ImportFromPem(TestKeys.PrivateKey);
        var ours = RsaKeyPem.DecodePrivate(TestKeys.PrivateKey);
        var theirs = imported.ExportParameters(true);
        ours.Modulus.ShouldBe(theirs.Modulus);
        ours.D.ShouldBe(theirs.D);
        ours.P.ShouldBe(theirs.P);
        ours.InverseQ.ShouldBe(theirs.InverseQ);
    }

    [Fact]
    public void RsaKeyPem_RejectsBrokenInput()
    {
        Should.Throw<ArgumentNullException>(() => RsaKeyPem.DecodePublic(null!));
        Should.Throw<ArgumentException>(() => RsaKeyPem.DecodePublic("not a key")).Message.ShouldContain("PEM");
        Should.Throw<ArgumentException>(() => RsaKeyPem.DecodePublic(TestKeys.PrivateKey));
        Should.Throw<ArgumentException>(() =>
                RsaKeyPem.DecodePublic("-----BEGIN PUBLIC KEY-----\n%%%\n-----END PUBLIC KEY-----")).Message
            .ShouldContain("Base64");
        Should.Throw<ArgumentException>(() => RsaKeyPem.DecodePublic(Pem("PUBLIC KEY", [0x04, 0x00]))).Message
            .ShouldContain("unexpected element");
        Should.Throw<ArgumentException>(() => RsaKeyPem.DecodePublic(Pem("PUBLIC KEY", [0x30, 0x05, 0x30]))).Message
            .ShouldContain("ends unexpectedly");
        Should.Throw<ArgumentException>(() =>
                RsaKeyPem.DecodePublic(Pem("PUBLIC KEY", [0x30, 0x85, 0x01, 0x01, 0x01, 0x01, 0x01]))).Message
            .ShouldContain("length");
        Should.Throw<ArgumentException>(() => RsaKeyPem.DecodePublic(Pem("PUBLIC KEY", [0x30, 0x80, 0x00, 0x00])))
            .Message.ShouldContain("length");
        Should.Throw<ArgumentException>(() =>
                RsaKeyPem.DecodePublic(Pem("PUBLIC KEY", [0x30, 0x04, 0x30, 0x00, 0x03, 0x00]))).Message
            .ShouldContain("bit string");
        Should.Throw<ArgumentException>(() => RsaKeyPem.DecodePublic(Pem("PUBLIC KEY", [0x30, 0x00]))).Message
            .ShouldContain("ends unexpectedly");
        Should.Throw<ArgumentException>(() => RsaKeyPem.DecodePublic(Pem("PUBLIC KEY", [0x30, 0x02, 0x05, 0x00])))
            .Message.ShouldContain("ends unexpectedly");
        // Truncated or hostile lengths: a missing length byte, a long form that runs past the end, a long form longer than three bytes.
        Should.Throw<ArgumentException>(() => RsaKeyPem.DecodePublic(Pem("PUBLIC KEY", [0x30]))).Message
            .ShouldContain("ends unexpectedly");
        Should.Throw<ArgumentException>(() => RsaKeyPem.DecodePublic(Pem("PUBLIC KEY", [0x30, 0x82, 0x01]))).Message
            .ShouldContain("ends unexpectedly");
        Should.Throw<ArgumentException>(() =>
                RsaKeyPem.DecodePublic(Pem("PUBLIC KEY", [0x30, 0x84, 0xFF, 0xFF, 0xFF, 0xFF]))).Message
            .ShouldContain("length");
        Should.Throw<ArgumentException>(() =>
                RsaKeyPem.DecodePublic(Pem("PUBLIC KEY", [0x30, 0x83, 0x7F, 0xFF, 0xFF, 0x30]))).Message
            .ShouldContain("ends unexpectedly");
        Should.Throw<ArgumentException>(() =>
            RsaKeyPem.EncodePrivate(new RSAParameters { Modulus = [1], Exponent = [1] }));
        Should.Throw<ArgumentException>(() => RsaKeyPem.EncodePublic(new RSAParameters { Modulus = [1] }));
        Should.Throw<ArgumentException>(() =>
            PackageSigning.FromPublicKey(Pem("PUBLIC KEY", RsaKeyPemFixtures.PublicKeyWithZeroModulus)));
        Should.Throw<ArgumentException>(() => RsaKeyPem.DecodePrivate(RsaKeyPemFixtures.PrivateKeyWithOversizedPrime))
            .Message.ShouldContain("inconsistent");
    }

    [Fact]
    public void RsaKeyPem_HandlesLongLengthsAndLeadingZeros()
    {
        // 4096-bit keys need multi-byte DER lengths; the modulus has its high bit set and therefore a leading zero in DER.
        using var rsa = RSA.Create(4096);
        var pem = RsaKeyPem.EncodePublic(rsa.ExportParameters(false));
        RsaKeyPem.DecodePublic(pem).Modulus.ShouldBe(rsa.ExportParameters(false).Modulus);
        RsaKeyPem.EncodePrivate(rsa.ExportParameters(true)).ShouldBe(rsa.ExportPkcs8PrivateKeyPem());
        // Values with leading zeros are trimmed when written and padded back on read.
        var padded = RsaKeyPem.DecodePublic(RsaKeyPem.EncodePublic(new RSAParameters
        { Modulus = [0, 0, 0x7F, 0x01], Exponent = [0, 3] }));
        padded.Modulus.ShouldBe(new byte[] { 0x7F, 0x01 });
        padded.Exponent.ShouldBe(new byte[] { 3 });
    }

    private static string Pem(string label, byte[] der) =>
        $"-----BEGIN {label}-----\n{Convert.ToBase64String(der)}\n-----END {label}-----";
}

/// <summary>Hand-made DER structures that are syntactically fine but not usable keys.</summary>
internal static class RsaKeyPemFixtures
{
    /// <summary>A SubjectPublicKeyInfo whose modulus is 0.</summary>
    public static byte[] PublicKeyWithZeroModulus { get; } =
    [
        0x30, 0x1A,
        0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00,
        0x03, 0x09, 0x00, 0x30, 0x06, 0x02, 0x01, 0x00, 0x02, 0x01, 0x03,
    ];

    /// <summary>A PKCS#8 key whose prime P is longer than half the modulus.</summary>
    public static string PrivateKeyWithOversizedPrime { get; } = "-----BEGIN PRIVATE KEY-----\n" +
                                                                 Convert.ToBase64String(
                                                                 [
                                                                     0x30, 0x34,
                                                                     0x02, 0x01, 0x00,
                                                                     0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86,
                                                                     0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00,
                                                                     0x04, 0x20, 0x30, 0x1E,
                                                                     0x02, 0x01, 0x00, // version
                                                                     0x02, 0x02, 0x01, 0x01, // n (2 bytes)
                                                                     0x02, 0x01, 0x03, // e
                                                                     0x02, 0x01, 0x05, // d
                                                                     0x02, 0x03, 0x01, 0x01,
                                                                     0x01, // p: 3 bytes, more than half of n
                                                                     0x02, 0x01, 0x07, // q
                                                                     0x02, 0x01, 0x01, // dp
                                                                     0x02, 0x01, 0x01, // dq
                                                                     0x02, 0x01, 0x01, // qinv
                                                                 ]) + "\n-----END PRIVATE KEY-----";
}
