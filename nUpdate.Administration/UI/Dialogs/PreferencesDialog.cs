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
            new UpdateManager(new Uri("http://www.nupdate.net/updates.json"), "<RSAKeyValue><Modulus>0PARrhQkpmJTm68X896pPoeRUvmhk4DCwfbzuAJEh30QCyZpoQAJdWSkKb/UO9GAkvQ4Hcj7hVOXzEfz9O+WQqOIbsrqnYIGqFrbSkXEai2qNZ2WCWj7ir+j5BOkfPSzlgbDCle0+mbMNeK0z9GRWjD/kPfU8UxIXPux3FtOSP67A4SyiYDQ7BD4QcA757juJzC3GLMNUtuZVesR2sVwx98vGP0F1/iJvUKs3lZV4PM0aCWktcrkQsGpSX4yPaKj9JXEnnEyUycIzdl49xUFw8FW4G0tGWRyjWot/mKVAleU3uiTFNihIIhqbGZheGzSRDVpgxUa6fjBYcvmVeBEqC7OiZLNH4Q+Ns3NgZsJckua4fKcPWjMkizsohvm6z23NlGytTBAh4lnoHOS3ucDiLP41MRG5t2zNzz0QDzzUoWE2zZ5y7nrjIf+jLD4ki3ze/HiR3ejvbf0niN528KP/jBkRLwbZMvswSfB9gPHGEKaFPYVpgY+ohtJ41hJSJKFHMeUlRfER1TFwUPN/PHku+7a+fVsgvv5o7X/VK77X+1erTmqw/lfMPxLfuJF+lFmnmIpDdh9s8Wus4+7VqGisalGtNrfBY2cEzxPLeD5gnaRUuxy9nCJBP1RYE7/V3wxnZJ+mJlDKhRJsoQ1Nx2EbeElBbidJ5t0ZzIluJNv0FZdZHfiraCOlzM+qIWDWwBxZ/t3QLUelfmmwYj1E5ff3hR5NYu4hTm0Z/Z/IheRnTnG/HQGgEHwqoFF+xkfJSptdUS4mxrZr7bsYRosk/+nkHrYX6STFPIJcW+VaHQVUyZfvdwR0wFW5uc4ZNtls87oNbcJrgp1fa/iI/f0DwmRGQhvpB/+8uMLv8GkS0J5FbXTW2hl2xleZjCm8r5+HW55ZAASGmD8px/1Zs6iAqrWFtf2RggwlJFts7mW6d+0H+/zN6rldwzUb3v3Ox0H1Gso0XRMwPMHPvKOulG2ZVJpZhirWavXXXOWt4T8aLL/sfKPbLy1ofhgwZpC0KIP2ZpOV1brQz+AgTqThOZWeDc+oFtSu8V6vSR/i2C2tSq26QzQ8kjmU+mneGb4X1IrfAz/DXmkLKctV3512+niRqoX2zP8+3skYXQkV+f/Bo+txbdUJLfg6Sd1pZKwWmiJNPvS8qnNnH1B93TAkpF/TVac4ccl7u6xL+sqiivkcZi9sH+LRoFEm0T5XcxW+OjUI+GyshBpSQT+6Vlysf7zLInC5n0kkS6jcZNAiCAUF7vhwaKC9t9kAFlr0RGNsX9r2PYeqp2+/NelI/WUtn0eY+DgtWg04/UDjK7JMAvkWKCfmDS4PvcRAwXeYixUPd5pq1odgXFy0e4GVFoFNENNH33Hmw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new CultureInfo("en"));

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