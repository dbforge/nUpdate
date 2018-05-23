// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ionic.Zip;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core;
using nUpdate.Updating;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectImportDialog : BaseDialog
    {
        private List<ProjectConfiguration> _projectConfigurations;
        private TabPage _sender;

        public ProjectImportDialog()
        {
            InitializeComponent();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (_sender == importTabPage || _sender == shareTabPage)
            {
                wizardTabControl.SelectedTab = optionTabPage;
                _sender = optionTabPage;
                backButton.Enabled = false;
            }
            else if (_sender == importTabPage1)
            {
                wizardTabControl.SelectedTab = importTabPage;
                _sender = importTabPage;
            }
            else if (_sender == shareTabPage1)
            {
                wizardTabControl.SelectedTab = shareTabPage;
                _sender = shareTabPage;
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            backButton.Enabled = true;
            if (_sender == optionTabPage)
            {
                if (importProjectRadioButton.Checked)
                {
                    wizardTabControl.SelectedTab = importTabPage;
                    _sender = importTabPage;
                }
                else if (shareProjectRadioButton.Checked)
                {
                    wizardTabControl.SelectedTab = shareTabPage;
                    _sender = shareTabPage;
                }
            }
            else if (_sender == importTabPage)
            {
                if (!ValidationManager.Validate(importTabPage))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                if (!Path.IsPathRooted(projectToImportTextBox.Text) || !File.Exists(projectToImportTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.",
                        "The given local path for the project is invalid.", PopupButtons.Ok);
                    return;
                }

                wizardTabControl.SelectedTab = importTabPage1;
                _sender = importTabPage1;
            }
            else if (_sender == importTabPage1)
            {
                if (!ValidationManager.Validate(importTabPage1))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                if (_projectConfigurations.Any(item => item.Name == projectNameTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Project already existing.",
                        $"A project with the name \"{projectNameTextBox.Text}\" does already exist.",
                        PopupButtons.Ok);
                    return;
                }

                try
                {
                    string folderPath = Path.Combine(Program.Path, "ImpProj");
                    string statisticsFilePath = Path.Combine(folderPath, "statistics.php");
                    string projectFilePath = Path.Combine(folderPath,
                        $"{projectNameTextBox.Text}.nupdproj");
                    Directory.CreateDirectory(folderPath);

                    var updateProject = UpdateProject.LoadProject(projectFilePath);
                    if (updateProject.UseStatistics)
                    {
                        Popup.ShowPopup(this, SystemIcons.Warning, "Incompatible project.",
                            "This project cannot be imported because the support for projects using statistics is currently missing. It will be available in the next version(s) of nUpdate Administration.",
                            PopupButtons.Ok);
                        Directory.Delete(folderPath);
                        return;
                    }

                    //if (updateProject.ConfigVersion != "3b2")
                    //{
                    //    Popup.ShowPopup(this, SystemIcons.Warning, "Incompatible project.", "This project is not compatible to this version of nUpdate Administration. Please download the newest version of nUpdate Administration and then export the project again.", PopupButtons.Ok);
                    //    Directory.Delete(folderPath);
                    //    return;
                    //}

                    updateProject.Path = projectFilePathTextBox.Text;
                    //if (updateProject.UseStatistics)
                    //{
                    //    var statisticsServers = Serializer.Deserialize<List<StatisticsServer>>(Path.Combine(Program.Path, "statservers.json"));
                    //    if (!statisticsServers.Any(item => item.WebUrl == updateProject.SqlWebUrl && item.DatabaseName == updateProject.SqlDatabaseName && item.Username == updateProject.SqlUsername))
                    //    {
                    //        if (Popup.ShowPopup(this, SystemIcons.Information, "New statistics server found.", "This project uses a statistics server that isn't currently available on this computer. Should nUpdate Administration add this server to your list?", PopupButtons.YesNo) == DialogResult.Yes)
                    //        {
                    //            statisticsServers.Add(new StatisticsServer(updateProject.SqlWebUrl, updateProject.SqlDatabaseName, updateProject.SqlUsername)); 
                    //            File.WriteAllText(Path.Combine(Program.Path, "statservers.json"), Serializer.Serialize(statisticsServers));
                    //        }
                    //    }
                    //}

                    UpdateProject.SaveProject(updateProject.Path, updateProject);

                    string projectPath = Path.Combine(Program.Path, "Projects", projectNameTextBox.Text);
                    if (!Directory.Exists(projectPath))
                        Directory.CreateDirectory(projectPath);

                    using (var zip = new ZipFile(projectToImportTextBox.Text))
                    {
                        zip.ExtractAll(folderPath);
                    }

                    if (File.Exists(statisticsFilePath))
                        File.Move(statisticsFilePath, Path.Combine(projectPath, "statistics.php"));
                    File.Move(projectFilePath, projectFilePathTextBox.Text);

                    foreach (var versionDirectory in new DirectoryInfo(folderPath).GetDirectories())
                        Directory.Move(versionDirectory.FullName, Path.Combine(projectPath, versionDirectory.Name));

                    Directory.Delete(folderPath);
                    _projectConfigurations.Add(new ProjectConfiguration(projectNameTextBox.Text,
                        projectFilePathTextBox.Text));
                    File.WriteAllText(Program.ProjectsConfigFilePath, Serializer.Serialize(_projectConfigurations));
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while importing the project.", ex, PopupButtons.Ok);
                    return;
                }

                Close();
            }
            else if (_sender == shareTabPage)
            {
                wizardTabControl.SelectedTab = shareTabPage1;
                _sender = shareTabPage1;
            }
            else if (_sender == shareTabPage1)
            {
                if (!ValidationManager.Validate(shareTabPage1))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.Ok);
                    return;
                }

                if (!Path.IsPathRooted(projectOutputPathTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.",
                        "The given local path for the project is invalid.", PopupButtons.Ok);
                    return;
                }

                try
                {
                    string projectPath =
                        Path.Combine(Program.Path, "Projects", projectsListBox.SelectedItem.ToString());
                    using (var zip = new ZipFile())
                    {
                        string statisticsFilePath = Path.Combine(projectPath, "statistics.php");
                        if (File.Exists(statisticsFilePath))
                            zip.AddFile(statisticsFilePath, "/");
                        zip.AddFile(
                            _projectConfigurations.First(item => item.Name == projectsListBox.SelectedItem.ToString())
                                .Path, "/");

                        foreach (
                            var versionDirectory in
                            new DirectoryInfo(projectPath).GetDirectories()
                                .Where(item => UpdateVersion.IsValid(item.Name)))
                        {
                            zip.AddDirectoryByName(versionDirectory.Name);
                            zip.AddDirectory(versionDirectory.FullName, versionDirectory.Name);
                        }

                        zip.Save(projectOutputPathTextBox.Text);
                    }
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while sharing the project.", ex, PopupButtons.Ok);
                    return;
                }

                Close();
            }
        }

        private void projectFilePathTextBox_ButtonClicked(object sender, EventArgs e)
        {
            using (var fileDialog = new SaveFileDialog())
            {
                fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
                fileDialog.CheckFileExists = false;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                    projectFilePathTextBox.Text = fileDialog.FileName;
            }
        }

        private void ProjectImportDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Program.VersionString);
            _sender = optionTabPage;

            projectFilePathTextBox.Initialize();
            projectOutputPathTextBox.Initialize();
            projectToImportTextBox.Initialize();

            _projectConfigurations = ProjectConfiguration.Load().ToList();
            if (_projectConfigurations.Any())
            {
                projectsListBox.Items.AddRange(
                    _projectConfigurations.Select(item => item.Name).Cast<object>().ToArray());
                projectsListBox.SelectedIndex = 0;
            }
            else
            {
                shareProjectRadioButton.Enabled = false;
            }
        }

        private void projectOutputPathTextBox_ButtonClicked(object sender, EventArgs e)
        {
            using (var fileDialog = new SaveFileDialog())
            {
                fileDialog.Filter = "Shared Project Files (*.zip)|*.zip";
                fileDialog.CheckFileExists = false;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                    projectOutputPathTextBox.Text = fileDialog.FileName;
            }
        }

        private void projectToImportTextBox_ButtonClicked(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "Shared Project Files (*.zip)|*.zip";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                    projectToImportTextBox.Text = fileDialog.FileName;
            }
        }
    }
}