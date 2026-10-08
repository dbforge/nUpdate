using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace nUpdate.Security;

/// <summary>
///     Signs and verifies package files with RSA-PSS and SHA-512. Keys travel as PEM; the Administration converts the
///     XML keys of earlier projects once.
/// </summary>
internal sealed class PackageSigning : IDisposable
{
    public const int DefaultKeySize = 8192;

    private readonly RSA _rsa;
    private bool _disposed;

    private PackageSigning(RSA rsa)
    {
        _rsa = rsa;
    }

    public int KeySize => _rsa.KeySize;

    public string PublicKeyPem => RsaKeyPem.EncodePublic(_rsa.ExportParameters(false));

    /// <exception cref="CryptographicException">The instance holds a public key only.</exception>
    public string PrivateKeyPem => RsaKeyPem.EncodePrivate(_rsa.ExportParameters(true));

    /// <exception cref="ArgumentException">The text is not a PEM public key.</exception>
    public static PackageSigning FromPublicKey(string pem) => Import(RsaKeyPem.DecodePublic(pem));

    /// <exception cref="ArgumentException">The text is not a PEM private key.</exception>
    public static PackageSigning FromPrivateKey(string pem) => Import(RsaKeyPem.DecodePrivate(pem));

    /// <summary>Loads a key in the XML format nUpdate used before 5.0.</summary>
    /// <exception cref="ArgumentException">The text is not an RSA XML key.</exception>
    public static PackageSigning FromXml(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            throw new ArgumentNullException(nameof(xml));
        var rsa = CreateRsa();
        try
        {
            rsa.FromXmlString(xml);
            return new PackageSigning(rsa);
        }
        catch (Exception ex) when (ex is CryptographicException or System.Xml.XmlException or FormatException or ArgumentException or InvalidOperationException)
        {
            rsa.Dispose();
            throw new ArgumentException("The key is not a valid RSA XML key.", nameof(xml), ex);
        }
    }

    /// <summary>Generates a new key pair. 8192 bit keys take several seconds to create.</summary>
    public static PackageSigning Generate(int keySize = DefaultKeySize)
    {
        if (keySize < 512)
            throw new ArgumentOutOfRangeException(nameof(keySize), "The key size must be at least 512 bit.");
        var rsa = CreateRsa();
        rsa.KeySize = keySize;
        return new PackageSigning(rsa);
    }

    public byte[] Sign(Stream data) => _rsa.SignData(data, HashAlgorithmName.SHA512, RSASignaturePadding.Pss);

    public byte[] Sign(byte[] data) => _rsa.SignData(data, HashAlgorithmName.SHA512, RSASignaturePadding.Pss);

    public bool Verify(Stream data, byte[] signature) => _rsa.VerifyData(data, signature, HashAlgorithmName.SHA512, RSASignaturePadding.Pss);

    public bool Verify(byte[] data, byte[] signature) => _rsa.VerifyData(data, signature, HashAlgorithmName.SHA512, RSASignaturePadding.Pss);

    public void Dispose()
    {
        if (_disposed)
            return;
        _rsa.Dispose();
        _disposed = true;
    }

    private static PackageSigning Import(RSAParameters parameters)
    {
        var rsa = CreateRsa();
        try
        {
            rsa.ImportParameters(parameters);
            return new PackageSigning(rsa);
        }
        catch (CryptographicException ex)
        {
            rsa.Dispose();
            throw new ArgumentException("The key is not a valid RSA key.", nameof(parameters), ex);
        }
    }

    // .NET Framework's default RSA implementation cannot do PSS padding; CNG can. Only reachable on .NET Framework.
    [ExcludeFromCodeCoverage]
    private static RSA CreateRsa() =>
        RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.Ordinal) ? new RSACng() : RSA.Create();
}
