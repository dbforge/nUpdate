using nUpdate.Administration.Core.Models;
using nUpdate.Tests.Administration.Support;

namespace nUpdate.Tests.Administration.Core;

public class ProjectSecretsTests
{
    [Fact]
    public void ToTransferCredentials_CopiesTheTransferSecrets()
    {
        var credentials = new ProjectSecrets { TransferPassword = "p", SftpKeyPassphrase = "k", ProxyPassword = "x" }.ToTransferCredentials();
        credentials.Password.ShouldBe("p");
        credentials.SftpKeyPassphrase.ShouldBe("k");
        credentials.ProxyPassword.ShouldBe("x");
    }

    [Fact]
    public void CloneAndCopyTo_CopyEverySecret()
    {
        var secrets = AdminTestContext.NewSecrets(statistics: true);
        secrets.HttpAuthenticationPassword = "hp";
        secrets.ProxyPassword = "pp";
        secrets.SftpKeyPassphrase = "kp";
        var clone = secrets.Clone();
        clone.ShouldNotBeSameAs(secrets);
        clone.TransferPassword.ShouldBe(secrets.TransferPassword);

        var target = new ProjectSecrets();
        secrets.CopyTo(target);
        target.TransferPassword.ShouldBe(secrets.TransferPassword);
        target.SftpKeyPassphrase.ShouldBe("kp");
        target.ProxyPassword.ShouldBe("pp");
        target.HttpAuthenticationPassword.ShouldBe("hp");
        target.StatisticsAdminSecret.ShouldBe(secrets.StatisticsAdminSecret);
        target.StatisticsDatabasePassword.ShouldBe(secrets.StatisticsDatabasePassword);
        target.PrivateKey.ShouldBe(secrets.PrivateKey);
        Should.Throw<ArgumentNullException>(() => secrets.CopyTo(null!));
    }
}
