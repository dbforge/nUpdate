using System.Security.Cryptography;

namespace nUpdate.Administration.Core.Migration;

/// <summary>The package signatures of nUpdate 3 and 4: RSA PKCS#1 v1.5 over the zip with SHA-512, stored as Base64 in <c>updates.json</c>.</summary>
public static class LegacySignature
{
    /// <summary>Whether the zip carries the signature of the project's key pair; anything unreadable counts as not signed.</summary>
    public static bool Verify(Stream zip, string publicKeyPem, string? base64Signature)
    {
        ArgumentNullException.ThrowIfNull(zip);
        ArgumentNullException.ThrowIfNull(publicKeyPem);
        if (string.IsNullOrEmpty(base64Signature))
            return false;
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);
            return rsa.VerifyData(zip, Convert.FromBase64String(base64Signature), HashAlgorithmName.SHA512,
                RSASignaturePadding.Pkcs1);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException or CryptographicException)
        {
            return false;
        }
    }
}
