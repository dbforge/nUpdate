using nUpdate.Administration.Core;
using nUpdate.Tests.Administration.Support;

namespace nUpdate.Tests.Administration.Core;

public class ProjectHttpClientFactoryTests
{
    private readonly AdminTestContext _context = new();

    [Fact]
    public void Create_ConfiguresClients()
    {
        var project = _context.NewProject();
        var secrets = AdminTestContext.NewSecrets();
        using var stubbed = _context.HttpClientFactory.Create(project, secrets);
        stubbed.DefaultRequestHeaders.UserAgent.ToString().ShouldStartWith("nUpdate.Administration/");
        stubbed.Timeout.ShouldBe(TimeSpan.FromSeconds(60));

        var real = new ProjectHttpClientFactory();
        using var plain = real.Create(project, secrets);
        plain.ShouldNotBeNull();
        project.Transfer.Proxy = new nUpdate.Administration.TransferInterface.ProxySettings { Address = "http://proxy:8080", Username = "pu" };
        project.HttpAuthentication = new nUpdate.Administration.Core.Models.HttpAuthenticationSettings { Username = "web" };
        using var configured = real.Create(project, secrets);
        configured.ShouldNotBeNull();
        project.Transfer.Proxy.Username = null;
        using var proxyWithoutUser = real.Create(project, secrets);
        proxyWithoutUser.ShouldNotBeNull();

        Should.Throw<ArgumentNullException>(() => new ProjectHttpClientFactory(null!));
        Should.Throw<ArgumentNullException>(() => real.Create(null!, secrets));
        Should.Throw<ArgumentNullException>(() => real.Create(project, null!));
    }
}
