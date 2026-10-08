using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Administration.Support;

/// <summary>A transfer plugin: the test assembly names <see cref="TestPluginProvider" /> in its <see cref="ServiceProviderAttribute" />.</summary>
public sealed class TestPluginProvider : IServiceProvider
{
    public object? GetService(Type serviceType) => serviceType == typeof(ITransferProviderFactory) ? new TestPluginFactory() : null;
}

public sealed class TestPluginFactory : ITransferProviderFactory
{
    public IReadOnlyCollection<TransferProtocol> SupportedProtocols { get; } = [TransferProtocol.Plugin];

    public ITransferProvider Create(TransferSettings settings, TransferCredentials credentials) => Substitute.For<ITransferProvider>();
}
