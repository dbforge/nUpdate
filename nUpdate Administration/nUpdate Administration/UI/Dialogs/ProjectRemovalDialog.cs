// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 28-08-2014 17:49
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectRemovalDialog : BaseDialog
    {
        private const int COR_E_ENDOFSTREAM = unchecked((int)0x80070026);
        private const int COR_E_FILELOAD = unchecked((int)0x80131621);
        private const int COR_E_FILENOTFOUND = unchecked((int)0x80070002);
        private const int COR_E_DIRECTORYNOTFOUND = unchecked((int)0x80070003);

        public ProjectRemovalDialog()
        {
            InitializeComponent();
        }

        private void CheckProjectsAreAvailable()
        {
            if (projectsTreeView.Nodes[0].Nodes.Count != 0) return;
            projectsTreeView.Visible = false;
            noProjectsLabel.Visible = true;
        }

        private void ProjectRemovalDialog_Load(object sender, EventArgs e)
        {
            var mainNode = new TreeNode("nUpdate Administration (Projects)");
            projectsTreeView.Nodes.Add(mainNode);
            projectsTreeView.Nodes[0].HideCheckBox();

            foreach (var projectNode in Program.ExisitingProjects.Select(existingProject => new TreeNode(existingProject.Key) { Checked = true }))
            {
                projectsTreeView.Nodes[0].Nodes.Add(projectNode);

                if (!projectsTreeView.Nodes[0].IsExpanded)
                    projectsTreeView.Nodes[0].Toggle();
            }

            CheckProjectsAreAvailable();
        }

        /// <summary>
        ///     Returns the name/description of the current exception.
        /// </summary>
        private string GetNameOfExceptionType(Exception ex)
        {
            int hrEx = Marshal.GetHRForException(ex);
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

        private void projectsTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
                return;

            var projectName = e.Node.Text;

            if (
                Popup.ShowPopup(this, SystemIcons.Question, String.Format("Delete project {0}?", projectName),
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
                        String.Format("Error while removing the project {0}.", projectName), ex, PopupButtons.Ok);
                    e.Node.Checked = true;
                    return;
                }
            }

            try
            {
                File.Delete(Program.ExisitingProjects.First(item => item.Key == projectName).Value);
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
                if (Program.ExisitingProjects.Any(item => item.Key == projectName))
                {
                    Program.ExisitingProjects.Remove(
                        Program.ExisitingProjects.First(item => item.Key == projectName).Key);

                    var projectEntries =
                        Program.ExisitingProjects.Select(
                            projectEntry => String.Format("{0}%{1}", projectEntry.Key, projectEntry.Value)).ToList();

                    File.WriteAllText(Program.ProjectsConfigFilePath, String.Join("\n", projectEntries));
                }
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