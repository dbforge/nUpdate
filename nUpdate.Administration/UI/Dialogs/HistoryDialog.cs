// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.History;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class HistoryDialog : BaseDialog
    {
        private readonly Stack<ActionListItem> _logItemsStack = new Stack<ActionListItem>();

        public HistoryDialog()
        {
            InitializeComponent();
        }

        private void clearLog_Click(object sender, EventArgs e)
        {
            try
            {
                Project.Log.Clear();
                UpdateProject.SaveProject(Project.Path, Project);
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while sclearing the log.", ex, PopupButtons.Ok)));
            }

            Close();
        }

        private void HistoryDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Project.Name, Program.VersionString);
            orderComboBox.SelectedIndex = 0;
            InitializeLog();
            SetActivationState();
        }

        /// <summary>
        ///     Initializes the log.
        /// </summary>
        private void InitializeLog()
        {
            if (Project.Log == null) return;
            if (Project.Log.Count == 0) return;
            foreach (var logEntry in Project.Log)
            {
                var item = new ActionListItem
                {
                    ItemText = $"{logEntry.PackageVersion} - {logEntry.EntryTime}"
                };

                switch (logEntry.Entry)
                {
                    case LogEntry.Create:
                        item.HeaderText = "Created package";
                        item.ItemImage = Resources.Create;
                        break;
                    case LogEntry.Delete:
                        item.HeaderText = "Deleted package";
                        item.ItemImage = Resources.Remove;
                        break;
                    case LogEntry.Upload:
                        item.HeaderText = "Uploaded package";
                        item.ItemImage = Resources.Upload;
                        break;
                }

                _logItemsStack.Push(item);
            }

            foreach (var item in _logItemsStack) historyList.Items.Add(item);
        }

        /// <summary>
        ///     Orders the listbox items ascending.
        /// </summary>
        private void OrderAscending()
        {
            var ascendingItems = new Stack<ActionListItem>();
            foreach (var item in _logItemsStack) ascendingItems.Push(item);

            historyList.Items.Clear();
            foreach (var orderedItem in ascendingItems) historyList.Items.Add(orderedItem);
        }

        private void orderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (orderComboBox.SelectedIndex)
            {
                case 0:
                    OrderDescending();
                    break;
                case 1:
                    OrderAscending();
                    break;
            }
        }

        /// <summary>
        ///     Orders the listbox items descending.
        /// </summary>
        private void OrderDescending()
        {
            historyList.Items.Clear();
            foreach (var orderedItem in _logItemsStack) historyList.Items.Add(orderedItem);
        }

        private void saveToFileButton_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text files (*.txt)|*.txt";
                if (sfd.ShowDialog() != DialogResult.OK) return;
                var logEntryList =
                    Project.Log.Select(
                            logEntry =>
                                $"{logEntry.PackageVersion}-{logEntry.Entry}-{logEntry.EntryTime}")
                        .ToList();
                File.WriteAllLines(sfd.FileName, logEntryList);
            }
        }

        private void SetActivationState()
        {
            if (_logItemsStack.Count == 0)
            {
                noHistoryLabel.Visible = true;
                clearLogButton.Enabled = false;
                saveToFileButton.Enabled = false;
                orderComboBox.Enabled = false;
            }
            else
            {
                noHistoryLabel.Visible = false;
                clearLogButton.Enabled = true;
                saveToFileButton.Enabled = true;
                orderComboBox.Enabled = true;
            }
        }
    }
}