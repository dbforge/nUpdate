using nUpdate.Administration.Core.Transfer;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Administration.Core;

public class ProxyEndpointTests
{
    [Theory]
    [InlineData("http://proxy.example.com:3128", "proxy.example.com", 3128)]
    [InlineData("proxy:8081", "proxy", 8081)]
    [InlineData("http://proxy.example.com", "proxy.example.com", 8080)]
    [InlineData("http://proxy.example.com:80", "proxy.example.com", 80)]
    [InlineData(" socks5://10.0.0.1:1080 ", "10.0.0.1", 1080)]
    public void FromSettings_ParsesAddresses(string address, string host, int port)
    {
        var endpoint = ProxyEndpoint.FromSettings(new ProxySettings { Address = address })!;
        endpoint.Host.ShouldBe(host);
        endpoint.Port.ShouldBe(port);
    }

    [Fact]
    public void FromSettings_IsNullWithoutAProxyAndRejectsGarbage()
    {
        ProxyEndpoint.FromSettings(null).ShouldBeNull();
        ProxyEndpoint.FromSettings(new ProxySettings { Address = " " }).ShouldBeNull();
        Should.Throw<ArgumentException>(() => ProxyEndpoint.FromSettings(new ProxySettings { Address = "http://" }));
        Should.Throw<ArgumentException>(() => new ProxyEndpoint(" ", 1));
        Should.Throw<ArgumentOutOfRangeException>(() => new ProxyEndpoint("h", 0));
        Should.Throw<ArgumentOutOfRangeException>(() => new ProxyEndpoint("h", 70000));
    }
}
