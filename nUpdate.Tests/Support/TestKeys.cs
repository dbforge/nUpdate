using System.Security.Cryptography;
using nUpdate.Security;

namespace nUpdate.Tests.Support;

/// <summary>One 2048-bit key pair shared by all tests, generated once per run.</summary>
public static class TestKeys
{
    private static readonly Lazy<(string PublicKey, string PrivateKey)> Pair = new(() =>
    {
        using var signing = PackageSigning.Generate(2048);
        return (signing.PublicKeyPem, signing.PrivateKeyPem);
    });

    public static string PublicKey => Pair.Value.PublicKey;

    public static string PrivateKey => Pair.Value.PrivateKey;

    /// <summary>The Base64 RSA-PSS/SHA-512 signature of the data.</summary>
    public static string Sign(byte[] data)
    {
        using var signer = PackageSigning.FromPrivateKey(PrivateKey);
        return Convert.ToBase64String(signer.Sign(data));
    }

    /// <summary>The Base64 SHA-512 of the data, as the feed carries it.</summary>
    public static string Sha512(byte[] data) => Convert.ToBase64String(SHA512.HashData(data));
}
