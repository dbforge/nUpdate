// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectRemovalDialog : BaseDialog
    {
        private List<ProjectConfiguration> _projectConfiguration;

        public ProjectRemovalDialog()
        {
            InitializeComponent();
        }

        private void CheckProjectsAvailability()
        {
            if (projectsTreeView.Nodes[0].Nodes.Count != 0)
                return;
            projectsTreeView.Visible = false;
            noProjectsLabel.Visible = true;
        }

        private void ProjectRemovalDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Program.VersionString);
            var mainNode = new TreeNode("nUpdate Administration (Projects)");
            projectsTreeView.Nodes.Add(mainNode);
            projectsTreeView.Nodes[0].HideCheckBox();
            _projectConfiguration = ProjectConfiguration.Load().ToList();

            foreach (
                var projectNode in _projectConfiguration.Select(project => new TreeNode(project.Name) {Checked = true, Tag = project.Guid}))
            {
                projectsTreeView.Nodes[0].Nodes.Add(projectNode);

                if (!projectsTreeView.Nodes[0].IsExpanded)
                    projectsTreeView.Nodes[0].Toggle();
            }

            CheckProjectsAvailability();
        }

        private void projectsTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
                return;

            var projectGuid = (Guid)e.Node.Tag;
            var index = _projectConfiguration.FindIndex(item => item.Guid == projectGuid);

            if (
                Popup.ShowPopup(this, SystemIcons.Question, $"Delete project {e.Node.Text}?",
                    "Are you sure you want to remove the selected project from nUpdate Administration?",
                    PopupButtons.YesNo) == DialogResult.No)
            {
                e.Node.Checked = true;
                return;
            }

            try
            {
                Directory.Delete(Path.Combine(Program.Path, "Projects", e.Node.Text), true);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(DirectoryNotFoundException))
                {
                    Popup.ShowPopup(this, SystemIcons.Error,
                        $"Error while removing the project {e.Node.Text}.", ex, PopupButtons.Ok);
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
                if (ex.GetType() != typeof(FileNotFoundException))
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
            CheckProjectsAvailability();
        }
    }
}