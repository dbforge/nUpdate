using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using FluentFTP;
using FluentFTP.Exceptions;
using FluentFTP.Proxy.AsyncProxy;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Core.Transfer;

/// <summary>FTP and FTPS through FluentFTP, optionally through an HTTP proxy. Covered by the integration tests against a real server.</summary>
[ExcludeFromCodeCoverage] // Exercised by nUpdate.Administration.IntegrationTest against pure-ftpd; the unit suite cannot reach it.
public sealed class FtpTransferProvider : ITransferProvider
{
    private readonly TransferSettings _settings;
    private readonly IFileSystem _fileSystem;
    private readonly AsyncFtpClient _client;

    public FtpTransferProvider(TransferSettings settings, TransferCredentials credentials, IFileSystem fileSystem)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        ArgumentNullException.ThrowIfNull(credentials);
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

        var ftpCredentials = new NetworkCredential(settings.Username, credentials.Password ?? string.Empty);
        var proxy = ProxyEndpoint.FromSettings(settings.Proxy);
        _client = proxy is null
            ? new AsyncFtpClient(settings.Host, ftpCredentials, settings.Port)
            : new AsyncFtpClientHttp11Proxy(new FtpProxyProfile
            {
                ProxyHost = proxy.Host,
                ProxyPort = proxy.Port,
                ProxyCredentials = string.IsNullOrEmpty(settings.Proxy!.Username) ? null : new NetworkCredential(settings.Proxy.Username, credentials.ProxyPassword ?? string.Empty),
                FtpHost = settings.Host,
                FtpPort = settings.Port,
                FtpCredentials = ftpCredentials,
            });
        _client.Config.EncryptionMode = settings.Protocol switch
        {
            TransferProtocol.FtpsExplicit => FtpEncryptionMode.Explicit,
            TransferProtocol.FtpsImplicit => FtpEncryptionMode.Implicit,
            _ => FtpEncryptionMode.None,
        };
        _client.Config.DataConnectionType = settings.UsePassiveMode ? FtpDataConnectionType.AutoPassive : FtpDataConnectionType.AutoActive;
        _client.Config.ValidateAnyCertificate = false;
        _client.Config.RetryAttempts = 2;
        _client.ValidateCertificate += OnValidateCertificate;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.Connect(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (IsTransferError(ex))
        {
            if (ex is FtpAuthenticationException)
                throw new TransferException($"The FTP server \"{_settings.Host}\" rejected the credentials.", ex);
            throw new TransferException($"The FTP server \"{_settings.Host}:{_settings.Port}\" could not be reached: {ex.Message}", ex);
        }
    }

    public Task<bool> FileExistsAsync(string remotePath, CancellationToken cancellationToken = default) =>
        Guard(() => _client.FileExists(Absolute(remotePath), cancellationToken));

    public Task<bool> DirectoryExistsAsync(string remotePath, CancellationToken cancellationToken = default) =>
        Guard(() => _client.DirectoryExists(Absolute(remotePath), cancellationToken));

    public Task<IReadOnlyList<ServerItem>> ListAsync(string remotePath, bool recursive, CancellationToken cancellationToken = default) =>
        Guard<IReadOnlyList<ServerItem>>(async () =>
        {
            var items = await _client.GetListing(Absolute(remotePath), recursive ? FtpListOption.Recursive : FtpListOption.Auto, cancellationToken).ConfigureAwait(false);
            return items.Select(item => new ServerItem(item.Name, item.FullName, item.Size, item.Modified == DateTime.MinValue ? null : item.Modified,
                item.Type switch
                {
                    FtpObjectType.Directory => ServerItemType.Directory,
                    FtpObjectType.File => ServerItemType.File,
                    _ => ServerItemType.Other,
                })).ToList();
        });

    public Task CreateDirectoryAsync(string remotePath, CancellationToken cancellationToken = default) =>
        Guard(() => _client.CreateDirectory(Absolute(remotePath), true, cancellationToken));

