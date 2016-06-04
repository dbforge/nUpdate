// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.Administration.Application;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    internal partial class ProjectDialog : BaseDialog
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Dictionary<UpdateVersion, StatisticsChart> _dataGridViewRowTags =
            new Dictionary<UpdateVersion, StatisticsChart>();

        public ProjectDialog()
        {
            InitializeComponent();
            LoadingPanel = loadingPanel;
        }

        private async void ProjectDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Session.ActiveProject.Name, Program.VersionString);
            string[] programmingLanguages = {"VB.NET", "C#"};
            programmingLanguageComboBox.DataSource = programmingLanguages;
            programmingLanguageComboBox.SelectedIndex = 0;
            cancelToolTip.SetToolTip(cancelLabel, "Click here to cancel the package upload.");
            updateStatisticsButtonToolTip.SetToolTip(updateStatisticsButton, "Update the statistics.");
            assemblyPathTextBox.ButtonClicked += BrowseAssemblyButtonClicked;
            assemblyPathTextBox.Initialize();

            packagesList.DoubleBuffer();
            projectDataPartsTabControl.DoubleBuffer();
            packagesList.MakeCollapsable();
            statisticsDataGridView.RowHeadersVisible = false;

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

            await CheckPackageDataFile();
        }

        private void ProjectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AllowCancel)
                e.Cancel = true;
        }

        private void Initialize()
        {
            Invoke(new Action(() =>
            {
                nameTextBox.Text = Session.ActiveProject.Name;
                updateUriTextBox.Text = Session.ActiveProject.UpdateDirectoryUri.ToString();
                amountLabel.Text =
                    Session.ActiveProject.Packages?.Count.ToString(CultureInfo.InvariantCulture) ?? "0";

                // TODO: Else in designer
                if (!string.IsNullOrEmpty(Session.ActiveProject.AssemblyVersionPath))
                {
                    loadFromAssemblyRadioButton.Checked = true;
                    assemblyPathTextBox.Text = Session.ActiveProject.AssemblyVersionPath;
                }

                if (Session.ActiveProject.Packages == null)
                {
                    Session.ActiveProject.Packages = new List<UpdatePackage>();
                    Session.ActiveProject.Save();
                }

                newestPackageLabel.Text = Session.ActiveProject.Packages.Count > 0
                    ? UpdateVersion.GetHighestUpdateVersion(
                        Session.ActiveProject.Packages?.Select(item => new UpdateVersion(item.LiteralVersion)))
                        .Description
                    : "-";

                projectIdTextBox.Text = Session.ActiveProject.Guid.ToString();
                publicKeyTextBox.Text = Session.ActiveProject.PublicKey;

                if (packagesList.Items.Count > 0)
                    packagesList.Items.Clear();
            }));

            foreach (var package in Session.ActiveProject.Packages)
            {
                var packageVersion = new UpdateVersion(package.LiteralVersion);
                var packageListViewItem = new ListViewItem(packageVersion.Description);
                var packageFileInfo =
                    new FileInfo(Path.Combine(FilePathProvider.Path, "Projects", Session.ActiveProject.Guid.ToString(),
                        packageVersion.ToString(),
                        $"{Session.ActiveProject.Guid}.zip"));
                if (packageFileInfo.Exists)
                {
                    packageListViewItem.SubItems.Add(packageFileInfo.CreationTime.ToString(CultureInfo.InvariantCulture));
                    packageListViewItem.SubItems.Add(SizeHelper.ToAdequateSizeString(packageFileInfo.Length));
                }
                else
                {
                    Invoke(new Action(() =>
                        Popup.ShowPopup(this, SystemIcons.Information, "Missing package file.",
                            $"The update package of version \"{packageVersion.Description}\" could not be found on your computer. Specific actions and information won't be available.",
                            PopupButtons.Ok)));

                    for (uint i = 0; i < 2; ++i) // Add two "-"-signs as we don't have any information
                        packageListViewItem.SubItems.Add("-");
                }

                packageListViewItem.SubItems.Add(package.Description);
                packageListViewItem.Group = package.IsReleased ? packagesList.Groups[0] : packagesList.Groups[1];
                packageListViewItem.Tag = packageVersion;
                Invoke(new Action(() => packagesList.Items.Add(packageListViewItem)));
            }
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
                checkUpdateConfigurationLinkLabel.Enabled = enabled;
                addButton.Enabled = enabled;
                deleteButton.Enabled = enabled;
                // TODO: Designer
                noStatisticsLabel.Text = "No network connection available.";

                // Prevent that the user is inside this TabPage when we want to make it invisible
                if (projectDataPartsTabControl.SelectedTab == statisticsTabPage)
                    projectDataPartsTabControl.SelectTab(overviewTabPage);

                statisticsTabPage.Visible = false;
                //foreach (
                //    var c in
                //        from Control c in statisticsTabPage.Controls where c.GetType() != typeof(Panel) select c)
                //{
                //    c.Visible = enabled;
                //}
            }));
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            var matchingItem = packagesList.FindItemWithText(searchTextBox.Text, true, 0);
            if (matchingItem != null)
                packagesList.Items[matchingItem.Index].Selected = true;
            else
                packagesList.SelectedItems.Clear();

            searchTextBox.Clear();
            e.SuppressKeyPress = true;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var packageAddDialog = new PackageAddDialog();
            if (packageAddDialog.ShowDialog() != DialogResult.OK)
                return;

            // Re-initialize our data
            Initialize();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (packagesList.SelectedItems.Count == 0)
                return;

            var packageVersion = new UpdateVersion((string) packagesList.SelectedItems[0].Tag);
            UpdatePackage correspondingPackage;

            try
            {
                correspondingPackage =
                    Session.ActiveProject.Packages.First(
                        item => Equals(new UpdateVersion(item.LiteralVersion), packageVersion));
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while selecting the corresponding package.", ex,
                    PopupButtons.Ok);
                return;
            }

            var packageEditDialog = new PackageEditDialog
            {
                PackageVersion = packageVersion,
                IsReleased = correspondingPackage.IsReleased,
                PackageData = Session.ActiveProject.Packages
            };

            if (packageEditDialog.ShowDialog() != DialogResult.OK)
                return;

            // Re-initialize our data
            Initialize();
        }

        private void copySourceButton_Click(object sender, EventArgs e)
        {
            var vbSource =
                $"Dim manager As New UpdateManager(New Uri(\"{new Uri(new Uri(updateUriTextBox.Text), "updates.json")}\"), \"{publicKeyTextBox.Text}\", New CultureInfo(\"en\"))";
            var cSharpSource =
                $"UpdateManager manager = new UpdateManager(new Uri(\"{new Uri(new Uri(updateUriTextBox.Text), "updates.json")}\"), \"{publicKeyTextBox.Text}\", new CultureInfo(\"en\"));";

            try
            {
                switch (programmingLanguageComboBox.SelectedIndex)
                {
                    case 0:
                        Clipboard.SetText(vbSource);
                        break;
                    case 1:
                        Clipboard.SetText(cSharpSource);
                        break;
                }
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while copying the source-code.", ex, PopupButtons.Ok);
            }
        }

        private void historyButton_Click(object sender, EventArgs e)
        {
            new HistoryDialog().ShowDialog();
        }

        private void packagesList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            editButton.Enabled = false;
            uploadButton.Enabled = false;
            deleteButton.Enabled = false;

            editToolStripMenuItem.Enabled = false;
            uploadToolStripMenuItem.Enabled = false;
            deleteToolStripMenuItem.Enabled = false;

            if (packagesList.SelectedItems.Count == 1)
            {
                editButton.Enabled = true;
                editToolStripMenuItem.Enabled = true;
                deleteButton.Enabled = true;
                deleteToolStripMenuItem.Enabled = true;

                // If any of the selected items is already released, exit the void.
                if (packagesList.SelectedItems.Cast<ListViewItem>().Any(item => item.Group != packagesList.Groups[1]))
                    return;
                uploadButton.Enabled = true;
                uploadToolStripMenuItem.Enabled = true;
            }
            else if (packagesList.SelectedItems.Count > 1)
            {
                deleteButton.Enabled = true;
                deleteToolStripMenuItem.Enabled = true;
            }
        }

        private void BrowseAssemblyButtonClicked(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Multiselect = false;
                fileDialog.SupportMultiDottedExtensions = false;
                fileDialog.Filter = "Executable files (*.exe)|*.exe|Dynamic link libraries (*.dll)|*.dll";

                if (fileDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    var projectAssembly = Assembly.LoadFile(fileDialog.FileName);
                    FileVersionInfo.GetVersionInfo(projectAssembly.Location);
                }
                catch
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid assembly found.",
                        "The version of the assembly of the selected file could not be read.",
                        PopupButtons.Ok);
                    enterVersionManuallyRadioButton.Checked = true;
                    return;
                }

                assemblyPathTextBox.Text = fileDialog.FileName;
                Session.ActiveProject.AssemblyVersionPath = assemblyPathTextBox.Text;

                try
                {
                    Session.ActiveProject.Save();
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving new project data.", ex,
                                    PopupButtons.Ok)));
                }
            }
        }

        private void updateStatisticsButton_Click(object sender, EventArgs e)
        {
            //InitializeStatisticsData();
        }

        private void loadFromAssemblyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!loadFromAssemblyRadioButton.Checked)
                return;

            assemblyPathTextBox.Enabled = true;
        }

        private void enterVersionManuallyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!enterVersionManuallyRadioButton.Checked)
                return;

            assemblyPathTextBox.Enabled = false;
            Session.ActiveProject.AssemblyVersionPath = null;
            Session.ActiveProject.Save();

            assemblyPathTextBox.Clear();
            Initialize();
        }

        private void statisticsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            updateStatisticsButton.Enabled = false;
            chartPanel.Visible = true;
            var chart =
                _dataGridViewRowTags.First(
                    item =>
                        Equals(item.Key,
                            UpdateVersion.FromDescription(
                                (string) statisticsDataGridView.Rows[e.RowIndex].Cells[0].Value)))
                    .Value;
            chart.TotalDownloadCount = Convert.ToInt32(statisticsDataGridView.Rows[e.RowIndex].Cells[1].Value);
            chart.StatisticsChartClosed += CurrentChartClosed;
            chart.Dock = DockStyle.Fill;
            chartPanel.Controls.Add(chart);
            statisticsDataGridView.Visible = false;
        }

        private void CurrentChartClosed(object sender, EventArgs e)
        {
            chartPanel.Controls.Remove((StatisticsChart) sender);
            chartPanel.Visible = false;
            updateStatisticsButton.Enabled = true;
            statisticsDataGridView.Visible = true;
        }

        private void readOnlyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control || (e.KeyCode != Keys.A))
                return;
            ((TextBox) sender)?.SelectAll();
            e.Handled = true;
        }

        private void overviewHeader_Click(object sender, EventArgs e)
        {
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            if (packagesList.SelectedItems.Count == 0)
                return;

            AdjustControlsForAction(async () =>
            {
                for (int i = 0; i < packagesList.SelectedItems.Count; ++i)
                {
                    var currentVersion = new UpdateVersion((string) packagesList.SelectedItems[i].Tag);
                    loadingLabel.Text = $"Uploading package {currentVersion.Description}";
                    await UploadPackage(currentVersion);
                }
            }, true);
        }

        private async Task UploadPackage(IUpdateVersion packageVersion)
        {
            var updatePackage =
                Session.ActiveProject.Packages.First(
                    item => new UpdateVersion(item.LiteralVersion).Equals(packageVersion));
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

        private async void checkUpdateConfigurationLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            await CheckPackageDataFile();
        }

        private async Task CheckPackageDataFile()
        {
            tickPictureBox.Visible = false;
            checkingUrlPictureBox.Visible = true;
            checkUpdateConfigurationLinkLabel.Enabled = false;

            HttpStatusCode statusCode = HttpStatusCode.OK;
            using (var client = new WebClientWrapper(5000))
            {
                ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };
                try
                {
                    var stream =
                        await
                            client.OpenReadTaskAsync(new Uri(Session.ActiveProject.UpdateDirectoryUri, "updates.json"));
                    if (stream != null)
                    {
                        tickPictureBox.Visible = true;
                        checkingUrlPictureBox.Visible = false;
                        checkUpdateConfigurationLinkLabel.Enabled = true;
                        return;
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                        statusCode = ((HttpWebResponse) ex.Response).StatusCode;
                }
            }

            DialogResult hintPopupResult = DialogResult.None;
            Invoke(new Action(() =>
            {
                if (statusCode == HttpStatusCode.NotFound)
                    hintPopupResult = Popup.ShowPopup(this, SystemIcons.Information, "Package data file not found.",
                        "No package data file associated with this project could be found on the server. Would you like to upload the default package data file?",
                        PopupButtons.YesNo);
                else
                {
                    hintPopupResult = Popup.ShowPopup(this, SystemIcons.Information,
                        "Package data file could not be checked.",
                        $"The package data file associated with this project could not be checked as the server returned code {statusCode}.",
                        PopupButtons.Ok);
                }
            }));

            if (hintPopupResult == DialogResult.No || hintPopupResult == DialogResult.OK)
                return;
            
            AdjustControlsForAction(async () =>
            {
                try
                {
                    await Session.UpdateFactory.PushDefaultPackageData(_cancellationTokenSource.Token, null);
                    tickPictureBox.Visible = true;
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the default package data file.", ex,
                        PopupButtons.Ok);
                }
                finally
                {
                    checkingUrlPictureBox.Visible = false;
                    checkUpdateConfigurationLinkLabel.Enabled = true;
                }
            }, false);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (packagesList.SelectedItems.Count == 0)
                return;

            var answer = Popup.ShowPopup(this, SystemIcons.Question,
                "Delete the selected update packages?",
                $"Are you sure that you want to delete {(packagesList.SelectedItems.Count > 1 ? "these packages" : "this package")}?",
                PopupButtons.YesNo);
            if (answer != DialogResult.Yes)
                return;

            AdjustControlsForAction(async () =>
            {
                for (int i = 0; i < packagesList.SelectedItems.Count; ++i)
                {
                    var currentVersion = new UpdateVersion((string) packagesList.SelectedItems[i].Tag);
                    try
                    {
                        var progress = new Progress<TransferProgressEventArgs>();
                        progress.ProgressChanged += (s, args) =>
                        {
                            loadingLabel.Text =
                                $"Deleting package {currentVersion.Description}... {$"{Math.Round(args.Percentage, 1)}% | {args.BytesPerSecond/1024}KB/s"}";
                        };
                        await
                            Session.UpdateFactory.RemoveUpdate(currentVersion, _cancellationTokenSource.Token, progress);
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, $"Removing package {currentVersion} failed.", ex,
                            PopupButtons.Ok);
                    }
                }
            }, true);
            Initialize();
        }

        private void packageFromTemplateToolStripButton_Click(object sender, EventArgs e)
        {
        }
    }
}