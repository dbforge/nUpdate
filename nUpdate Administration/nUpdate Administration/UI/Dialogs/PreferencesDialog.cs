using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Dialogs;
using nUpdate.Administration.UI.Popups;
using nUpdate.Dialogs;
using nUpdate.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Forms;

namespace nUpdate.Administration
{
    public partial class PreferencesDialog : BaseDialog
    {
        private UpdateManager manager = new UpdateManager(new Uri("http://www.nupdate.net/administration/updates.json"), "NochNix", new nUpdate.Internal.UpdateVersion("1.1.0.0"));
        private CultureInfo[] cultureInfos = new CultureInfo[] { };
        private List<string> cultureNames = new List<string>();

        public PreferencesDialog()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var info in cultureInfos)
            {
                this.languagesComboBox.Items.Add(String.Format("{0} - {1}", info.EnglishName, info.Name));
                cultureNames.Add(info.Name);
            }
            
            this.versionLabel.Text += " 1.1.0.0";
            this.includeAlphaCheckBox.Checked = Properties.Settings.Default.IncludeAlpha;
            this.includeBetaCheckBox.Checked = Properties.Settings.Default.IncludeBeta;
            this.saveCredentialsCheckBox.Checked = Properties.Settings.Default.SaveCredentials;
            this.languagesComboBox.SelectedIndex = this.cultureNames.FindIndex(item => item == Properties.Settings.Default.Language.Name);
        }

        private void searchUpdatesButton_Click(object sender, EventArgs e)
        {
            //this.manager.LanguageCulture = nUpdate.Core.Language.Language.Spanish;
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

        private void languagesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = new CultureInfo(this.languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split(new char[] { '-' })[1].Trim()).Name;
            if (!File.Exists(Path.Combine(Program.LanguagesDirectory, String.Format("{0}.json", name))))
            {
                editLanguageButton.Enabled = false;
                if (Popup.ShowPopup(this, SystemIcons.Information, "Could not select language.", "There is no language file defined for the selected language. Would you like to create one, now?", PopupButtons.YesNo) == DialogResult.Yes)
                {
                    var jsonEditorDialog = new JSONEditorDialog();
                    LocalizationProperties lp = new LocalizationProperties();
                    jsonEditorDialog.LanguageContent = Serializer.Serialize(lp);
                    jsonEditorDialog.CultureName = name;
                    jsonEditorDialog.ShowDialog(this);
                }
            }
            else
            {
                editLanguageButton.Enabled = true;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!this.saveCredentialsCheckBox.Checked && Properties.Settings.Default.SaveCredentials)
            {
                List<string> unnecessaryKeys = new List<string>();
                foreach (var projectEntry in Program.ExisitingProjects)
                {
                    try
                    {
                        UpdateProject project = ApplicationInstance.LoadProject(projectEntry.Value);
                        project.FtpUsername = null;
                        project.FtpPassword = null;
                        ApplicationInstance.SaveProject(project.Path, project);
                    }
                    catch (FileNotFoundException)
                    {
                        unnecessaryKeys.Add(projectEntry.Key);
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, String.Format("Error while editing {0}", projectEntry.Key), ex, PopupButtons.OK);
                    }
                }

                unnecessaryKeys.ForEach(x => Program.ExisitingProjects.Remove(x));
            }

            Properties.Settings.Default.SaveCredentials = this.saveCredentialsCheckBox.Checked;
            Properties.Settings.Default.Language = new CultureInfo(this.languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split(new char[] {'-'})[1].Trim());
            Properties.Settings.Default.IncludeAlpha = this.includeAlphaCheckBox.Checked;
            Properties.Settings.Default.IncludeBeta = this.includeBetaCheckBox.Checked;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void editLanguageButton_Click(object sender, EventArgs e)
        {
            LocalizationProperties lp = new LocalizationProperties();
            var name = new CultureInfo(this.languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split(new char[] { '-' })[1].Trim()).Name;

            var jsonEditorDialog = new JSONEditorDialog();
            jsonEditorDialog.LanguageContent = Serializer.Serialize(lp);
            jsonEditorDialog.CultureName = name;
            jsonEditorDialog.ShowDialog(this);
        }
    }
}
