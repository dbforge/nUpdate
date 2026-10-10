using System.IO.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Security;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.Core.Transfer;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Tests.Integration.Support;

/// <summary>A real Administration wired against the container servers, with its data folder and its projects in a temp directory.</summary>
public sealed class IntegrationContext : IDisposable
{
    public IntegrationContext(ServerFixture server)
    {
        Server = server;
        FileSystem = new FileSystem();
        Root = Path.Combine(Path.GetTempPath(), "nupdate-integration", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Root);
        Paths = new AdministrationPaths(FileSystem, Path.Combine(Root, "admin"), Path.Combine(Root, "projects"));
        Protector = new DataProtectionCredentialProtector(new EphemeralDataProtectionProvider());
        HttpClient = new HttpClient();
        HttpClientFactory = new ProjectHttpClientFactory();
        Store = new ProjectStore(FileSystem, Paths, Protector);
        Passwords = new ProjectPasswordStore(FileSystem, Paths, Protector);
        Logger = new ProjectLogger(() => DateTimeOffset.UtcNow, () => "integration");
        TransferFactory = new TransferProviderFactory(FileSystem);
        Statistics = new StatisticsApiClient(HttpClientFactory);
        Feeds = new FeedStore(FileSystem, HttpClientFactory);
        Signer = new PackageSigner(FileSystem);
        Migrator = new LegacyFeedMigrator(FileSystem, Paths, HttpClientFactory, Feeds, Signer, TransferFactory,
            Statistics, Store, Logger);
        Projects = new ProjectService(FileSystem, Paths, Store, Passwords, TransferFactory, Statistics, Migrator,
            Logger, _ => (nUpdate.Tests.Support.TestKeys.PublicKey, nUpdate.Tests.Support.TestKeys.PrivateKey));
        Publisher = new PublishService(FileSystem, new PackageBuilder(FileSystem), Signer, Feeds, TransferFactory,
            Statistics, Store, Logger);
    }

    public ServerFixture Server { get; }

    public IFileSystem FileSystem { get; }

    public string Root { get; }

    public AdministrationPaths Paths { get; }

    public ICredentialProtector Protector { get; }

    public HttpClient HttpClient { get; }

    public ProjectHttpClientFactory HttpClientFactory { get; }

    public ProjectStore Store { get; }

    public ProjectPasswordStore Passwords { get; }

    public ProjectLogger Logger { get; }

    public TransferProviderFactory TransferFactory { get; }

    public StatisticsApiClient Statistics { get; }

    public FeedStore Feeds { get; }

    public PackageSigner Signer { get; }

    public LegacyFeedMigrator Migrator { get; }

    public ProjectService Projects { get; }

    public PublishService Publisher { get; }

    /// <summary>The folder a project of the name is created in.</summary>
    public string ProjectFolder(string name) => Paths.SuggestedProjectFolder(name);

    public TransferSettings FtpSettings(TransferProtocol protocol = TransferProtocol.Ftp,
        string? trustedFingerprint = null) => new()
        {
            Protocol = protocol,
            Host = Server.FtpHost,
            Port = Server.FtpPort,
            Username = ServerFixture.FtpUser,
            Directory = "/",
            UsePassiveMode = true,
            TrustedCertificateFingerprint = trustedFingerprint,
        };

    public TransferSettings SftpSettings(string? trustedHostKey = null) => new()
    {
        Protocol = TransferProtocol.Sftp,
        Host = Server.SftpHost,
        Port = Server.SftpPort,
        Username = ServerFixture.SftpUser,
        Directory = "/updates",
        TrustedHostKeyFingerprint = trustedHostKey,
    };

    public static TransferCredentials FtpCredentials => new() { Password = ServerFixture.FtpPassword };

    public static TransferCredentials SftpCredentials => new() { Password = ServerFixture.SftpPassword };

    /// <summary>Opens a connected provider after learning the server's certificate or host key on first contact.</summary>
    public async Task<ITransferProvider> ConnectTrustedAsync(TransferSettings settings, TransferCredentials credentials)
    {
        var first = TransferFactory.Create(settings, credentials);
        try
        {
            await first.ConnectAsync();
            return first;
        }
        catch (UntrustedServerException ex)
        {
            await first.DisposeAsync();
            if (settings.Protocol == TransferProtocol.Sftp)
                settings.TrustedHostKeyFingerprint = ex.Fingerprint;
            else
                settings.TrustedCertificateFingerprint = ex.Fingerprint;
            var second = TransferFactory.Create(settings, credentials);
            await second.ConnectAsync();
            return second;
        }
    }

    public string WriteFile(string name, string content)
    {
        var path = Path.Combine(Root, "files", name);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }

    public string WriteBytes(string name, byte[] content)
    {
        var path = Path.Combine(Root, "files", name);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, content);
        return path;
    }

    /// <summary>The HTTP status of a file below the update URL.</summary>
    public async Task<System.Net.HttpStatusCode> StatusAsync(string relativePath)
    {
        using var response = await HttpClient.GetAsync(Server.HttpBaseUrl + relativePath);
        return response.StatusCode;
    }

    public void Dispose()
    {
        HttpClient.Dispose();
        try
        {
            Directory.Delete(Root, true);
        }
        catch (IOException)
        {
            // temp cleanup is best effort
        }
    }
}
