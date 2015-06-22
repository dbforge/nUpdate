// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using nUpdate.Core;
using nUpdate.Updating;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PreferencesDialog : BaseDialog
    {
        private CultureInfo[] _cultureInfos = {};
        private readonly List<string> _cultureNames = new List<string>();

        private readonly UpdateManager _manager =
            new UpdateManager(new Uri("http://www.nupdate.net/updates.json"), "<RSAKeyValue><Modulus>vU+LMKv535ujKwvGjnmwEf8SgtUmhg9vdlAVozJLtRmvtRHG78p07kjq0eLQxA1TiQ5Sg73j9yuHhcC9L4b+hxs4E9FyuK4L8gJBrPHljUjVDv/y1T67voBBIO1q+3oTGKLmoefQY/VQyWR15EAWSCqlxSrgjL2r2ZDPWY13R7OIdL37X98PPjf5c/+423TZdmVrnAsaUCjqiW9VXIehk0pC4gEJ/0KCqEPXhUml6HUO2606BXGv4oMm/7Vf+zKKI0EC9sN6+wbLa5qq5c+BXzxcOmmklcvt7Y2GxKYWtk0ulPlklMiUukdMCEvy4cePv0Caj/ijh1Q1FMNthcEfIGPowFUdmqtd+mGMt+w5I5O+0JQ+gn75KfcGN3NqKBCvIjuvH0duzI4cVXOjhufnuhetLMPd6vpMrxIqDoo2bMm5NP5t94+uy1HYtfTWcUAdRnb4QBXIhrtvl/dF3JKnZYv9hScbu066rAI+uZ6fVKWHzJbIHKrtGgIipM9maKQ3tMx8PS4W2Rk0Iv4smQvUfWHP2UgPDBSepkirq61jq0lWhjhEPlgcmt4CZxA79xoELA7b04LxmanvnthvlwXWE3Ml1YAUAUGrcz2GsEkroLGw+8GApU4CWAFzAa2+bL1FbmqJZlWjrr71UHyUFyUjJEDCWQgy6Aqx4OPf+Y/UEo1EjAK95rx8LJTNWtTtNNPcg3BpZ2vXGBtAtXXnfsvOrlZfRVLcxFpZPgb9mv4GJFLDDFYEWGGc9HIMQc9acPbo2kzaFnbwJ2eeXo/3cF+2ECmJGvxdyttJFZJDPvfKXn3DYM2KqVU/1ASUyk6RTan9Vxh7x6j2ogC0cioWazn9kZ7ZzVWdVShQ8mWULbG67Ov18fcoipJUjoUX51+V5FVBtnaYFcdjqJWSDdyN7IiLjZvxV6w03sIaAO9TYZu84bXA50CsxSYDM1Wy3GfJLise7cgYRNf755rt9eg8YckSq8vTYJAQIc1Z27JBcv16NtSil5Ytzj0qPyXEQMfM2HRsoItpsl/Bvr5dg7xxuNRiwNub8jr3OJ8/mCBJQOtLvs7X2a9IxGtsATZnFxtNi0kXJuJfamsGVtxpHYUcuqdcVyyPf0LwmZ4ilLsTu4qImOxEWCu/j3EKLxAOU64D7C1MVmz8iFrbfpEp0mWBgCveSfB61K3JV+4NAKroqaru2KIOcjc5PQapiNtU429bmX73RVq5oqtz/O77HMYMjHR0cNx0SThC/11Mxm8rIfFuha6bAquP0OH61f81JexLNdJULjGZgq8e6uPyXEvaXLoB8Gb2PFlYP5ioTe3M/9m/uBA6v/x4ItUb3bfjBrn7RwL1gszx8GDQK4lavxIQzBe2qQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new CultureInfo("en"));

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
            versionLabel.Text += " 3.0.0.0 Beta 2";
            includeAlphaCheckBox.Checked = Settings.Default.IncludeAlpha;
            includeBetaCheckBox.Checked = Settings.Default.IncludeBeta;
            languagesComboBox.SelectedIndex = _cultureNames.FindIndex(item => item == Settings.Default.Language.Name);
            programPathTextBox.Text = Settings.Default.ProgramPath;
            programPathTextBox.Initialize();
        }

        private void searchUpdatesButton_Click(object sender, EventArgs e)
        {
            var updaterUi = new UpdaterUI(_manager, SynchronizationContext.Current);
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
                    var projectConfiguration = ProjectConfiguration.Load();
                    if (projectConfiguration != null)
                    {
                        foreach (var project in projectConfiguration.Select(config => UpdateProject.LoadProject(config.Path)).Where(project => project.Packages != null))
                        {
                            foreach (var package in project.Packages)
                            {
                                package.LocalPackagePath = Path.Combine(programPathTextBox.Text, "Projects",
                                    project.Name,
                                    Directory.GetParent(package.LocalPackagePath).Name,
                                    Path.GetFileName(package.LocalPackagePath));
                            }

                            UpdateProject.SaveProject(project.Path, project);
                        }

                        CopyFilesRecursively(Settings.Default.ProgramPath, programPathTextBox.Text);
                    }
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