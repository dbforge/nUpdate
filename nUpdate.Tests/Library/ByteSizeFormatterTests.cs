using System.Globalization;
using nUpdate.Ui;

namespace nUpdate.Tests.Library;

public class ByteSizeFormatterTests
{
    [Theory]
    [InlineData(0, "0 bytes")]
    [InlineData(1023, "1023 bytes")]
    [InlineData(1024, "1.00 KB")]
    [InlineData(1536, "1.50 KB")]
    [InlineData(10 * 1024, "10.0 KB")]
    [InlineData(100 * 1024, "100 KB")]
    [InlineData(5 * 1024 * 1024, "5.00 MB")]
    [InlineData(3L * 1024 * 1024 * 1024, "3.00 GB")]
    [InlineData(2L * 1024 * 1024 * 1024 * 1024, "2.00 TB")]
    [InlineData(7L * 1024 * 1024 * 1024 * 1024 * 1024, "7.00 PB")]
    [InlineData(long.MaxValue, "8192 PB")]
    public void Format_IsHumanReadable(long bytes, string expected)
    {
        ByteSizeFormatter.Format(bytes).ShouldBe(expected);
    }

    [Fact]
    public void Format_UsesCultureAndRejectsNegative()
    {
        ByteSizeFormatter.Format(1536, new CultureInfo("de-DE")).ShouldBe("1,50 KB");
        Should.Throw<ArgumentOutOfRangeException>(() => ByteSizeFormatter.Format(-1));
    }
}
