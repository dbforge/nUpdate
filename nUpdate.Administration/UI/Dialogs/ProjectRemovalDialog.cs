// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectRemovalDialog : BaseDialog
    {
        private const int COR_E_ENDOFSTREAM = unchecked((int) 0x80070026);
        private const int COR_E_FILELOAD = unchecked((int) 0x80131621);
        private const int COR_E_FILENOTFOUND = unchecked((int) 0x80070002);
        private const int COR_E_DIRECTORYNOTFOUND = unchecked((int) 0x80070003);
        private List<ProjectConfiguration> _projectConfiguration;

        public ProjectRemovalDialog()
        {
            InitializeComponent();
        }

        private void CheckProjectsAreAvailable()
        {
            if (projectsTreeView.Nodes[0].Nodes.Count != 0)
                return;
            projectsTreeView.Visible = false;
            noProjectsLabel.Visible = true;
        }

        /// <summary>
        ///     Returns the name/description of the current exception.
        /// </summary>
        private string GetNameOfExceptionType(Exception ex)
        {
            var hrEx = Marshal.GetHRForException(ex);
            switch (hrEx)
            {
                case COR_E_DIRECTORYNOTFOUND:
                    return "DirectoryNotFound";
                case COR_E_ENDOFSTREAM:
                    return "EndOfStream";
                case COR_E_FILELOAD:
                    return "FileLoadException";
                case COR_E_FILENOTFOUND:
                    return "FileNotFound";
            }

            return "Unknown Exception";
        }

        private void ProjectRemovalDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Program.VersionString);
            var mainNode = new TreeNode("nUpdate Administration (Projects)");
            projectsTreeView.Nodes.Add(mainNode);
            projectsTreeView.Nodes[0].HideCheckBox();
            _projectConfiguration =
                Serializer.Deserialize<List<ProjectConfiguration>>(
                    File.ReadAllText(Program.ProjectsConfigFilePath)) ?? new List<ProjectConfiguration>();

            foreach (
                var projectNode in _projectConfiguration.Select(project => new TreeNode(project.Name) {Checked = true}))
            {
                projectsTreeView.Nodes[0].Nodes.Add(projectNode);

                if (!projectsTreeView.Nodes[0].IsExpanded)
                    projectsTreeView.Nodes[0].Toggle();
            }

            CheckProjectsAreAvailable();
        }

        private void projectsTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
                return;

            var projectName = e.Node.Text;
            var index = _projectConfiguration.FindIndex(item => item.Name == projectName);

            if (
                Popup.ShowPopup(this, SystemIcons.Question, $"Delete project {projectName}?",
                    "Are you sure you want to remove the selected project from nUpdate Administration?",
                    PopupButtons.YesNo) == DialogResult.No)
            {
                e.Node.Checked = true;
                return;
            }

            try
            {
                Directory.Delete(Path.Combine(Program.Path, "Projects", projectName), true);
            }
            catch (Exception ex)
            {
                if (GetNameOfExceptionType(ex) != "DirectoryNotFound")
                {
                    Popup.ShowPopup(this, SystemIcons.Error,
                        $"Error while removing the project {projectName}.", ex, PopupButtons.Ok);
                    e.Node.Checked = true;
                    return;
                }
            }

            try
            {
                File.Delete(_projectConfiguration[index].Path);
            }
            catch (Exception ex)
            {
                if (GetNameOfExceptionType(ex) != "FileNotFound")
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while deleting the project file.", ex,
                        PopupButtons.Ok);
                    e.Node.Checked = true;
                    return;
                }
            }

            try
            {
                _projectConfiguration.RemoveAt(index);
                File.WriteAllText(Program.ProjectsConfigFilePath, Serializer.Serialize(_projectConfiguration));
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while editing the project confiuration file.", ex,
                    PopupButtons.Ok);
                e.Node.Checked = true;
                return;
            }

            projectsTreeView.Nodes.Remove(e.Node);
            CheckProjectsAreAvailable();
        }
    }
}