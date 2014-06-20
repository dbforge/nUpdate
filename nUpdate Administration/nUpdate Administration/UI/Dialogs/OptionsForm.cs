using nUpdate.Dialogs;
using nUpdate.Internal;
using System;
using System.Reflection;

namespace nUpdate.Administration
{
    public partial class OptionsForm : BaseForm
    {
        private UpdateManager manager = new UpdateManager(new Uri("http://www.trade-programming.de/nupdate/administration/updates.json"), "NochNix", Assembly.GetExecutingAssembly().GetName().Version);

        public OptionsForm()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            this.versionLabel.Text += String.Format(" {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (!this.ownRadioButton.Checked)
            {
                this.languageFileTextBox.Enabled = false;
                this.languageFileSearchButton.Enabled = false;
            }

            this.saveCredentialsCheckBox.Checked = Properties.Settings.Default.SaveCredentials;
        }

        private void searchUpdatesButton_Click(object sender, EventArgs e)
        {
            this.manager.Language = nUpdate.Core.Language.Language.English;
            var updaterUI = new UpdaterUI(this.manager);
            updaterUI.ShowUserInterface();
        }

        private void includeAlphaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.manager.IncludeAlpha = this.includeAlphaCheckBox.Checked;
        }

        private void includeBetaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.manager.IncludeBeta = this.includeBetaCheckBox.Checked;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveCredentials = this.saveCredentialsCheckBox.Checked;
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
