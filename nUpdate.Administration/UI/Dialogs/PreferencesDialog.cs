// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PreferencesDialog : BaseDialog
    {
        private readonly List<string> _cultureNames = new List<string>();
        private CultureInfo[] _cultureInfos = { };

        public PreferencesDialog()
        {
            InitializeComponent();
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
                File.Move(file.FullName, Path.Combine(destinationPath, file.Name));
        }

        private void editLanguageButton_Click(object sender, EventArgs e)
        {
            var lp = new LocalizationProperties();
            var name =
                new CultureInfo(
                    languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split('-')[1].Trim()).Name;

            var jsonEditorDialog =
                new JsonEditorDialog {LanguageContent = Serializer.Serialize(lp), CultureName = name};
            jsonEditorDialog.ShowDialog(this);
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

        private void PreferencesDialog_Load(object sender, EventArgs e)
        {
            _cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var info in _cultureInfos)
            {
                languagesComboBox.Items.Add($"{info.EnglishName} - {info.Name}");
                _cultureNames.Add(info.Name);
            }

            Text = string.Format(Text, Program.VersionString);
            languagesComboBox.SelectedIndex = _cultureNames.FindIndex(item => item == Settings.Default.Language.Name);
            programPathTextBox.Text = Settings.Default.ProgramPath;
            programPathTextBox.Initialize();
        }

        private void programPathTextBox_ButtonClicked(object sender, EventArgs e)
        {
            using (var browserDialog = new FolderBrowserDialog())
            {
                if (browserDialog.ShowDialog() == DialogResult.OK)
                    programPathTextBox.Text = browserDialog.SelectedPath;
            }
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
                try
                {
                    var projectConfiguration = ProjectConfiguration.Load();
                    if (projectConfiguration != null)
                    {
                        foreach (
                            var project in
                            projectConfiguration.Select(config => UpdateProject.LoadProject(config.Path))
                                .Where(project => project.Packages != null))
                        {
                            foreach (var package in project.Packages)
                                package.LocalPackagePath = Path.Combine(programPathTextBox.Text, "Projects",
                                    project.Name,
                                    Directory.GetParent(package.LocalPackagePath).Name,
                                    Path.GetFileName(package.LocalPackagePath));

                            UpdateProject.SaveProject(project.Path, project);
                        }

                        CopyFilesRecursively(Settings.Default.ProgramPath, programPathTextBox.Text);
                    }
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while moving the program data.", ex,
                        PopupButtons.Ok);
                    return;
                }

            Settings.Default.Language =
                new CultureInfo(
                    languagesComboBox.GetItemText(languagesComboBox.SelectedItem).Split('-')[1].Trim());
            Settings.Default.ProgramPath = programPathTextBox.Text;
            Settings.Default.Save();
            Settings.Default.Reload();
            Close();
        }
    }
}