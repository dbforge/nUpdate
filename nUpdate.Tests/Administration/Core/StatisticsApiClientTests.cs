using System.Net;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Tests.Support;
using nUpdate.Updating;

namespace nUpdate.Tests.Administration.Core;

public sealed class StatisticsApiClientTests : IDisposable
{
    private const string Base = "https://updates.example.com/demo/nupdate-statistics.php/v2";
    private static readonly Uri Endpoint = new("https://updates.example.com/demo/nupdate-statistics.php");
    private readonly StubHttpMessageHandler _http = new();
    private readonly Guid _projectId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    private static readonly UpdateProject Project = new() { Name = "Demo", UpdateUrl = "https://updates.example.com/demo/" };
    private static readonly ProjectSecrets Secrets = new();

    private StatisticsApiClient Client => new(new ProjectHttpClientFactory(_http));

    private static StatisticsEndpoint Secured => new(Endpoint, "s3cret", Project, Secrets);

    public void Dispose() => _http.Dispose();

    [Fact]
    public async Task StatisticsApiClient_UsesRestRoutesWithBearer()
    {
        _http.Text(HttpMethod.Get, Base, """{"api":"nupdate-statistics","version":2}""");
        _http.On(r => r.Method == HttpMethod.Put, (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent)));
        _http.On(r => r.Method == HttpMethod.Delete, (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent)));
        _http.Text(HttpMethod.Get, $"{Base}/projects/{_projectId}/statistics", """{"total":3,"versions":[{"version":"1.0.0","downloads":3,"byOperatingSystem":{"Windows 11":2,"Linux":1}}]}""");

        await Client.VerifyAsync(Secured);
        await Client.RegisterVersionAsync(Secured, _projectId, new UpdateVersion("1.0.0-beta.1+build.7"));
        await Client.DeleteVersionAsync(Secured, _projectId, new UpdateVersion("1.0.0"));
        await Client.DeleteProjectAsync(Secured, _projectId);
        var statistics = await Client.GetStatisticsAsync(Secured, _projectId);

        statistics.Total.ShouldBe(3);
        statistics.Versions.Single().Version.ShouldBe(new UpdateVersion("1.0.0"));
        statistics.Versions.Single().ByOperatingSystem["Linux"].ShouldBe(1);
        _http.Requests.Select(r => $"{r.Method} {r.RequestUri}").ShouldBe([
            $"GET {Base}",
            $"PUT {Base}/projects/{_projectId}/versions/1.0.0-beta.1%2Bbuild.7",
            $"DELETE {Base}/projects/{_projectId}/versions/1.0.0",
            $"DELETE {Base}/projects/{_projectId}",
            $"GET {Base}/projects/{_projectId}/statistics",
        ]);
        _http.Requests.ShouldAllBe(r => r.Headers.Authorization!.Scheme == "Bearer" && r.Headers.Authorization.Parameter == "s3cret");
        _http.RequestBodies.ShouldAllBe(b => b == "");
    }

    [Fact]
    public async Task StatisticsApiClient_ReportsErrors()
    {
        _http.Text(HttpMethod.Get, Base, """{"error":{"code":"unauthorized","message":"nope"}}""", HttpStatusCode.Unauthorized);
        var ex = await Should.ThrowAsync<StatisticsException>(() => Client.VerifyAsync(Secured));
        ex.Message.ShouldContain("401");
        ex.Message.ShouldContain("nope");
        ex.Code.ShouldBe("unauthorized");
        ex.StatusCode.ShouldBe(401);

        _http.Text(HttpMethod.Get, Base, "<html>gateway</html>", HttpStatusCode.BadGateway);
        var html = await Should.ThrowAsync<StatisticsException>(() => Client.VerifyAsync(Secured));
        html.Code.ShouldBe("unknown");
        html.Message.ShouldContain("gateway");
        _http.Text(HttpMethod.Get, Base, """{"message":"flat"}""", HttpStatusCode.BadRequest);
        (await Should.ThrowAsync<StatisticsException>(() => Client.VerifyAsync(Secured))).Code.ShouldBe("unknown");
        _http.Text(HttpMethod.Get, Base, """{"error":{"code":null,"message":""}}""", HttpStatusCode.BadRequest);
        var empty = await Should.ThrowAsync<StatisticsException>(() => Client.VerifyAsync(Secured));
        empty.Code.ShouldBe("unknown");
        empty.Message.ShouldContain("\"error\"");

        _http.Text(HttpMethod.Get, Base, """{"api":"other","version":1}""");
        (await Should.ThrowAsync<StatisticsException>(() => Client.VerifyAsync(Secured))).Message.ShouldContain("version 2");
        _http.Text(HttpMethod.Get, Base, "<html>");
        (await Should.ThrowAsync<StatisticsException>(() => Client.VerifyAsync(Secured))).Code.ShouldBeNull();

        var statistics = $"{Base}/projects/{_projectId}/statistics";
        _http.Text(HttpMethod.Get, statistics, "{broken");
        await Should.ThrowAsync<StatisticsException>(() => Client.GetStatisticsAsync(Secured, _projectId));
        _http.Text(HttpMethod.Get, statistics, "null");
        (await Client.GetStatisticsAsync(Secured, _projectId)).Total.ShouldBe(0);

        _http.On(r => r.RequestUri!.ToString() == Base, (_, _) => throw new HttpRequestException("down"));
        (await Should.ThrowAsync<StatisticsException>(() => Client.VerifyAsync(Secured))).Message.ShouldContain("down");
        _http.On(r => r.RequestUri!.ToString() == Base, (_, _) => throw new TaskCanceledException("client timeout"));
        (await Should.ThrowAsync<StatisticsException>(() => Client.VerifyAsync(Secured))).Message.ShouldContain("did not respond in time");
        using (var cancelled = new CancellationTokenSource())
        {
            cancelled.Cancel();
            await Should.ThrowAsync<TaskCanceledException>(() => Client.VerifyAsync(Secured, cancelled.Token));
        }

        await Should.ThrowAsync<ArgumentNullException>(() => Client.VerifyAsync(null!));
        await Should.ThrowAsync<ArgumentNullException>(() => Client.RegisterVersionAsync(Secured, _projectId, null!));
        await Should.ThrowAsync<ArgumentNullException>(() => Client.DeleteVersionAsync(Secured, _projectId, null!));
        Should.Throw<ArgumentNullException>(() => new StatisticsApiClient(null!));
        Should.Throw<ArgumentNullException>(() => new StatisticsEndpoint(null!, "s", Project, Secrets));
        Should.Throw<ArgumentNullException>(() => new StatisticsEndpoint(Endpoint, null!, Project, Secrets));
        Should.Throw<ArgumentNullException>(() => new StatisticsEndpoint(Endpoint, "s", null!, Secrets));
        Should.Throw<ArgumentNullException>(() => new StatisticsEndpoint(Endpoint, "s", Project, null!));
        Secured.Project.ShouldBeSameAs(Project);
        Secured.Secrets.ShouldBeSameAs(Secrets);
        var inner = new InvalidOperationException();
        new StatisticsException().ShouldNotBeNull();
        new StatisticsException("m").Message.ShouldBe("m");
        new StatisticsException("m", inner).InnerException.ShouldBe(inner);
    }
}
