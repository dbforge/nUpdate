using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using nUpdate.Ui;
using nUpdate.UI.WindowsForms.Win32;
using nUpdate.Updating;

namespace nUpdate.UI.WindowsForms.Dialogs;

/// <summary>Lists the found updates with their changelog and lets the user start the installation.</summary>
internal sealed partial class NewUpdateDialog : BaseDialog
{
    internal NewUpdateDialog(UpdateManager updateManager)
        : base(updateManager)
    {
        InitializeComponent();
    }

    private void changelogTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        if (e.LinkText is { Length: > 0 } link)
            Process.Start(link);
    }

    private void installButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void NewUpdateDialog_Load(object sender, EventArgs e)
    {
        var summary = new UpdateSummary(UpdateManager, "\n");
        headerLabel.Text = summary.Header;
        infoLabel.Text = summary.InfoText;
        newestVersionLabel.Text = summary.AvailableVersionsText;
        currentVersionLabel.Text = summary.CurrentVersionText;
        updateSizeLabel.Text = summary.UpdateSizeText;
        accessLabel.Text = summary.TouchesText;
        changelogLabel.Text = Localization.Changelog;
        changelogTextBox.Text = summary.ChangelogText;
        cancelButton.Text = Localization.Cancel;
        installButton.Text = Localization.Install;

        if (ApplicationIcon.Get() is { } icon)
        {
            iconPictureBox.Image = icon.ToBitmap();
            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;
        }

        if (UpdateManager.RunInstallerAsAdmin)
            NativeMethods.AddShieldToButton(installButton);
    }
}
