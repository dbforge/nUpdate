using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nUpdate.Administration;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Dialogs;
using nUpdate.Administration.UI.Popups;
using nUpdate.Administration.Core.Application;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class HistoryForm : BaseForm
    {
        Stack<ActionListItem> historyItemsStack = new Stack<ActionListItem>();

        /// <summary>
        /// Sets the name of the project.
        /// </summary>
        public string ProjectName
        {
            get;
            set;
        }

        public HistoryForm()
        {
            InitializeComponent();
        }

        private void HistoryForm_Load(object sender, EventArgs e)
        {
            string logFilePath = Path.Combine(Program.Path, "Projects", ProjectName, "log.log");
            string[] existingLog = null;

            try
            {
                existingLog = File.ReadAllLines(Path.Combine(Program.Path, "Projects", ProjectName, "log.log"));
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while reading the history.", ex, PopupButtons.OK);
                Close();
                return;
            }

            for (int i = 0; i <= existingLog.Length - 1; i++)
            {
                string[] parts = existingLog[i].Split('-');
                if (parts == null || parts.Length != 3)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while reading the history.", String.Format("The arguments for line {0} are invalid.", i +1), PopupButtons.OK);
                    continue;
                }

                string time = parts[0];
                string packageStatus = parts[1];
                string pathVersion = parts[2];

                string fullPath = Path.Combine(Program.Path, "Projects", ProjectName, "Packages", pathVersion);

                ActionListItem historyItem = new ActionListItem();

                switch (packageStatus)
                {
                    case "C":
                        historyItem.HeaderText = "Created package";
                        historyItem.HeaderImage = Properties.Resources.Create;
                        break;
                    case "U":
                        historyItem.HeaderText = "Uploaded package";
                        historyItem.HeaderImage = Properties.Resources.Upload;
                        break;
                    case "D":
                        historyItem.HeaderText = "Deleted package";
                        historyItem.HeaderImage = Properties.Resources.Remove;
                        break;
                    case "E":
                        historyItem.HeaderText = "Edited package";
                        historyItem.HeaderImage = Properties.Resources.Edit;
                        break;
                }
                
                historyItem.ItemText = String.Format("{0} - {1}", time, Path.GetFileName(fullPath));
                historyItemsStack.Push(historyItem);
            }

            foreach (ActionListItem item in historyItemsStack)
            {
                historyList.Items.Add(item);
            }

            if (int.Equals(historyList.Items.Count, 0))
            {
                openButton.Enabled = false;
                showDetailsButton.Enabled = false;
                clearLogButton.Enabled = false;
                noHistoryLabel.Visible = true;
            }

            Text = String.Format(Text, ProjectName);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            ActionListItem selectedItem = ActionListItem.TryParse(historyList.SelectedItem);
            if (selectedItem != null)
            {
                string descriptionPath = selectedItem.Description.Split('-').Last().Trim().Substring(1);

                bool isValidWindowsPath = true;

                if (!Directory.Exists(descriptionPath))
                    isValidWindowsPath = false;

                switch (isValidWindowsPath)
                {
                    case true:
                        System.Diagnostics.Process.Start("explorer.exe", descriptionPath);
                        break;
                    case false:
                        return;
                }
            }
        }

        private void clearLog_Click(object sender, EventArgs e)
        {
            //Log log = new Log(ProjectName);
            //log.Clear();

            Close();
        }
    } 
}
