using System.IO.Abstractions;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Core.Statistics;

/// <summary>Uploads <c>nupdate-statistics.php</c> and its configuration for a project and checks that the API answers; shared by project creation, the settings and the legacy migration.</summary>
public sealed class StatisticsDeployer(
    IFileSystem fileSystem,
    ITransferProviderFactory transferFactory,
    IStatisticsApi statistics)
{
    private readonly IFileSystem _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    private readonly ITransferProviderFactory _transferFactory =
        transferFactory ?? throw new ArgumentNullException(nameof(transferFactory));

    private readonly IStatisticsApi _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));

    /// <exception cref="InvalidOperationException">The project has no database settings or no admin secret.</exception>
    public async Task DeployAsync(UpdateProject project, ProjectSecrets secrets,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        var database = project.Statistics.Database ??
                       throw new InvalidOperationException("Statistics need database settings.");
        var endpoint = PublishService.Endpoint(project, secrets);
        var config = StatisticsScript.RenderConfig(database.Host, database.Name, database.Username,
            secrets.StatisticsDatabasePassword ?? string.Empty, endpoint.AdminSecret);

        // Both files only exist locally while they are uploaded; the config carries the database password in clear text.
        var scriptPath = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), $"nupdate-{Guid.NewGuid():N}.php");
        var configPath = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), $"nupdate-{Guid.NewGuid():N}.php");
        await _fileSystem.File.WriteAllTextAsync(scriptPath, StatisticsScript.Script, cancellationToken)
            .ConfigureAwait(false);
        await _fileSystem.File.WriteAllTextAsync(configPath, config, cancellationToken).ConfigureAwait(false);
        try
        {
            await using var transfer = _transferFactory.Create(project.Transfer, secrets.ToTransferCredentials());
            await transfer.ConnectAsync(cancellationToken).ConfigureAwait(false);
            await transfer.UploadFileAsync(scriptPath, StatisticsScript.ScriptFileName, null, cancellationToken)
                .ConfigureAwait(false);
            await transfer.UploadFileAsync(configPath, StatisticsScript.ConfigFileName, null, cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _fileSystem.File.Delete(scriptPath);
            _fileSystem.File.Delete(configPath);
        }

        await _statistics.VerifyAsync(endpoint, cancellationToken).ConfigureAwait(false);
    }
}
