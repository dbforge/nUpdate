// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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

            await CheckMasterChannelFile();
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
                DialogResult popupResult = DialogResult.None;
                Invoke(
                    new Action(
                        () => popupResult = Popup.ShowPopup(this, SystemIcons.Error,
                            "Upload failed.",
                            "The package file of the selected version does not exist on your computer and cannot be uploaded. Should its reference be removed from the project?",
                            PopupButtons.YesNo)));

                if (popupResult == DialogResult.Yes)
                {
                    Session.ActiveProject.Packages.Remove(updatePackage);
                    Session.ActiveProject.Save();
                }
            }

            var progress = new Progress<TransferProgressEventArgs>();
            progress.ProgressChanged += (sender, args) =>
            {
                loadingLabel.Text =
                    $"Uploading... {$"{Math.Round(args.Percentage, 1)}% | {args.BytesPerSecond/1024}KB/s"}";
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
            await CheckMasterChannelFile();
        }

        private async Task CheckMasterChannelFile()
        {
            checkMasterChannelLinkLabel.Enabled = false;
            tickPictureBox.Visible = false;
            activeTaskLabel.Text = "Checking MasterChannel file...";

            var statusCode = HttpStatusCode.OK;
            using (var client = new WebClientEx(5000))
            {
                ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };
                try
                {
                    using (var stream =
                        await
                            client.OpenReadTaskAsync(new Uri(Session.ActiveProject.UpdateDirectoryUri,
                                "masterchannel.json")))
                    {
                        if (stream != null)
                        {
                            tickPictureBox.Visible = true;
                            checkMasterChannelLinkLabel.Enabled = true;
                            activeTaskLabel.Text = "No active tasks.";
                            return;
                        }
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                        statusCode = ((HttpWebResponse) ex.Response).StatusCode;
                }
            }

            if (statusCode != HttpStatusCode.NotFound)
            {
                Popup.ShowPopup(this, SystemIcons.Error,
                    "MasterChannel file could not be checked.",
                    $"Checking the MasterChannel file failed because the server returned code {statusCode}.",
                    PopupButtons.Ok);
                activeTaskLabel.Text = "No active tasks.";
                return;
            }

            activeTaskLabel.Text = "Uploading default channel data...";

            bool overrideChannelFiles = false;
            // TODO: Fix
            if (await Session.TransferManager.Exists("channels") &&
                (await Session.TransferManager.List("channels", false)).Any())
            {
                overrideChannelFiles = Popup.ShowPopup(this, SystemIcons.Question, "Override old update channels?",
                    "nUpdate Administration found existing update channels on your server. Overriding these files will result in loss of these channels. Should these files be overriden? (If you only lost/deleted the MasterChannel file and want to keep the current channels, press \"No\". nUpdate Administration will then help you to recover the MasterChannel file correctly.)",
                    PopupButtons.YesNo) == DialogResult.Yes;
            }

            AdjustControlsForAction(async () =>
            {
                try
                {
                    await
                        Session.UpdateFactory.PushDefaultMasterChannel(overrideChannelFiles,
                            _cancellationTokenSource.Token, null);
                    tickPictureBox.Visible = true;
                    activeTaskLabel.Text = "No active tasks.";
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the default package data file.", ex,
                        PopupButtons.Ok);
                }
                finally
                {
                    checkMasterChannelLinkLabel.Enabled = true;
                    activeTaskLabel.Text = "No active tasks.";
                }
            }, false);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (packageListView.SelectedItems.Count == 0)
                return;

            var answer = Popup.ShowPopup(this, SystemIcons.Question,
                "Delete the selected update packages?",
                $"Are you sure that you want to delete {(packageListView.SelectedItems.Count > 1 ? "these packages" : "this package")}?",
                PopupButtons.YesNo);
            if (answer != DialogResult.Yes)
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