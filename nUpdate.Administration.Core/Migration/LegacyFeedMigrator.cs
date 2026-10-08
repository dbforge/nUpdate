using System.IO.Abstractions;
using System.IO.Compression;
using System.Text;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.TransferInterface;
using nUpdate.Packaging;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Migration;

/// <summary>Whether a project's server still needs the 5.0 migration.</summary>
public sealed class MigrationStatus
{
    public MigrationStatus(bool legacyFeedPresent, bool feedPresent)
    {
        LegacyFeedPresent = legacyFeedPresent;
        FeedPresent = feedPresent;
    }

    /// <summary>The server has an <c>updates.json</c> written by nUpdate Administration 3.x or 4.x.</summary>
    public bool LegacyFeedPresent { get; }

    /// <summary>The server has a <c>nupdate.json</c>.</summary>
    public bool FeedPresent { get; }

    /// <summary>Only the legacy feed exists: nothing can be published until the packages are migrated.</summary>
    public bool NeedsMigration => LegacyFeedPresent && !FeedPresent;
}

/// <summary>
///     Moves a project published by nUpdate Administration 3.x or 4.x to the 5.0 layout on the server, next to the old
///     files, so clients of both versions keep finding their updates.
/// </summary>
public interface ILegacyFeedMigrator
{
    Task<MigrationStatus> CheckAsync(UpdateProject project, ProjectSecrets secrets, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Reads <c>updates.json</c> and <c>nupdate.json</c> and, for every package that is not in <c>nupdate.json</c> yet,
    ///     finds the old zip (the local copy, else a download) and reads its files and operations, so the migration can
    ///     be reviewed before anything changes. A package that cannot be found or read is reported in the plan.
    /// </summary>
    Task<MigrationPlan> PrepareAsync(UpdateProject project, ProjectSecrets secrets, IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Repacks, re-signs and uploads the included packages of the plan to <c>packages/&lt;version&gt;/&lt;platform&gt;.zip</c>, writes
    ///     <c>nupdate.json</c> (empty when nothing is included and there is none yet), uploads the statistics script v2 and
    ///     registers the versions when the project has statistics, and marks the packages as released in the project.
    ///     Nothing of nUpdate 3 and 4 is changed, on the server or on this computer.
    /// </summary>
    /// <returns>The versions that were migrated.</returns>
    Task<IReadOnlyList<UpdateVersion>> RunAsync(UpdateProject project, ProjectSecrets secrets, MigrationPlan plan, IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     What nUpdate 3 and 4 left for the project: <c>updates.json</c>, the package folders and <c>statistics.php</c> on the
    ///     server, the local package copies of nUpdate Administration 4 and of the 5.0 pre-releases on this computer.
    /// </summary>
    Task<LegacyFiles> FindLegacyFilesAsync(UpdateProject project, ProjectSecrets secrets, CancellationToken cancellationToken = default);

    /// <summary>Deletes the given legacy files; <c>updates.json</c> goes last, so a run that is interrupted can be repeated.</summary>
    Task DeleteLegacyFilesAsync(UpdateProject project, ProjectSecrets secrets, LegacyFiles files, CancellationToken cancellationToken = default);
}

public sealed class LegacyFeedMigrator : ILegacyFeedMigrator
{
    private const string LegacyOperationsFileName = "operations.json";

    private readonly IFileSystem _fileSystem;
    private readonly AdministrationPaths _paths;
    private readonly IProjectHttpClientFactory _httpClientFactory;
    private readonly IFeedStore _feeds;
    private readonly IPackageSigner _signer;
    private readonly ITransferProviderFactory _transferFactory;
    private readonly IStatisticsApi _statistics;
    private readonly StatisticsDeployer _deployer;
    private readonly IProjectStore _projects;
    private readonly IProjectLogger _logger;
    private readonly Func<DateTimeOffset> _now;

    public LegacyFeedMigrator(IFileSystem fileSystem, AdministrationPaths paths, IProjectHttpClientFactory httpClientFactory, IFeedStore feeds, IPackageSigner signer,
        ITransferProviderFactory transferFactory, IStatisticsApi statistics, IProjectStore projects, IProjectLogger logger)
        : this(fileSystem, paths, httpClientFactory, feeds, signer, transferFactory, statistics, projects, logger, () => DateTimeOffset.UtcNow)
    {
    }

    public LegacyFeedMigrator(IFileSystem fileSystem, AdministrationPaths paths, IProjectHttpClientFactory httpClientFactory, IFeedStore feeds, IPackageSigner signer,
        ITransferProviderFactory transferFactory, IStatisticsApi statistics, IProjectStore projects, IProjectLogger logger, Func<DateTimeOffset> now)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _paths = paths ?? throw new ArgumentNullException(nameof(paths));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _feeds = feeds ?? throw new ArgumentNullException(nameof(feeds));
        _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        _transferFactory = transferFactory ?? throw new ArgumentNullException(nameof(transferFactory));
        _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        _projects = projects ?? throw new ArgumentNullException(nameof(projects));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _now = now ?? throw new ArgumentNullException(nameof(now));
        _deployer = new StatisticsDeployer(_fileSystem, _transferFactory, _statistics);
    }

    public async Task<MigrationStatus> CheckAsync(UpdateProject project, ProjectSecrets secrets, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        var legacy = await _feeds.LegacyFeedExistsAsync(project, secrets, cancellationToken).ConfigureAwait(false);
        var current = await _feeds.LoadRemoteAsync(project, secrets, cancellationToken).ConfigureAwait(false) is not null;
        return new MigrationStatus(legacy, current);
    }

    public async Task<MigrationPlan> PrepareAsync(UpdateProject project, ProjectSecrets secrets, IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        progress?.Report(new PipelineProgress("Reading the feeds on the server", 0, 1));
        var unreadable = new List<string>();
        var legacyEntries = await LoadLegacyFeedAsync(project, secrets, unreadable, cancellationToken).ConfigureAwait(false);
        var feed = await _feeds.LoadRemoteAsync(project, secrets, cancellationToken).ConfigureAwait(false);
        var entries = (legacyEntries ?? []).OrderBy(e => e.Version).ToList();
        var downloads = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), "nupdate-migration-" + Guid.NewGuid().ToString("N"));
        var packages = new List<MigrationPackage>();
        try
        {
            for (var i = 0; i < entries.Count; i++)
            {
                var legacy = entries[i];
                if (feed is not null && feed.Packages.Any(p => p.Version == legacy.Version))
                {
                    packages.Add(MigrationPackage.Migrated(legacy));
                    continue;
                }

                // Two spellings of one version would land in the same packages/<version>/ folder and twice in nupdate.json.
                if (packages.FirstOrDefault(p => p.Version == legacy.Version) is { } first)
                {
                    packages.Add(MigrationPackage.Failed(legacy, null, $"updates.json lists this version twice, as {first.LiteralVersion} and {legacy.LiteralVersion}; only {first.LiteralVersion} is migrated."));
                    continue;
                }

                if (_fileSystem.Directory.Exists(project.PackageDirectory(legacy.Version)))
                {
                    packages.Add(MigrationPackage.Failed(legacy, null, $"This project has a package {legacy.Version} of its own already ({project.PackageDirectory(legacy.Version)}). Publish or delete it instead of migrating the old one."));
                    continue;
                }

                progress?.Report(new PipelineProgress($"Reading {legacy.Version}", i + 1, entries.Count + 1));
                packages.Add(await AnalyzeAsync(project, secrets, legacy, downloads, cancellationToken).ConfigureAwait(false));
            }
        }
        catch
        {
            DeleteDirectory(downloads);
            throw;
        }

        return new MigrationPlan(project.Id, legacyEntries is not null, feed, packages, () => DeleteDirectory(downloads), unreadable);
    }

