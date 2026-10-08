namespace nUpdate.Administration.TransferInterface;

/// <summary>An HTTP proxy for transfers and HTTP requests.</summary>
public sealed class ProxySettings
{
    public string Address { get; set; } = string.Empty;

    public string? Username { get; set; }
}
