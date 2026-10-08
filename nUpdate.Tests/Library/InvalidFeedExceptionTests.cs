using nUpdate.Exceptions;

namespace nUpdate.Tests.Library;

public class InvalidFeedExceptionTests
{
    [Fact]
    public void Constructor_AllOverloadsWork()
    {
        var inner = new InvalidOperationException("inner");
        new InvalidFeedException().Message.ShouldNotBeNull();
        new InvalidFeedException("m").Message.ShouldBe("m");
        new InvalidFeedException("m", inner).InnerException.ShouldBe(inner);
    }
}
