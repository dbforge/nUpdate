// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Administration.Localization;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UserInterface.Popups;
using nUpdate.UI.WinForms;

namespace nUpdate.Administration.UserInterface.Dialogs
{
    internal partial class PreferencesDialog : BaseDialog
    {
        private readonly List<string> _cultureNames = new List<string>();
        private CultureInfo[] _cultureInfos = {};
        private Updater _updater;

        public PreferencesDialog()
        {
            InitializeComponent();
        }

        private void PreferencesDialog_Load(object sender, EventArgs e)
        {
            _cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var info in _cultureInfos)
            {
                languagesComboBox.Items.Add($"{info.EnglishName} - {info.Name}");
                _cultureNames.Add(info.Name);
            }

            Text = string.Format(Text, Program.VersionString);
            versionLabel.Text += " 4.0.0";
            includeAlphaCheckBox.Checked = Settings.Default.IncludeAlpha;
            includeBetaCheckBox.Checked = Settings.Default.IncludeBeta;
            languagesComboBox.SelectedIndex = _cultureNames.FindIndex(item => item == Settings.Default.Language.Name);
            programPathTextBox.Text = Settings.Default.ProgramPath;
            programPathTextBox.Initialize();
        }

        private void searchUpdatesButton_Click(object sender, EventArgs e)
        {
            _updater = new Updater(new Uri("http://www.nupdate.net/updates/updates.json"),
                "<RSAKeyValue><Modulus>lLM5SxeIVrEJZyT/QfsZANZ5rdxHriQ85axYspvfmtVEV1QcoOcNhjieODYpTYlpfVieFCP+ktwS56Y4Q1nsesembolrmnS3nUsFZs39jWkkJZG41e9IliwdJRbn6vueBxsUOUDsXVQVmXyrSjrzxCT6AaTcK9IQoAp+/3Ecrib+dMbzwGeCKR/b2gNyFSjAagILLrq1NuRlYGYKlT4MUHbLgHlj4M3GgVEunu32RjAYNcafYPHjesIa92PqJhCNz2ksw66+tDgBmgYHaMwlDDxJFqbB+ALMqGAp6Mkuc9wCEVoYZZPEH4WSH7tz9OwRvkJqGYrJYCuyii9O3xWLZJiGmSAs5GMdmtzcC/RlgEGOQ63GSwCze/AM+8gw7pjF3PgApEkGbXz4GJRe4iPhbLb5bekFbu66KU1UsYOmx/dIVfcIokvVo0CKj6QWnzaBUiuhs7zH/qzW4BWLG+FsPbMDif9zQ86kdedvBY6YmtsZ3/zurAVa+ad63QCYchO1qenIjfRNZJqv33FKyVsH1ZV4u3TqaG6ygL4iZ0wJCPYc91mi6wUr5t8inHro20zASpwcldTEZjZwFuJIOa3vGmF+dI4xKyVUnDfEzC4JiNs5T1lLnaJvhLR6205zSwfKCBNVv5u9dvGZvEaifAfokC322B7euFzkzbgwXZXbj+ubxfQpRF3ZzwUsdvVvRN9FVGy8WW5fMnjflyGXYAi8rqpHPISRunGA/35I5tH63RCRl6OjkGHqcH1tCNCZP3Q2zraY4nazgFjATp9JgvThCc7VHhRIWOIvFe+5HhmNT43Rp/dZlCzXgSQgkaMC9VDHOhP0Q8Bpnzw+fKzn41ysPxiUnl1aDNzmieHaIdd0DDbmIO6H7qpQW+7Tp08zfxzn0TOzHrBeRyUUTTR8dRCWFgUerjNYTMA6C58aRIlOXxJ6xycBpa38g0eneV/WOKis2uzR4NlxPmkpAQVbS8L0EK/On80phRXSpsn0pob60hU5Yy2VrBW1ENwrGBu5o0TDq2a/Bg3aXQ/7FUUiukv6aIButv7D9YNDahnx5HFdIlkyyXcXeJSve8MliOjLPxm7LZFb7GceQz7JJSrobB6KapLJwZVBECeA1vmCGYr4m2nO3KS1Plj/7ugMSIfVIkoPtDYLydoGXkSEAa8V0rl+Cx9NcIOs722PgVlGTw4GcFSxAGWLddmIe//kmeflS+EbbtFZr3+dCYbSGg4R9Qvx/g8SpwsXIssjmrlD76jyNyKEO3l0xDO4EgvgsUUP0C2e/bBo2w2Gi6Hdb/0gMrCTE/LOwaQy1ic3yy4uHT5GPEkKOMVnZ93tsaPk8QFATFft89PTKrFSwiB6jkn6oFjFhQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
            var interactionUpdater = new DefaultInteractionUpdater(_updater, SynchronizationContext.Current);
            interactionUpdater.Begin();
        }

        private void includeAlphaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _updater.IncludeAlpha = includeAlphaCheckBox.Checked;
        }

        private void includeBetaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _updater.IncludeBeta = includeBetaCheckBox.Checked;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!Path.IsPathRooted(programPathTextBox.Text))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Invalid path set.",
                    "The current path for the program data is not valid.", PopupButtons.Ok);
                return;
            }

            if (programPathTextBox.Text != Settings.Default.ProgramPath)
            {
                try
                {
                    CopyFilesRecursively(Settings.Default.ProgramPath, programPathTextBox.Text);
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while moving the program data.", ex, PopupButtons.Ok);
                    return;
                }
            }

            Settings.Default.Language =
                new CultureInfo(
                    languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split('-')[1].Trim());
            Settings.Default.IncludeAlpha = includeAlphaCheckBox.Checked;
            Settings.Default.IncludeBeta = includeBetaCheckBox.Checked;
            Settings.Default.ProgramPath = programPathTextBox.Text;
            Settings.Default.Save();
            Settings.Default.Reload();
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

        private void CopyFilesRecursively(string sourcePath, string destinationPath)
        {
            foreach (var directory in new DirectoryInfo(sourcePath).GetDirectories())
            {
                string destination = Path.Combine(destinationPath, directory.Name);
                Directory.CreateDirectory(destination);
                CopyFilesRecursively(directory.FullName, destination);
                Directory.Delete(directory.FullName, true);
            }

            foreach (var file in new DirectoryInfo(sourcePath).GetFiles())
            {
                File.Move(file.FullName, Path.Combine(destinationPath, file.Name));
            }
        }

        private void programPathTextBox_ButtonClicked(object sender, EventArgs e)
        {
            using (var browserDialog = new FolderBrowserDialog())
            {
                if (browserDialog.ShowDialog() == DialogResult.OK)
                    programPathTextBox.Text = browserDialog.SelectedPath;
            }
        }
    }
}