using nUpdate.Administration.Core.Security;

namespace nUpdate.Tests.Administration.Core;

public class SecretGeneratorTests
{
    [Fact]
    public void SecretGenerator_CreatesUrlSafeSecretsAndHashes()
    {
        var secret = SecretGenerator.CreateSecret();
        secret.Length.ShouldBeGreaterThanOrEqualTo(40);
        secret.ShouldNotContain("+");
        secret.ShouldNotContain("/");
        secret.ShouldNotContain("=");
        SecretGenerator.CreateSecret().ShouldNotBe(secret);
        SecretGenerator.HashSecret("abc").ShouldBe("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad");
        Should.Throw<ArgumentException>(() => SecretGenerator.HashSecret(""));
    }
}
