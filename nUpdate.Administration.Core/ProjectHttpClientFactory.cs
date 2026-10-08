using System.Net;
using nUpdate.Administration.Core.Models;

namespace nUpdate.Administration.Core;

/// <summary>Creates HTTP clients configured with a project's proxy and HTTP authentication.</summary>
public interface IProjectHttpClientFactory
{
    HttpClient Create(UpdateProject project, ProjectSecrets secrets);
}

public sealed class ProjectHttpClientFactory : IProjectHttpClientFactory
{
    private readonly HttpMessageHandler? _handler;

    public ProjectHttpClientFactory()
    {
    }

    /// <summary>For tests: every created client sends through the given handler.</summary>
    public ProjectHttpClientFactory(HttpMessageHandler handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public HttpClient Create(UpdateProject project, ProjectSecrets secrets)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);

        HttpClient client;
        if (_handler is not null)
        {
            client = new HttpClient(_handler, disposeHandler: false);
        }
        else
        {
            var handler = new HttpClientHandler();
            if (project.Transfer.Proxy is { } proxy && !string.IsNullOrEmpty(proxy.Address))
            {
                var webProxy = new WebProxy(proxy.Address);
                if (!string.IsNullOrEmpty(proxy.Username))
                    webProxy.Credentials = new NetworkCredential(proxy.Username, secrets.ProxyPassword);
                handler.Proxy = webProxy;
                handler.UseProxy = true;
            }

            if (project.HttpAuthentication is { } auth && !string.IsNullOrEmpty(auth.Username))
            {
                handler.Credentials = new NetworkCredential(auth.Username, secrets.HttpAuthenticationPassword);
                handler.PreAuthenticate = true;
            }

            client = new HttpClient(handler, disposeHandler: true);
        }

        client.Timeout = TimeSpan.FromSeconds(60);
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", $"nUpdate.Administration/{typeof(ProjectHttpClientFactory).Assembly.GetName().Version!.ToString(2)}");
        client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
        return client;
    }
}
