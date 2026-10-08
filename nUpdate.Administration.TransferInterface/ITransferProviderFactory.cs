namespace nUpdate.Administration.TransferInterface;

/// <summary>Creates transfer providers for a project's settings. Plugins export one through <see cref="ServiceProviderAttribute" />.</summary>
public interface ITransferProviderFactory
{
    /// <summary>The protocols this factory supports.</summary>
    IReadOnlyCollection<TransferProtocol> SupportedProtocols { get; }

    ITransferProvider Create(TransferSettings settings, TransferCredentials credentials);
}
