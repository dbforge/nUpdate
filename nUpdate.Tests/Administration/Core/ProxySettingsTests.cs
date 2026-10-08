using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Administration.Core;

public class ProxySettingsTests
{
    [Fact]
    public void Username_DefaultsToNull()
    {
        new ProxySettings().Username.ShouldBeNull();
    }
}
