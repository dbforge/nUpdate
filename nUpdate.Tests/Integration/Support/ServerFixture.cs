using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using DotNet.Testcontainers.Volumes;

namespace nUpdate.Tests.Integration.Support;

/// <summary>
///     One update server for the whole test run: an FTP/FTPS server and an SFTP server writing into a shared volume
///     that a PHP/Apache container serves over HTTP, plus the MySQL database behind <c>nupdate-statistics.php</c>.
/// </summary>
public sealed class ServerFixture : IAsyncLifetime
{
    public const string FtpUser = "ftpuser";
    public const string FtpPassword = "ftp-secret";
    public const string SftpUser = "sftpuser";
    public const string SftpPassword = "sftp-secret";
    public const string DbName = "nupdate";
    public const string DbUser = "nupdate";
    public const string DbPassword = "db-secret";

    private const string VolumePath = "/srv/updates";

    // Passive FTP data connections need host ports that map 1:1; this range is unlikely to collide with other services.
    private const int PassivePortStart = 41000;
    private const int PassivePortCount = 6;

    private INetwork? _network;
    private IVolume? _volume;
    private IContainer? _ftp;
    private IContainer? _sftp;
    private IContainer? _mysql;
    private IContainer? _php;

    public static bool DockerAvailable { get; } = ProbeDocker();

    /// <summary>Whether a missing Docker fails the integration tests instead of skipping them. CI sets NUPDATE_REQUIRE_DOCKER=1.</summary>
    public static bool DockerRequired => Environment.GetEnvironmentVariable("NUPDATE_REQUIRE_DOCKER") == "1";

    public string FtpHost => _ftp!.Hostname;

    public int FtpPort => _ftp!.GetMappedPublicPort(21);

    public string SftpHost => _sftp!.Hostname;

    public int SftpPort => _sftp!.GetMappedPublicPort(22);

    /// <summary>The HTTP base URL of the shared directory, with a trailing slash.</summary>
    public string HttpBaseUrl => $"http://{_php!.Hostname}:{_php.GetMappedPublicPort(80)}/";

    public async ValueTask InitializeAsync()
    {
        if (!DockerAvailable)
        {
            if (DockerRequired)
                throw new InvalidOperationException("NUPDATE_REQUIRE_DOCKER is 1, but \"docker info\" failed or did not answer within a minute.");
            return;
        }

        _network = new NetworkBuilder().Build();
        _volume = new VolumeBuilder().Build();
        await _network.CreateAsync();
        await _volume.CreateAsync();

        _mysql = new ContainerBuilder("mysql:8.4")
            .WithNetwork(_network).WithNetworkAliases("mysql")
            .WithEnvironment("MYSQL_ROOT_PASSWORD", "root-secret")
            .WithEnvironment("MYSQL_DATABASE", DbName)
            .WithEnvironment("MYSQL_USER", DbUser)
            .WithEnvironment("MYSQL_PASSWORD", DbPassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("ready for connections.*port: 3306"))
            .Build();

        _ftp = new ContainerBuilder("stilliard/pure-ftpd:latest")
            .WithNetwork(_network)
            .WithEnvironment("FTP_USER_NAME", FtpUser)
            .WithEnvironment("FTP_USER_PASS", FtpPassword)
            .WithEnvironment("FTP_USER_HOME", VolumePath)
            .WithEnvironment("FTP_USER_UID", "33")
            .WithEnvironment("FTP_USER_GID", "33")
            .WithEnvironment("PUBLICHOST", "localhost")
            .WithEnvironment("ADDED_FLAGS", "--tls=1")
            .WithEnvironment("TLS_CN", "localhost")
            .WithEnvironment("TLS_ORG", "nUpdate")
            .WithEnvironment("TLS_C", "DE")
            .WithPortBinding(21, true)
            .WithEnvironment("FTP_PASSIVE_PORTS", $"{PassivePortStart}:{PassivePortStart + PassivePortCount - 1}")
            .WithPortBinding(PassivePortStart, PassivePortStart).WithPortBinding(PassivePortStart + 1, PassivePortStart + 1).WithPortBinding(PassivePortStart + 2, PassivePortStart + 2)
            .WithPortBinding(PassivePortStart + 3, PassivePortStart + 3).WithPortBinding(PassivePortStart + 4, PassivePortStart + 4).WithPortBinding(PassivePortStart + 5, PassivePortStart + 5)
            .WithVolumeMount(_volume, VolumePath)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(21))
            .Build();

        _sftp = new ContainerBuilder("atmoz/sftp:alpine")
            .WithNetwork(_network)
            .WithCommand($"{SftpUser}:{SftpPassword}:33:33")
            .WithPortBinding(22, true)
            .WithVolumeMount(_volume, $"/home/{SftpUser}/updates")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(22))
            .Build();

        var phpImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(Path.Combine(AppContext.BaseDirectory, "Integration", "Docker"))
            .WithDockerfile("php.Dockerfile")
            .WithName("nupdate-integration-php:latest")
            .WithCleanUp(false)
            .Build();
        await phpImage.CreateAsync();

        _php = new ContainerBuilder(phpImage)
            .WithNetwork(_network)
            .WithPortBinding(80, true)
            .WithVolumeMount(_volume, "/var/www/html")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(80))
            .Build();

        await Task.WhenAll(_mysql.StartAsync(), _ftp.StartAsync(), _sftp.StartAsync(), _php.StartAsync());
        // The volume is created root-owned; let the FTP/SFTP user (uid 33, the same as www-data) write into it.
        await _php.ExecAsync(["chown", "33:33", "/var/www/html"]);
        await _php.ExecAsync(["chmod", "755", "/var/www/html"]);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var container in new[] { _php, _ftp, _sftp, _mysql })
        {
            if (container is not null)
                await container.DisposeAsync();
        }

        if (_volume is not null)
            await _volume.DisposeAsync();
        if (_network is not null)
            await _network.DisposeAsync();
    }

    /// <summary>Removes everything in the shared directory so tests start from an empty server.</summary>
    public async Task ResetAsync()
    {
        await _php!.ExecAsync(["sh", "-c", "rm -rf /var/www/html/* /var/www/html/.[!.]* 2>/dev/null; true"]);
        await _mysql!.ExecAsync(["mysql", $"-u{DbUser}", $"-p{DbPassword}", DbName, "-e", "DROP TABLE IF EXISTS nupdate_download; DROP TABLE IF EXISTS nupdate_version; DROP TABLE IF EXISTS nupdate_application;"]);
    }

    private static bool ProbeDocker()
    {
        try
        {
            using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("docker", "info") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false })!;
            // Drain both streams, so a long answer cannot fill a pipe and keep docker from exiting.
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            // The daemon of a fresh CI runner can take well over ten seconds to answer.
            if (!process.WaitForExit(60_000))
            {
                process.Kill();
                return false;
            }

            return process.ExitCode == 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

[CollectionDefinition(Name)]
public sealed class ServerCollectionFixture : ICollectionFixture<ServerFixture>
{
    public const string Name = "update-server";
}

/// <summary>Skips a test when Docker is not available instead of failing it.</summary>
public sealed class DockerFactAttribute : FactAttribute
{
    public DockerFactAttribute([System.Runtime.CompilerServices.CallerFilePath] string? sourceFilePath = null, [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = -1)
        : base(sourceFilePath, sourceLineNumber)
    {
        if (!ServerFixture.DockerAvailable && !ServerFixture.DockerRequired)
            Skip = "Docker is not available on this machine.";
    }
}
