using System.Security.Cryptography;
using nUpdate.Administration.Core.Security;

namespace nUpdate.Tests.Administration.Core;

public class PasswordProtectedDataTests
{
    [Fact]
    public void PasswordProtectedData_RoundTripsAndDetectsTampering()
    {
        var plain = new byte[1000];
        new Random(7).NextBytes(plain);
        var encrypted = PasswordProtectedData.Encrypt(plain, "pw");
        PasswordProtectedData.Decrypt(encrypted, "pw").ShouldBe(plain);
        Should.Throw<CryptographicException>(() => PasswordProtectedData.Decrypt(encrypted, "other"));

        var tampered = (byte[])encrypted.Clone();
        tampered[^1] ^= 1;
        Should.Throw<CryptographicException>(() => PasswordProtectedData.Decrypt(tampered, "pw"));

        Should.Throw<InvalidDataException>(() => PasswordProtectedData.Decrypt([1, 2, 3], "pw"));
        var wrongMagic = (byte[])encrypted.Clone();
        wrongMagic[0] = (byte)'X';
        Should.Throw<InvalidDataException>(() => PasswordProtectedData.Decrypt(wrongMagic, "pw"));
        var wrongVersion = (byte[])encrypted.Clone();
        wrongVersion[4] = 9;
        Should.Throw<InvalidDataException>(() => PasswordProtectedData.Decrypt(wrongVersion, "pw"));

        PasswordProtectedData.Encrypt([], "pw").Length.ShouldBe(4 + 1 + 16 + 12 + 16);
        Should.Throw<ArgumentNullException>(() => PasswordProtectedData.Encrypt(null!, "pw"));
        Should.Throw<ArgumentException>(() => PasswordProtectedData.Encrypt(plain, ""));
        Should.Throw<ArgumentNullException>(() => PasswordProtectedData.Decrypt(null!, "pw"));
        Should.Throw<ArgumentException>(() => PasswordProtectedData.Decrypt(encrypted, ""));
        PasswordProtectedData.Iterations.ShouldBe(600_000);
    }
}
