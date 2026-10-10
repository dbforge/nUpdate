using nUpdate.Administration.Core.Models;

namespace nUpdate.Administration.Core.Projects;

/// <summary>Reads and writes project files and the list of known projects.</summary>
public interface IProjectStore
{
    /// <summary>The projects registered in <c>projects.json</c>, or those of nUpdate Administration 3 and 4 until this list exists.</summary>
    Task<IReadOnlyList<ProjectRegistration>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Loads a project file, migrating older formats in memory. The file on disk is not changed. For a format 6 file
    ///     the password is the project password. For a file written by nUpdate Administration 3.x or 4.x that did not
    ///     save its credentials it may be the master password of that time; the application does not ask for it and lets
    ///     the user enter the secrets instead, so such a file loads with empty secrets and <see cref="SecretsState.Loaded" />.
    /// </summary>
    /// <exception cref="Exceptions.UnsupportedFormatException">The file's format is unknown.</exception>
    /// <exception cref="InvalidDataException">The file is not valid.</exception>
    Task<ProjectLoadResult> LoadAsync(string path, string? password = null,
        CancellationToken cancellationToken = default);

    /// <summary>Writes the project file and makes sure it is registered.</summary>
    Task SaveAsync(UpdateProject project, CancellationToken cancellationToken = default);

    Task RegisterAsync(ProjectRegistration registration, CancellationToken cancellationToken = default);

    Task UnregisterAsync(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>Removes the registration of a project file by its path, for entries of earlier versions that carry no id.</summary>
    Task UnregisterPathAsync(string path, CancellationToken cancellationToken = default);
}

/// <summary>What the secrets of a loaded project look like.</summary>
public enum SecretsState
{
    /// <summary>The file holds no secrets; the user enters them when needed.</summary>
    NotSaved,

    /// <summary>The secrets were read.</summary>
    Loaded,

    /// <summary>The file holds secrets but no password was given.</summary>
    PasswordRequired,

    /// <summary>The secrets could not be read: wrong password, or saved by another user or machine by an earlier version.</summary>
    Unreadable,
}

/// <summary>A loaded project plus the secrets that could be recovered from the file.</summary>
public sealed class ProjectLoadResult(
    UpdateProject project,
    ProjectSecrets secrets,
    bool migrated,
    SecretsState secretsState)
{
    public UpdateProject Project { get; } = project ?? throw new ArgumentNullException(nameof(project));

    /// <summary>The secrets; those that could not be read are <c>null</c>.</summary>
    public ProjectSecrets Secrets { get; } = secrets ?? throw new ArgumentNullException(nameof(secrets));

    /// <summary>True when the file was in an older format. The project has no <c>secrets</c> yet; saving it with a project password writes the current format.</summary>
    public bool Migrated { get; } = migrated;

    public SecretsState SecretsState { get; } = secretsState;
}
