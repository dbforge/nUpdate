using System.IO.Abstractions;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using nUpdate.Administration.Core.Models;
using nUpdate.Administration.Core.Security;
using nUpdate.Exceptions;

namespace nUpdate.Administration.Core.Projects;

/// <summary>File-based <see cref="IProjectStore" />.</summary>
public sealed class ProjectStore : IProjectStore
{
    private readonly IFileSystem _fileSystem;
    private readonly AdministrationPaths _paths;
    private readonly ProjectMigrator _migrator;

    public ProjectStore(IFileSystem fileSystem, AdministrationPaths paths, ICredentialProtector protector)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _paths = paths ?? throw new ArgumentNullException(nameof(paths));
        ArgumentNullException.ThrowIfNull(protector);
        _migrator = new ProjectMigrator(protector);
    }

    public async Task<IReadOnlyList<ProjectRegistration>> ListAsync(CancellationToken cancellationToken = default)
    {
        if (!_fileSystem.File.Exists(_paths.ProjectsConfigFile))
            return await ReadLegacyListAsync(cancellationToken).ConfigureAwait(false);
        var content = await _fileSystem.File.ReadAllTextAsync(_paths.ProjectsConfigFile, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(content))
            return [];
        if (JToken.Parse(content) is not JObject document)
            return [];
        FormatVersion.Check(document.Value<int?>("format") ?? ProjectList.CurrentFormat, ProjectList.CurrentFormat, "project list");
        return document.ToObject<ProjectList>(Newtonsoft.Json.JsonSerializer.Create(Serializer.Settings))!.Projects ?? [];
    }

    /// <summary>
    ///     The projects nUpdate Administration 3 or 4 knows (<c>projconf.json</c>, a plain array of <c>{ Name, Path }</c>),
    ///     offered until this version writes its own list. The file belongs to the other version, so it is only read and
    ///     anything unexpected in it means there is nothing to take over.
    /// </summary>
    private async Task<IReadOnlyList<ProjectRegistration>> ReadLegacyListAsync(CancellationToken cancellationToken)
    {
        if (!_fileSystem.File.Exists(_paths.LegacyProjectsConfigFile))
            return [];
        var content = await _fileSystem.File.ReadAllTextAsync(_paths.LegacyProjectsConfigFile, cancellationToken).ConfigureAwait(false);
        try
        {
            return JToken.Parse(content) is JArray legacy
                ? legacy.OfType<JObject>().Select(e => new ProjectRegistration(Guid.Empty, Text(e["Name"]), Text(e["Path"]))).Where(r => r.Path.Length > 0).ToList()
                : [];
        }
        catch (Newtonsoft.Json.JsonException)
        {
            return [];
        }
    }

    private static string Text(JToken? token) => token is null || token.Type == JTokenType.Null ? string.Empty : token.ToString();

    public async Task<ProjectLoadResult> LoadAsync(string path, string? password = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var content = await _fileSystem.File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
        JObject json;
        try
        {
            json = JObject.Parse(content);
        }
        catch (Newtonsoft.Json.JsonException ex)
        {
            throw new InvalidDataException($"The project file \"{path}\" is not valid JSON.", ex);
        }

        ProjectLoadResult result;
        if (json.TryGetValue("format", StringComparison.Ordinal, out var formatToken))
        {
            FormatVersion.Check(formatToken.Type == JTokenType.Integer ? formatToken.Value<int>() : -1, UpdateProject.CurrentFormat, "project file");
            // Deriving the key from the password takes a moment; keep it off the UI thread.
            result = await Task.Run(() => LoadCurrent(json, password), cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var configVersion = json.Value<string>("ConfigVersion");
            if (ProjectMigrator.IsV5(configVersion))
                result = _migrator.FromV5(json);
            else if (ProjectMigrator.IsV3(configVersion))
                result = ProjectMigrator.FromV3(json, password);
            else
                throw new UnsupportedFormatException($"The project file \"{path}\" has format version \"{configVersion}\", which this version of nUpdate Administration cannot read.");
        }

        result.Project.Path = path;
        return result;
    }

    private static ProjectLoadResult LoadCurrent(JObject json, string? password)
    {
        var project = json.ToObject<UpdateProject>(Newtonsoft.Json.JsonSerializer.Create(Serializer.Settings))!; // a JObject never maps to null
        project.Transfer ??= new TransferInterface.TransferSettings();
        project.Statistics ??= new StatisticsSettings();
        project.Packages ??= [];
        project.Log ??= [];
        if (string.IsNullOrEmpty(project.Secrets))
            return new ProjectLoadResult(project, new ProjectSecrets(), migrated: false, SecretsState.NotSaved);
        if (string.IsNullOrEmpty(password))
            return new ProjectLoadResult(project, new ProjectSecrets(), migrated: false, SecretsState.PasswordRequired);
        try
        {
            return new ProjectLoadResult(project, ProjectSecretsProtection.Unprotect(project.Secrets, password), migrated: false, SecretsState.Loaded);
        }
        catch (Exception ex) when (ex is CryptographicException or InvalidDataException)
        {
            return new ProjectLoadResult(project, new ProjectSecrets(), migrated: false, SecretsState.Unreadable);
        }
    }

    public async Task SaveAsync(UpdateProject project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentException.ThrowIfNullOrWhiteSpace(project.Path);

        project.Format = UpdateProject.CurrentFormat;
        var directory = _fileSystem.Path.GetDirectoryName(project.Path);
        if (!string.IsNullOrEmpty(directory))
            _fileSystem.Directory.CreateDirectory(directory);
        await _fileSystem.File.WriteAllTextAsync(project.Path, Serializer.Serialize(project, indented: true), cancellationToken).ConfigureAwait(false);
        await RegisterAsync(new ProjectRegistration(project.Id, project.Name, project.Path), cancellationToken).ConfigureAwait(false);
    }

    public async Task RegisterAsync(ProjectRegistration registration, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(registration);
        var entries = (await ListAsync(cancellationToken).ConfigureAwait(false)).ToList();
        entries.RemoveAll(e => e.Id == registration.Id || string.Equals(e.Path, registration.Path, StringComparison.Ordinal));
        entries.Add(registration);
        await WriteListAsync(entries, cancellationToken).ConfigureAwait(false);
    }

    public async Task UnregisterAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var entries = (await ListAsync(cancellationToken).ConfigureAwait(false)).ToList();
        if (entries.RemoveAll(e => e.Id == projectId) > 0)
            await WriteListAsync(entries, cancellationToken).ConfigureAwait(false);
    }

    public async Task UnregisterPathAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var entries = (await ListAsync(cancellationToken).ConfigureAwait(false)).ToList();
        if (entries.RemoveAll(e => string.Equals(e.Path, path, StringComparison.Ordinal)) > 0)
            await WriteListAsync(entries, cancellationToken).ConfigureAwait(false);
    }

    private async Task WriteListAsync(List<ProjectRegistration> entries, CancellationToken cancellationToken)
    {
        _fileSystem.Directory.CreateDirectory(_paths.Root);
        var list = new ProjectList { Projects = entries.OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase).ToList() };
        await _fileSystem.File.WriteAllTextAsync(_paths.ProjectsConfigFile, Serializer.Serialize(list, indented: true), cancellationToken).ConfigureAwait(false);
    }
}