    public async Task<IReadOnlyList<UpdateVersion>> RunAsync(UpdateProject project, ProjectSecrets secrets, MigrationPlan plan, IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        ArgumentNullException.ThrowIfNull(plan);
        if (plan.ProjectId != project.Id)
            throw new ArgumentException("The migration was prepared for another project.", nameof(plan));
        var included = plan.Included.ToList();
        if (included.Count == 0 && plan.ExistingFeed is not null)
            return [];
        var privateKey = included.Count == 0
            ? string.Empty
            : secrets.PrivateKey ?? throw new InvalidOperationException("The private key of the project is missing. Enter the project credentials first.");
        var statisticsEndpoint = project.Statistics.Enabled ? PublishService.Endpoint(project, secrets) : null;

        var previous = plan.ExistingFeed;
        var feed = previous ?? new UpdateFeed { ProjectId = project.Id };
        var migrated = new List<UpdateVersion>();
        var entries = new List<PackageInfo>();
        var pipeline = new CompensatingPipeline();
        foreach (var package in included)
        {
            var entry = BuildEntry(package.Legacy, project);
            var file = entry.Files.Single();
            var packageDirectory = project.PackageDirectory(package.Version);
            var packagePath = project.PackageFilePath(package.Version, file.Platform);
            pipeline.Add($"Repacking {package.Version}", async ct =>
            {
                var manifest = await RepackAsync(package, project.Id, file.Platform, packagePath, ct).ConfigureAwait(false);
                file.Touches = manifest.Touches.ToList();
                file.Size = _fileSystem.FileInfo.New(packagePath).Length;
                file.Sha512 = _signer.Hash(packagePath);
                file.Signature.Value = _signer.Sign(packagePath, privateKey);
                await _feeds.SaveEntryAsync(project, entry, ct).ConfigureAwait(false);
            }, () =>
            {
                DeleteDirectory(packageDirectory);
                return Task.CompletedTask;
            }, compensatesOwnFailure: true); // a half-written package must not block the next attempt
            pipeline.Add($"Uploading {package.Version}", async ct =>
            {
                await using var transfer = await ConnectAsync(project, secrets, ct).ConfigureAwait(false);
                await transfer.CreateDirectoryAsync(PackageLayout.RemoteVersionDirectory(package.Version), ct).ConfigureAwait(false);
                await transfer.UploadFileAsync(packagePath, file.Path, null, ct).ConfigureAwait(false);
                entries.Add(entry);
            }, async () =>
            {
                await using var transfer = await ConnectAsync(project, secrets, CancellationToken.None).ConfigureAwait(false);
                await transfer.DeleteDirectoryAsync(PackageLayout.RemoteVersionDirectory(package.Version), CancellationToken.None).ConfigureAwait(false);
            });
        }

        pipeline.Add("Uploading the feed", async ct =>
        {
            var publishedAt = _now();
            foreach (var entry in entries)
                entry.PublishedAt = publishedAt;
            var updated = new UpdateFeed { ProjectId = project.Id, Packages = feed.Packages.Concat(entries).OrderBy(p => p.Version).ToList() };
            await using var transfer = await ConnectAsync(project, secrets, ct).ConfigureAwait(false);
            await _feeds.UploadAsync(transfer, updated, ct).ConfigureAwait(false);
        }, async () =>
        {
            await using var transfer = await ConnectAsync(project, secrets, CancellationToken.None).ConfigureAwait(false);
            if (previous is not null)
                await _feeds.UploadAsync(transfer, previous, CancellationToken.None).ConfigureAwait(false);
            else
                await transfer.DeleteFileAsync(UpdateFeed.FileName, CancellationToken.None).ConfigureAwait(false);
        });
        if (statisticsEndpoint is not null)
        {
            // The script of this version goes next to the statistics.php of nUpdate 3 and 4, which keeps serving old clients.
            pipeline.Add("Setting up the statistics", ct => _deployer.DeployAsync(project, secrets, ct));
            if (included.Count > 0)
            {
                pipeline.Add("Registering the versions in the statistics", async ct =>
                {
                    foreach (var entry in entries)
                        await _statistics.RegisterVersionAsync(statisticsEndpoint, project.Id, entry.Version, ct).ConfigureAwait(false);
                });
            }
        }

        if (included.Count > 0)
        {
            pipeline.Add("Updating the project", async ct =>
            {
                // The in-memory project is the caller's; when the save fails it must look as before.
                var added = new List<UpdatePackage>();
                var released = new List<UpdatePackage>();
                var logCount = project.Log.Count;
                foreach (var entry in entries)
                {
                    var package = project.FindPackage(entry.Version);
                    if (package is null)
                    {
                        package = new UpdatePackage { Version = entry.Version, CreatedAt = entry.PublishedAt };
                        project.Packages.Add(package);
                        added.Add(package);
                    }
                    else if (!package.Released)
                    {
                        released.Add(package);
                    }

                    package.Released = true;
                    migrated.Add(entry.Version);
                    _logger.Write(project, LogEntryKind.Migrate, entry.Version);
                }

                try
                {
                    await _projects.SaveAsync(project, ct).ConfigureAwait(false);
                }
                catch
                {
                    foreach (var package in added)
                        project.Packages.Remove(package);
                    foreach (var package in released)
                        package.Released = false;
                    while (project.Log.Count > logCount)
                        project.Log.RemoveAt(project.Log.Count - 1);
                    migrated.Clear();
                    throw;
                }
            });
        }

        await pipeline.RunAsync(progress, cancellationToken).ConfigureAwait(false);
        return migrated;
    }

