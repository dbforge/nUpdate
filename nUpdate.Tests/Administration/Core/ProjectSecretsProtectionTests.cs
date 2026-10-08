using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Security;
using nUpdate.Administration.TransferInterface;
using nUpdate.Tests.Administration.Support;

namespace nUpdate.Tests.Administration.Core;

public class ProjectSecretsProtectionTests
{
    private readonly AdminTestContext _context = new();

    [Fact]
    public void ProjectSecretsProtection_ValidatesAndChecksCompleteness()
    {
        var project = _context.NewProject(statistics: true);
        ProjectSecretsProtection.IsComplete(project, new ProjectSecrets()).ShouldBeFalse();
        ProjectSecretsProtection.IsComplete(project, new ProjectSecrets { PrivateKey = "k" }).ShouldBeFalse();
        ProjectSecretsProtection.IsComplete(project, new ProjectSecrets { PrivateKey = "k", TransferPassword = "t" }).ShouldBeFalse();
        ProjectSecretsProtection.IsComplete(project, new ProjectSecrets { PrivateKey = "k", TransferPassword = "t", StatisticsAdminSecret = "a" }).ShouldBeTrue();
        project.Statistics.Enabled = false;
        ProjectSecretsProtection.IsComplete(project, new ProjectSecrets { PrivateKey = "k", TransferPassword = "t" }).ShouldBeTrue();
        project.Transfer.Protocol = TransferProtocol.Sftp;
        project.Transfer.SftpPrivateKeyPath = "/key";
        ProjectSecretsProtection.IsComplete(project, new ProjectSecrets { PrivateKey = "k" }).ShouldBeTrue();

        Should.Throw<ArgumentNullException>(() => ProjectSecretsProtection.IsComplete(null!, new ProjectSecrets()));
        Should.Throw<ArgumentNullException>(() => ProjectSecretsProtection.IsComplete(project, null!));
        Should.Throw<ArgumentNullException>(() => ProjectSecretsProtection.Protect(null!, "pw"));
        Should.Throw<ArgumentException>(() => ProjectSecretsProtection.Protect(new ProjectSecrets(), ""));
        Should.Throw<ArgumentException>(() => ProjectSecretsProtection.Unprotect("", "pw"));
        Should.Throw<ArgumentException>(() => ProjectSecretsProtection.Unprotect("x", ""));
        Should.Throw<InvalidDataException>(() => ProjectSecretsProtection.Unprotect("!!", "pw"));
        var notJson = Convert.ToBase64String(PasswordProtectedData.Encrypt("{broken"u8.ToArray(), "pw"));
        Should.Throw<InvalidDataException>(() => ProjectSecretsProtection.Unprotect(notJson, "pw"));
        var nullJson = Convert.ToBase64String(PasswordProtectedData.Encrypt("null"u8.ToArray(), "pw"));
        ProjectSecretsProtection.Unprotect(nullJson, "pw").PrivateKey.ShouldBeNull();
    }
}
