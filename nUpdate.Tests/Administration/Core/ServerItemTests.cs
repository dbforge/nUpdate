using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Administration.Core;

public class ServerItemTests
{
    [Fact]
    public void Constructor_KeepsTheValuesAndRejectsNull()
    {
        var item = new ServerItem("a", "/x/a", 3, DateTimeOffset.UnixEpoch, ServerItemType.File);
        item.Name.ShouldBe("a");
        item.FullPath.ShouldBe("/x/a");
        item.Size.ShouldBe(3);
        item.Modified.ShouldBe(DateTimeOffset.UnixEpoch);
        item.ItemType.ShouldBe(ServerItemType.File);
        Should.Throw<ArgumentNullException>(() => new ServerItem(null!, "/x", 0, null, ServerItemType.Other));
        Should.Throw<ArgumentNullException>(() => new ServerItem("a", null!, 0, null, ServerItemType.Other));
    }
}
