using nUpdate.Updating;

namespace nUpdate.Installer;

/// <summary>
///     Everything the update installer needs to run, written by the host application as a JSON file whose path is the
///     installer's only command line argument.
/// </summary>
public sealed class InstallerOptions
{
    public const int CurrentFormat = 2;

    public int Format { get; set; } = CurrentFormat;

    /// <summary>The downloaded packages, oldest version first. Version and operations come from each package's manifest.</summary>
    public List<InstallerPackage> Packages { get; set; } = [];

    public ApplicationOptions Application { get; set; } = new();

    public HostOptions Host { get; set; } = new();

    /// <summary>Arguments the installer passes to the restarted application.</summary>
    public List<InstallerArgument> Arguments { get; set; } = [];

    public InstallerUiOptions Ui { get; set; } = new();

    /// <summary>Localized texts shown by the installer, keyed by <see cref="InstallerText" /> names.</summary>
    public Dictionary<string, string> Texts { get; set; } = new(StringComparer.Ordinal);

    public string Text(InstallerText key) =>
        Texts.TryGetValue(key.ToString(), out var text) ? text : InstallerTexts.Default(key);

    /// <summary>The text for <paramref name="key" /> with the arguments filled in, formatted for the current culture.</summary>
    public string Text(InstallerText key, params object[] arguments) =>
        string.Format(System.Globalization.CultureInfo.CurrentCulture, Text(key), arguments);
}

/// <summary>A downloaded package file.</summary>
public sealed class InstallerPackage
{
    public string Path { get; set; } = string.Empty;
}

/// <summary>The application that is updated.</summary>
public sealed class ApplicationOptions
{
    public string Name { get; set; } = string.Empty;

    /// <summary>The directory of the application's executable, which the <c>Program</c> root of each package is copied into.</summary>
    public string Directory { get; set; } = string.Empty;

    public string ExecutablePath { get; set; } = string.Empty;

    /// <summary>
    ///     The macOS application bundle (<c>…/MyApp.app</c>) the executable lives in, or <c>null</c>. For a bundle the
    ///     <c>Program</c> root of a package is the whole bundle, which replaces the installed one.
    /// </summary>
    public string? Bundle { get; set; }

    /// <summary>What <c>%program%</c> and the <c>Program</c> root stand for: the bundle when there is one, else <see cref="Directory" />.</summary>
    [Newtonsoft.Json.JsonIgnore]
    public string ProgramDirectory => string.IsNullOrEmpty(Bundle) ? Directory : Bundle!;
}

/// <summary>The running host process and what to do with it.</summary>
public sealed class HostOptions
{
    /// <summary>The process the installer waits for before it replaces files, or <c>null</c> when the host keeps running.</summary>
    public int? ProcessId { get; set; }

    public AfterInstall AfterInstall { get; set; } = AfterInstall.Restart;
}

/// <summary>How the installer presents itself.</summary>
public sealed class InstallerUiOptions
{
    /// <summary>Whether the installer shows a window. Without a display it runs without one anyway.</summary>
    public bool ShowWindow { get; set; } = true;

    /// <summary>A PNG the window shows as its icon, or <c>null</c> for the nUpdate icon.</summary>
    public string? IconPath { get; set; }

    /// <summary>The accent color of the window as <c>#RRGGBB</c> or <c>#AARRGGBB</c>, or <c>null</c> for the default.</summary>
    public string? AccentColor { get; set; }
}
