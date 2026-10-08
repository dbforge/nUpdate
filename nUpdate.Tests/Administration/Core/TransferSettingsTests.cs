using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Administration.Core;

public class TransferSettingsTests
{
    [Fact]
    public void TransferSettings_DefaultsToPassiveSftpOnPort22()
    {
        new TransferSettings().Protocol.ShouldBe(TransferProtocol.Sftp);
        new TransferSettings().Port.ShouldBe(22);
        new TransferSettings().UsePassiveMode.ShouldBeTrue();
    }
}
