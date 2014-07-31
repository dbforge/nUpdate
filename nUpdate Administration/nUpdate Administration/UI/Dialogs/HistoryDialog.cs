using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Core.Update.History;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class HistoryDialog : BaseDialog
    {
        private Stack<ActionListItem> logItemsStack = new Stack<ActionListItem>();

        public HistoryDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the log.
        /// </summary>
        private void InitializeLog()
        {
            if (this.Project.Log != null)
            {
                foreach (Log logEntry in this.Project.Log)
                {
                    var item = new ActionListItem();
                    item.ItemText = String.Format("{0} - {1}", logEntry.PackageVersion, logEntry.EntryTime);
                    switch (logEntry.Entry)
                    {
                        case LogEntry.Create:
                            item.HeaderText = "Created package";
                            item.HeaderImage = Properties.Resources.Create;
                            break;
                        case LogEntry.Delete:
                            item.HeaderText = "Deleted package";
                            item.HeaderImage = Properties.Resources.Remove;
                            break;
                        case LogEntry.Upload:
                            item.HeaderText = "Uploaded package";
                            item.HeaderImage = Properties.Resources.Upload;
                            break;
                    }
                    this.logItemsStack.Push(item);
                }

                foreach (ActionListItem item in this.logItemsStack)
                {
                    this.historyList.Items.Add(item);
                }

                if (this.logItemsStack.Count == 0)
                {
                    this.noHistoryLabel.Visible = true;
                    this.clearLogButton.Enabled = false;
                    this.saveToFileButton.Enabled = false;
                    this.orderComboBox.Enabled = false;
                }
                else
                {
                    this.noHistoryLabel.Visible = false;
                    this.clearLogButton.Enabled = true;
                    this.saveToFileButton.Enabled = true;
                    this.orderComboBox.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Orders the listbox items ascending.
        /// </summary>
        private void OrderAscending()
        {
            Stack<ActionListItem> ascendingItems = new Stack<ActionListItem>();
            foreach (ActionListItem item in this.logItemsStack)
            {
                ascendingItems.Push(item);
            }

            this.historyList.Items.Clear();
            foreach (ActionListItem orderedItem in ascendingItems)
            {
                this.historyList.Items.Add(orderedItem);
            }
        }

        /// <summary>
        /// Orders the listbox items descending.
        /// </summary>
        private void OrderDescending()
        {
            this.historyList.Items.Clear();
            foreach (ActionListItem orderedItem in this.logItemsStack)
            {
                this.historyList.Items.Add(orderedItem);
            }
        }

        private void HistoryDialog_Load(object sender, EventArgs e)
        {
            this.Text = String.Format(this.Text, this.Project.Name);
            this.orderComboBox.SelectedIndex = 0;
            this.InitializeLog();
        }

        private void clearLog_Click(object sender, EventArgs e)
        {
            this.Project.Log.Clear();
            ApplicationInstance.SaveProject(this.Project.Path, this.Project);
            this.Close();
        }

        private void orderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.orderComboBox.SelectedIndex)
            {
                case 0:
                    this.OrderDescending();
                    break;
                case 1:
                    this.OrderAscending();
                    break;
            }
        }

        private void saveToFileButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
               sfd.Filter = "Text files (*.txt)|*.txt";
               if (sfd.ShowDialog() == DialogResult.OK)
               {
                   List<string> logEntryList = new List<string>();
                   foreach (Log logEntry in this.Project.Log)
                   {
                       logEntryList.Add(String.Format("{0}-{1}-{2}", logEntry.PackageVersion, logEntry.Entry, logEntry.EntryTime));
                   }
                   File.WriteAllLines(sfd.FileName, logEntryList);
               }
            }
        }
    }
}
