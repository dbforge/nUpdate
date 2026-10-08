namespace nUpdate.Administration.TransferInterface;

/// <summary>A file or directory on the server.</summary>
public sealed class ServerItem
{
    public ServerItem(string name, string fullPath, long size, DateTimeOffset? modified, ServerItemType itemType)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        FullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
        Size = size;
        Modified = modified;
        ItemType = itemType;
    }

    public string Name { get; }

    public string FullPath { get; }

    public long Size { get; }

    public DateTimeOffset? Modified { get; }

    public ServerItemType ItemType { get; }
}

public enum ServerItemType
{
    Directory,
    File,
    Other,
}
