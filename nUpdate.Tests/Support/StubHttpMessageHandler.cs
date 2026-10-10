using System.Net;

namespace nUpdate.Tests.Support;

/// <summary>Routes requests to handlers registered per method and URI; unregistered requests return 404.</summary>
public sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly
        List<(Func<HttpRequestMessage, bool> Matches,
            Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> Respond)> _routes = [];

    public List<HttpRequestMessage> Requests { get; } = [];

    public List<string> RequestBodies { get; } = [];

    public StubHttpMessageHandler On(HttpMethod method, string uri,
        Func<HttpRequestMessage, HttpResponseMessage> respond) =>
        On(request => request.Method == method && request.RequestUri?.ToString() == uri,
            (request, _) => Task.FromResult(respond(request)));

    public StubHttpMessageHandler On(HttpMethod method, string uri,
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond) =>
        On(request => request.Method == method && request.RequestUri?.ToString() == uri, respond);

    public StubHttpMessageHandler On(Func<HttpRequestMessage, bool> matches,
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond)
    {
        _routes.Insert(0, (matches, respond)); // later registrations win
        return this;
    }

    public StubHttpMessageHandler Text(HttpMethod method, string uri, string body,
        HttpStatusCode status = HttpStatusCode.OK) =>
        On(method, uri, _ => new HttpResponseMessage(status) { Content = new StringContent(body) });

    public StubHttpMessageHandler Bytes(string uri, byte[] body, bool includeLengthOnHead = true)
    {
        On(HttpMethod.Head, uri, _ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent([]) };
            response.Content.Headers.ContentLength = includeLengthOnHead ? body.Length : null;
            return response;
        });
        return On(HttpMethod.Get, uri,
            _ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(body) });
    }

    public HttpClient CreateClient() => new(this, disposeHandler: false);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Requests.Add(request);
        RequestBodies.Add(request.Content is null
            ? string.Empty
            : await request.Content.ReadAsStringAsync(cancellationToken));
        foreach (var (matches, respond) in _routes)
        {
            if (matches(request))
                return await respond(request, cancellationToken);
        }

        return new HttpResponseMessage(HttpStatusCode.NotFound)
        { Content = new StringContent($"No stub for {request.Method} {request.RequestUri}") };
    }
}
