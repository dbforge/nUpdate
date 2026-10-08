using nUpdate.Exceptions;

namespace nUpdate.Tests.Library;

public class UnsupportedFormatExceptionTests
{
    [Fact]
    public void Constructor_AllOverloadsWork()
    {
        var inner = new InvalidOperationException("inner");
        new UnsupportedFormatException().Message.ShouldNotBeNull();
        new UnsupportedFormatException("m").Message.ShouldBe("m");
        new UnsupportedFormatException("m", inner).InnerException.ShouldBe(inner);
    }
}
