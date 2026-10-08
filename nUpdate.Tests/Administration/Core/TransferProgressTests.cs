using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Administration.Core;

public class TransferProgressTests
{
    [Fact]
    public void TransferProgress_ComputesThePercentageAndRejectsNegativeValues()
    {
        var progress = new TransferProgress(50, 200);
        progress.Percentage.ShouldBe(25);
        new TransferProgress(5, 0).Percentage.ShouldBe(0);
        new TransferProgress(300, 200).Percentage.ShouldBe(100);
        Should.Throw<ArgumentOutOfRangeException>(() => new TransferProgress(-1, 0));
        Should.Throw<ArgumentOutOfRangeException>(() => new TransferProgress(0, -1));
    }
}
