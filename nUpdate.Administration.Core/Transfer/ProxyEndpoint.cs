using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Core.Transfer;

/// <summary>The host and port of an HTTP proxy, parsed from <see cref="ProxySettings.Address" /> ("http://proxy:8080" or "proxy:8080").</summary>
public sealed class ProxyEndpoint
{
    private const int DefaultPort = 8080;

    public ProxyEndpoint(string host, int port)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        if (port is < 1 or > 65535)
            throw new ArgumentOutOfRangeException(nameof(port));
        Host = host;
        Port = port;
    }

    public string Host { get; }

    public int Port { get; }

    /// <returns><c>null</c> when no proxy is configured.</returns>
    /// <exception cref="ArgumentException">The address cannot be parsed.</exception>
    public static ProxyEndpoint? FromSettings(ProxySettings? settings)
    {
        if (settings is null || string.IsNullOrWhiteSpace(settings.Address))
            return null;

        var address = settings.Address.Trim();
        if (!address.Contains("://", StringComparison.Ordinal))
            address = "http://" + address;
        if (!Uri.TryCreate(address, UriKind.Absolute, out var uri) || string.IsNullOrEmpty(uri.Host))
            throw new ArgumentException($"\"{settings.Address}\" is not a valid proxy address.", nameof(settings));
        return new ProxyEndpoint(uri.Host, uri.IsDefaultPort && !address.EndsWith(":80", StringComparison.Ordinal) ? DefaultPort : uri.Port);
    }
}
