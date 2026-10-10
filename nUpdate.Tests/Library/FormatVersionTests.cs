namespace nUpdate.Tests.Library;

public class FormatVersionTests
{
    [Fact]
    public void Check_AcceptsOnlyTheCurrentFormat()
    {
        FormatVersion.Check(2, 2, "test document");
        Should.Throw<Exceptions.UnsupportedFormatException>(() => FormatVersion.Check(3, 2, "test document")).Message
            .ShouldContain("newer");
        Should.Throw<Exceptions.UnsupportedFormatException>(() => FormatVersion.Check(1, 2, "test document")).Message
            .ShouldContain("older");
    }
}
