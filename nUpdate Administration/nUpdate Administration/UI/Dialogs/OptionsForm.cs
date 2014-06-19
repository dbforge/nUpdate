using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using nUpdate.Internal;
using nUpdate.Administration.UI.Controls;
using nUpdate.Dialogs;

namespace nUpdate.Administration
{
    public partial class OptionsForm : BaseForm
    {
        UpdateManager manager = new UpdateManager(new Uri("http://www.trade-programming.de/nupdate/administration/updates.json"), "NochNix", Assembly.GetExecutingAssembly().GetName().Version);

        public OptionsForm()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            versionLabel.Text += String.Format(" {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (!ownRadioButton.Checked)
            {
                languageFileTextBox.Enabled = false;
                languageFileSearchButton.Enabled = false;
            }

            saveCredentialsCheckBox.Checked = Properties.Settings.Default.SaveCredentials;
        }

        private void searchUpdatesButton_Click(object sender, EventArgs e)
        {
            manager.Language = nUpdate.Core.Language.Language.English;
            var updaterUI = new UpdaterUI(manager);
            updaterUI.ShowUserInterface();
        }

        private void includeAlphaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            manager.IncludeAlpha = includeAlphaCheckBox.Checked;
        }

        private void includeBetaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            manager.IncludeBeta = includeBetaCheckBox.Checked;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveCredentials = saveCredentialsCheckBox.Checked;
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
