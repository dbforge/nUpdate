// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PreferencesDialog : BaseDialog
    {
        private readonly List<string> _cultureNames = new List<string>();

        private readonly UpdateManager _manager =
            new UpdateManager(new Uri("http://www.nupdate.net/updates.json"), "NochNix",
                new UpdateVersion("0.1.0.0"), new CultureInfo("en"));

        private CultureInfo[] _cultureInfos = {};

        public PreferencesDialog()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            _cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo info in _cultureInfos)
            {
                languagesComboBox.Items.Add(String.Format("{0} - {1}", info.EnglishName, info.Name));
                _cultureNames.Add(info.Name);
            }

            versionLabel.Text += " 0.1.0.0";
            includeAlphaCheckBox.Checked = Settings.Default.IncludeAlpha;
            includeBetaCheckBox.Checked = Settings.Default.IncludeBeta;
            languagesComboBox.SelectedIndex = _cultureNames.FindIndex(item => item == Settings.Default.Language.Name);
        }

        private void searchUpdatesButton_Click(object sender, EventArgs e)
        {
            //this.manager.LanguageCulture = nUpdate.Core.Language.Language.Spanish;
            var updaterUi = new UpdaterUI(_manager);
            updaterUi.ShowUserInterface();
        }

        private void includeAlphaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _manager.IncludeAlpha = includeAlphaCheckBox.Checked;
        }

        private void includeBetaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _manager.IncludeBeta = includeBetaCheckBox.Checked;
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
                    var jsonEditorDialog = new JsonEditorDialog();
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

            var jsonEditorDialog = new JsonEditorDialog();
            jsonEditorDialog.LanguageContent = Serializer.Serialize(lp);
            jsonEditorDialog.CultureName = name;
            jsonEditorDialog.ShowDialog(this);
        }
    }
}