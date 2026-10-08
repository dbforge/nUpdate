using System.Net.Http;
using System.Text;
using nUpdate.Updating;

namespace nUpdate.Statistics;

/// <summary>The body a client posts to <c>/v2/downloads</c> after downloading a package.</summary>
internal sealed class DownloadReport
{
    public DownloadReport(Guid projectId, UpdateVersion version, string os)
    {
        ProjectId = projectId;
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Os = os ?? throw new ArgumentNullException(nameof(os));
    }

    public Guid ProjectId { get; }

    public UpdateVersion Version { get; }

    public string Os { get; }
}

/// <summary>Talks to the statistics API v2 of a project.</summary>
internal static class StatisticsApi
{
    public const int Version = 2;

    /// <summary>The URL of a route below the endpoint's base URL, for example <c>…/nupdate-statistics.php/v2/downloads</c>.</summary>
    public static Uri Route(Uri endpoint, string route)
    {
        if (endpoint is null)
            throw new ArgumentNullException(nameof(endpoint));
        if (route is null)
            throw new ArgumentNullException(nameof(route));
        var path = route.Trim('/');
        return new Uri(endpoint.ToString().TrimEnd('/') + "/v" + Version.ToString(System.Globalization.CultureInfo.InvariantCulture) + (path.Length == 0 ? string.Empty : "/" + path));
    }

    /// <summary>Posts a download report.</summary>
    /// <exception cref="HttpRequestException">The endpoint did not accept the report.</exception>
    public static async Task ReportDownloadAsync(HttpClient httpClient, Uri endpoint, DownloadReport report, CancellationToken cancellationToken = default)
    {
        if (httpClient is null)
            throw new ArgumentNullException(nameof(httpClient));
        if (report is null)
            throw new ArgumentNullException(nameof(report));

        using var content = new StringContent(Serializer.Serialize(report), Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync(Route(endpoint, "downloads"), content, cancellationToken).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        throw new HttpRequestException($"The statistics endpoint responded with {(int)response.StatusCode}: {body}");
    }
}
