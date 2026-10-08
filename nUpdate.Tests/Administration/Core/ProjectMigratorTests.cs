using Newtonsoft.Json.Linq;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.TransferInterface;
using nUpdate.Tests.Administration.Support;

namespace nUpdate.Tests.Administration.Core;

public class ProjectMigratorTests
{
    private readonly AdminTestContext _context = new();

    [Fact]
    public void FromV3_HandlesSparseLegacyFiles()
    {
        var project = ProjectMigrator.FromV3(JObject.Parse("""{"Name":"Sparse","FtpProtocol":4,"FtpTransferAssemblyFilePath":"C:\\plugin.dll","UseStatistics":false,"Log":[{"Entry":3}]}"""), null).Project;
        project.Id.ShouldNotBe(Guid.Empty);
        project.Transfer.Protocol.ShouldBe(TransferProtocol.Plugin);
        project.Transfer.PluginAssemblyPath.ShouldBe("C:\\plugin.dll");
        project.Transfer.Proxy.ShouldBeNull();
        project.HttpAuthentication.ShouldBeNull();
        project.Statistics.Enabled.ShouldBeFalse();
        project.Statistics.Database.ShouldBeNull();
        project.Packages.ShouldBeEmpty();
        project.Log.Single().Kind.ShouldBe(LogEntryKind.Edit);
        project.PublicKey.ShouldBe("");

        ProjectMigrator.FromV3(JObject.Parse("""{"FtpProtocol":5}"""), null).Project.Transfer.Protocol.ShouldBe(TransferProtocol.FtpsImplicit);

        var sparseStatistics = ProjectMigrator.FromV3(JObject.Parse("""{"UseStatistics":true,"UpdateUrl":"not a url","Packages":[{}],"Log":[{},{"Entry":"Bogus"},{"Entry":"upload"}]}"""), null);
        sparseStatistics.Project.Statistics.Enabled.ShouldBeTrue();
        sparseStatistics.Project.Statistics.EndpointUrl.ShouldBeNull();
        sparseStatistics.Project.Statistics.Database!.Host.ShouldBe("localhost");
        sparseStatistics.Project.Statistics.Database.Name.ShouldBe("");
        sparseStatistics.Project.Statistics.Database.Username.ShouldBe("");
        sparseStatistics.Project.Packages.ShouldBeEmpty();
        sparseStatistics.Project.Log.Select(l => l.Kind).ShouldBe([LogEntryKind.Edit, LogEntryKind.Edit, LogEntryKind.Upload]);
        sparseStatistics.Secrets.StatisticsDatabasePassword.ShouldBeNull();
        sparseStatistics.Secrets.PrivateKey.ShouldBeNull();
        ProjectMigrator.FromV3(JObject.Parse("""{"FtpProtocol":0}"""), null).Project.Transfer.Protocol.ShouldBe(TransferProtocol.Ftp);
        ProjectMigrator.FromV3(JObject.Parse("""{"HttpAuthenticationCredentials":{"UserName":""}}"""), null).Project.HttpAuthentication.ShouldBeNull();
        Should.Throw<ArgumentNullException>(() => ProjectMigrator.FromV3(null!, null));
        Should.Throw<ArgumentNullException>(() => new ProjectMigrator(_context.Protector).FromV5(null!));
        Should.Throw<ArgumentNullException>(() => new ProjectMigrator(null!));
        ProjectMigrator.ConvertKey(" ", isPrivate: true).ShouldBeNull();
        ProjectMigrator.ConvertKey("plain", isPrivate: false).ShouldBe("plain");
        Should.Throw<ArgumentException>(() => ProjectMigrator.ConvertKey("<nope>", isPrivate: false));
    }
}
