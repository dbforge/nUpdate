// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using nUpdate.Administration.UserInterface.Controls;
using nUpdate.Administration.UserInterface.Popups;

namespace nUpdate.Administration.UserInterface.Dialogs
{
    internal partial class StatisticalServerDialog : BaseDialog
    {
        private List<StatisticsServer> _statisticalServers;

        public StatisticalServerDialog()
        {
            InitializeComponent();
        }
        
        public Uri WebUri { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public bool ReactsOnKeyDown { get; set; }

        private bool InitializeServers()
        {
            if (serverList.Items.Count > 0)
                serverList.Items.Clear();

            try
            {
                var sourceContent = File.ReadAllText(FilePathProvider.StatisticServersFilePath);
                _statisticalServers = Serializer.Deserialize<List<StatisticsServer>>(sourceContent);
                if (_statisticalServers == null || _statisticalServers.Count == 0)
                {
                    _statisticalServers = new List<StatisticsServer>();
                    noServersLabel.Visible = true;
                }
                else
                {
                    noServersLabel.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the servers.",
                    ex, PopupButtons.Ok);
                return false;
            }

            foreach (var server in _statisticalServers)
            {
                try
                {
                    var listItem = new ServerListItem
                    {
                        ItemImage = imageList1.Images[0],
                        HeaderText = server.DatabaseName,
                        ItemText = $"Web-URL: \"{server.WebUri}\" - Database: \"{server.DatabaseName}\""
                    };

                    serverList.Items.Add(listItem);
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, $"Error while loading \"{server.DatabaseName}\"", ex, PopupButtons.Ok);
                    return false;
                }
            }

            return true;
        }

        private void StatisticsServerDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Program.VersionString);
            if (!InitializeServers())
            {
                Close();
                return;
            }

            if (ReactsOnKeyDown)
            {
                Popup.ShowPopup(this, SystemIcons.Information, "Selecting a statistics-server.",
                    "To select a statistics server, select one in the list and press \"Enter\".", PopupButtons.Ok);
            }
        }

        private void addServerButton_Click(object sender, EventArgs e)
        {
            var statisticsServerAddDialog = new StatisticalServerAddDialog();
            if (statisticsServerAddDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                _statisticalServers.Add(new StatisticsServer(statisticsServerAddDialog.WebUri,
                    statisticsServerAddDialog.DatabaseName, statisticsServerAddDialog.Username));
                File.WriteAllText(FilePathProvider.StatisticServersFilePath, Serializer.Serialize(_statisticalServers));
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the server data.",
                    ex, PopupButtons.Ok);
                return;
            }

            InitializeServers(); // Re-initialize the servers again
        }

        private void deleteServerButton_Click(object sender, EventArgs e)
        {
            if (serverList.SelectedItem == null)
                return;
            if (Popup.ShowPopup(this, SystemIcons.Warning, "Delete this server?",
                "Are you sure that you want to delete this server from the server list?", PopupButtons.YesNo) !=
                TaskDialogResult.Yes)
                return;

            try
            {
                _statisticalServers.RemoveAt(serverList.SelectedIndex);
                File.WriteAllText(FilePathProvider.StatisticServersFilePath, Serializer.Serialize(_statisticalServers));
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the server data.",
                    ex, PopupButtons.Ok);
                return;
            }

            InitializeServers(); // Re-initialize the servers
        }

        private void StatisticsServerDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (serverList.SelectedItem == null)
                return;

            if (e.KeyCode != Keys.Enter || !ReactsOnKeyDown)
                return;

            var statisticsServer = _statisticalServers.ElementAt(serverList.SelectedIndex);
            DatabaseName = statisticsServer.DatabaseName;
            WebUri = statisticsServer.WebUri;
            Username = statisticsServer.Username;

            DialogResult = DialogResult.OK;
        }

        private void editServerButton_Click(object sender, EventArgs e)
        {
            if (serverList.SelectedItem == null)
                return;

            var itemIndex = serverList.SelectedIndex;
            var statisticsServer = _statisticalServers.ElementAt(itemIndex);
            var statisticsServerEditDialog = new StatisticalServerEditDialog
            {
                DatabaseName = statisticsServer.DatabaseName,
                WebUri = statisticsServer.WebUri,
                Username = statisticsServer.Username
            };

            if (statisticsServerEditDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                _statisticalServers[itemIndex] = new StatisticsServer(statisticsServerEditDialog.WebUri,
                    statisticsServerEditDialog.DatabaseName, statisticsServerEditDialog.Username);
                File.WriteAllText(FilePathProvider.StatisticServersFilePath, Serializer.Serialize(_statisticalServers));
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the server data.",
                    ex, PopupButtons.Ok);
                return;
            }

            InitializeServers(); // Re-initialize the servers again
        }
    }
}