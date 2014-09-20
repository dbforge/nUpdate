// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 28-08-2014 17:49
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectManagementDialog : Form
    {
        public ProjectManagementDialog()
        {
            InitializeComponent();
        }

        public void ProjectManagementForm_Load(object sender, EventArgs e)
        {
            foreach (var existingProject in Program.ExisitingProjects)
            {
                string projectPath = existingProject.Value;
                FileInfo info = null;
                try
                {
                    info = new FileInfo(projectPath);
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project information.",
                        String.Format("The information for the project \"{0}\" couldn't be loaded."), PopupButtons.Ok);
                    continue;
                }

                var item = new ListViewItem(existingProject.Key);
                item.SubItems.Add(projectPath);
                item.SubItems.Add(info.CreationTime.ToString());
            }
        }

        public void deleteButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null) // TODO: Rename ListView
            {
                ListViewItem item = listView1.SelectedItems[0];
                listView1.Items.Remove(item);
            }
        }
    }
}