using System.IO.Abstractions;
using nUpdate.Exceptions;

namespace nUpdate.Updating;

/// <summary>Reads and validates <c>nupdate.json</c>.</summary>
internal static class FeedLoader
{
    /// <exception cref="InvalidFeedException">The content is not a valid feed.</exception>
    /// <exception cref="UnsupportedFormatException">The feed has another format than this nUpdate reads.</exception>
    public static UpdateFeed Parse(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidFeedException("The update feed is empty.");

        if (content!.TrimStart().StartsWith("[", StringComparison.Ordinal))
            throw new UnsupportedFormatException(
                "The update feed is an updates.json written by nUpdate 3 or 4. Point the application at the nupdate.json that nUpdate Administration 5 publishes.");

        UpdateFeed feed;
        try
        {
            feed = Serializer.Deserialize<UpdateFeed>(content!) ??
                   throw new InvalidFeedException("The update feed is empty.");
        }
        catch (Newtonsoft.Json.JsonException ex)
        {
            throw new InvalidFeedException("The update feed is not valid JSON: " + ex.Message, ex);
        }

        FormatVersion.Check(feed.Format, UpdateFeed.CurrentFormat, "update feed");
        feed.Packages ??= [];
        var seen = new HashSet<UpdateVersion>();
        foreach (var package in feed.Packages)
        {
            if (package is null || package.Version is null)
                throw new InvalidFeedException("The feed contains a package without a version.");
            if (!seen.Add(package.Version))
                throw new InvalidFeedException($"Version \"{package.Version}\" appears more than once in the feed.");
            if (package.Files is null || package.Files.Count == 0)
                throw new InvalidFeedException($"The package \"{package.Version}\" has no files.");
            var platforms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in package.Files)
            {
                if (file is null || string.IsNullOrWhiteSpace(file.Platform))
                    throw new InvalidFeedException($"The package \"{package.Version}\" has a file without a platform.");
                if (!platforms.Add(file.Platform))
                    throw new InvalidFeedException(
                        $"The package \"{package.Version}\" has more than one file for the platform \"{file.Platform}\".");
                if (string.IsNullOrWhiteSpace(file.Path))
                    throw new InvalidFeedException(
                        $"The {file.Platform} file of the package \"{package.Version}\" has no path.");
                if (file.Signature is null || string.IsNullOrWhiteSpace(file.Signature.Value))
                    throw new InvalidFeedException(
                        $"The {file.Platform} file of the package \"{package.Version}\" has no signature.");
                file.Touches ??= [];
            }

            // Whether the running application can stay open while its files are replaced only the application knows.
            if (package.AfterInstall is { } afterInstall && afterInstall != AfterInstall.Restart &&
                afterInstall != AfterInstall.Close)
                throw new InvalidFeedException(
                    $"The package \"{package.Version}\" asks for \"{afterInstall}\" after the update; a package can only ask to restart the application or to leave it closed.");
            package.Changelog ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            package.UnsupportedVersions ??= [];
            package.Rollout ??= new RolloutSettings();
            package.Rollout.Conditions ??= [];
            if (package.UnsupportedVersions.Any(v => v is null) || package.Rollout.Conditions.Any(c => c is null))
                throw new InvalidFeedException(
                    $"The package \"{package.Version}\" has an empty entry in its unsupported versions or rollout conditions.");
        }

        return feed;
    }

    public static UpdateFeed FromFile(IFileSystem fileSystem, string path)
    {
        if (fileSystem is null)
            throw new ArgumentNullException(nameof(fileSystem));
        return Parse(fileSystem.File.ReadAllText(path));
    }

    /// <exception cref="HttpRequestException">The feed could not be downloaded.</exception>
    public static async Task<UpdateFeed> LoadAsync(HttpClient httpClient, Uri feedUri,
        CancellationToken cancellationToken = default)
    {
        if (httpClient is null)
            throw new ArgumentNullException(nameof(httpClient));
        if (feedUri is null)
            throw new ArgumentNullException(nameof(feedUri));

        using var response = await httpClient
            .GetAsync(feedUri, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"The update feed \"{feedUri}\" could not be downloaded: {(int)response.StatusCode} {response.ReasonPhrase}.");
        return Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
    }
}
