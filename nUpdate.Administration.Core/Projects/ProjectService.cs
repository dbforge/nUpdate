using System.IO.Abstractions;
using nUpdate.Administration.Core.Migration;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Publishing;
using nUpdate.Administration.Core.Security;
using nUpdate.Administration.Core.Statistics;
using nUpdate.Administration.TransferInterface;
using nUpdate.Packaging;
using nUpdate.Security;
using nUpdate.Updating;

namespace nUpdate.Administration.Core.Projects;

/// <summary>Creates, changes and deletes projects.</summary>
public interface IProjectService
{
    /// <summary>Creates the key pair, the project folder with <c>project.nupdproj</c> and <c>packages/</c>, and (if enabled) the statistics script on the server.</summary>
    Task<ProjectLoadResult> CreateAsync(NewProjectRequest request, IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>Checks that the transfer settings work. Throws <see cref="TransferException" /> when they do not.</summary>
    Task TestConnectionAsync(TransferSettings settings, TransferCredentials credentials, CancellationToken cancellationToken = default);

    /// <summary>Renames the project (name and registration only; the folder stays) and saves it.</summary>
    Task RenameAsync(UpdateProject project, string newName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Writes changed settings and secrets to the project file. With a project password the secrets are encrypted
    ///     into the file and the password is remembered for this user; without one the file holds no secrets.
    /// </summary>
    Task SaveAsync(UpdateProject project, ProjectSecrets secrets, string? projectPassword, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Writes a project converted from an earlier format as <c>project.nupdproj</c> in its folder (the old file is
    ///     left in place) and registers it, with the secrets saved like <see cref="SaveAsync" />.
    /// </summary>
    Task SaveMigratedAsync(UpdateProject project, ProjectSecrets secrets, string? projectPassword, CancellationToken cancellationToken = default);

    /// <summary>Uploads <c>nupdate-statistics.php</c> and its configuration and checks the API.</summary>
    Task SetupStatisticsAsync(UpdateProject project, ProjectSecrets secrets, CancellationToken cancellationToken = default);

    /// <summary>Removes the project from the list and optionally deletes its folder and its files on the server (including legacy files).</summary>
    Task DeleteAsync(UpdateProject project, ProjectSecrets secrets, bool deleteLocalFiles, bool deleteServerFiles, CancellationToken cancellationToken = default);
}

/// <summary>Input for a new project.</summary>
public sealed class NewProjectRequest
{
    public string Name { get; set; } = string.Empty;

    /// <summary>The folder the project file and the packages are written to. It must not already hold a project.</summary>
    public string Folder { get; set; } = string.Empty;

    public string UpdateUrl { get; set; } = string.Empty;

    public TransferSettings Transfer { get; set; } = new();

    public ProjectSecrets Secrets { get; set; } = new();

    public HttpAuthenticationSettings? HttpAuthentication { get; set; }

    public StatisticsSettings Statistics { get; set; } = new();

    /// <summary>The password the secrets are encrypted with in the project file; <c>null</c> keeps them out of the file.</summary>
    public string? ProjectPassword { get; set; }

    /// <summary>The RSA key size; 8192 (the default) takes a while.</summary>
    public int KeySize { get; set; } = PackageSigning.DefaultKeySize;

    /// <summary>Skip the connection test (for example when the server is not reachable yet).</summary>
    public bool TestConnection { get; set; } = true;
}

public sealed class ProjectService : IProjectService
{
    private readonly IFileSystem _fileSystem;
    private readonly IProjectStore _projects;
    private readonly IProjectPasswordStore _passwords;
    private readonly ITransferProviderFactory _transferFactory;
    private readonly IStatisticsApi _statistics;
    private readonly ILegacyFeedMigrator _migrator;
    private readonly IProjectLogger _logger;
    private readonly StatisticsDeployer _deployer;
    private readonly AdministrationPaths _paths;
    private readonly Func<int, (string PublicKey, string PrivateKey)> _keyGenerator;

    public ProjectService(IFileSystem fileSystem, AdministrationPaths paths, IProjectStore projects, IProjectPasswordStore passwords, ITransferProviderFactory transferFactory,
        IStatisticsApi statistics, ILegacyFeedMigrator migrator, IProjectLogger logger)
        : this(fileSystem, paths, projects, passwords, transferFactory, statistics, migrator, logger, GenerateKeyPair)
    {
    }

    public ProjectService(IFileSystem fileSystem, AdministrationPaths paths, IProjectStore projects, IProjectPasswordStore passwords, ITransferProviderFactory transferFactory,
        IStatisticsApi statistics, ILegacyFeedMigrator migrator, IProjectLogger logger, Func<int, (string PublicKey, string PrivateKey)> keyGenerator)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _paths = paths ?? throw new ArgumentNullException(nameof(paths));
        _projects = projects ?? throw new ArgumentNullException(nameof(projects));
        _passwords = passwords ?? throw new ArgumentNullException(nameof(passwords));
        _transferFactory = transferFactory ?? throw new ArgumentNullException(nameof(transferFactory));
        _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        _migrator = migrator ?? throw new ArgumentNullException(nameof(migrator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _keyGenerator = keyGenerator ?? throw new ArgumentNullException(nameof(keyGenerator));
        _deployer = new StatisticsDeployer(fileSystem, transferFactory, statistics);
    }

    public async Task<ProjectLoadResult> CreateAsync(NewProjectRequest request, IProgress<PipelineProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateName(request.Name);
        if (string.IsNullOrWhiteSpace(request.Folder))
            throw new ArgumentException("The project folder is empty.", nameof(request));
        if (!UpdateProject.IsValidUpdateUrl(request.UpdateUrl))
            throw new ArgumentException($"\"{request.UpdateUrl}\" is not a valid absolute URL.", nameof(request));
        if ((await _projects.ListAsync(cancellationToken).ConfigureAwait(false)).Any(p => string.Equals(p.Name, request.Name, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException($"A project named \"{request.Name}\" already exists.", nameof(request));
        if (request.Statistics.Enabled && request.Statistics.Database is null)
            throw new ArgumentException("Statistics need database settings.", nameof(request));
        var folder = _fileSystem.Path.GetFullPath(request.Folder.Trim());
        var projectFile = _fileSystem.Path.Combine(folder, UpdateProject.FileName);
        if (_fileSystem.File.Exists(projectFile))
            throw new ArgumentException($"The folder \"{folder}\" already holds a project.", nameof(request));

        var secrets = request.Secrets;
        var project = new UpdateProject
        {
            Name = request.Name,
            UpdateUrl = UpdateProject.NormalizeUpdateUrl(request.UpdateUrl),
            Transfer = request.Transfer,
            HttpAuthentication = request.HttpAuthentication,
            Statistics = request.Statistics,
            Path = projectFile,
        };
        if (project.Statistics.Enabled)
            secrets.StatisticsAdminSecret ??= SecretGenerator.CreateSecret();

        var folderCreated = false;
        var pipeline = new CompensatingPipeline()
            .Add("Testing the connection", ct => request.TestConnection ? TestConnectionAsync(project.Transfer, secrets.ToTransferCredentials(), ct) : Task.CompletedTask)
            .Add("Generating the key pair", async ct =>
            {
                // 8192-bit keys take seconds; keep the UI thread free.
                var (publicKey, privateKey) = await Task.Run(() => _keyGenerator(request.KeySize), ct).ConfigureAwait(false);
                project.PublicKey = publicKey;
                secrets.PrivateKey = privateKey;
            })
            .Add("Creating the project folder", _ =>
            {
                folderCreated = !_fileSystem.Directory.Exists(folder);
                _fileSystem.Directory.CreateDirectory(project.PackagesDirectory);
                return Task.CompletedTask;
            }, () =>
            {
                if (folderCreated && _fileSystem.Directory.Exists(folder))
                    _fileSystem.Directory.Delete(folder, recursive: true);
                return Task.CompletedTask;
            });
        if (project.Statistics.Enabled)
            pipeline.Add("Setting up the statistics", ct => _deployer.DeployAsync(project, secrets, ct));
        pipeline.Add("Saving the project", ct =>
        {
            _logger.Write(project, LogEntryKind.Create, null);
            return SaveAsync(project, secrets, request.ProjectPassword, ct);
        });

        await pipeline.RunAsync(progress, cancellationToken).ConfigureAwait(false);
        return new ProjectLoadResult(project, secrets, migrated: false, request.ProjectPassword is null ? SecretsState.NotSaved : SecretsState.Loaded);
    }

    public async Task TestConnectionAsync(TransferSettings settings, TransferCredentials credentials, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(credentials);
        await using var transfer = _transferFactory.Create(settings, credentials);
        await transfer.ConnectAsync(cancellationToken).ConfigureAwait(false);
        await transfer.ListAsync(string.Empty, false, cancellationToken).ConfigureAwait(false);
    }

    public async Task RenameAsync(UpdateProject project, string newName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ValidateName(newName);
        if (string.Equals(project.Name, newName, StringComparison.Ordinal))
            return;
        if ((await _projects.ListAsync(cancellationToken).ConfigureAwait(false)).Any(p => string.Equals(p.Name, newName, StringComparison.OrdinalIgnoreCase) && p.Id != project.Id))
            throw new ArgumentException($"A project named \"{newName}\" already exists.", nameof(newName));

        project.Name = newName;
        _logger.Write(project, LogEntryKind.Edit, null);
        await _projects.SaveAsync(project, cancellationToken).ConfigureAwait(false);
    }

    public async Task SaveAsync(UpdateProject project, ProjectSecrets secrets, string? projectPassword, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        // The blob is written first; the remembered password only changes once the file holds secrets for it.
        project.Secrets = projectPassword is null ? null : await Task.Run(() => ProjectSecretsProtection.Protect(secrets, projectPassword), cancellationToken).ConfigureAwait(false);
        await _projects.SaveAsync(project, cancellationToken).ConfigureAwait(false);
        if (projectPassword is null)
            await _passwords.RemoveAsync(project.Id, cancellationToken).ConfigureAwait(false);
        else
            await _passwords.SetAsync(project.Id, projectPassword, cancellationToken).ConfigureAwait(false);
    }

    public async Task SaveMigratedAsync(UpdateProject project, ProjectSecrets secrets, string? projectPassword, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        ArgumentException.ThrowIfNullOrWhiteSpace(project.Path);
        var oldPath = project.Path;
        project.Path = MigratedProjectFile(project);
        if (!string.Equals(oldPath, project.Path, StringComparison.Ordinal))
            project.LegacyProjectFile = oldPath;
        _fileSystem.Directory.CreateDirectory(project.PackagesDirectory);
        _logger.Write(project, LogEntryKind.Migrate, null);
        await SaveAsync(project, secrets, projectPassword, cancellationToken).ConfigureAwait(false);
        // The registration of the old file is replaced by the new one; the file itself stays as a backup.
        if (!string.Equals(oldPath, project.Path, StringComparison.Ordinal))
            await _projects.UnregisterPathAsync(oldPath, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Where a converted project is written. The old file is never overwritten, since nUpdate Administration 4 keeps
    ///     using it, and the project never goes into the data folder of nUpdate Administration 4, which deletes a
    ///     project's folder there when it removes the project. Otherwise the project gets a folder of its own name: the
    ///     old folder when it is named after the project (the 5.0 pre-release layout), else a new subfolder, so a file
    ///     that lived in a shared folder like Documents gets a folder of its own.
    /// </summary>
    /// <exception cref="InvalidOperationException">The target already holds another project.</exception>
    private string MigratedProjectFile(UpdateProject project)
    {
        var folder = project.Folder;
        var fileName = _fileSystem.Path.GetFileName(project.Path);
        var folderName = _fileSystem.Path.GetFileName(folder.TrimEnd(_fileSystem.Path.DirectorySeparatorChar, _fileSystem.Path.AltDirectorySeparatorChar));
        if (IsSameOrBelow(folder, _paths.LegacyProjectsDirectory))
            folder = _paths.SuggestedProjectFolder(project.Name);
        else if (!string.Equals(folderName, project.Name, StringComparison.OrdinalIgnoreCase))
            folder = _fileSystem.Path.Combine(folder, project.Name);

        var target = _fileSystem.Path.Combine(folder, UpdateProject.FileName);
        if (IsSameOrBelow(project.Path, target))
        {
            // The old file is called project.nupdproj already.
            folder = _fileSystem.Path.Combine(folder, project.Name);
            target = _fileSystem.Path.Combine(folder, UpdateProject.FileName);
        }

        if (_fileSystem.File.Exists(target))
        {
            var existing = Serializer.Deserialize<UpdateProject>(_fileSystem.File.ReadAllText(target));
            if (existing is null || existing.Id != project.Id)
                throw new InvalidOperationException($"The folder \"{folder}\" already holds another project. Move the file \"{fileName}\" to a folder of its own and open it there.");
        }

        return target;
    }

    private bool IsSameOrBelow(string path, string directory)
    {
        var parent = _fileSystem.Path.GetFullPath(directory).TrimEnd(_fileSystem.Path.DirectorySeparatorChar, _fileSystem.Path.AltDirectorySeparatorChar);
        var child = _fileSystem.Path.GetFullPath(path);
        return string.Equals(child, parent, StringComparison.OrdinalIgnoreCase)
               || child.StartsWith(parent + _fileSystem.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    public Task SetupStatisticsAsync(UpdateProject project, ProjectSecrets secrets, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        if (!project.Statistics.Enabled)
            throw new InvalidOperationException("Statistics are disabled for this project.");
        secrets.StatisticsAdminSecret ??= SecretGenerator.CreateSecret();
        return _deployer.DeployAsync(project, secrets, cancellationToken);
    }

    public async Task DeleteAsync(UpdateProject project, ProjectSecrets secrets, bool deleteLocalFiles, bool deleteServerFiles, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(secrets);
        if (deleteServerFiles)
        {
            if (project.Statistics.Enabled && !string.IsNullOrEmpty(secrets.StatisticsAdminSecret))
                await _statistics.DeleteProjectAsync(PublishService.Endpoint(project, secrets), project.Id, cancellationToken).ConfigureAwait(false);
            // The server files of nUpdate 3 and 4 go with the project; their local copies belong to nUpdate Administration 4.
            var legacy = await _migrator.FindLegacyFilesAsync(project, secrets, cancellationToken).ConfigureAwait(false);
            await _migrator.DeleteLegacyFilesAsync(project, secrets, legacy.ServerOnly(), cancellationToken).ConfigureAwait(false);
            await using var transfer = _transferFactory.Create(project.Transfer, secrets.ToTransferCredentials());
            await transfer.ConnectAsync(cancellationToken).ConfigureAwait(false);
            foreach (var package in project.Packages.Where(p => p.Released))
                await transfer.DeleteDirectoryAsync(PackageLayout.RemoteVersionDirectory(package.Version), cancellationToken).ConfigureAwait(false);
            await transfer.DeleteFileAsync(UpdateFeed.FileName, cancellationToken).ConfigureAwait(false);
            await transfer.DeleteFileAsync(StatisticsScript.ScriptFileName, cancellationToken).ConfigureAwait(false);
            await transfer.DeleteFileAsync(StatisticsScript.ConfigFileName, cancellationToken).ConfigureAwait(false);
        }

        await _projects.UnregisterAsync(project.Id, cancellationToken).ConfigureAwait(false);
        await _passwords.RemoveAsync(project.Id, cancellationToken).ConfigureAwait(false);
        if (deleteLocalFiles)
        {
            // Only what the project owns: its file and its packages; the folder itself goes when nothing else is in it.
            if (_fileSystem.File.Exists(project.Path))
                _fileSystem.File.Delete(project.Path);
            if (_fileSystem.Directory.Exists(project.PackagesDirectory))
                _fileSystem.Directory.Delete(project.PackagesDirectory, recursive: true);
            if (_fileSystem.Directory.Exists(project.Folder) && !_fileSystem.Directory.EnumerateFileSystemEntries(project.Folder).Any())
                _fileSystem.Directory.Delete(project.Folder);
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The project name is empty.", nameof(name));
        if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || name.Trim() != name)
            throw new ArgumentException($"\"{name}\" is not a valid project name: it must be usable as a folder name.", nameof(name));
    }

    private static (string PublicKey, string PrivateKey) GenerateKeyPair(int keySize)
    {
        using var signing = PackageSigning.Generate(keySize);
        return (signing.PublicKeyPem, signing.PrivateKeyPem);
    }
}
