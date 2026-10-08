using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using nUpdate.Administration.TransferInterface;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace nUpdate.Administration.Core.Transfer;

/// <summary>SFTP through SSH.NET, optionally through an HTTP proxy. Covered by the integration tests against a real server.</summary>
[ExcludeFromCodeCoverage] // Exercised by nUpdate.Administration.IntegrationTest against an SFTP container; the unit suite cannot reach it.
public sealed class SftpTransferProvider : ITransferProvider
{
    private readonly TransferSettings _settings;
    private readonly TransferCredentials _credentials;
    private readonly IFileSystem _fileSystem;
    private SftpClient? _client;

    public SftpTransferProvider(TransferSettings settings, TransferCredentials credentials, IFileSystem fileSystem)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    private SftpClient Client => _client ?? throw new InvalidOperationException("The SFTP provider is not connected.");

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _client ??= CreateClient();
            await _client.ConnectAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (SshAuthenticationException ex)
        {
            throw new TransferException($"The SFTP server \"{_settings.Host}\" rejected the credentials.", ex);
        }
        catch (Exception ex) when (ex is SshException or System.Net.Sockets.SocketException or IOException or InvalidOperationException)
        {
            throw new TransferException($"The SFTP server \"{_settings.Host}:{_settings.Port}\" could not be reached: {ex.Message}", ex);
        }
    }

    public Task<bool> FileExistsAsync(string remotePath, CancellationToken cancellationToken = default) =>
        Guard(async () =>
        {
            var path = Absolute(remotePath);
            if (!await Client.ExistsAsync(path, cancellationToken).ConfigureAwait(false))
                return false;
            return (await Client.GetAsync(path, cancellationToken).ConfigureAwait(false)).IsRegularFile;
        });

    public Task<bool> DirectoryExistsAsync(string remotePath, CancellationToken cancellationToken = default) =>
        Guard(async () =>
        {
            var path = Absolute(remotePath);
            if (!await Client.ExistsAsync(path, cancellationToken).ConfigureAwait(false))
                return false;
            return (await Client.GetAsync(path, cancellationToken).ConfigureAwait(false)).IsDirectory;
        });

    public Task<IReadOnlyList<ServerItem>> ListAsync(string remotePath, bool recursive, CancellationToken cancellationToken = default) =>
        Guard<IReadOnlyList<ServerItem>>(async () =>
        {
            var result = new List<ServerItem>();
            await ListInto(Absolute(remotePath), recursive, result, cancellationToken).ConfigureAwait(false);
            return result;
        });

    public Task CreateDirectoryAsync(string remotePath, CancellationToken cancellationToken = default) =>
        Guard(async () =>
        {
            var path = Absolute(remotePath);
            var current = string.Empty;
            foreach (var part in path.Split('/', StringSplitOptions.RemoveEmptyEntries))
            {
                current += "/" + part;
                if (!await Client.ExistsAsync(current, cancellationToken).ConfigureAwait(false))
                    await Client.CreateDirectoryAsync(current, cancellationToken).ConfigureAwait(false);
            }
        });

    public Task DeleteFileAsync(string remotePath, CancellationToken cancellationToken = default) =>
        Guard(async () =>
        {
            var path = Absolute(remotePath);
            if (await Client.ExistsAsync(path, cancellationToken).ConfigureAwait(false))
                await Client.DeleteFileAsync(path, cancellationToken).ConfigureAwait(false);
        });

    public Task DeleteDirectoryAsync(string remotePath, CancellationToken cancellationToken = default) =>
        Guard(() => DeleteRecursively(Absolute(remotePath), cancellationToken));

    public Task RenameAsync(string remotePath, string newRemotePath, CancellationToken cancellationToken = default) =>
        Guard(() => Client.RenameFileAsync(Absolute(remotePath), Absolute(newRemotePath), cancellationToken));

    public Task UploadFileAsync(string localPath, string remotePath, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default) =>
        Guard(async () =>
        {
            var total = _fileSystem.FileInfo.New(localPath).Length;
            using var input = _fileSystem.File.OpenRead(localPath);
            var report = progress is null ? null : new Progress<UploadFileProgressReport>(r => progress.Report(new TransferProgress((long)r.TotalBytesUploaded, total)));
            await Client.UploadFileAsync(input, Absolute(remotePath), report, cancellationToken).ConfigureAwait(false);
        });

    public Task DownloadFileAsync(string remotePath, string localPath, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default) =>
        Guard(async () =>
        {
            var path = Absolute(remotePath);
            var total = (await Client.GetAsync(path, cancellationToken).ConfigureAwait(false)).Length;
            using var output = _fileSystem.File.Create(localPath);
            var report = progress is null ? null : new Progress<DownloadFileProgressReport>(r => progress.Report(new TransferProgress((long)r.TotalBytesDownloaded, total)));
            await Client.DownloadFileAsync(path, output, report, cancellationToken).ConfigureAwait(false);
        });

    public ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            if (_client.IsConnected)
                _client.Disconnect();
            _client.Dispose();
            _client = null;
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>Builds the client; reading the key file happens here so that its errors surface as transfer errors.</summary>
    private SftpClient CreateClient()
    {
        var methods = new List<AuthenticationMethod>();
        if (!string.IsNullOrEmpty(_settings.SftpPrivateKeyPath))
        {
            PrivateKeyFile keyFile;
            try
            {
                using var keyStream = _fileSystem.File.OpenRead(_settings.SftpPrivateKeyPath);
                keyFile = string.IsNullOrEmpty(_credentials.SftpKeyPassphrase) ? new PrivateKeyFile(keyStream) : new PrivateKeyFile(keyStream, _credentials.SftpKeyPassphrase);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SshException or InvalidOperationException)
            {
                throw new TransferException($"The private key \"{_settings.SftpPrivateKeyPath}\" could not be read: {ex.Message}", ex);
            }

            methods.Add(new PrivateKeyAuthenticationMethod(_settings.Username, keyFile));
        }

        if (!string.IsNullOrEmpty(_credentials.Password))
            methods.Add(new PasswordAuthenticationMethod(_settings.Username, _credentials.Password));
        if (methods.Count == 0)
            methods.Add(new PasswordAuthenticationMethod(_settings.Username, string.Empty));

        var proxy = ProxyEndpoint.FromSettings(_settings.Proxy);
        var connection = proxy is null
            ? new ConnectionInfo(_settings.Host, _settings.Port, _settings.Username, [.. methods])
            : new ConnectionInfo(_settings.Host, _settings.Port, _settings.Username, ProxyTypes.Http, proxy.Host, proxy.Port,
                _settings.Proxy!.Username ?? string.Empty, _credentials.ProxyPassword ?? string.Empty, [.. methods]);
        connection.Timeout = TimeSpan.FromSeconds(30);
        var client = new SftpClient(connection);
        client.HostKeyReceived += OnHostKeyReceived;
        return client;
    }

    private static bool IsTransferError(Exception ex) => ex is SshException or IOException or System.Net.Sockets.SocketException;

    private static Task Guard(Func<Task> action) => TransferGuard.RunAsync(action, IsTransferError, ex => ex.Message);

    private static Task<T> Guard<T>(Func<Task<T>> action) => TransferGuard.RunAsync(action, IsTransferError, ex => ex.Message);

    private string Absolute(string remotePath) => RemotePath.Combine(_settings.Directory, remotePath);

    private async Task ListInto(string path, bool recursive, List<ServerItem> result, CancellationToken cancellationToken)
    {
        await foreach (var item in Client.ListDirectoryAsync(path, cancellationToken).ConfigureAwait(false))
        {
            if (item.Name is "." or "..")
                continue;
            var type = item.IsDirectory ? ServerItemType.Directory : item.IsRegularFile ? ServerItemType.File : ServerItemType.Other;
            result.Add(new ServerItem(item.Name, item.FullName, item.Length, item.LastWriteTimeUtc, type));
            if (recursive && item.IsDirectory)
                await ListInto(item.FullName, true, result, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task DeleteRecursively(string path, CancellationToken cancellationToken)
    {
        if (!await Client.ExistsAsync(path, cancellationToken).ConfigureAwait(false))
            return;
        await foreach (var item in Client.ListDirectoryAsync(path, cancellationToken).ConfigureAwait(false))
        {
            if (item.Name is "." or "..")
                continue;
            if (item.IsDirectory)
                await DeleteRecursively(item.FullName, cancellationToken).ConfigureAwait(false);
            else
                await Client.DeleteFileAsync(item.FullName, cancellationToken).ConfigureAwait(false);
        }

        await Client.DeleteDirectoryAsync(path, cancellationToken).ConfigureAwait(false);
    }

    private void OnHostKeyReceived(object? sender, HostKeyEventArgs e)
    {
        var fingerprint = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(e.HostKey)).ToLowerInvariant();
        if (!string.IsNullOrEmpty(_settings.TrustedHostKeyFingerprint))
        {
            e.CanTrust = string.Equals(fingerprint, _settings.TrustedHostKeyFingerprint, StringComparison.OrdinalIgnoreCase);
            if (!e.CanTrust)
                throw new UntrustedServerException($"The host key of \"{_settings.Host}\" changed.", fingerprint, e.HostKeyName);
            return;
        }

        e.CanTrust = false;
        throw new UntrustedServerException($"The host key of \"{_settings.Host}\" is not trusted yet.", fingerprint, e.HostKeyName);
    }
}
