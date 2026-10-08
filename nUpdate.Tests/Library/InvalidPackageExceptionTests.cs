using nUpdate.Exceptions;

namespace nUpdate.Tests.Library;

public class InvalidPackageExceptionTests
{
    [Fact]
    public void Constructor_AllOverloadsWork()
    {
        var inner = new InvalidOperationException("inner");
        new InvalidPackageException().Message.ShouldNotBeNull();
        new InvalidPackageException("m").Message.ShouldBe("m");
        new InvalidPackageException("m", inner).InnerException.ShouldBe(inner);
    }
}
