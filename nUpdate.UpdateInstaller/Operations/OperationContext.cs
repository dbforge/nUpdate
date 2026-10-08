using nUpdate.Installer;

namespace nUpdate.UpdateInstaller.Operations;

/// <summary>What an operation handler needs besides the operation itself.</summary>
public sealed class OperationContext
{
    public OperationContext(InstallerOptions options, InstallerServices services, PathPlaceholderResolver paths,
        ProgressTracker progress, IProgressReporter reporter)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Paths = paths ?? throw new ArgumentNullException(nameof(paths));
        Progress = progress ?? throw new ArgumentNullException(nameof(progress));
        Reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
    }

    public InstallerOptions Options { get; }

    public InstallerServices Services { get; }

    public PathPlaceholderResolver Paths { get; }

    public ProgressTracker Progress { get; }

    public IProgressReporter Reporter { get; }

    /// <summary>Marks a task as done and reports it with the localized text for <paramref name="key" />.</summary>
    public void Report(InstallerText key, params object[] arguments) => Reporter.ReportOperationProgress(Progress.Advance(), Options.Text(key, arguments));

    /// <summary>Reports what is happening without finishing a task, for example while waiting for a process.</summary>
    public void Status(InstallerText key, params object[] arguments) => Reporter.ReportOperationProgress(Progress.Percentage, Options.Text(key, arguments));
}
