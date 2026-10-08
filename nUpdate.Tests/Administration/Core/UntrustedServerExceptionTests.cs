using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Administration.Core;

public class UntrustedServerExceptionTests
{
    [Fact]
    public void Constructor_KeepsMessageInnerExceptionFingerprintAndSubject()
    {
        var inner = new InvalidOperationException();
        new UntrustedServerException().Fingerprint.ShouldBeNull();
        new UntrustedServerException("m").Message.ShouldBe("m");
        new UntrustedServerException("m", inner).InnerException.ShouldBe(inner);
        var untrusted = new UntrustedServerException("m", "abc", "CN=x");
        untrusted.Fingerprint.ShouldBe("abc");
        untrusted.Subject.ShouldBe("CN=x");
    }
}
