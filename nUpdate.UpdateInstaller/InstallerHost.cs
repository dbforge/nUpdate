using nUpdate.Installer;
using nUpdate.UpdateInstaller.Reporting;

namespace nUpdate.UpdateInstaller;

/// <summary>
///     The entry point of every installer executable, the built-in one and yours: reads the options file the
///     application wrote, shows your window (or none without a display), installs and reports the result as the exit code.
/// </summary>
/// <example>
///     <code>
///     [STAThread]
///     static int Main(string[] args) => InstallerHost.Run(args, session => new MyProgressWindow(session));
///     </code>
/// </example>
public static class InstallerHost
{
    /// <summary>The log written next to the options file, which is in the installer's temp folder.</summary>
    public const string LogFileName = "install.log";

    /// <summary>The update was installed.</summary>
    public const int Succeeded = 0;

    /// <summary>The update failed; the log has the details.</summary>
    public const int Failed = 1;

    /// <summary>The installer could not start: no options file, or one it cannot read.</summary>
    public const int CouldNotStart = 2;

    /// <summary>
    ///     Runs the installer. <paramref name="createWindow" /> builds the window; it is not called when the options ask
    ///     for no window or when there is no display, and a window that fails is replaced by the windowless reporter.
    /// </summary>
    /// <param name="args">The command line: the path of the options file.</param>
    /// <param name="createWindow">Builds the window from the options and the log path.</param>
    /// <param name="services">The system services; the production ones when <c>null</c>.</param>
    /// <returns><see cref="Succeeded" />, <see cref="Failed" /> or <see cref="CouldNotStart" />.</returns>
    public static int Run(string[] args, Func<InstallerSession, IProgressReporter> createWindow,
        InstallerServices? services = null)
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));
        if (createWindow is null)
            throw new ArgumentNullException(nameof(createWindow));
        services ??= new InstallerServices();
        var fileSystem = services.FileSystem;

        var optionsPath = args.Length == 1 && !string.IsNullOrWhiteSpace(args[0]) ? args[0] : null;
        var logDirectory = optionsPath is null
            ? null
            : fileSystem.Path.GetDirectoryName(fileSystem.Path.GetFullPath(optionsPath));
        using var log = new InstallLog(fileSystem,
            string.IsNullOrEmpty(logDirectory) || !fileSystem.Directory.Exists(logDirectory)
                ? null
                : fileSystem.Path.Combine(logDirectory, LogFileName), services.Clock);
        using var windowless = new WindowlessProgressReporter(services);

        InstallerOptions options;
        try
        {
            if (optionsPath is null)
                throw new ArgumentException("Expected exactly one argument: the path of the installer options file.",
                    nameof(args));
            options = InstallerOptionsReader.Read(fileSystem, optionsPath);
        }
        catch (Exception ex)
        {
            log.Write(InstallerTexts.Default(InstallerText.InitializingErrorCaption) + Environment.NewLine +
                      ex.ToString());
            ShowStartupError(ex, createWindow, services, log, windowless);
            return CouldNotStart;
        }

        log.Write(
            $"Updating {options.Application.Name} in \"{options.Application.ProgramDirectory}\" with {options.Packages.Count} package(s): {string.Join(", ", options.Packages.Select(p => p.Path))}");
        var logging =
            new LoggingProgressReporter(CreateReporter(options, createWindow, services, log, windowless), log);
        var engine = new InstallEngine(services);
        var run = Task.Run(() => engine.Run(options, logging));
        logging.Initialize();
        return run.GetAwaiter().GetResult().Succeeded ? Succeeded : Failed;
    }

    /// <summary>The window, wrapped so the windowless reporter takes over when it fails, or the windowless reporter.</summary>
    private static IProgressReporter CreateReporter(InstallerOptions options,
        Func<InstallerSession, IProgressReporter> createWindow, InstallerServices services,
        InstallLog log, IProgressReporter windowless)
    {
        if (!options.Ui.ShowWindow)
        {
            log.Write("Running without a window, as the options ask.");
            return windowless;
        }

        if (!services.EnvironmentInfo.HasDisplay)
        {
            log.Write("Running without a window: there is no display.");
            return windowless;
        }

        try
        {
            return new FallbackProgressReporter(createWindow(new InstallerSession(options, log.Path)), windowless, log);
        }
        catch (Exception ex)
        {
            log.Write("The installer window could not be created; continuing without it: " + ex.ToString());
            return windowless;
        }
    }

    /// <summary>
    ///     Shows why the installer could not start. The application has already closed, so on a desktop the window is the
    ///     only sign the user gets; without one the error goes to the error output and the Windows event log.
    /// </summary>
    private static void ShowStartupError(Exception exception, Func<InstallerSession, IProgressReporter> createWindow,
        InstallerServices services, InstallLog log,
        IProgressReporter windowless)
    {
        var options = new InstallerOptions();
        options.Texts[nameof(InstallerText.WindowTitle)] = "nUpdate";
        options.Texts[nameof(InstallerText.UpdatingErrorCaption)] =
            InstallerTexts.Default(InstallerText.InitializingErrorCaption);
        var reporter = CreateReporter(options, createWindow, services, log, windowless);
        var report = Task.Run(() =>
        {
            reporter.Fail(exception);
            reporter.Terminate();
        });
        reporter.Initialize();
        report.GetAwaiter().GetResult();
    }
}
