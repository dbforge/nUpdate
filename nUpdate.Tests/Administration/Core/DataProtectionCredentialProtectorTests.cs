using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using nUpdate.Administration.Core.Security;

namespace nUpdate.Tests.Administration.Core;

public class DataProtectionCredentialProtectorTests
{
    [Fact]
    public void DataProtectionCredentialProtector_RoundTripsAndRejectsForeignData()
    {
        var protector = new DataProtectionCredentialProtector(new EphemeralDataProtectionProvider());
        var protectedText = protector.Protect("secret");
        protectedText.ShouldNotBe("secret");
        protector.Unprotect(protectedText).ShouldBe("secret");
        Should.Throw<CryptographicException>(() => new DataProtectionCredentialProtector(new EphemeralDataProtectionProvider()).Unprotect(protectedText));
        Should.Throw<ArgumentNullException>(() => protector.Protect(null!));
        Should.Throw<ArgumentNullException>(() => protector.Unprotect(null!));
        Should.Throw<ArgumentNullException>(() => new DataProtectionCredentialProtector(null!));
    }

    [Fact]
    public void CreateForDirectory_PersistsKeys()
    {
        var directory = Path.Combine(Path.GetTempPath(), "nupdate-test-keys-" + Guid.NewGuid().ToString("N"));
        try
        {
            var protectedText = DataProtectionCredentialProtector.CreateForDirectory(directory).Protect("x");
            DataProtectionCredentialProtector.CreateForDirectory(directory).Unprotect(protectedText).ShouldBe("x");
            Directory.GetFiles(directory).ShouldNotBeEmpty();
        }
        finally
        {
            Directory.Delete(directory, true);
        }

        Should.Throw<ArgumentException>(() => DataProtectionCredentialProtector.CreateForDirectory(" "));
    }
}
