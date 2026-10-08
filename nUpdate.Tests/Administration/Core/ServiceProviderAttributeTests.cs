using nUpdate.Administration.TransferInterface;
using nUpdate.Tests.Administration.Support;

namespace nUpdate.Tests.Administration.Core;

public class ServiceProviderAttributeTests
{
    [Fact]
    public void Constructor_AcceptsOnlyServiceProviderTypes()
    {
        new ServiceProviderAttribute(typeof(TestPluginProvider)).ServiceType.ShouldBe(typeof(TestPluginProvider));
        Should.Throw<ArgumentNullException>(() => new ServiceProviderAttribute(null!));
        Should.Throw<ArgumentException>(() => new ServiceProviderAttribute(typeof(string)));
    }
}
