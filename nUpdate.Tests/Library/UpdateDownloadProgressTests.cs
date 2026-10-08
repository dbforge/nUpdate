using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class UpdateDownloadProgressTests
{
    [Fact]
    public void UpdateDownloadProgress_ComputesPercentage()
    {
        new UpdateDownloadProgress(50, 200).Percentage.ShouldBe(25f);
        new UpdateDownloadProgress(0, 0).Percentage.ShouldBe(0f);
        new UpdateDownloadProgress(300, 200).Percentage.ShouldBe(100f);
        new UpdateDownloadProgress(5, 10).BytesReceived.ShouldBe(5);
        new UpdateDownloadProgress(5, 10).TotalBytesToReceive.ShouldBe(10);
        Should.Throw<ArgumentOutOfRangeException>(() => new UpdateDownloadProgress(-1, 0));
        Should.Throw<ArgumentOutOfRangeException>(() => new UpdateDownloadProgress(0, -1));
    }
}
