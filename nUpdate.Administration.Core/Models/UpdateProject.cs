using Newtonsoft.Json;
using nUpdate.Administration.TransferInterface;
using nUpdate.Packaging;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Models;

/// <summary>
///     An update project as stored in <c>project.nupdproj</c> (format 6). The file lives in the project folder next to
///     <c>packages/</c>, so a folder can be copied to any machine and opened there.
/// </summary>
public sealed class UpdateProject
{
    public const int CurrentFormat = 6;

    public const string FileName = "project.nupdproj";

    public const string PackagesFolderName = "packages";

    /// <summary>Whether the text is an absolute http(s) URL, which is what clients can download updates from.</summary>
    public static bool IsValidUpdateUrl(string? url) =>
        Uri.TryCreate(url?.Trim(), UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    /// <summary>The URL with a trailing slash, so relative paths resolve below it.</summary>
    public static string NormalizeUpdateUrl(string url)
    {
        ArgumentNullException.ThrowIfNull(url);
        var trimmed = url.Trim();
        return trimmed.EndsWith('/') ? trimmed : trimmed + "/";
    }

    public int Format { get; set; } = CurrentFormat;

    /// <summary>Identifies the project everywhere: the feed, the manifests, the statistics.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    /// <summary>The path of the project file on this machine. Not stored in the file.</summary>
    [JsonIgnore]
    public string Path { get; set; } = string.Empty;

    /// <summary>The HTTP(S) URL under which the feed and the packages are served; always ends with a slash.</summary>
    public string UpdateUrl { get; set; } = string.Empty;

    /// <summary>Optional path of an assembly whose version is suggested for new packages.</summary>
    public string? AssemblyVersionPath { get; set; }

    /// <summary>
    ///     The project file of nUpdate Administration 3 or 4 this project was converted from, or <c>null</c>. The migration
    ///     assistant names it, since that file keeps publishing to the old feed until every client has moved.
    /// </summary>
    public string? LegacyProjectFile { get; set; }

    public TransferSettings Transfer { get; set; } = new();

    public HttpAuthenticationSettings? HttpAuthentication { get; set; }

    public StatisticsSettings Statistics { get; set; } = new();

    /// <summary>The RSA public key (PEM) that client applications embed.</summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>The <see cref="ProjectSecrets" /> encrypted under the project password, or <c>null</c> when credentials are not saved.</summary>
    public string? Secrets { get; set; }

    public List<UpdatePackage> Packages { get; set; } = [];

    public List<LogEntry> Log { get; set; } = [];

    /// <summary>The folder of the project file.</summary>
    [JsonIgnore]
    public string Folder => System.IO.Path.GetDirectoryName(Path) ?? string.Empty;

    /// <summary>The folder that holds one subfolder per package.</summary>
    [JsonIgnore]
    public string PackagesDirectory => System.IO.Path.Combine(Folder, PackagesFolderName);

    /// <summary>The URL of the feed.</summary>
    [JsonIgnore]
    public Uri FeedUri => Resolve(UpdateFeed.FileName);

    /// <summary>A URL below the update URL.</summary>
    /// <exception cref="InvalidOperationException">The update URL is not a valid absolute URL.</exception>
    public Uri Resolve(string relativePath)
    {
        ArgumentNullException.ThrowIfNull(relativePath);
        if (!IsValidUpdateUrl(UpdateUrl))
            throw new InvalidOperationException($"The update URL \"{UpdateUrl}\" is not a valid absolute URL.");
        return new Uri(new Uri(NormalizeUpdateUrl(UpdateUrl)), relativePath);
    }

    /// <summary>The local folder of a version: <c>packages/&lt;version&gt;</c> with its <c>feed-entry.json</c> and a folder per platform.</summary>
    public string PackageDirectory(UpdateVersion version)
    {
        ArgumentNullException.ThrowIfNull(version);
        return System.IO.Path.Combine(PackagesDirectory, version.ToString());
    }

    /// <summary>The local folder of the package file of a platform: <c>packages/&lt;version&gt;/&lt;platform&gt;</c>, with the zip and its <c>manifest.json</c>.</summary>
    public string PlatformDirectory(UpdateVersion version, string platform)
    {
        ArgumentNullException.ThrowIfNull(platform);
        return System.IO.Path.Combine(PackageDirectory(version), platform);
    }

    /// <summary>The local package zip of a platform: <c>packages/&lt;version&gt;/&lt;platform&gt;/&lt;platform&gt;.zip</c>.</summary>
    public string PackageFilePath(UpdateVersion version, string platform) =>
        System.IO.Path.Combine(PlatformDirectory(version, platform), PackageLayout.PackageFileName(platform));

    public UpdatePackage? FindPackage(UpdateVersion version)
    {
        ArgumentNullException.ThrowIfNull(version);
        return Packages.FirstOrDefault(p => p.Version == version);
    }
}
