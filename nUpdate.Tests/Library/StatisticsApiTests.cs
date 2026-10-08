using System.Net;
using nUpdate.Statistics;
using nUpdate.Tests.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Library;

public class StatisticsApiTests
{
    [Fact]
    public async Task ReportDownload_PostsCamelCaseJsonToTheDownloadsRoute()
    {
        var http = new StubHttpMessageHandler().On(HttpMethod.Post, "https://h/u/statistics.php/v2/downloads", _ => new HttpResponseMessage(HttpStatusCode.NoContent));
        using var client = http.CreateClient();
        var projectId = Guid.NewGuid();

        await StatisticsApi.ReportDownloadAsync(client, new Uri("https://h/u/statistics.php"), new DownloadReport(projectId, new UpdateVersion("1.0.0-beta.2"), "Windows 11"));

        var request = http.Requests.Single();
        request.Content!.Headers.ContentType!.MediaType.ShouldBe("application/json");
        http.RequestBodies.Single().ShouldBe($$"""{"projectId":"{{projectId}}","version":"1.0.0-beta.2","os":"Windows 11"}""");
    }

    [Fact]
    public void Route_AppendsTheApiVersion()
    {
        StatisticsApi.Route(new Uri("https://h/statistics.php"), "downloads").ToString().ShouldBe("https://h/statistics.php/v2/downloads");
        StatisticsApi.Route(new Uri("https://h/api/"), "/projects/1/statistics").ToString().ShouldBe("https://h/api/v2/projects/1/statistics");
        StatisticsApi.Route(new Uri("https://h/statistics.php"), "").ToString().ShouldBe("https://h/statistics.php/v2");
        StatisticsApi.Route(new Uri("https://h/statistics.php"), "/").ToString().ShouldBe("https://h/statistics.php/v2");
        Should.Throw<ArgumentNullException>(() => StatisticsApi.Route(null!, "x"));
        Should.Throw<ArgumentNullException>(() => StatisticsApi.Route(new Uri("https://h"), null!));
    }

    [Fact]
    public async Task ReportDownload_ThrowsOnFailure()
    {
        var http = new StubHttpMessageHandler().Text(HttpMethod.Post, "https://h/u/statistics.php/v2/downloads", "nope", HttpStatusCode.Forbidden);
        using var client = http.CreateClient();
        var ex = await Should.ThrowAsync<HttpRequestException>(() => StatisticsApi.ReportDownloadAsync(client, new Uri("https://h/u/statistics.php"), new DownloadReport(Guid.Empty, new UpdateVersion("1.0.0"), "os")));
        ex.Message.ShouldContain("403");
        ex.Message.ShouldContain("nope");
    }

    [Fact]
    public async Task ReportDownload_ValidatesArguments()
    {
        using var client = new StubHttpMessageHandler().CreateClient();
        var report = new DownloadReport(Guid.Empty, new UpdateVersion("1.0.0"), "os");
        await Should.ThrowAsync<ArgumentNullException>(() => StatisticsApi.ReportDownloadAsync(null!, new Uri("https://h"), report));
        await Should.ThrowAsync<ArgumentNullException>(() => StatisticsApi.ReportDownloadAsync(client, new Uri("https://h"), null!));
        await Should.ThrowAsync<ArgumentNullException>(() => StatisticsApi.ReportDownloadAsync(client, null!, report));
        Should.Throw<ArgumentNullException>(() => new DownloadReport(Guid.Empty, null!, "os"));
        Should.Throw<ArgumentNullException>(() => new DownloadReport(Guid.Empty, new UpdateVersion("1.0.0"), null!));
    }
}