    public async Task<LegacyFiles> FindLegacyFilesAsync(UpdateProject project, ProjectSecrets secrets, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        var entries = await LoadLegacyFeedAsync(project, secrets, [], cancellationToken).ConfigureAwait(false);
        var serverFiles = new List<string>();
        var serverDirectories = new List<string>();
        if (entries is not null)
        {
            serverFiles.Add(LegacyFeed.FileName);
            serverDirectories.AddRange(entries.Select(e => e.RemoteDirectory).Distinct(StringComparer.Ordinal));
        }

        await using (var transfer = await ConnectAsync(project, secrets, cancellationToken).ConfigureAwait(false))
        {
            if (await transfer.FileExistsAsync(StatisticsScript.LegacyScriptFileName, cancellationToken).ConfigureAwait(false))
                serverFiles.Add(StatisticsScript.LegacyScriptFileName);
        }

        // Only folders that hold this project's old zip, and never one the project itself or its old file lives in.
        var local = (entries ?? []).SelectMany(e => LegacyPackageDirectories(project, e))
            .Distinct(StringComparer.Ordinal)
            .Where(d => _fileSystem.File.Exists(_fileSystem.Path.Combine(d, $"{project.Id}.zip")))
            .Where(d => !Contains(d, project.Folder) && (project.LegacyProjectFile is null || !Contains(d, project.LegacyProjectFile)))
            .ToList();
        return new LegacyFiles(serverFiles, serverDirectories, local);
    }

