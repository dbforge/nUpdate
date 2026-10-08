namespace nUpdate.Administration.Core.Models;

/// <summary>HTTP basic authentication for reading the feed and the packages. The password is one of the project's secrets.</summary>
public sealed class HttpAuthenticationSettings
{
    public string Username { get; set; } = string.Empty;
}