    public Task DeleteFileAsync(string remotePath, CancellationToken cancellationToken = default) =>
        Guard(async () =>
        {
            var path = Absolute(remotePath);
            if (await _client.FileExists(path, cancellationToken).ConfigureAwait(false))
                await _client.DeleteFile(path, cancellationToken).ConfigureAwait(false);
        });

    public Task DeleteDirectoryAsync(string remotePath, CancellationToken cancellationToken = default) =>
        Guard(async () =>
        {
            var path = Absolute(remotePath);
            if (await _client.DirectoryExists(path, cancellationToken).ConfigureAwait(false))
                await _client.DeleteDirectory(path, FtpListOption.Recursive, cancellationToken).ConfigureAwait(false);
        });

    public Task RenameAsync(string remotePath, string newRemotePath, CancellationToken cancellationToken = default) =>
        Guard(() => _client.Rename(Absolute(remotePath), Absolute(newRemotePath), cancellationToken));

    public Task UploadFileAsync(string localPath, string remotePath, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default) =>
        Guard(async () =>
        {
            var total = _fileSystem.FileInfo.New(localPath).Length;
            var status = await _client.UploadFile(localPath, Absolute(remotePath), FtpRemoteExists.Overwrite, true, FtpVerify.None,
                progress is null ? null : new Progress<FtpProgress>(p => progress.Report(new TransferProgress(p.TransferredBytes, total))), cancellationToken).ConfigureAwait(false);
            if (status == FtpStatus.Failed)
                throw new TransferException($"Uploading \"{remotePath}\" failed.");
        });

    public Task DownloadFileAsync(string remotePath, string localPath, IProgress<TransferProgress>? progress = null, CancellationToken cancellationToken = default) =>
        Guard(async () =>
        {
            var path = Absolute(remotePath);
            var total = progress is null ? 0 : Math.Max(0, await _client.GetFileSize(path, 0, cancellationToken).ConfigureAwait(false));
            var status = await _client.DownloadFile(localPath, path, FtpLocalExists.Overwrite, FtpVerify.None,
                progress is null ? null : new Progress<FtpProgress>(p => progress.Report(new TransferProgress(p.TransferredBytes, total))), cancellationToken).ConfigureAwait(false);
            if (status == FtpStatus.Failed)
                throw new TransferException($"Downloading \"{remotePath}\" failed.");
        });

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_client.IsConnected)
                await _client.Disconnect().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Disconnect failures are irrelevant when disposing.
        }

        _client.Dispose();
    }

    private static bool IsTransferError(Exception ex) => ex is FtpException or IOException or System.Net.Sockets.SocketException or TimeoutException;

    private static Task Guard(Func<Task> action) => TransferGuard.RunAsync(action, IsTransferError);

    private static Task<T> Guard<T>(Func<Task<T>> action) => TransferGuard.RunAsync(action, IsTransferError);

    private string Absolute(string remotePath) => RemotePath.Combine(_settings.Directory, remotePath);

    private void OnValidateCertificate(FluentFTP.Client.BaseClient.BaseFtpClient control, FtpSslValidationEventArgs e)
    {
        if (e.PolicyErrors == System.Net.Security.SslPolicyErrors.None)
        {
            e.Accept = true;
            return;
        }

        var fingerprint = e.Certificate is null ? string.Empty : Convert.ToHexString(new X509Certificate2(e.Certificate).GetCertHash(System.Security.Cryptography.HashAlgorithmName.SHA256)).ToLowerInvariant();
        if (!string.IsNullOrEmpty(_settings.TrustedCertificateFingerprint) && string.Equals(fingerprint, _settings.TrustedCertificateFingerprint, StringComparison.OrdinalIgnoreCase))
        {
            e.Accept = true;
            return;
        }

        e.Accept = false;
        throw new UntrustedServerException($"The certificate of \"{_settings.Host}\" is not trusted ({e.PolicyErrors}).", fingerprint, e.Certificate?.Subject ?? string.Empty);
    }
}
