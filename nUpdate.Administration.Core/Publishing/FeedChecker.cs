using System.IO.Abstractions;
using System.IO.Compression;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Exceptions;
using nUpdate.Packaging;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Publishing;

/// <summary>Checks the published feed the way an nUpdate 5 client uses it.</summary>
public interface IFeedChecker
{
    /// <summary>
    ///     Downloads <c>nupdate.json</c> and every package file it lists and checks size, SHA-512 hash, signature and manifest
    ///     against the feed and the project's public key, and asks the statistics API whether it answers. Problems are
    ///     reported in the result, not thrown.
    /// </summary>
    Task<FeedCheckResult> CheckAsync(UpdateProject project, ProjectSecrets secrets,
        IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default);
}

/// <summary>The outcome of <see cref="IFeedChecker.CheckAsync" />.</summary>
public sealed class FeedCheckResult(
    string? feedProblem,
    IReadOnlyList<PackageCheck> packages,
    bool statisticsChecked,
    string? statisticsProblem)
{
    /// <summary>Why <c>nupdate.json</c> could not be read, or <c>null</c>.</summary>
    public string? FeedProblem { get; } = feedProblem;

    public IReadOnlyList<PackageCheck> Packages { get; } =
        packages ?? throw new ArgumentNullException(nameof(packages));

    /// <summary>The project has statistics, so the API was asked.</summary>
    public bool StatisticsChecked { get; } = statisticsChecked;

    public string? StatisticsProblem { get; } = statisticsProblem;

    public bool Succeeded => FeedProblem is null && Packages.All(p => p.Problem is null) && StatisticsProblem is null;
}

/// <summary>The check of one package file of the feed.</summary>
public sealed class PackageCheck(UpdateVersion version, string platform, Uri uri, string? problem)
{
    public UpdateVersion Version { get; } = version ?? throw new ArgumentNullException(nameof(version));

    public string Platform { get; } = platform ?? throw new ArgumentNullException(nameof(platform));

    public Uri Uri { get; } = uri ?? throw new ArgumentNullException(nameof(uri));

    /// <summary>What a client would reject, or <c>null</c> when the package is fine.</summary>
    public string? Problem { get; } = problem;
}

