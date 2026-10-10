using System.Globalization;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Packages;
using nUpdate.Administration.Core.Projects;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.TransferInterface;
using nUpdate.Packaging;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Publishing;

/// <summary>Creates packages and publishes them; every step that changed something is undone when a later one fails.</summary>
public interface IPublishService
{
    /// <summary>Builds and signs the package file of every platform, registers them and (optionally) uploads them.</summary>
    /// <exception cref="MigrationRequiredException">The request publishes and the server still serves only the legacy feed.</exception>
    Task<UpdatePackage> CreatePackageAsync(PublishRequest request, IProgress<PipelineProgress>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>Uploads a package that was created without publishing.</summary>
    Task PublishExistingAsync(UpdateProject project, ProjectSecrets secrets, UpdateVersion version,
        IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>Removes a package locally, from the server and from the statistics.</summary>
    Task DeletePackageAsync(UpdateProject project, ProjectSecrets secrets, UpdateVersion version,
        IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>Replaces the feed entry of a package (changelog, conditions and so on) locally and, when released, on the server.</summary>
    Task UpdateEntryAsync(UpdateProject project, ProjectSecrets secrets, PackageInfo entry,
        IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Extracts the package files of an existing version into <paramref name="workingDirectory" /> (one sub folder per
    ///     platform) and returns their files and operations, so they can be changed and built again with
    ///     <see cref="RebuildPackageAsync" />.
    /// </summary>
    /// <param name="project">The project of the package.</param>
    /// <param name="version">The version of the package.</param>
    /// <param name="workingDirectory">
    ///     A folder outside the project's <c>packages</c> folder that the files are extracted to; it has to stay until the
    ///     rebuild is done, and the caller deletes it afterwards.
    /// </param>
    /// <param name="cancellationToken">Cancels the extraction.</param>
    /// <exception cref="FileNotFoundException">The local package file of a platform is missing.</exception>
    Task<PackageDefinition> OpenPackageAsync(UpdateProject project, UpdateVersion version, string workingDirectory,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Builds the package files of an existing version again; the version stays the same. <paramref name="rebuild" />
    ///     names the platforms of the request's package whose files or operations changed: each is built and signed again
    ///     and replaces its local package file. The other platforms of the request keep their package file as it is (their
    ///     files and operations in the request are not looked at), and platforms the request no longer has are removed.
    ///     The feed entry is built from the request like a new one, but keeps its publishing time. A released version goes
    ///     to the server again: the rebuilt files under new names (see <see cref="PublishService.RevisionPath" />), then
    ///     the feed, then the replaced files are deleted, so no client downloads a file whose hash does not match its feed.
    ///     Clients that installed the version keep what they have. A local package stays local;
    ///     <see cref="PublishRequest.Publish" /> is ignored.
    /// </summary>
    /// <exception cref="MigrationRequiredException">The version is released and the server still serves only the legacy feed.</exception>
    Task<UpdatePackage> RebuildPackageAsync(PublishRequest request, IReadOnlyCollection<string> rebuild,
        IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default);
}

public sealed class PublishService(
    IFileSystem fileSystem,
    IPackageBuilder builder,
    IPackageSigner signer,
    IFeedStore feeds,
    ITransferProviderFactory transferFactory,
    IStatisticsApi statistics,
    IProjectStore projects,
    IProjectLogger logger,
    IPackageContentReader contentReader,
    Func<DateTimeOffset> now)
    : IPublishService
{
    private readonly IFileSystem _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    private readonly IPackageBuilder _builder = builder ?? throw new ArgumentNullException(nameof(builder));
    private readonly IPackageSigner _signer = signer ?? throw new ArgumentNullException(nameof(signer));
    private readonly IFeedStore _feeds = feeds ?? throw new ArgumentNullException(nameof(feeds));

    private readonly ITransferProviderFactory _transferFactory =
        transferFactory ?? throw new ArgumentNullException(nameof(transferFactory));

    private readonly IStatisticsApi _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
    private readonly IProjectStore _projects = projects ?? throw new ArgumentNullException(nameof(projects));
    private readonly IProjectLogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly IPackageContentReader _contentReader =
        contentReader ?? throw new ArgumentNullException(nameof(contentReader));

    private readonly Func<DateTimeOffset> _now = now ?? throw new ArgumentNullException(nameof(now));

    public PublishService(IFileSystem fileSystem, IPackageBuilder builder, IPackageSigner signer, IFeedStore feeds,
        ITransferProviderFactory transferFactory, IStatisticsApi statistics, IProjectStore projects,
        IProjectLogger logger, IPackageContentReader contentReader)
        : this(fileSystem, builder, signer, feeds, transferFactory, statistics, projects, logger, contentReader,
            () => DateTimeOffset.UtcNow)
    {
    }

    public PublishService(IFileSystem fileSystem, IPackageBuilder builder, IPackageSigner signer, IFeedStore feeds,
        ITransferProviderFactory transferFactory, IStatisticsApi statistics, IProjectStore projects,
        IProjectLogger logger)
        : this(fileSystem, builder, signer, feeds, transferFactory, statistics, projects, logger,
            new PackageContentReader(fileSystem))
    {
    }

    public async Task<UpdatePackage> CreatePackageAsync(PublishRequest request,
        IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var project = request.Project;
        var version = request.Package.Version;
        Validate(request);

        var packageDirectory = project.PackageDirectory(version);
        var package = new UpdatePackage
        { Version = version, Description = request.Description, Released = false, CreatedAt = _now() };
        var entry = BuildEntry(request, project);
        UpdateFeed? feed = null;

        var pipeline = new CompensatingPipeline()
            .Add("Building the package", async ct =>
                {
                    foreach (var platform in request.Package.Platforms)
                    {
                        var packagePath = project.PackageFilePath(version, platform.Platform);
                        var manifest = await _builder.BuildAsync(platform, version, project.Id, packagePath, ct)
                            .ConfigureAwait(false);
                        entry.Files.Add(new PackageFile
                        {
                            Platform = platform.Platform,
                            Path = PackageLayout.RemotePackagePath(version, platform.Platform),
                            Size = _fileSystem.FileInfo.New(packagePath).Length,
                            Sha512 = _signer.Hash(packagePath),
                            Touches = manifest.Touches.ToList(),
                        });
                    }
                }, () => DeleteLocalPackage(packageDirectory),
                compensatesOwnFailure: true) // a later platform may fail after earlier ones were built
            .Add("Signing the package", _ =>
            {
                var privateKey = RequirePrivateKey(request.Secrets);
                foreach (var file in entry.Files)
                    file.Signature.Value = _signer.Sign(project.PackageFilePath(version, file.Platform), privateKey);
                return Task.CompletedTask;
            })
            .Add("Registering the package", async ct =>
            {
                await _feeds.SaveEntryAsync(project, entry, ct).ConfigureAwait(false);
                project.Packages.Add(package);
                _logger.Write(project, LogEntryKind.Create, version);
            }, () =>
            {
                project.Packages.Remove(package);
                return Task.CompletedTask;
            });

        if (request.Publish)
        {
            pipeline.Add("Loading the current feed",
                async ct => feed =
                    await LoadFeedForPublishingAsync(project, request.Secrets, ct).ConfigureAwait(false));
            AddUploadSteps(pipeline, project, request.Secrets, package, entry, () => feed);
        }

        pipeline.Add("Saving the project", ct => _projects.SaveAsync(project, ct));

        await pipeline.RunAsync(progress, cancellationToken).ConfigureAwait(false);
        return package;
    }

    public async Task PublishExistingAsync(UpdateProject project, ProjectSecrets secrets, UpdateVersion version,
        IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        ArgumentNullException.ThrowIfNull(version);
        var package = project.FindPackage(version) ??
                      throw new InvalidOperationException($"The project has no package {version}.");
        if (package.Released)
            throw new InvalidOperationException($"The package {version} is already released.");
        if (project.Statistics.Enabled && string.IsNullOrEmpty(secrets.StatisticsAdminSecret))
            throw new InvalidOperationException("The statistics admin secret is missing.");

        var entry = await _feeds.LoadEntryAsync(project, version, cancellationToken).ConfigureAwait(false)
                    ?? throw new InvalidOperationException($"The local feed entry of {version} is missing.");
        foreach (var file in entry.Files)
        {
            var packagePath = project.PackageFilePath(version, file.Platform);
            if (!_fileSystem.File.Exists(packagePath))
                throw new FileNotFoundException($"The {file.Platform} package file of {version} is missing.",
                    packagePath);
        }

        UpdateFeed? feed = null;
        var pipeline = new CompensatingPipeline()
            .Add("Loading the current feed",
                async ct => feed = await LoadFeedForPublishingAsync(project, secrets, ct).ConfigureAwait(false));
        AddUploadSteps(pipeline, project, secrets, package, entry, () => feed);
        pipeline.Add("Saving the project", ct => _projects.SaveAsync(project, ct));
        await pipeline.RunAsync(progress, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeletePackageAsync(UpdateProject project, ProjectSecrets secrets, UpdateVersion version,
        IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        ArgumentNullException.ThrowIfNull(version);
        var package = project.FindPackage(version) ??
                      throw new InvalidOperationException($"The project has no package {version}.");
        var packageDirectory = project.PackageDirectory(version);
        if (package.Released && project.Statistics.Enabled)
            _ = Endpoint(project, secrets); // fail before anything is removed when the admin secret is missing
        var pipeline = new CompensatingPipeline();

        if (package.Released)
        {
            UpdateFeed? previous = null;
            pipeline.Add("Removing the package from the feed", async ct =>
            {
                previous = await LoadFeedForPublishingAsync(project, secrets, ct).ConfigureAwait(false);
                var remote = previous ?? new UpdateFeed { ProjectId = project.Id };
                var updated = new UpdateFeed
                {
                    ProjectId = remote.ProjectId,
                    Packages = remote.Packages.Where(p => p.Version != version).ToList()
                };
                await using var transfer = await ConnectAsync(project, secrets, ct).ConfigureAwait(false);
                await _feeds.UploadAsync(transfer, updated, ct).ConfigureAwait(false);
            }, () => RestoreFeedAsync(project, secrets, () => previous));
            if (project.Statistics.Enabled)
            {
                pipeline.Add("Removing the statistics",
                    ct => _statistics.DeleteVersionAsync(Endpoint(project, secrets), project.Id, version, ct),
                    () => _statistics.RegisterVersionAsync(Endpoint(project, secrets), project.Id, version,
                        CancellationToken.None));
            }

            pipeline.Add("Deleting the package from the server", async ct =>
            {
                await using var transfer = await ConnectAsync(project, secrets, ct).ConfigureAwait(false);
                await transfer.DeleteDirectoryAsync(PackageLayout.RemoteVersionDirectory(version), ct)
                    .ConfigureAwait(false);
            });
        }

        pipeline.Add("Deleting the local package", _ => DeleteLocalPackage(packageDirectory));
        pipeline.Add("Saving the project", ct =>
        {
            project.Packages.Remove(package);
            _logger.Write(project, LogEntryKind.Delete, version);
            return _projects.SaveAsync(project, ct);
        });
        await pipeline.RunAsync(progress, cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateEntryAsync(UpdateProject project, ProjectSecrets secrets, PackageInfo entry,
        IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        ArgumentNullException.ThrowIfNull(entry);
        var version = entry.Version;
        var package = project.FindPackage(version) ??
                      throw new InvalidOperationException($"The project has no package {version}.");

        var pipeline = new CompensatingPipeline()
            .Add("Updating the local feed entry", ct => _feeds.SaveEntryAsync(project, entry, ct));
        if (package.Released)
        {
            pipeline.Add("Updating the feed on the server", async ct =>
            {
                var remote = await LoadFeedForPublishingAsync(project, secrets, ct).ConfigureAwait(false) ??
                             new UpdateFeed { ProjectId = project.Id };
                var updated = new UpdateFeed
                { ProjectId = remote.ProjectId, Packages = Replace(remote.Packages, entry) };
                await using var transfer = await ConnectAsync(project, secrets, ct).ConfigureAwait(false);
                await _feeds.UploadAsync(transfer, updated, ct).ConfigureAwait(false);
            });
        }

        pipeline.Add("Saving the project", ct =>
        {
            _logger.Write(project, LogEntryKind.Edit, version);
            return _projects.SaveAsync(project, ct);
        });
        await pipeline.RunAsync(progress, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PackageDefinition> OpenPackageAsync(UpdateProject project, UpdateVersion version,
        string workingDirectory, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(workingDirectory);
        if (project.FindPackage(version) is null)
            throw new InvalidOperationException($"The project has no package {version}.");
        var entry = await _feeds.LoadEntryAsync(project, version, cancellationToken).ConfigureAwait(false)
                    ?? throw new InvalidOperationException($"The local feed entry of {version} is missing.");
        foreach (var file in entry.Files)
        {
            var packagePath = project.PackageFilePath(version, file.Platform);
            if (!_fileSystem.File.Exists(packagePath))
                throw new FileNotFoundException(
                    $"The {file.Platform} package file of {version} is missing, so its files cannot be changed. " +
                    $"It belongs at {packagePath}.",
                    packagePath);
        }

        var definition = new PackageDefinition(version);
        foreach (var file in entry.Files)
        {
            var content = await _contentReader.ExtractAsync(project.PackageFilePath(version, file.Platform),
                _fileSystem.Path.Combine(workingDirectory, file.Platform), cancellationToken).ConfigureAwait(false);
            var platform = definition.GetOrAddPlatform(file.Platform);
            platform.Files.AddRange(content.Entries.Select(e =>
                new PackageFileEntry(e.Root, e.RelativePath, e.ExtractedPath!)
                { UnixMode = e.Mode == 0 ? null : e.Mode, CodeSignature = e.CodeSignature }));
            platform.Operations.AddRange(content.Manifest?.Operations ?? []);
        }

        return definition;
    }

    public async Task<UpdatePackage> RebuildPackageAsync(PublishRequest request, IReadOnlyCollection<string> rebuild,
        IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(rebuild);
        var project = request.Project;
        var secrets = request.Secrets;
        var version = request.Package.Version;
        var platforms = request.Package.Platforms;
        var package = project.FindPackage(version) ??
                      throw new InvalidOperationException($"The project has no package {version}.");
        var unknown = rebuild.FirstOrDefault(name => platforms.All(p => p.Platform != name));
        if (unknown is not null)
            throw new ArgumentException($"The {unknown} package is to be built, but the package has no such platform.",
                nameof(rebuild));
        var rebuilt = platforms.Where(p => rebuild.Contains(p.Platform)).ToList();
        ValidateContent(request, rebuilt);
        var privateKey = RequirePrivateKey(secrets);
        var previous = await _feeds.LoadEntryAsync(project, version, cancellationToken).ConfigureAwait(false)
                       ?? throw new InvalidOperationException($"The local feed entry of {version} is missing.");
        var kept = previous.Files
            .Where(f => platforms.Any(p => p.Platform == f.Platform) && !rebuild.Contains(f.Platform)).ToList();
        var unbuilt = platforms.FirstOrDefault(p => !rebuilt.Contains(p) && kept.All(f => f.Platform != p.Platform));
        if (unbuilt is not null)
            throw new ArgumentException(
                $"The {unbuilt.Platform} package has no package file yet, so it has to be built.", nameof(rebuild));
        var replaced = previous.Files.Except(kept).ToList(); // the files of rebuilt and of removed platforms

        var entry = BuildEntry(request, project);
        entry.PublishedAt = previous.PublishedAt;
        var built = new List<PackageFile>();
        var backup = new PlatformBackup(_fileSystem, project, version);
        var description = package.Description;
        var logCount = project.Log.Count;
        UpdateFeed? feed = null;
        var uploaded = new List<string>();
        var deleted = new List<PackageFile>();

        var pipeline = new CompensatingPipeline();
        if (package.Released)
        {
            pipeline.Add("Loading the current feed",
                async ct => feed = await LoadFeedForPublishingAsync(project, secrets, ct).ConfigureAwait(false));
        }

        pipeline.Add("Building the package", async ct =>
            {
                foreach (var platform in rebuilt.Select(p => p.Platform).Union(replaced.Select(f => f.Platform)))
                    backup.MoveAside(platform);
                foreach (var platform in platforms)
                {
                    if (!rebuilt.Contains(platform))
                    {
                        entry.Files.Add(kept.First(f => f.Platform == platform.Platform));
                        continue;
                    }

                    backup.Building(platform.Platform);
                    var packagePath = project.PackageFilePath(version, platform.Platform);
                    var manifest = await _builder.BuildAsync(platform, version, project.Id, packagePath, ct)
                        .ConfigureAwait(false);
                    var current = previous.Files.FirstOrDefault(f => f.Platform == platform.Platform);
                    var file = new PackageFile
                    {
                        Platform = platform.Platform,
                        Path = package.Released // nothing of a local package is on the server, so its name stays
                            ? RevisionPath(version, platform.Platform, current?.Path ?? string.Empty)
                            : PackageLayout.RemotePackagePath(version, platform.Platform),
                        Size = _fileSystem.FileInfo.New(packagePath).Length,
                        Sha512 = _signer.Hash(packagePath),
                        Signature = new PackageSignature { Value = _signer.Sign(packagePath, privateKey) },
                        Touches = manifest.Touches.ToList(),
                    };
                    built.Add(file);
                    entry.Files.Add(file);
                }
            }, () =>
            {
                backup.Restore();
                return Task.CompletedTask;
            },
            compensatesOwnFailure: true); // a later platform may fail after earlier ones were moved aside or built

        if (package.Released)
        {
            pipeline.Add("Uploading the package", async ct =>
            {
                await using var transfer = await ConnectAsync(project, secrets, ct).ConfigureAwait(false);
                await transfer.CreateDirectoryAsync(PackageLayout.RemoteVersionDirectory(version), ct)
                    .ConfigureAwait(false);
                foreach (var file in built)
                {
                    uploaded.Add(file.Path);
                    await transfer.UploadFileAsync(project.PackageFilePath(version, file.Platform), file.Path, null, ct)
                        .ConfigureAwait(false);
                }
            }, async () =>
            {
                await using var transfer =
                    await ConnectAsync(project, secrets, CancellationToken.None).ConfigureAwait(false);
                foreach (var path in uploaded)
                    await transfer.DeleteFileAsync(path, CancellationToken.None).ConfigureAwait(false);
            }, compensatesOwnFailure: true); // a later file may fail after earlier ones were uploaded
        }

        pipeline.Add("Updating the local feed entry", async ct =>
        {
            await _feeds.SaveEntryAsync(project, entry, ct).ConfigureAwait(false);
            package.Description = request.Description;
        }, async () =>
        {
            package.Description = description;
            await _feeds.SaveEntryAsync(project, previous, CancellationToken.None).ConfigureAwait(false);
        });

        if (package.Released)
        {
            // The feed is the commit point: before it clients download the old files, after it the new ones.
            pipeline.Add("Uploading the feed", async ct =>
            {
                var updated = new UpdateFeed
                {
                    ProjectId = project.Id,
                    Packages = Replace(feed?.Packages ?? [], entry),
                };
                await using var transfer = await ConnectAsync(project, secrets, ct).ConfigureAwait(false);
                await _feeds.UploadAsync(transfer, updated, ct).ConfigureAwait(false);
            }, () => RestoreFeedAsync(project, secrets, () => feed));
            pipeline.Add("Deleting the replaced package files from the server", async ct =>
            {
                await using var transfer = await ConnectAsync(project, secrets, ct).ConfigureAwait(false);
                foreach (var file in replaced)
                {
                    deleted.Add(file);
                    await transfer.DeleteFileAsync(file.Path, ct).ConfigureAwait(false);
                }
            }, async () =>
            {
                // The previous feed is put back after this, so the files it lists have to be there again first.
                await using var transfer =
                    await ConnectAsync(project, secrets, CancellationToken.None).ConfigureAwait(false);
                foreach (var file in deleted)
                    await transfer.UploadFileAsync(backup.PackageFilePath(file.Platform), file.Path, null,
                        CancellationToken.None).ConfigureAwait(false);
            }, compensatesOwnFailure: true); // a later file may fail after earlier ones were deleted
        }

        pipeline.Add("Saving the project", ct =>
        {
            _logger.Write(project, LogEntryKind.Rebuild, version);
            return _projects.SaveAsync(project, ct);
        }, () =>
        {
            project.Log.RemoveRange(logCount, project.Log.Count - logCount);
            return Task.CompletedTask;
        }, compensatesOwnFailure: true);

        await pipeline.RunAsync(progress, cancellationToken).ConfigureAwait(false);
        backup.Delete();
        return package;
    }

    /// <summary>Builds the feed entry for a request, without the files, which building the packages adds.</summary>
    public static PackageInfo BuildEntry(PublishRequest request, UpdateProject project)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(project);
        var version = request.Package.Version;
        return new PackageInfo
        {
            Version = version,
            Changelog = request.Changelog.ToDictionary(pair => pair.Key.Name, pair => pair.Value,
                StringComparer.OrdinalIgnoreCase),
            Necessary = request.Necessary,
            AfterInstall = request.AfterInstall,
            UnsupportedVersions = request.UnsupportedVersions.ToList(),
            Rollout = new RolloutSettings
            {
                Mode = request.RolloutConditionMode,
                Conditions = request.RolloutConditions
                    .Where(c => !string.IsNullOrEmpty(c.Key) && !string.IsNullOrEmpty(c.Value)).ToList(),
            },
            Statistics = project.Statistics.Enabled
                ? new PackageStatistics { Url = StatisticsUrl(project), Enabled = request.IncludeInStatistics }
                : null,
        };
    }

    /// <summary>Entries with the version of <paramref name="entry" /> replaced by it (or appended), in version order.</summary>
    public static List<PackageInfo> Replace(IEnumerable<PackageInfo> packages, PackageInfo entry)
    {
        ArgumentNullException.ThrowIfNull(packages);
        ArgumentNullException.ThrowIfNull(entry);
        return packages.Where(p => p.Version != entry.Version).Append(entry).OrderBy(p => p.Version).ToList();
    }

    /// <summary>
    ///     The path a rebuilt package file of a released version gets on the server: one revision above the name in
    ///     <paramref name="currentPath" />, so <c>packages/1.0.0/win-x64.zip</c> becomes <c>packages/1.0.0/win-x64-r2.zip</c>
    ///     and <c>-r2</c> becomes <c>-r3</c>. A file never changes under its name, so a client that still has the previous
    ///     feed downloads the previous file, which matches it.
    /// </summary>
    /// <param name="version">The version of the package.</param>
    /// <param name="platform">The platform of the package file.</param>
    /// <param name="currentPath">The path of the platform's file in the feed entry, or an empty string when the version had none.</param>
    public static string RevisionPath(UpdateVersion version, string platform, string currentPath)
    {
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(platform);
        ArgumentNullException.ThrowIfNull(currentPath);
        var match = Regex.Match(currentPath, $@"(?:^|/){Regex.Escape(platform)}-r([0-9]{{1,9}})\.zip$",
            RegexOptions.CultureInvariant);
        var revision = match.Success ? int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) : 1;
        return $"{PackageLayout.RemoteVersionDirectory(version)}/{platform}-r{revision + 1}.zip";
    }

    /// <summary>
    ///     The upload steps shared by creating and re-publishing: zips, feed entry and statistics. The files of
    ///     <paramref name="entry" /> may still be added by an earlier step.
    ///     <paramref name="feed" /> is the feed on the server before this publish, or <c>null</c> when there was none; the
    ///     rollback restores exactly that.
    /// </summary>
    private void AddUploadSteps(CompensatingPipeline pipeline, UpdateProject project, ProjectSecrets secrets,
        UpdatePackage package, PackageInfo entry, Func<UpdateFeed?> feed)
    {
        var version = package.Version;
        var remoteDirectory = PackageLayout.RemoteVersionDirectory(version);
        pipeline.Add("Uploading the package", async ct =>
        {
            await using var transfer = await ConnectAsync(project, secrets, ct).ConfigureAwait(false);
            await transfer.CreateDirectoryAsync(remoteDirectory, ct).ConfigureAwait(false);
            foreach (var file in entry.Files)
                await transfer.UploadFileAsync(project.PackageFilePath(version, file.Platform), file.Path, null, ct)
                    .ConfigureAwait(false);
        }, async () =>
        {
            await using var transfer =
                await ConnectAsync(project, secrets, CancellationToken.None).ConfigureAwait(false);
            await transfer.DeleteDirectoryAsync(remoteDirectory, CancellationToken.None).ConfigureAwait(false);
        }, compensatesOwnFailure: true); // a later file may fail after earlier ones were uploaded
        pipeline.Add("Uploading the feed", async ct =>
        {
            entry.PublishedAt = _now();
            var current = feed()?.Packages ?? [];
            var updated = new UpdateFeed { ProjectId = project.Id, Packages = Replace(current, entry) };
            await _feeds.SaveEntryAsync(project, entry, ct).ConfigureAwait(false);
            await using var transfer = await ConnectAsync(project, secrets, ct).ConfigureAwait(false);
            await _feeds.UploadAsync(transfer, updated, ct).ConfigureAwait(false);
        }, () => RestoreFeedAsync(project, secrets, feed));
        if (project.Statistics.Enabled)
        {
            pipeline.Add("Registering the version in the statistics",
                ct => _statistics.RegisterVersionAsync(Endpoint(project, secrets), project.Id, version, ct),
                () => _statistics.DeleteVersionAsync(Endpoint(project, secrets), project.Id, version,
                    CancellationToken.None));
        }

        pipeline.Add("Marking the package as released", _ =>
        {
            package.Released = true;
            _logger.Write(project, LogEntryKind.Upload, version);
            return Task.CompletedTask;
        }, () =>
        {
            package.Released = false;
            return Task.CompletedTask;
        });
    }

    /// <summary>The feed on the server, or <c>null</c> when there is none yet; refuses when only the legacy feed exists.</summary>
    private async Task<UpdateFeed?> LoadFeedForPublishingAsync(UpdateProject project, ProjectSecrets secrets,
        CancellationToken cancellationToken)
    {
        var feed = await _feeds.LoadRemoteAsync(project, secrets, cancellationToken).ConfigureAwait(false);
        if (feed is not null)
            return feed;
        if (await _feeds.LegacyFeedExistsAsync(project, secrets, cancellationToken).ConfigureAwait(false))
            throw new MigrationRequiredException();
        return null;
    }

    /// <summary>Puts the feed back the way it was. When there was none the file is removed instead of leaving an empty feed that did not exist before.</summary>
    private async Task RestoreFeedAsync(UpdateProject project, ProjectSecrets secrets, Func<UpdateFeed?> previous)
    {
        await using var transfer = await ConnectAsync(project, secrets, CancellationToken.None).ConfigureAwait(false);
        var feed = previous();
        if (feed is null)
            await transfer.DeleteFileAsync(UpdateFeed.FileName, CancellationToken.None).ConfigureAwait(false);
        else
            await _feeds.UploadAsync(transfer, feed, CancellationToken.None).ConfigureAwait(false);
    }

    private async Task<ITransferProvider> ConnectAsync(UpdateProject project, ProjectSecrets secrets,
        CancellationToken cancellationToken)
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

    private Task DeleteLocalPackage(string packageDirectory)
    {
        if (_fileSystem.Directory.Exists(packageDirectory))
            _fileSystem.Directory.Delete(packageDirectory, recursive: true);
        return Task.CompletedTask;
    }

    private static void Validate(PublishRequest request)
    {
        var version = request.Package.Version;
        if (version.Release == new UpdateVersion(0, 0, 0))
            throw new ArgumentException("The version 0.0.0 is reserved.", nameof(request));
        if (request.Project.Packages.Any(p => p.Version == version))
            throw new ArgumentException($"The project already has a package {version}.", nameof(request));
        ValidateContent(request, request.Package.Platforms);
        if (request.Publish && request.Project.Statistics.Enabled &&
            string.IsNullOrEmpty(request.Secrets.StatisticsAdminSecret))
            throw new InvalidOperationException("The statistics admin secret is missing.");
    }

    /// <summary>The rules for every package: an absolute update URL, an English changelog, a platform, and for each platform that is built files or operations that fit it.</summary>
    private static void ValidateContent(PublishRequest request, IEnumerable<PlatformPackage> built)
    {
        var project = request.Project;
        if (!UpdateProject.IsValidUpdateUrl(project.UpdateUrl))
            throw new ArgumentException(
                $"The update URL \"{project.UpdateUrl}\" of the project is not an absolute URL.", nameof(request));
        var english = request.Changelog
            .FirstOrDefault(c => string.Equals(c.Key.Name, "en", StringComparison.OrdinalIgnoreCase)).Value;
        if (string.IsNullOrWhiteSpace(english))
            throw new ArgumentException("An English changelog is required.", nameof(request));
        if (request.Package.Platforms.Count == 0)
            throw new ArgumentException("A package needs at least one platform.", nameof(request));
        foreach (var platform in built)
        {
            if (platform.Files.Count == 0 && platform.Operations.Count == 0)
                throw new ArgumentException($"The {platform.Platform} package needs at least one file or operation.",
                    nameof(request));
            if (!PackagePlatform.IsWindows(platform.Platform) && platform.Operations.Any(o => o.RequiresWindows))
                throw new ArgumentException(
                    $"The {platform.Platform} package contains registry or service operations, which only exist on Windows.",
                    nameof(request));
        }
    }

    private static string RequirePrivateKey(ProjectSecrets secrets) =>
        secrets.PrivateKey ??
        throw new InvalidOperationException(
            "The private key of the project is missing. Enter the project credentials first.");

    /// <summary>The statistics endpoint as the feed carries it: <c>nupdate-statistics.php</c> relative to the feed, or the configured absolute URL.</summary>
    public static string StatisticsUrl(UpdateProject project)
    {
        ArgumentNullException.ThrowIfNull(project);
        return string.IsNullOrEmpty(project.Statistics.EndpointUrl)
            ? StatisticsScript.ScriptFileName
            : project.Statistics.EndpointUrl;
    }

    /// <summary>The absolute statistics endpoint of the project.</summary>
    public static Uri StatisticsUri(UpdateProject project)
    {
        ArgumentNullException.ThrowIfNull(project);
        return project.Resolve(StatisticsUrl(project));
    }

    /// <summary>The endpoint with the admin secret; throws when the secret is missing.</summary>
    public static StatisticsEndpoint Endpoint(UpdateProject project, ProjectSecrets secrets)
    {
        ArgumentNullException.ThrowIfNull(secrets);
        return new StatisticsEndpoint(StatisticsUri(project),
            secrets.StatisticsAdminSecret ??
            throw new InvalidOperationException("The statistics admin secret is missing."), project, secrets);
    }

    /// <summary>
    ///     The platform folders of a version that a rebuild replaces or removes, moved into a folder next to them while
    ///     the rebuild runs: <see cref="Restore" /> puts them back after a failure, <see cref="Delete" /> drops them after
    ///     success. Moving keeps the previous package files at hand without copying them.
    /// </summary>
    private sealed class PlatformBackup(IFileSystem fileSystem, UpdateProject project, UpdateVersion version)
    {
        private readonly string _directory =
            fileSystem.Path.Combine(project.PackageDirectory(version), $".backup-{Guid.NewGuid():N}");

        private readonly List<string> _moved = [];
        private readonly List<string> _built = [];

        /// <summary>Moves the folder of the platform aside, when there is one.</summary>
        public void MoveAside(string platform)
        {
            var directory = project.PlatformDirectory(version, platform);
            if (!fileSystem.Directory.Exists(directory))
                return;
            fileSystem.Directory.CreateDirectory(_directory);
            fileSystem.Directory.Move(directory, fileSystem.Path.Combine(_directory, platform));
            _moved.Add(platform);
        }

        /// <summary>Notes that the folder of the platform is written next, so <see cref="Restore" /> removes it.</summary>
        public void Building(string platform) => _built.Add(platform);

        /// <summary>The previous package file of a platform that was moved aside.</summary>
        public string PackageFilePath(string platform) =>
            fileSystem.Path.Combine(_directory, platform, PackageLayout.PackageFileName(platform));

        public void Restore()
        {
            foreach (var platform in _built)
                DeleteDirectory(project.PlatformDirectory(version, platform));
            foreach (var platform in _moved)
                fileSystem.Directory.Move(fileSystem.Path.Combine(_directory, platform),
                    project.PlatformDirectory(version, platform));
            DeleteDirectory(_directory);
        }

        /// <summary>Removes the backup. One that cannot be removed only takes space, since the rebuild is complete.</summary>
        public void Delete()
        {
            try
            {
                DeleteDirectory(_directory);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Left for the user to delete; nothing refers to it.
            }
        }

        private void DeleteDirectory(string directory)
        {
            if (fileSystem.Directory.Exists(directory))
                fileSystem.Directory.Delete(directory, recursive: true);
        }
    }
}
