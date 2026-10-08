using System.Security.Cryptography;
using nUpdate.Administration.Core.Security;

namespace nUpdate.Tests.Administration.Core;

public class LegacyAesCredentialDecryptorTests
{
    [Fact]
    public void LegacyAesCredentialDecryptor_RoundTripsAndMatchesKnownVector()
    {
        var cipher = LegacyAesCredentialDecryptor.Encrypt("Pa$$w0rd", LegacyAesCredentialDecryptor.BuiltInKeyPassword, LegacyAesCredentialDecryptor.BuiltInIvPassword);
        LegacyAesCredentialDecryptor.Decrypt(cipher, LegacyAesCredentialDecryptor.BuiltInKeyPassword, LegacyAesCredentialDecryptor.BuiltInIvPassword).ShouldBe("Pa$$w0rd");
        // Independent vector: PBKDF2-HMAC-SHA1 (1000 iterations, the fixed salt) via Python hashlib, AES-256-CBC via OpenSSL, plaintext "hello".
        LegacyAesCredentialDecryptor.Decrypt("fVpqFtPz+9oiS5HxLBXMNA==", LegacyAesCredentialDecryptor.BuiltInKeyPassword, LegacyAesCredentialDecryptor.BuiltInIvPassword).ShouldBe("hello");
        Should.Throw<CryptographicException>(() => LegacyAesCredentialDecryptor.Decrypt(cipher, "wrong", "wrong"));
        Should.Throw<ArgumentException>(() => LegacyAesCredentialDecryptor.Decrypt("", "k", "i"));
        Should.Throw<ArgumentException>(() => LegacyAesCredentialDecryptor.Decrypt("x", "", "i"));
        Should.Throw<ArgumentException>(() => LegacyAesCredentialDecryptor.Decrypt("x", "k", ""));
        Should.Throw<ArgumentException>(() => LegacyAesCredentialDecryptor.Encrypt("", "k", "i"));
        Should.Throw<ArgumentException>(() => LegacyAesCredentialDecryptor.Encrypt("x", "", "i"));
        Should.Throw<ArgumentException>(() => LegacyAesCredentialDecryptor.Encrypt("x", "k", ""));
    }
}
