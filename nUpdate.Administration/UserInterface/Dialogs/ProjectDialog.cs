// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.UserInterface.Popups;

namespace nUpdate.Administration.UserInterface.Dialogs
{
    internal partial class ProjectDialog : BaseDialog
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public ProjectDialog()
        {
            InitializeComponent();
            LoadingPanel = loadingPanel;
            packageListView.SetRowHeight(24);
            packageListView.DoubleBuffer();
            packageListView.MakeCollapsable();
            tabControl.DoubleBuffer();
        }

        private async void ProjectDialog_Shown(object sender, EventArgs e)
        {
            Text = string.Format(Text, Session.ActiveProject.Name, Program.VersionString);
            cancelToolTip.SetToolTip(cancelLabel, "Click here to cancel the package upload.");

            await Task.Run(() =>
            {
                DisableControls(true);
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Initializing project..."));
                Initialize();
                EnableControls();
            });

            NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;
            if (!WebConnection.IsAvailable())
            {
                SetNetworkDependentControlsState(false);
                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "No network connection available.",
                    "No active network connection is available. Most of the functions require a network connection in order to connect to services on the internet and have been deactivated until a network connection can be established.",
                    PopupButtons.Ok)));
                return;
            }

            checkMasterChannelLinkLabel.Enabled = false;
            tickPictureBox.Visible = false;
            await CheckRemoteData();
        }

        private void Initialize()
        {
            Invoke(new Action(() =>
            {
                nameTextBox.Text = Session.ActiveProject.Name;
                updateUriTextBox.Text = Session.ActiveProject.UpdateDirectoryUri.ToString();
                publicKeyTextBox.Text = Session.ActiveProject.PublicKey;
                projectIdTextBox.Text = Session.ActiveProject.Guid.ToString();

                string packageAmountString = Session.ActiveProject.Packages?.Count.ToString(CultureInfo.InvariantCulture) ?? "0";
                amountLabel.Text = packageAmountString;
                packagesCountLabel.Text = packageAmountString + " update packages.";
                    
                var listViewItems = Session.ActiveProject.Packages?.Select(x => new ListViewItem(new[]
                {
                    GetVersionDescription(x), x.Description, x.ReleaseDate.ToShortDateString(),
                    x.NecessaryUpdate.ToString()
                })).ToArray();

                if (listViewItems == null)
                    return;

                packageListView.Items.AddRange(listViewItems);
                newestPackageLabel.Text = listViewItems.Any() ? "" : "There aren't any update packages available.";
            }));
        }

        private static string GetVersionDescription(UpdatePackage package)
        {
            var versionStringBuilder = new StringBuilder(package.Version.ToString());
            if (package.ChannelName.ToLowerInvariant() != "release")
                versionStringBuilder.Append($" {package.ChannelName}");
            return versionStringBuilder.ToString();
        }

        public void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            SetNetworkDependentControlsState(e.IsAvailable);
            if (!e.IsAvailable)
                Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "No network connection available.",
                    "No active network connection is available. Most of the functions require a network connection in order to connect to services on the internet and have been deactivated until a network connection can be established.",
                    PopupButtons.Ok)));
        }

        private void SetNetworkDependentControlsState(bool enabled)
        {
            Invoke(new Action(() =>
            {
                checkMasterChannelLinkLabel.Enabled = enabled;
                addButton.Enabled = enabled;
                deleteButton.Enabled = enabled;
            }));
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            var matchingItem = packageListView.FindItemWithText(searchTextBox.Text, true, 0);
            if (matchingItem != null)
                packageListView.Items[matchingItem.Index].Selected = true;
            else
                packageListView.SelectedItems.Clear();

            searchTextBox.Clear();
            e.SuppressKeyPress = true;
        }

        private async void addButton_Click(object sender, EventArgs e)
        {
            var packageAddDialog = new PackageAddDialog();
            if (packageAddDialog.ShowDialog() != DialogResult.OK)
                return;

            // Re-initialize our data
            await Task.Run(() => Initialize());
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            // TODO: Implement
        }

        private void historyButton_Click(object sender, EventArgs e)
        {
            new HistoryDialog().ShowDialog();
        }

        private void packageListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            editButton.Enabled = false;
            uploadButton.Enabled = false;
            deleteButton.Enabled = false;

            editToolStripMenuItem.Enabled = false;
            uploadToolStripMenuItem.Enabled = false;
            deleteToolStripMenuItem.Enabled = false;

            if (packageListView.SelectedItems.Count == 1)
            {
                editButton.Enabled = true;
                editToolStripMenuItem.Enabled = true;
                deleteButton.Enabled = true;
                deleteToolStripMenuItem.Enabled = true;

                // If any of the selected items is already released, exit.
                if (
                    packageListView.SelectedItems.Cast<ListViewItem>()
                        .Any(item => item.Group != packageListView.Groups[1]))
                    return;
                uploadButton.Enabled = true;
                uploadToolStripMenuItem.Enabled = true;
            }
            else if (packageListView.SelectedItems.Count > 1)
            {
                deleteButton.Enabled = true;
                deleteToolStripMenuItem.Enabled = true;
            }
        }

        private void readOnlyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control || (e.KeyCode != Keys.A))
                return;
            ((TextBox) sender)?.SelectAll();
            e.Handled = true;
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            // TODO: Implement
        }

        private async Task UploadPackage(Version packageVersion)
        {
            var updatePackage =
                Session.ActiveProject.Packages.First(
                    item => item.Version.Equals(packageVersion));
            string packageFilePath = Path.Combine(Session.PackagesPath, packageVersion.ToString());
            if (!File.Exists(packageFilePath))
            {
                TaskDialogResult popupResult = TaskDialogResult.None;
                Invoke(
                    new Action(
                        () => popupResult = Popup.ShowPopup(this, SystemIcons.Error,
                            "Upload failed.",
                            "The package file of the selected version does not exist on your computer and cannot be uploaded. Should its reference be removed from the project?",
                            PopupButtons.YesNo)));

                if (popupResult == TaskDialogResult.Yes)
                {
                    Session.ActiveProject.Packages.Remove(updatePackage);
                    Session.ActiveProject.Save();
                }
            }

            var progress = new Progress<ITransferProgressData>();
            progress.ProgressChanged += (sender, args) =>
            {
                loadingLabel.Text =
                    $"Uploading... {$"{args.PercentComplete}% | {args.BytesPerSecond/1024}KB/s"}";
            };

            var factoryPackage = new UpdateFactoryPackage(packageFilePath, updatePackage);
            await Session.UpdateFactory.PushUpdate(factoryPackage, _cancellationTokenSource.Token, progress);
        }

        private void cancelLabel_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel(false);
        }

        private async void checkMasterChannelnLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            checkMasterChannelLinkLabel.Enabled = false;
            tickPictureBox.Visible = false;
            await CheckRemoteData();
        }

        private async Task CheckRemoteData()
        {
            var releaseControls = new Action(() => 
            {
                checkMasterChannelLinkLabel.Enabled = true;
                activeTaskLabel.Text = "No active tasks.";
            });

            IEnumerable<UpdateChannel> masterChannel;
            try
            {
                activeTaskLabel.Text = "Checking master channel...";
                masterChannel =
                    await
                        UpdateChannel.TryGetMasterChannel(
                            new Uri(Session.ActiveProject.UpdateDirectoryUri, "masterchannel.json"),
                            Session.ProxyManager.Data.Proxy);
            }
            catch (Exception ex) // There has been an error that is not 404.
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while downloading the master channel.", ex,
                        PopupButtons.Ok);

                releaseControls();
                return;
            }

            if (masterChannel == null) // Master channel does not exist.
            {
                try
                {
                    activeTaskLabel.Text = "Synchronizing master channel with the server...";
                    await Session.UpdateFactory.SynchronizeMasterChannel();
                    masterChannel = Session.ActiveProject.MasterChannel;
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while synchronizing the master channel with the server.", ex,
                        PopupButtons.Ok);

                    releaseControls();
                    return;
                }
            }

            // Master channel exists, so we check the update channels.
            var masterChannelArray = masterChannel.ToArray();
            try
            {
                activeTaskLabel.Text = "Checking update channels...";
                if (!await Session.UpdateFactory.CheckUpdateChannels(masterChannelArray))
                {
                    // There are update channels missing and we need to upload them.
                    activeTaskLabel.Text = "Synchronizing update channels with the server...";
                    await Session.UpdateFactory.SynchronizeUpdateChannels(masterChannelArray);
                }
                tickPictureBox.Visible = true;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while checking/synchronizing the update channels.", ex,
                    PopupButtons.Ok);
            }

            releaseControls();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (packageListView.SelectedItems.Count == 0)
                return;

            var answer = Popup.ShowPopup(this, SystemIcons.Question,
                "Delete the selected update packages?",
                $"Are you sure that you want to delete {(packageListView.SelectedItems.Count > 1 ? "these packages" : "this package")}?",
                PopupButtons.YesNo);
            if (answer != TaskDialogResult.Yes)
                return;

            // TODO: Implement
            Initialize();
        }

        private void packageFromTemplateToolStripButton_Click(object sender, EventArgs e)
        {
        }

        private void overviewContentSwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (overviewContentSwitch.Checked)
                tabControl.SelectedTab = overviewTabPage;
            else if (packagesContentSwitch.Checked)
                tabControl.SelectedTab = packageTabPage;
        }
    }
}