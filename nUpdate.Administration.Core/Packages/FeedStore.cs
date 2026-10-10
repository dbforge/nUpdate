using System.IO.Abstractions;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.TransferInterface;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Packages;

/// <summary>Reads and writes a project's feed on the server and the feed entries kept next to the local packages.</summary>
public interface IFeedStore
{
    const string EntryFileName = "feed-entry.json";

    /// <summary>Downloads the feed from the update URL, or <c>null</c> when the server has none.</summary>
    Task<UpdateFeed?> LoadRemoteAsync(UpdateProject project, ProjectSecrets secrets,
        CancellationToken cancellationToken = default);

    /// <summary>Whether the server still has an <c>updates.json</c> written by nUpdate Administration 3.x or 4.x.</summary>
    Task<bool> LegacyFeedExistsAsync(UpdateProject project, ProjectSecrets secrets,
        CancellationToken cancellationToken = default);

    /// <summary>Uploads the feed as <c>nupdate.json</c> to the project's base directory.</summary>
    Task UploadAsync(ITransferProvider transfer, UpdateFeed feed, CancellationToken cancellationToken = default);

    /// <summary>Writes the entry of a package next to its local zip.</summary>
    Task SaveEntryAsync(UpdateProject project, PackageInfo entry, CancellationToken cancellationToken = default);

    /// <summary>Reads the local entry of a package; <c>null</c> when there is none.</summary>
    Task<PackageInfo?> LoadEntryAsync(UpdateProject project, UpdateVersion version,
        CancellationToken cancellationToken = default);
}

public sealed class FeedStore(IFileSystem fileSystem, IProjectHttpClientFactory httpClientFactory)
    : IFeedStore
{
    private readonly IFileSystem _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    private readonly IProjectHttpClientFactory _httpClientFactory =
        httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

    public async Task<UpdateFeed?> LoadRemoteAsync(UpdateProject project, ProjectSecrets secrets,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        using var client = _httpClientFactory.Create(project, secrets);
        using var response = await client.GetAsync(project.FeedUri, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return FeedLoader.Parse(content);
    }

    public async Task<bool> LegacyFeedExistsAsync(UpdateProject project, ProjectSecrets secrets,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        using var client = _httpClientFactory.Create(project, secrets);
        using var response = await client.GetAsync(project.Resolve(Migration.LegacyFeed.FileName),
            HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task UploadAsync(ITransferProvider transfer, UpdateFeed feed,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transfer);
        ArgumentNullException.ThrowIfNull(feed);
        var temp = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), $"nupdate-{Guid.NewGuid():N}.json");
        try
        {
            await _fileSystem.File
                .WriteAllTextAsync(temp, Serializer.Serialize(feed, indented: true), cancellationToken)
                .ConfigureAwait(false);
            await transfer.UploadFileAsync(temp, UpdateFeed.FileName, null, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (_fileSystem.File.Exists(temp))
                _fileSystem.File.Delete(temp);
        }
    }

    public Task SaveEntryAsync(UpdateProject project, PackageInfo entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(entry);
        var directory = project.PackageDirectory(entry.Version);
        _fileSystem.Directory.CreateDirectory(directory);
        return _fileSystem.File.WriteAllTextAsync(_fileSystem.Path.Combine(directory, IFeedStore.EntryFileName),
            Serializer.Serialize(entry, indented: true), cancellationToken);
    }

    public async Task<PackageInfo?> LoadEntryAsync(UpdateProject project, UpdateVersion version,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(version);
        var path = _fileSystem.Path.Combine(project.PackageDirectory(version), IFeedStore.EntryFileName);
        if (!_fileSystem.File.Exists(path))
            return null;
        return Serializer.Deserialize<PackageInfo>(await _fileSystem.File.ReadAllTextAsync(path, cancellationToken)
            .ConfigureAwait(false));
    }
}
