namespace nUpdate.Administration.TransferInterface;

/// <summary>Progress of a single file transfer.</summary>
public sealed class TransferProgress
{
    public TransferProgress(long bytesTransferred, long totalBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bytesTransferred);
        ArgumentOutOfRangeException.ThrowIfNegative(totalBytes);
        BytesTransferred = bytesTransferred;
        TotalBytes = totalBytes;
    }

    public long BytesTransferred { get; }

    public long TotalBytes { get; }

    /// <summary>0 to 100; 0 when the total is unknown.</summary>
    public double Percentage => TotalBytes == 0 ? 0 : Math.Min(100, 100.0 * BytesTransferred / TotalBytes);
}
