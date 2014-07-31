using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Dialogs;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal;
using UpdateVersion = nUpdate.Internal.UpdateVersion;

namespace nUpdate.Administration
{
    public partial class PreferencesDialog : BaseDialog
    {
        private readonly List<string> cultureNames = new List<string>();

        private readonly UpdateManager manager =
            new UpdateManager(new Uri("http://www.nupdate.net/administration/updates.json"), "NochNix",
                new UpdateVersion("1.1.0.0"));

        private CultureInfo[] cultureInfos = {};

        public PreferencesDialog()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo info in cultureInfos)
            {
                languagesComboBox.Items.Add(String.Format("{0} - {1}", info.EnglishName, info.Name));
                cultureNames.Add(info.Name);
            }

            versionLabel.Text += " 1.1.0.0";
            includeAlphaCheckBox.Checked = Settings.Default.IncludeAlpha;
            includeBetaCheckBox.Checked = Settings.Default.IncludeBeta;
            saveCredentialsCheckBox.Checked = Settings.Default.SaveCredentials;
            languagesComboBox.SelectedIndex = cultureNames.FindIndex(item => item == Settings.Default.Language.Name);
        }

        private void searchUpdatesButton_Click(object sender, EventArgs e)
        {
            //this.manager.LanguageCulture = nUpdate.Core.Language.Language.Spanish;
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

        private void languagesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name =
                new CultureInfo(
                    languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split(new[] {'-'})[1].Trim()).Name;
            if (!File.Exists(Path.Combine(Program.LanguagesDirectory, String.Format("{0}.json", name))))
            {
                editLanguageButton.Enabled = false;
                if (
                    Popup.ShowPopup(this, SystemIcons.Information, "Could not select language.",
                        "There is no language file defined for the selected language. Would you like to create one, now?",
                        PopupButtons.YesNo) == DialogResult.Yes)
                {
                    var jsonEditorDialog = new JSONEditorDialog();
                    var lp = new LocalizationProperties();
                    jsonEditorDialog.LanguageContent = Serializer.Serialize(lp);
                    jsonEditorDialog.CultureName = name;
                    jsonEditorDialog.ShowDialog(this);
                }
            }
            else
                editLanguageButton.Enabled = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!saveCredentialsCheckBox.Checked && Settings.Default.SaveCredentials)
            {
                var unnecessaryKeys = new List<string>();
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
                        Popup.ShowPopup(this, SystemIcons.Error,
                            String.Format("Error while editing {0}", projectEntry.Key), ex, PopupButtons.OK);
                    }
                }

                unnecessaryKeys.ForEach(x => Program.ExisitingProjects.Remove(x));
            }

            Settings.Default.SaveCredentials = saveCredentialsCheckBox.Checked;
            Settings.Default.Language =
                new CultureInfo(
                    languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split(new[] {'-'})[1].Trim());
            Settings.Default.IncludeAlpha = includeAlphaCheckBox.Checked;
            Settings.Default.IncludeBeta = includeBetaCheckBox.Checked;
            Settings.Default.Save();
            Close();
        }

        private void editLanguageButton_Click(object sender, EventArgs e)
        {
            var lp = new LocalizationProperties();
            string name =
                new CultureInfo(
                    languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split(new[] {'-'})[1].Trim()).Name;

            var jsonEditorDialog = new JSONEditorDialog();
            jsonEditorDialog.LanguageContent = Serializer.Serialize(lp);
            jsonEditorDialog.CultureName = name;
            jsonEditorDialog.ShowDialog(this);
        }
    }
}