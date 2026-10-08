using System.Globalization;
using nUpdate.Updating;

namespace nUpdate.Ui;

/// <summary>The texts of the dialog that presents the found updates, shared by the built-in user interfaces.</summary>
internal sealed class UpdateSummary
{
    /// <param name="manager">A manager whose last check found updates.</param>
    /// <param name="newLine">The line break of the changelog.</param>
    public UpdateSummary(UpdateManager manager, string newLine)
    {
        if (manager is null)
            throw new ArgumentNullException(nameof(manager));

        var culture = CultureInfo.CurrentCulture;
        var texts = manager.Texts;
        var packages = manager.AvailableUpdates;
        Header = string.Format(culture, packages.Count > 1 ? texts.NewUpdatesTitle : texts.NewUpdateTitle, packages.Count);
        InfoText = string.Format(culture, texts.NewUpdateInfo, manager.ApplicationName);
        AvailableVersionsText = string.Format(culture, texts.AvailableVersions, VersionRangeFormatter.Format(packages.Select(p => p.Version)));
        CurrentVersionText = string.Format(culture, texts.CurrentVersion, manager.CurrentVersion.ToString());
        UpdateSizeText = string.Format(culture, texts.TotalSize, ByteSizeFormatter.Format(manager.TotalDownloadSize, culture));
        var touches = TouchesFormatter.Describe(packages, manager.Platform, texts);
        TouchesText = $"{texts.Touches} {(touches.Count == 0 ? "-" : string.Join(", ", touches))}";
        ChangelogText = ChangelogFormatter.Format(packages, manager.Culture, newLine);
    }

    public string Header { get; }

    public string InfoText { get; }

    public string AvailableVersionsText { get; }

    public string CurrentVersionText { get; }

    public string UpdateSizeText { get; }

    /// <summary>What the operations of the packages touch, or a dash.</summary>
    public string TouchesText { get; }

    public string ChangelogText { get; }
}
