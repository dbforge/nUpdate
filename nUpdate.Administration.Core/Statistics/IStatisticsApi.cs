using nUpdate.Updating;

namespace nUpdate.Administration.Core.Statistics;

/// <summary>Talks to the statistics API v2 (<c>nupdate-statistics.php</c>) of a project.</summary>
public interface IStatisticsApi
{
    /// <summary>Checks that the endpoint answers with API version 2 and that the database is reachable.</summary>
    Task VerifyAsync(StatisticsEndpoint endpoint, CancellationToken cancellationToken = default);

    Task RegisterVersionAsync(StatisticsEndpoint endpoint, Guid projectId, UpdateVersion version,
        CancellationToken cancellationToken = default);

    Task DeleteVersionAsync(StatisticsEndpoint endpoint, Guid projectId, UpdateVersion version,
        CancellationToken cancellationToken = default);

    /// <summary>Removes every version and download of the project.</summary>
    Task DeleteProjectAsync(StatisticsEndpoint endpoint, Guid projectId, CancellationToken cancellationToken = default);

    Task<ProjectStatistics> GetStatisticsAsync(StatisticsEndpoint endpoint, Guid projectId,
        CancellationToken cancellationToken = default);
}

/// <summary>Where the statistics API lives and how to authenticate, plus the project whose proxy and HTTP authentication the requests go through.</summary>
public sealed class StatisticsEndpoint(
    Uri uri,
    string adminSecret,
    Models.UpdateProject project,
    Models.ProjectSecrets secrets)
{
    public Uri Uri { get; } = uri ?? throw new ArgumentNullException(nameof(uri));

    public string AdminSecret { get; } = adminSecret ?? throw new ArgumentNullException(nameof(adminSecret));

    public Models.UpdateProject Project { get; } = project ?? throw new ArgumentNullException(nameof(project));

    public Models.ProjectSecrets Secrets { get; } = secrets ?? throw new ArgumentNullException(nameof(secrets));
}

/// <summary>Download statistics of one project.</summary>
public sealed class ProjectStatistics
{
    public long Total { get; set; }

    public List<VersionStatistics> Versions { get; set; } = [];
}

public sealed class VersionStatistics
{
    public UpdateVersion Version { get; set; } = new();

    public long Downloads { get; set; }

    public Dictionary<string, long> ByOperatingSystem { get; set; } = [];
}

/// <summary>The statistics API rejected a request.</summary>
public class StatisticsException : Exception
{
    public StatisticsException()
    {
    }

    public StatisticsException(string message)
        : base(message)
    {
    }

    public StatisticsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public StatisticsException(string code, string message, int statusCode)
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }

    /// <summary>The error code the API returned, for example <c>unauthorized</c>; <c>null</c> for transport errors.</summary>
    public string? Code { get; }

    public int StatusCode { get; }
}
