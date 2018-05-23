// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class StatisticsServerDialog : BaseDialog
    {
        private List<StatisticsServer> _statisticsServers;

        public StatisticsServerDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Sets if the dialog should react on key inputs, e. g. when a server should be selected.
        /// </summary>
        public bool ReactsOnKeyDown { get; set; }

        /// <summary>
        ///     The name of the SQL-database to use.
        /// </summary>
        public string SqlDatabaseName { get; set; }

        /// <summary>
        ///     The username for the SQL-login.
        /// </summary>
        public string SqlUsername { get; set; }

        /// <summary>
        ///     The url of the SQL-connection.
        /// </summary>
        public string SqlWebUrl { get; set; }

        private void addServerButton_Click(object sender, EventArgs e)
        {
            var statisticsServerAddDialog = new StatisticsServerAddDialog();
            if (statisticsServerAddDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                _statisticsServers.Add(new StatisticsServer(statisticsServerAddDialog.WebUrl,
                    statisticsServerAddDialog.DatabaseName, statisticsServerAddDialog.Username));
                File.WriteAllText(Program.StatisticServersFilePath, Serializer.Serialize(_statisticsServers));
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
                DialogResult.Yes)
                return;

            try
            {
                _statisticsServers.RemoveAt(serverList.SelectedIndex);
                File.WriteAllText(Program.StatisticServersFilePath, Serializer.Serialize(_statisticsServers));
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the server data.",
                    ex, PopupButtons.Ok);
                return;
            }

            InitializeServers(); // Re-initialize the servers
        }

        private void editServerButton_Click(object sender, EventArgs e)
        {
            if (serverList.SelectedItem == null)
                return;

            var itemIndex = serverList.SelectedIndex;
            var statisticsServer = _statisticsServers.ElementAt(itemIndex);
            var statisticsServerEditDialog = new StatisticsServerEditDialog
            {
                DatabaseName = statisticsServer.DatabaseName,
                WebUrl = statisticsServer.WebUrl,
                Username = statisticsServer.Username
            };

            if (statisticsServerEditDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                _statisticsServers[itemIndex] = new StatisticsServer(statisticsServerEditDialog.WebUrl,
                    statisticsServerEditDialog.DatabaseName, statisticsServerEditDialog.Username);
                File.WriteAllText(Program.StatisticServersFilePath, Serializer.Serialize(_statisticsServers));
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the server data.",
                    ex, PopupButtons.Ok);
                return;
            }

            InitializeServers(); // Re-initialize the servers again
        }

        /// <summary>
        ///     Initializes the statistic servers.
        /// </summary>
        private bool InitializeServers()
        {
            if (serverList.Items.Count > 0)
                serverList.Items.Clear();

            try
            {
                var sourceContent = File.ReadAllText(Program.StatisticServersFilePath);
                _statisticsServers = Serializer.Deserialize<List<StatisticsServer>>(sourceContent);
                if (_statisticsServers == null || _statisticsServers.Count == 0)
                {
                    _statisticsServers = new List<StatisticsServer>();
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

            foreach (var server in _statisticsServers)
                try
                {
                    var listItem = new ServerListItem
                    {
                        ItemImage = imageList1.Images[0],
                        HeaderText = server.DatabaseName,
                        ItemText = $"Web-URL: \"{server.WebUrl}\" - Database: \"{server.DatabaseName}\""
                    };

                    serverList.Items.Add(listItem);
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, $"Error while loading \"{server.DatabaseName}\"", ex,
                        PopupButtons.Ok);
                    return false;
                }

            return true;
        }

        private void StatisticsServerDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (serverList.SelectedItem == null)
                return;

            if (e.KeyCode != Keys.Enter || !ReactsOnKeyDown)
                return;

            var statisticsServer = _statisticsServers.ElementAt(serverList.SelectedIndex);
            SqlDatabaseName = statisticsServer.DatabaseName;
            SqlWebUrl = statisticsServer.WebUrl;
            SqlUsername = statisticsServer.Username;

            DialogResult = DialogResult.OK;
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
                Popup.ShowPopup(this, SystemIcons.Information, "Selecting a statistics-server.",
                    "To select a statistics server, select one in the list and press \"Enter\".", PopupButtons.Ok);
        }
    }
}