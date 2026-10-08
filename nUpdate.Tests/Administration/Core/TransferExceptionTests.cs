using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Administration.Core;

public class TransferExceptionTests
{
    [Fact]
    public void Constructor_KeepsMessageAndInnerException()
    {
        var inner = new InvalidOperationException();
        new TransferException().ShouldNotBeNull();
        new TransferException("m").Message.ShouldBe("m");
        new TransferException("m", inner).InnerException.ShouldBe(inner);
    }
}
