namespace nUpdate.Administration.Core.Models;

/// <summary>The statistics endpoint of a project (the <c>nupdate-statistics.php</c> API next to the feed).</summary>
public sealed class StatisticsSettings
{
    public bool Enabled { get; set; }

    /// <summary>The absolute URL of the statistics endpoint. Defaults to <c>nupdate-statistics.php</c> next to the update URL.</summary>
    public string? EndpointUrl { get; set; }

    /// <summary>Database settings used when generating <c>nupdate-statistics.config.php</c>. Only needed at setup time.</summary>
    public StatisticsDatabaseSettings? Database { get; set; }
}

/// <summary>MySQL connection data written into the server-side configuration of the statistics script. The password is one of the project's secrets.</summary>
public sealed class StatisticsDatabaseSettings
{
    public string Host { get; set; } = "localhost";

    public string Name { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;
}
