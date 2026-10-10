using System.IO.Abstractions;
using nUpdate.Installer;

namespace nUpdate.UpdateInstaller;

/// <summary>Reads and validates the options file the host application wrote.</summary>
internal static class InstallerOptionsReader
{
    /// <exception cref="FileNotFoundException">The options file does not exist.</exception>
    /// <exception cref="InvalidDataException">The file is not valid.</exception>
    /// <exception cref="Exceptions.UnsupportedFormatException">The file was written for another installer version.</exception>
    public static InstallerOptions Read(IFileSystem fileSystem, string path)
    {
        if (fileSystem is null)
            throw new ArgumentNullException(nameof(fileSystem));
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("The options file path is empty.", nameof(path));
        if (!fileSystem.File.Exists(path))
            throw new FileNotFoundException($"The installer options file \"{path}\" does not exist.", path);

        return Parse(fileSystem.File.ReadAllText(path));
    }

    /// <exception cref="InvalidDataException">The content is not valid.</exception>
    /// <exception cref="Exceptions.UnsupportedFormatException">The content was written for another installer version.</exception>
    public static InstallerOptions Parse(string json)
    {
        InstallerOptions? options;
        try
        {
            options = Serializer.Deserialize<InstallerOptions>(json);
        }
        catch (Newtonsoft.Json.JsonException ex)
        {
            throw new InvalidDataException("The installer options are not valid JSON.", ex);
        }

        if (options is null)
            throw new InvalidDataException("The installer options are empty.");
        FormatVersion.Check(options.Format, InstallerOptions.CurrentFormat, "installer options");
        options.Packages ??= [];
        options.Application ??= new ApplicationOptions();
        options.Host ??= new HostOptions();
        options.Arguments ??= [];
        options.Ui ??= new InstallerUiOptions();
        options.Texts ??= new Dictionary<string, string>(StringComparer.Ordinal);
        if (options.Packages.Count == 0)
            throw new InvalidDataException("The installer options name no packages.");
        if (options.Packages.Any(p => string.IsNullOrWhiteSpace(p?.Path)))
            throw new InvalidDataException("The installer options contain a package without a path.");
        if (string.IsNullOrWhiteSpace(options.Application.Directory))
            throw new InvalidDataException("The installer options name no application directory.");
        if (options.Host.AfterInstall != AfterInstall.KeepRunning &&
            string.IsNullOrWhiteSpace(options.Application.ExecutablePath))
            throw new InvalidDataException(
                "The installer options name no application executable although the host application should be closed.");

        return options;
    }
}