    public async Task DeleteLegacyFilesAsync(UpdateProject project, ProjectSecrets secrets, LegacyFiles files, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        ArgumentNullException.ThrowIfNull(files);
        // updates.json goes last: it is what lists the package folders, so an interrupted run can be repeated.
        foreach (var directory in files.LocalDirectories)
            DeleteDirectory(directory);
        if (files.ServerFiles.Count == 0 && files.ServerDirectories.Count == 0)
            return;
        await using var transfer = await ConnectAsync(project, secrets, cancellationToken).ConfigureAwait(false);
        foreach (var directory in files.ServerDirectories)
            await transfer.DeleteDirectoryAsync(directory, cancellationToken).ConfigureAwait(false);
        foreach (var file in files.ServerFiles.OrderBy(f => f == LegacyFeed.FileName ? 1 : 0))
            await transfer.DeleteFileAsync(file, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Builds the new feed entry of a legacy entry with its one file (for <see cref="LegacyFeedEntry.Platform" />), without size, hash and signature.</summary>
    public static PackageInfo BuildEntry(LegacyFeedEntry legacy, UpdateProject project)
    {
        ArgumentNullException.ThrowIfNull(legacy);
        ArgumentNullException.ThrowIfNull(project);
        return new PackageInfo
        {
            Version = legacy.Version,
            Necessary = legacy.Necessary,
            Changelog = new Dictionary<string, string>(legacy.Changelog, StringComparer.OrdinalIgnoreCase),
            UnsupportedVersions = legacy.UnsupportedVersions.ToList(),
            Rollout = legacy.Rollout,
            Files = [new PackageFile { Platform = legacy.Platform, Path = PackageLayout.RemotePackagePath(legacy.Version, legacy.Platform) }],
            Statistics = project.Statistics.Enabled ? new PackageStatistics { Url = PublishService.StatisticsUrl(project), Enabled = legacy.UseStatistics } : null,
        };
    }

    /// <summary>The entries of <c>updates.json</c>, or <c>null</c> when the server has none; entries without a readable version go to <paramref name="unreadable" />.</summary>
    private async Task<List<LegacyFeedEntry>?> LoadLegacyFeedAsync(UpdateProject project, ProjectSecrets secrets, ICollection<string> unreadable, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.Create(project, secrets);
        using var response = await client.GetAsync(project.Resolve(LegacyFeed.FileName), cancellationToken).ConfigureAwait(false);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return LegacyFeed.Parse(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false), unreadable);
    }

    /// <summary>Whether <paramref name="path" /> is <paramref name="directory" /> or lies below it.</summary>
    private bool Contains(string directory, string path)
    {
        var parent = _fileSystem.Path.GetFullPath(directory).TrimEnd(_fileSystem.Path.DirectorySeparatorChar, _fileSystem.Path.AltDirectorySeparatorChar);
        var child = _fileSystem.Path.GetFullPath(path);
        return string.Equals(child, parent, StringComparison.OrdinalIgnoreCase)
               || child.StartsWith(parent + _fileSystem.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Where earlier versions kept the local copy of a package (<c>&lt;literal version&gt;/&lt;project id&gt;.zip</c>):
    ///     next to the project file (the 5.0 pre-releases, before and after the conversion), or in the data folder of
    ///     nUpdate Administration 4.
    /// </summary>
    private IEnumerable<string> LegacyPackageDirectories(UpdateProject project, LegacyFeedEntry legacy)
    {
        yield return _fileSystem.Path.Combine(project.Folder, legacy.LiteralVersion);
        if (project.LegacyProjectFile is { } legacyFile && _fileSystem.Path.GetDirectoryName(legacyFile) is { Length: > 0 } legacyFolder)
            yield return _fileSystem.Path.Combine(legacyFolder, legacy.LiteralVersion);
        if (LegacyDataDirectory(project) is { } data)
            yield return _fileSystem.Path.Combine(data, legacy.LiteralVersion);
    }

    /// <summary>
    ///     The folder of the project in the data folder of nUpdate Administration 4, unless the name cannot be a single
    ///     folder name there (separators, only dots, or trailing dots and spaces, which Windows removes).
    /// </summary>
    private string? LegacyDataDirectory(UpdateProject project)
    {
        var name = project.Name;
        if (string.IsNullOrWhiteSpace(name) || name.TrimEnd('.', ' ').Length != name.Length || name.IndexOfAny(['/', '\\', ':', '\0']) >= 0)
            return null;
        return _paths.LegacyProjectDataDirectory(name);
    }

    /// <summary>
    ///     Finds or downloads the old zip, checks that it carries the signature <c>updates.json</c> names for it (so nothing
    ///     the old clients would reject gets signed anew) and reads what the new package will contain.
    /// </summary>
    private async Task<MigrationPackage> AnalyzeAsync(UpdateProject project, ProjectSecrets secrets, LegacyFeedEntry legacy, string downloads, CancellationToken cancellationToken)
    {
        if (legacy.Signature is null)
            return MigrationPackage.Failed(legacy, legacy.PackageUri?.ToString(), "updates.json has no signature for this package, so it cannot be checked before it is signed for nUpdate 5.");
        var local = LegacyPackageDirectories(project, legacy).Select(d => _fileSystem.Path.Combine(d, $"{project.Id}.zip")).FirstOrDefault(f => _fileSystem.File.Exists(f) && IsSigned(project, legacy, f));
        var source = local ?? legacy.PackageUri?.ToString();
        if (source is null)
            return MigrationPackage.Failed(legacy, null, "The package is neither on this computer nor named in updates.json.");
        try
        {
            var path = local ?? await DownloadAsync(project, secrets, legacy.PackageUri!, downloads, cancellationToken).ConfigureAwait(false);
            if (local is null && !IsSigned(project, legacy, path))
                return MigrationPackage.Failed(legacy, source, "The downloaded zip does not carry the signature updates.json names for it, so it may have been changed. It is not migrated.");
            LegacyOperationConversion operations;
            List<string> files;
            await using (var stream = _fileSystem.File.OpenRead(path))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                operations = archive.GetEntry(LegacyOperationsFileName) is { } operationsEntry
                    ? LegacyOperationConverter.Convert(await ReadArrayAsync(operationsEntry, cancellationToken).ConfigureAwait(false))
                    : LegacyOperationConverter.Convert(legacy.Operations);
                files = archive.Entries.Where(e => !e.FullName.EndsWith('/')).Select(e => e.FullName).ToList();
            }

            var packaged = files.Count(f => PackageFileEntry.TryParseEntryName(f, out _, out _));
            var skipped = files.Where(f => f != LegacyOperationsFileName && !PackageFileEntry.TryParseEntryName(f, out _, out _)).ToList();
            return MigrationPackage.Ready(legacy, source, path, _fileSystem.FileInfo.New(path).Length, packaged, skipped, operations);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return MigrationPackage.Failed(legacy, source, "The download did not finish in time.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Old packages come in every shape; one that cannot be read must not stop the review of the others.
            return MigrationPackage.Failed(legacy, source, ex.Message);
        }
    }

    private bool IsSigned(UpdateProject project, LegacyFeedEntry legacy, string path)
    {
        using var stream = _fileSystem.File.OpenRead(path);
        return LegacySignature.Verify(stream, project.PublicKey, legacy.Signature);
    }

    private async Task<string> DownloadAsync(UpdateProject project, ProjectSecrets secrets, Uri uri, string downloads, CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.Create(project, secrets);
        using var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        _fileSystem.Directory.CreateDirectory(downloads);
        var path = _fileSystem.Path.Combine(downloads, Guid.NewGuid().ToString("N") + ".zip");
        await using var target = _fileSystem.File.Create(path);
        await response.Content.CopyToAsync(target, cancellationToken).ConfigureAwait(false);
        return path;
    }

    /// <summary>Writes the new zip: the files below the known roots plus a manifest with the converted operations.</summary>
    private async Task<PackageManifest> RepackAsync(MigrationPackage package, Guid projectId, string platform, string packagePath, CancellationToken cancellationToken)
    {
        var manifest = new PackageManifest { ProjectId = projectId, Version = package.Version, Platform = platform, CreatedAt = _now(), Operations = package.Operations.ToList() };
        var manifestJson = Serializer.Serialize(manifest, indented: true);

        var directory = _fileSystem.Path.GetDirectoryName(packagePath)!;
        _fileSystem.Directory.CreateDirectory(directory);
        await using (var input = _fileSystem.File.OpenRead(package.SourcePath!))
        using (var source = new ZipArchive(input, ZipArchiveMode.Read))
        await using (var output = _fileSystem.File.Create(packagePath))
        using (var target = new ZipArchive(output, ZipArchiveMode.Create))
        {
            foreach (var entry in source.Entries.Where(e => !e.FullName.EndsWith('/') && PackageFileEntry.TryParseEntryName(e.FullName, out _, out _)))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var copy = target.CreateEntry(entry.FullName, CompressionLevel.Optimal);
                using var from = entry.Open();
                using var to = copy.Open();
                await from.CopyToAsync(to, cancellationToken).ConfigureAwait(false);
            }

            using var manifestStream = target.CreateEntry(PackageLayout.ManifestFileName, CompressionLevel.Optimal).Open();
            await manifestStream.WriteAsync(Encoding.UTF8.GetBytes(manifestJson), cancellationToken).ConfigureAwait(false);
        }

        await _fileSystem.File.WriteAllTextAsync(_fileSystem.Path.Combine(directory, PackageLayout.ManifestFileName), manifestJson, cancellationToken).ConfigureAwait(false);
        return manifest;
    }

    private void DeleteDirectory(string directory)
    {
        if (_fileSystem.Directory.Exists(directory))
            _fileSystem.Directory.Delete(directory, recursive: true);
    }

    private static async Task<Newtonsoft.Json.Linq.JArray?> ReadArrayAsync(ZipArchiveEntry entry, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
        var content = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(content) ? null : Newtonsoft.Json.Linq.JToken.Parse(content) as Newtonsoft.Json.Linq.JArray;
    }

    private async Task<ITransferProvider> ConnectAsync(UpdateProject project, ProjectSecrets secrets, CancellationToken cancellationToken)
    {
        var transfer = _transferFactory.Create(project.Transfer, secrets.ToTransferCredentials());
        Exception? failure = null;
        try
        {
            await transfer.ConnectAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            failure = ex;
        }

        if (failure is null)
            return transfer;

        await transfer.DisposeAsync().ConfigureAwait(false);
        throw failure; // the provider's exception carries the original cause as InnerException
    }
}
