using System.Security.Cryptography;
using System.Text;

namespace nUpdate.Administration.Core.Security;

/// <summary>
///     Encrypts data under a user-chosen password for project exports: PBKDF2-SHA256 (600000 iterations) derives the
///     key, AES-256-GCM encrypts. Layout: magic, version, salt, nonce, tag, ciphertext.
/// </summary>
public static class PasswordProtectedData
{
    public const int Iterations = 600_000;

    private const int SaltSize = 16;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const int KeySize = 32;
    private static readonly byte[] Magic = "NUPD"u8.ToArray();
    private const byte Version = 1;

    public static byte[] Encrypt(byte[] plainText, string password)
    {
        ArgumentNullException.ThrowIfNull(plainText);
        ArgumentException.ThrowIfNullOrEmpty(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var tag = new byte[TagSize];
        var cipherText = new byte[plainText.Length];
        using (var aes = new AesGcm(DeriveKey(password, salt), TagSize))
            aes.Encrypt(nonce, plainText, cipherText, tag);

        var output = new byte[Magic.Length + 1 + SaltSize + NonceSize + TagSize + cipherText.Length];
        var offset = 0;
        Magic.CopyTo(output, offset);
        offset += Magic.Length;
        output[offset++] = Version;
        salt.CopyTo(output, offset);
        offset += SaltSize;
        nonce.CopyTo(output, offset);
        offset += NonceSize;
        tag.CopyTo(output, offset);
        offset += TagSize;
        cipherText.CopyTo(output, offset);
        return output;
    }

    /// <exception cref="InvalidDataException">The data is not an export.</exception>
    /// <exception cref="CryptographicException">The password is wrong or the data was tampered with.</exception>
    public static byte[] Decrypt(byte[] data, string password)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentException.ThrowIfNullOrEmpty(password);

        var headerLength = Magic.Length + 1 + SaltSize + NonceSize + TagSize;
        if (data.Length < headerLength || !data.AsSpan(0, Magic.Length).SequenceEqual(Magic))
            throw new InvalidDataException("The data is not a password-protected nUpdate export.");
        if (data[Magic.Length] != Version)
            throw new InvalidDataException(
                $"The export uses format version {data[Magic.Length]}, which this version cannot read.");

        var offset = Magic.Length + 1;
        var salt = data.AsSpan(offset, SaltSize).ToArray();
        offset += SaltSize;
        var nonce = data.AsSpan(offset, NonceSize).ToArray();
        offset += NonceSize;
        var tag = data.AsSpan(offset, TagSize).ToArray();
        offset += TagSize;
        var cipherText = data.AsSpan(offset).ToArray();

        var plainText = new byte[cipherText.Length];
        using var aes = new AesGcm(DeriveKey(password, salt), TagSize);
        aes.Decrypt(nonce, cipherText, tag, plainText);
        return plainText;
    }

    private static byte[] DeriveKey(string password, byte[] salt) =>
        Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, Iterations, HashAlgorithmName.SHA256,
            KeySize);
}
