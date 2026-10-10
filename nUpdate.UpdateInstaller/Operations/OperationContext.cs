using nUpdate.Installer;

namespace nUpdate.UpdateInstaller.Operations;

/// <summary>What an operation handler needs besides the operation itself.</summary>
internal sealed class OperationContext(
    InstallerOptions options,
    InstallerServices services,
    PathPlaceholderResolver paths,
    ProgressTracker progress,
    IProgressReporter reporter)
{
    public InstallerOptions Options { get; } = options ?? throw new ArgumentNullException(nameof(options));

    public InstallerServices Services { get; } = services ?? throw new ArgumentNullException(nameof(services));

    public PathPlaceholderResolver Paths { get; } = paths ?? throw new ArgumentNullException(nameof(paths));

    public ProgressTracker Progress { get; } = progress ?? throw new ArgumentNullException(nameof(progress));

    public IProgressReporter Reporter { get; } = reporter ?? throw new ArgumentNullException(nameof(reporter));

    /// <summary>Marks a task as done and reports it with the localized text for <paramref name="key" />.</summary>
    public void Report(InstallerText key, params object[] arguments) =>
        Reporter.ReportOperationProgress(Progress.Advance(), Options.Text(key, arguments));

    /// <summary>Reports what is happening without finishing a task, for example while waiting for a process.</summary>
    public void Status(InstallerText key, params object[] arguments) =>
        Reporter.ReportOperationProgress(Progress.Percentage, Options.Text(key, arguments));
}
