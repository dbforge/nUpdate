namespace nUpdate.Updating;

/// <summary>Progress of the package download across all packages.</summary>
public sealed class UpdateDownloadProgress
{
    public UpdateDownloadProgress(long bytesReceived, long totalBytesToReceive)
    {
        if (bytesReceived < 0)
            throw new ArgumentOutOfRangeException(nameof(bytesReceived));
        if (totalBytesToReceive < 0)
            throw new ArgumentOutOfRangeException(nameof(totalBytesToReceive));
        BytesReceived = bytesReceived;
        TotalBytesToReceive = totalBytesToReceive;
    }

    public long BytesReceived { get; }

    public long TotalBytesToReceive { get; }

    /// <summary>0 to 100. Zero when the total is unknown.</summary>
    public float Percentage => TotalBytesToReceive == 0 ? 0 : Math.Min(100f, (float)(100.0 * BytesReceived / TotalBytesToReceive));
}
