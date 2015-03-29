// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Globalization;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Properties;
using nUpdate.Core;
using nUpdate.Updating;
using UpdateVersion = nUpdate.Updating.UpdateVersion;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PreferencesDialog : BaseDialog
    {
        private CultureInfo[] _cultureInfos = {};
        private readonly List<string> _cultureNames = new List<string>();

        private readonly UpdateManager _manager =
            new UpdateManager(new Uri("http://www.nupdate.net/updates/updates.json"), "<RSAKeyValue><Modulus>lLM5SxeIVrEJZyT/QfsZANZ5rdxHriQ85axYspvfmtVEV1QcoOcNhjieODYpTYlpfVieFCP+ktwS56Y4Q1nsesembolrmnS3nUsFZs39jWkkJZG41e9IliwdJRbn6vueBxsUOUDsXVQVmXyrSjrzxCT6AaTcK9IQoAp+/3Ecrib+dMbzwGeCKR/b2gNyFSjAagILLrq1NuRlYGYKlT4MUHbLgHlj4M3GgVEunu32RjAYNcafYPHjesIa92PqJhCNz2ksw66+tDgBmgYHaMwlDDxJFqbB+ALMqGAp6Mkuc9wCEVoYZZPEH4WSH7tz9OwRvkJqGYrJYCuyii9O3xWLZJiGmSAs5GMdmtzcC/RlgEGOQ63GSwCze/AM+8gw7pjF3PgApEkGbXz4GJRe4iPhbLb5bekFbu66KU1UsYOmx/dIVfcIokvVo0CKj6QWnzaBUiuhs7zH/qzW4BWLG+FsPbMDif9zQ86kdedvBY6YmtsZ3/zurAVa+ad63QCYchO1qenIjfRNZJqv33FKyVsH1ZV4u3TqaG6ygL4iZ0wJCPYc91mi6wUr5t8inHro20zASpwcldTEZjZwFuJIOa3vGmF+dI4xKyVUnDfEzC4JiNs5T1lLnaJvhLR6205zSwfKCBNVv5u9dvGZvEaifAfokC322B7euFzkzbgwXZXbj+ubxfQpRF3ZzwUsdvVvRN9FVGy8WW5fMnjflyGXYAi8rqpHPISRunGA/35I5tH63RCRl6OjkGHqcH1tCNCZP3Q2zraY4nazgFjATp9JgvThCc7VHhRIWOIvFe+5HhmNT43Rp/dZlCzXgSQgkaMC9VDHOhP0Q8Bpnzw+fKzn41ysPxiUnl1aDNzmieHaIdd0DDbmIO6H7qpQW+7Tp08zfxzn0TOzHrBeRyUUTTR8dRCWFgUerjNYTMA6C58aRIlOXxJ6xycBpa38g0eneV/WOKis2uzR4NlxPmkpAQVbS8L0EK/On80phRXSpsn0pob60hU5Yy2VrBW1ENwrGBu5o0TDq2a/Bg3aXQ/7FUUiukv6aIButv7D9YNDahnx5HFdIlkyyXcXeJSve8MliOjLPxm7LZFb7GceQz7JJSrobB6KapLJwZVBECeA1vmCGYr4m2nO3KS1Plj/7ugMSIfVIkoPtDYLydoGXkSEAa8V0rl+Cx9NcIOs722PgVlGTw4GcFSxAGWLddmIe//kmeflS+EbbtFZr3+dCYbSGg4R9Qvx/g8SpwsXIssjmrlD76jyNyKEO3l0xDO4EgvgsUUP0C2e/bBo2w2Gi6Hdb/0gMrCTE/LOwaQy1ic3yy4uHT5GPEkKOMVnZ93tsaPk8QFATFft89PTKrFSwiB6jkn6oFjFhQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>",
                new UpdateVersion("0.1.0.0a3"), new CultureInfo("en"));

        public PreferencesDialog()
        {
            InitializeComponent();
        }

        private void PreferencesDialog_Load(object sender, EventArgs e)
        {
            _cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var info in _cultureInfos)
            {
                languagesComboBox.Items.Add(String.Format("{0} - {1}", info.EnglishName, info.Name));
                _cultureNames.Add(info.Name);
            }

            Text = String.Format(Text, Program.VersionString);
            versionLabel.Text += " 0.1.0.0 Alpha 5";
            includeAlphaCheckBox.Checked = Settings.Default.IncludeAlpha;
            includeBetaCheckBox.Checked = Settings.Default.IncludeBeta;
            languagesComboBox.SelectedIndex = _cultureNames.FindIndex(item => item == Settings.Default.Language.Name);
        }

        private void searchUpdatesButton_Click(object sender, EventArgs e)
        {
            var updaterUi = new UpdaterUi(_manager);
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
            //string name =
            //    new CultureInfo(
            //        languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split(new[] {'-'})[1].Trim()).Name;
            //if (!File.Exists(Path.Combine(Program.LanguagesDirectory, String.Format("{0}.json", name))))
            //{
            //    editLanguageButton.Enabled = false;
            //    if (Popup.ShowPopup(this, SystemIcons.Information, "Could not select language.",
            //        "There is no language file defined for the selected language. Would you like to create one, now?",
            //        PopupButtons.YesNo) != DialogResult.Yes) 
            //        return;
            //    var jsonEditorDialog = new JsonEditorDialog();
            //    var lp = new LocalizationProperties();
            //    jsonEditorDialog.LanguageContent = Serializer.Serialize(lp);
            //    jsonEditorDialog.CultureName = name;
            //    jsonEditorDialog.ShowDialog(this);
            //}
            //else
            //    editLanguageButton.Enabled = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Language =
                new CultureInfo(
                    languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split('-')[1].Trim());
            Settings.Default.IncludeAlpha = includeAlphaCheckBox.Checked;
            Settings.Default.IncludeBeta = includeBetaCheckBox.Checked;
            Settings.Default.Save();
            Close();
        }

        private void editLanguageButton_Click(object sender, EventArgs e)
        {
            var lp = new LocalizationProperties();
            var name =
                new CultureInfo(
                    languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split('-')[1].Trim()).Name;

            var jsonEditorDialog = new JsonEditorDialog {LanguageContent = Serializer.Serialize(lp), CultureName = name};
            jsonEditorDialog.ShowDialog(this);
        }
    }
}