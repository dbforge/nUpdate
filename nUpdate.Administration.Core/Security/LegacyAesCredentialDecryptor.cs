using System.Security.Cryptography;
using System.Text;

namespace nUpdate.Administration.Core.Security;

/// <summary>
///     Decrypts secrets stored by nUpdate Administration 3.x/4.x (AES-256-CBC with PBKDF2-SHA1 keys and a fixed
///     salt). Only used to migrate old project files.
/// </summary>
public static class LegacyAesCredentialDecryptor
{
    /// <summary>The passwords Administration 4.x used when credentials were saved with the project.</summary>
    public const string BuiltInKeyPassword = "VZh7mLRPNI";

    public const string BuiltInIvPassword = "cOijH2vgwR";

    private static readonly byte[] Salt = [0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84];

    public static string Decrypt(string base64CipherText, string keyPassword, string ivPassword)
    {
        ArgumentException.ThrowIfNullOrEmpty(base64CipherText);
        ArgumentException.ThrowIfNullOrEmpty(keyPassword);
        ArgumentException.ThrowIfNullOrEmpty(ivPassword);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = Derive(keyPassword, aes.KeySize / 8);
        aes.IV = Derive(ivPassword, aes.BlockSize / 8);

        using var decryptor = aes.CreateDecryptor();
        var plain = decryptor.TransformFinalBlock(Convert.FromBase64String(base64CipherText), 0,
            Convert.FromBase64String(base64CipherText).Length);
        return Encoding.UTF8.GetString(plain);
    }

    /// <summary>Encrypts the way the old Administration did. Exists so migration can be tested against real data.</summary>
    public static string Encrypt(string plainText, string keyPassword, string ivPassword)
    {
        ArgumentException.ThrowIfNullOrEmpty(plainText);
        ArgumentException.ThrowIfNullOrEmpty(keyPassword);
        ArgumentException.ThrowIfNullOrEmpty(ivPassword);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = Derive(keyPassword, aes.KeySize / 8);
        aes.IV = Derive(ivPassword, aes.BlockSize / 8);

        using var encryptor = aes.CreateEncryptor();
        var bytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(encryptor.TransformFinalBlock(bytes, 0, bytes.Length));
    }

    private static byte[] Derive(string password, int length)
    {
#pragma warning disable CA5379 // SHA1 with 1000 iterations is what the old format used; required for compatibility.
        return Rfc2898DeriveBytes.Pbkdf2(password, Salt, 1000, HashAlgorithmName.SHA1, length);
#pragma warning restore CA5379
    }
}
