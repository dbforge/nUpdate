// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.Application;
using nUpdate.Administration.Logging;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    internal partial class HistoryDialog : BaseDialog
    {
        private readonly Stack<ActionListItem> _logItemsStack = new Stack<ActionListItem>();

        public HistoryDialog()
        {
            InitializeComponent();
        }
        
        private void Initialize()
        {
            if (Session.ActiveProject.LogData == null)
            {
                Session.ActiveProject.LogData = new List<PackageActionLogData>();
            }

            foreach (var logDataEntry in Session.ActiveProject.LogData)
            {
                var item = new ActionListItem
                {
                    ItemText = $"{logDataEntry.PackageVersion} - {logDataEntry.EntryDateTime}"
                };

                switch (logDataEntry.PackageActionType)
                {
                    case PackageActionType.CreatePackage:
                        item.HeaderText = "Created package";
                        item.ItemImage = Resources.Create;
                        break;
                    case PackageActionType.DeletePackage:
                        item.HeaderText = "Deleted package";
                        item.ItemImage = Resources.Remove;
                        break;
                    case PackageActionType.UploadPackage:
                        item.HeaderText = "Uploaded package";
                        item.ItemImage = Resources.Upload;
                        break;
                }
                _logItemsStack.Push(item);
            }

            foreach (var item in _logItemsStack)
            {
                historyList.Items.Add(item);
            }
        }
        
        private void OrderAscending()
        {
            var ascendingItems = new Stack<ActionListItem>();
            foreach (var item in _logItemsStack)
            {
                ascendingItems.Push(item);
            }

            historyList.Items.Clear();
            foreach (var orderedItem in ascendingItems)
            {
                historyList.Items.Add(orderedItem);
            }
        }
        
        private void OrderDescending()
        {
            historyList.Items.Clear();
            foreach (var orderedItem in _logItemsStack)
            {
                historyList.Items.Add(orderedItem);
            }
        }

        private void HistoryDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Session.ActiveProject.Name, Program.VersionString);
            orderComboBox.SelectedIndex = 0;
            Initialize();

            if (!_logItemsStack.Any())
                return;

            noHistoryLabel.Visible = false;
            clearLogButton.Enabled = true;
            saveToFileButton.Enabled = true;
            orderComboBox.Enabled = true;
        }

        private void clearLog_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Logger.Clear();
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while clearing the log.", ex, PopupButtons.Ok)));
            }
            Close();
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

        private void saveToFileButton_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text files (*.txt)|*.txt";
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                var logEntryList =
                    Session.ActiveProject.LogData.Select(
                        logEntry =>
                            $"{logEntry.PackageVersion}-{logEntry.PackageActionType}-{logEntry.EntryDateTime}")
                        .ToList();
                File.WriteAllLines(sfd.FileName, logEntryList);
            }
        }
    }
}