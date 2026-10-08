using System.Globalization;
using System.Reflection;
using System.Windows.Input;
using nUpdate.Ui;
using nUpdate.Updating;

namespace nUpdate.UI.WPF.ViewModel;

/// <summary>Lists the found updates with their changelog and lets the user start the installation.</summary>
public sealed class ChangelogViewModel : DialogViewModel
{
    internal ChangelogViewModel(UpdateManager updateManager)
        : base(updateManager)
    {
        var summary = new UpdateSummary(updateManager, Environment.NewLine);
        Header = summary.Header;
        InfoText = summary.InfoText;
        AvailableVersionsText = summary.AvailableVersionsText;
        CurrentVersionText = summary.CurrentVersionText;
        UpdateSizeText = summary.UpdateSizeText;
        TouchesText = summary.TouchesText;
        ChangelogText = summary.ChangelogText;
        AfterInstallText = summary.AfterInstallText;
        ShowsShield = updateManager.RunInstallerAsAdmin;

        InstallCommand = new RelayCommand(() => RequestClose(true));
        CancelCommand = new RelayCommand(() => RequestClose(false));
    }

    public override string WindowTitle => UpdateManager.ApplicationName;

    public string Header { get; }

    public string InfoText { get; }

    public string AvailableVersionsText { get; }

    public string CurrentVersionText { get; }

    public string UpdateSizeText { get; }

    public string TouchesText { get; }

    public string ChangelogText { get; }

    /// <summary>That the application stays closed after the update, or <c>null</c> when it restarts or keeps running.</summary>
    public string? AfterInstallText { get; }

    public bool ShowsAfterInstallText => AfterInstallText is not null;

    /// <summary>Whether the install button shows the UAC shield because the installer runs elevated.</summary>
    public bool ShowsShield { get; }

    public ICommand InstallCommand { get; }

    public ICommand CancelCommand { get; }
}
