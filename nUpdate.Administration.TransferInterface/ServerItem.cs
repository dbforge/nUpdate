namespace nUpdate.Administration.TransferInterface;

/// <summary>A file or directory on the server.</summary>
public sealed class ServerItem(
    string name,
    string fullPath,
    long size,
    DateTimeOffset? modified,
    ServerItemType itemType)
{
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    public string FullPath { get; } = fullPath ?? throw new ArgumentNullException(nameof(fullPath));

    public long Size { get; } = size;

    public DateTimeOffset? Modified { get; } = modified;

    public ServerItemType ItemType { get; } = itemType;
}

public enum ServerItemType
{
    Directory,
    File,
    Other,
}
