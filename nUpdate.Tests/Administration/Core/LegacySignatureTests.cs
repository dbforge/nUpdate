using nUpdate.Administration.Core.Migration;
using nUpdate.Tests.Support;
using static nUpdate.Tests.Administration.Support.LegacyTestData;

namespace nUpdate.Tests.Administration.Core;

public class LegacySignatureTests
{
    [Fact]
    public void Verify_AcceptsOnlyTheSignatureOfTheProjectKey()
    {
        var zip = LegacyZip(false);
        LegacySignature.Verify(new MemoryStream(zip), TestKeys.PublicKey, Sign(zip)).ShouldBeTrue();
        LegacySignature.Verify(new MemoryStream(LegacyZip(false, ("Program/x", "x"))), TestKeys.PublicKey, Sign(zip))
            .ShouldBeFalse();
        LegacySignature.Verify(new MemoryStream(zip), TestKeys.PublicKey, "not base64!").ShouldBeFalse();
        LegacySignature.Verify(new MemoryStream(zip), "not a key", Sign(zip)).ShouldBeFalse();
        LegacySignature.Verify(new MemoryStream(zip), TestKeys.PublicKey, null).ShouldBeFalse();
        LegacySignature.Verify(new MemoryStream(zip), TestKeys.PublicKey, "").ShouldBeFalse();
        Should.Throw<ArgumentNullException>(() => LegacySignature.Verify(null!, TestKeys.PublicKey, "x"));
        Should.Throw<ArgumentNullException>(() => LegacySignature.Verify(new MemoryStream(zip), null!, "x"));
    }
}
