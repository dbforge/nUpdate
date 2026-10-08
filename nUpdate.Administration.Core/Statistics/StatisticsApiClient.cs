using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Statistics;

/// <summary>
///     HTTP implementation of <see cref="IStatisticsApi" />: REST routes below <c>nupdate-statistics.php/v2</c>, Bearer
///     authentication, JSON errors. Requests go through the project's proxy and HTTP authentication like the feed does.
/// </summary>
public sealed class StatisticsApiClient : IStatisticsApi
{
    private readonly IProjectHttpClientFactory _httpClientFactory;

    public StatisticsApiClient(IProjectHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task VerifyAsync(StatisticsEndpoint endpoint, CancellationToken cancellationToken = default)
    {
        var body = await SendAsync(endpoint, HttpMethod.Get, string.Empty, cancellationToken).ConfigureAwait(false);
        int? version = null;
        try
        {
            version = JObject.Parse(body).Value<int?>("version");
        }
        catch (Newtonsoft.Json.JsonException)
        {
            // handled below
        }

        if (version != nUpdate.Statistics.StatisticsApi.Version)
            throw new StatisticsException($"The statistics endpoint \"{endpoint.Uri}\" is not a nUpdate statistics API version {nUpdate.Statistics.StatisticsApi.Version}.");
    }

    public Task RegisterVersionAsync(StatisticsEndpoint endpoint, Guid projectId, UpdateVersion version, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(version);
        return SendAsync(endpoint, HttpMethod.Put, VersionRoute(projectId, version), cancellationToken);
    }

    public Task DeleteVersionAsync(StatisticsEndpoint endpoint, Guid projectId, UpdateVersion version, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(version);
        return SendAsync(endpoint, HttpMethod.Delete, VersionRoute(projectId, version), cancellationToken);
    }

    public Task DeleteProjectAsync(StatisticsEndpoint endpoint, Guid projectId, CancellationToken cancellationToken = default) =>
        SendAsync(endpoint, HttpMethod.Delete, $"projects/{projectId}", cancellationToken);

    public async Task<ProjectStatistics> GetStatisticsAsync(StatisticsEndpoint endpoint, Guid projectId, CancellationToken cancellationToken = default)
    {
        var body = await SendAsync(endpoint, HttpMethod.Get, $"projects/{projectId}/statistics", cancellationToken).ConfigureAwait(false);
        try
        {
            return Serializer.Deserialize<ProjectStatistics>(body) ?? new ProjectStatistics();
        }
        catch (Newtonsoft.Json.JsonException ex)
        {
            throw new StatisticsException("The statistics endpoint returned an invalid response.", ex);
        }
    }

    private static string VersionRoute(Guid projectId, UpdateVersion version) => $"projects/{projectId}/versions/{Uri.EscapeDataString(version.ToString())}";

    private async Task<string> SendAsync(StatisticsEndpoint endpoint, HttpMethod method, string route, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        using var request = new HttpRequestMessage(method, nUpdate.Statistics.StatisticsApi.Route(endpoint.Uri, route));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", endpoint.AdminSecret);
        request.Headers.TryAddWithoutValidation("X-nUpdate-Secret", endpoint.AdminSecret); // for hosts that strip Authorization
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var httpClient = _httpClientFactory.Create(endpoint.Project, endpoint.Secrets);
        HttpResponseMessage response;
        try
        {
            response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw new StatisticsException($"The statistics endpoint \"{endpoint.Uri}\" could not be reached: {ex.Message}", ex);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new StatisticsException($"The statistics endpoint \"{endpoint.Uri}\" did not respond in time.", ex);
        }

        using (response)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                return content;
            var (code, message) = ParseError(content);
            throw new StatisticsException(code, $"The statistics endpoint responded with {(int)response.StatusCode}: {message}", (int)response.StatusCode);
        }
    }

    private static (string Code, string Message) ParseError(string content)
    {
        try
        {
            if (JObject.Parse(content)["error"] is JObject error)
                return (error.Value<string>("code") is { Length: > 0 } code ? code : "unknown", error.Value<string>("message") is { Length: > 0 } message ? message : content);
        }
        catch (Newtonsoft.Json.JsonException)
        {
            // not a JSON error document
        }

        return ("unknown", content);
    }
}
