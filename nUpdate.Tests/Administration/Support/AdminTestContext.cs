using System.IO.Abstractions.TestingHelpers;
using Microsoft.AspNetCore.DataProtection;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Security;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.TransferInterface;
using nUpdate.Tests.Support;

namespace nUpdate.Tests.Administration.Support;

/// <summary>An in-memory Administration: mock file system, ephemeral protector, substituted transfer and statistics.</summary>
public sealed class AdminTestContext
{
    public static readonly DateTimeOffset Now = new(2026, 10, 6, 12, 0, 0, TimeSpan.Zero);

    public AdminTestContext()
    {
        FileSystem = new MockFileSystem();
        var temp = FileSystem.Path.GetTempPath();
        Paths = new AdministrationPaths(FileSystem, FileSystem.Path.Combine(temp, "nupdate-admin"),
            FileSystem.Path.Combine(temp, "nupdate-projects"));
        Protector = new DataProtectionCredentialProtector(new EphemeralDataProtectionProvider());
        Http = new StubHttpMessageHandler();
        HttpClientFactory = new ProjectHttpClientFactory(Http);
        Transfer = Substitute.For<ITransferProvider>();
        Transfer.FileExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        Transfer.DirectoryExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        Transfer.ListAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns([]);
        TransferFactory = Substitute.For<ITransferProviderFactory>();
        TransferFactory.Create(Arg.Any<TransferSettings>(), Arg.Any<TransferCredentials>()).Returns(Transfer);
        Statistics = Substitute.For<IStatisticsApi>();
        Logger = new ProjectLogger(() => Now, () => "tester");
        Store = new ProjectStore(FileSystem, Paths, Protector);
        Passwords = new ProjectPasswordStore(FileSystem, Paths, Protector);
        Feeds = new FeedStore(FileSystem, HttpClientFactory);
        Signer = new PackageSigner(FileSystem);
        Builder = new PackageBuilder(FileSystem, () => Now, OperatingSystem.IsWindows());
        ContentReader = new PackageContentReader(FileSystem, OperatingSystem.IsWindows());
        Migrator = new LegacyFeedMigrator(FileSystem, Paths, HttpClientFactory, Feeds, Signer, TransferFactory,
            Statistics, Store, Logger, () => Now);
    }

    public MockFileSystem FileSystem { get; }

    public AdministrationPaths Paths { get; }

    public ICredentialProtector Protector { get; }

    public StubHttpMessageHandler Http { get; }

    public ProjectHttpClientFactory HttpClientFactory { get; }

    public ITransferProvider Transfer { get; }

    public ITransferProviderFactory TransferFactory { get; }

    public IStatisticsApi Statistics { get; }

    public ProjectLogger Logger { get; }

    public ProjectStore Store { get; }

    public ProjectPasswordStore Passwords { get; }

    public FeedStore Feeds { get; }

    public PackageSigner Signer { get; }

    public PackageBuilder Builder { get; }

    public PackageContentReader ContentReader { get; }

    public LegacyFeedMigrator Migrator { get; }

    /// <summary>The folder a project of the name gets below the default projects directory.</summary>
    public string ProjectFolder(string name) => Paths.SuggestedProjectFolder(name);

    public UpdateProject NewProject(string name = "Demo", bool statistics = false)
    {
        var project = new UpdateProject
        {
            Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
            Name = name,
            UpdateUrl = "https://updates.example.com/demo/",
            PublicKey = TestKeys.PublicKey,
            Path = FileSystem.Path.Combine(ProjectFolder(name), UpdateProject.FileName),
            Transfer = new TransferSettings
            {
                Protocol = TransferProtocol.Ftp,
                Port = 21,
                Host = "ftp.example.com",
                Username = "user",
                Directory = "/demo"
            },
        };
        project.Statistics.Enabled = statistics;
        if (statistics)
            project.Statistics.Database = new StatisticsDatabaseSettings
            { Host = "db.example.com", Name = "stats", Username = "sqluser" };
        return project;
    }

    public static ProjectSecrets NewSecrets(bool statistics = false) => new()
    {
        TransferPassword = "ftp-secret",
        PrivateKey = TestKeys.PrivateKey,
        StatisticsAdminSecret = statistics ? "admin-secret" : null,
        StatisticsDatabasePassword = statistics ? "sql-pw" : null,
    };

    /// <summary>Adds a local file the package builder can pick up.</summary>
    public string AddSourceFile(string name, string content)
    {
        var path = FileSystem.Path.Combine(FileSystem.Path.GetTempPath(), "sources", name);
        FileSystem.AddFile(path, new MockFileData(content));
        return path;
    }

    /// <summary>Serves a feed (or 404 when <c>null</c>) at the project's feed URL.</summary>
    public void ServeFeed(Updating.UpdateFeed? feed)
    {
        if (feed is null)
            Http.Text(HttpMethod.Get, "https://updates.example.com/demo/nupdate.json", "not found",
                System.Net.HttpStatusCode.NotFound);
        else
            Http.Text(HttpMethod.Get, "https://updates.example.com/demo/nupdate.json", Serializer.Serialize(feed));
    }

    /// <summary>Serves a legacy feed (or 404) at the project's legacy feed URL.</summary>
    public void ServeLegacyFeed(string? json)
    {
        if (json is null)
            Http.Text(HttpMethod.Get, "https://updates.example.com/demo/updates.json", "not found",
                System.Net.HttpStatusCode.NotFound);
        else
            Http.Text(HttpMethod.Get, "https://updates.example.com/demo/updates.json", json);
    }
}