public sealed class FeedChecker(
    IFileSystem fileSystem,
    IProjectHttpClientFactory httpClientFactory,
    IFeedStore feeds,
    IPackageSigner signer,
    IStatisticsApi statistics)
    : IFeedChecker
{
    private readonly IFileSystem _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    private readonly IProjectHttpClientFactory _httpClientFactory =
        httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

    private readonly IFeedStore _feeds = feeds ?? throw new ArgumentNullException(nameof(feeds));
    private readonly IPackageSigner _signer = signer ?? throw new ArgumentNullException(nameof(signer));
    private readonly IStatisticsApi _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));

    public async Task<FeedCheckResult> CheckAsync(UpdateProject project, ProjectSecrets secrets,
        IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        progress?.Report(new PipelineProgress($"Reading {UpdateFeed.FileName}", 0, 1));
        UpdateFeed? feed;
        string? feedProblem = null;
        try
        {
            feed = await _feeds.LoadRemoteAsync(project, secrets, cancellationToken).ConfigureAwait(false);
            if (feed is null)
                feedProblem = $"There is no {UpdateFeed.FileName} at {project.FeedUri}.";
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidFeedException or UnsupportedFormatException)
        {
            feed = null;
            feedProblem = ex.Message;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            feed = null;
            feedProblem = $"{project.FeedUri} did not answer in time.";
        }

        var packages = new List<PackageCheck>();
        var files = (feed?.Packages ?? []).OrderBy(p => p.Version)
            .SelectMany(p => p.Files.Select(f => (Package: p, File: f))).ToList();
        for (var i = 0; i < files.Count; i++)
        {
            var (package, file) = files[i];
            progress?.Report(new PipelineProgress($"Checking {package.Version} for {file.Platform}", i + 1,
                files.Count + 1));
            packages.Add(
                await CheckPackageAsync(project, secrets, feed!.ProjectId, package.Version, file, cancellationToken)
                    .ConfigureAwait(false));
        }

        string? statisticsProblem = null;
        if (project.Statistics.Enabled)
            statisticsProblem = await CheckStatisticsAsync(project, secrets, cancellationToken).ConfigureAwait(false);
        return new FeedCheckResult(feedProblem, packages, project.Statistics.Enabled, statisticsProblem);
    }

    private async Task<PackageCheck> CheckPackageAsync(UpdateProject project, ProjectSecrets secrets,
        Guid feedProjectId, UpdateVersion version, PackageFile file,
        CancellationToken cancellationToken)
    {
        // Like the client: an absolute http(s) URL as it is, anything else relative to the feed.
        var uri = Uri.TryCreate(file.Path, UriKind.Absolute, out var absolute) &&
                  (absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps)
            ? absolute
            : new Uri(project.FeedUri, file.Path);
        var path = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), $"nupdate-check-{Guid.NewGuid():N}.zip");
        try
        {
            using (var client = _httpClientFactory.Create(project, secrets))
            using (var response = await client
                       .GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                       .ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                    return new PackageCheck(version, file.Platform, uri,
                        $"The server answered {(int)response.StatusCode} ({response.ReasonPhrase}).");
                await using var target = _fileSystem.File.Create(path);
                await response.Content.CopyToAsync(target, cancellationToken).ConfigureAwait(false);
            }

            return new PackageCheck(version, file.Platform, uri, Verify(project, feedProjectId, version, file, path));
        }
        catch (HttpRequestException ex)
        {
            return new PackageCheck(version, file.Platform, uri, ex.Message);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new PackageCheck(version, file.Platform, uri, "The download did not finish in time.");
        }
        finally
        {
            if (_fileSystem.File.Exists(path))
                _fileSystem.File.Delete(path);
        }
    }

    /// <summary>The checks of <c>UpdateManager.DownloadAsync</c> and <c>VerifyAsync</c>, in the same order.</summary>
    private string? Verify(UpdateProject project, Guid feedProjectId, UpdateVersion version, PackageFile file,
        string path)
    {
        var size = _fileSystem.FileInfo.New(path).Length;
        if (size != file.Size)
            return $"The server delivered {size} bytes, the feed announces {file.Size}.";
        if (!string.Equals(_signer.Hash(path), file.Sha512, StringComparison.Ordinal))
            return "The SHA-512 hash does not match the feed.";
        if (!string.Equals(file.Signature.Algorithm, PackageSignature.RsaPssSha512, StringComparison.OrdinalIgnoreCase))
            return $"The package is signed with \"{file.Signature.Algorithm}\", which nUpdate 5 does not verify.";
        try
        {
            if (!_signer.Verify(path, project.PublicKey, file.Signature.Value))
                return "The signature does not match the public key of the project.";
        }
        catch (FormatException)
        {
            return "The signature is not valid Base64.";
        }

        try
        {
            using var stream = _fileSystem.File.OpenRead(path);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            if (archive.GetEntry(PackageLayout.ManifestFileName) is not { } entry)
                return $"The package has no {PackageLayout.ManifestFileName}.";
            using var manifestStream = entry.Open();
            var manifest = Serializer.Deserialize<PackageManifest>(manifestStream);
            return manifest is not null && manifest.ProjectId == feedProjectId && manifest.Version == version
                   && string.Equals(manifest.Platform, file.Platform, StringComparison.OrdinalIgnoreCase)
                ? null
                : $"The {PackageLayout.ManifestFileName} of the package names another project, version or platform.";
        }
        catch (Exception ex) when (ex is InvalidDataException or Newtonsoft.Json.JsonException)
        {
            return $"The package cannot be read: {ex.Message}";
        }
    }

    private async Task<string?> CheckStatisticsAsync(UpdateProject project, ProjectSecrets secrets,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(secrets.StatisticsAdminSecret))
            return "The statistics admin secret is missing. Enter it in the project credentials.";
        try
        {
            await _statistics.VerifyAsync(PublishService.Endpoint(project, secrets), cancellationToken)
                .ConfigureAwait(false);
            return null;
        }
        catch (StatisticsException ex)
        {
            return ex.Message;
        }
    }
}
