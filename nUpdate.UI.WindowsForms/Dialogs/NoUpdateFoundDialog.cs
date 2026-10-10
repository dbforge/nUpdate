using nUpdate.Updating;

namespace nUpdate.UI.WindowsForms.Dialogs;

/// <summary>Tells the user that the application is up to date.</summary>
internal sealed partial class NoUpdateFoundDialog : BaseDialog
{
    internal NoUpdateFoundDialog(UpdateManager updateManager)
        : base(updateManager)
    {
        InitializeComponent();
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void NoUpdateFoundDialog_Load(object sender, EventArgs e)
    {
        closeButton.Text = Localization.Close;
        headerLabel.Text = Localization.NoUpdatesTitle;
        infoLabel.Text = Localization.NoUpdatesInfo;
    }
}
