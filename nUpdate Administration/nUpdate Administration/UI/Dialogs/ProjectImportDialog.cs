using System;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectImportDialog : BaseDialog
    {
        private TabPage _sender;

        public ProjectImportDialog()
        {
            InitializeComponent();
        }

        private void ProjectImportDialog_Load(object sender, EventArgs e)
        {
            Text = String.Format(Text, Program.VersionString);
            _sender = optionTabPage;

            projectFilePathTextBox.Initialize();
            projectPathTextBox.Initialize();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (_sender == optionTabPage)
            {
                if (importProjectRadioButton.Checked)
                {
                    wizardTabControl.SelectedTab = importTabPage;
                    _sender = importTabPage;
                }
                else if (shareProjectRadioButton.Checked)
                {
                    wizardTabControl.SelectedTab = shareTabPage;
                    _sender = shareTabPage;
                }
            }
        }
    }
}
