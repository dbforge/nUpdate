using System.IO.Abstractions;

namespace nUpdate.Administration.Core;

/// <summary>The local folder layout of nUpdate Administration: the per-user data folder and the default place for new projects.</summary>
public sealed class AdministrationPaths
{
    public const string ApplicationFolderName = "nUpdate Administration";

    public const string DefaultProjectsFolderName = "nUpdate Projects";

    private readonly IFileSystem _fileSystem;

    public AdministrationPaths(IFileSystem fileSystem, string root, string defaultProjectsDirectory)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        Root = root ?? throw new ArgumentNullException(nameof(root));
        DefaultProjectsDirectory = defaultProjectsDirectory ?? throw new ArgumentNullException(nameof(defaultProjectsDirectory));
    }

    /// <summary>The per-user data folder of the application and <c>Documents/nUpdate Projects</c> for new projects.</summary>
    public static AdministrationPaths Default(IFileSystem fileSystem)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        return new AdministrationPaths(fileSystem,
            fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationFolderName),
            fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DefaultProjectsFolderName));
    }

    public string Root { get; }

    /// <summary>Where the new project wizard suggests to put project folders.</summary>
    public string DefaultProjectsDirectory { get; }

    /// <summary>The list of known projects.</summary>
    public string ProjectsConfigFile => _fileSystem.Path.Combine(Root, "projects.json");

    /// <summary>
    ///     The project list of nUpdate Administration 3 and 4, which shares this data folder. It is read until this
    ///     version has written its own list and never written, so both versions can be installed side by side.
    /// </summary>
    public string LegacyProjectsConfigFile => _fileSystem.Path.Combine(Root, "projconf.json");

    /// <summary>Where nUpdate Administration 3 and 4 keep a folder per project with the local package copies and the statistics script.</summary>
    public string LegacyProjectsDirectory => _fileSystem.Path.Combine(Root, "Projects");

    /// <summary>The folder of one project below <see cref="LegacyProjectsDirectory" />.</summary>
    public string LegacyProjectDataDirectory(string projectName) => _fileSystem.Path.Combine(LegacyProjectsDirectory, projectName);

    /// <summary>The remembered project passwords, protected per user and machine.</summary>
    public string PasswordsFile => _fileSystem.Path.Combine(Root, "passwords.json");

    public string KeyRingDirectory => _fileSystem.Path.Combine(Root, "keys");

    /// <summary>The suggested folder of a new project.</summary>
    public string SuggestedProjectFolder(string projectName) => _fileSystem.Path.Combine(DefaultProjectsDirectory, projectName);
}
