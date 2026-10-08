namespace nUpdate.Administration.TransferInterface;

/// <summary>
///     Uploads, lists and deletes files on the update server. Remote paths are relative to the configured base
///     directory and use <c>/</c> as separator; an empty path means the base directory itself.
///     Implementations keep one connection open for their lifetime.
/// </summary>
public interface ITransferProvider : IAsyncDisposable
{
    /// <summary>Connects and authenticates. Throws <see cref="TransferException" /> on failure.</summary>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    Task<bool> FileExistsAsync(string remotePath, CancellationToken cancellationToken = default);

    Task<bool> DirectoryExistsAsync(string remotePath, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ServerItem>> ListAsync(string remotePath, bool recursive, CancellationToken cancellationToken = default);

    /// <summary>Creates the directory and any missing parents.</summary>
    Task CreateDirectoryAsync(string remotePath, CancellationToken cancellationToken = default);

    /// <summary>Deletes a file; a missing file is not an error.</summary>
    Task DeleteFileAsync(string remotePath, CancellationToken cancellationToken = default);

    /// <summary>Deletes a directory with everything below it; a missing directory is not an error.</summary>
    Task DeleteDirectoryAsync(string remotePath, CancellationToken cancellationToken = default);

    Task RenameAsync(string remotePath, string newRemotePath, CancellationToken cancellationToken = default);

    Task UploadFileAsync(string localPath, string remotePath, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);

    Task DownloadFileAsync(string remotePath, string localPath, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default);
}
