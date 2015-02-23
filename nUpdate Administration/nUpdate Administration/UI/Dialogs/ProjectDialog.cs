// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Ftp;
using nUpdate.Administration.Core.Ftp.EventArgs;
using nUpdate.Administration.Core.History;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectDialog : BaseDialog, IAsyncSupportable, IResettable
    {
        private const float KB = 1024;
        private const float MB = 1048576;
        private const float GB = 1073741824;
        private bool _allowCancel = true;
        private Uri _configurationFileUrl;
        private IEnumerable<UpdateConfiguration> _backupConfiguration;
        private IEnumerable<UpdateConfiguration> _editingUpdateConfiguration;
        private MySqlConnection _queryConnection;
        private readonly FtpManager _ftp = new FtpManager();
        private readonly ManualResetEvent _loadConfigurationResetEvent = new ManualResetEvent(false);
        private readonly Log _updateLog = new Log();
        private readonly Dictionary<UpdateVersion, StatisticsChart> _dataGridViewRowTags = new Dictionary<UpdateVersion, StatisticsChart>();
        private bool _isSetByUser;

        /* This variables relate to the upload */
        private bool _commandsExecuted;
        private bool _packageExisting;
        private bool _configurationUploaded;
        private bool _uploadCancelled;

        /// <summary>
        ///     The FTP-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public SecureString FtpPassword { get; set; }

        /// <summary>
        ///     The proxy-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public SecureString ProxyPassword { get; set; }

        /// <summary>
        ///     The MySQL-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public SecureString SqlPassword { get; set; }

        public ProjectDialog()
        {
            InitializeComponent();
        }

        /* The Reset-method is only used for the upload */
        public void Reset()
        {
            UpdateVersion packageVersion = null;
            Invoke(
                new Action(() => packageVersion = (UpdateVersion)packagesList.SelectedItems[0].Tag));

            if (_commandsExecuted)
            {
                Invoke(new Action(() => loadingLabel.Text = "Connecting to MySQL-server..."));

                var connectionString = String.Format("SERVER={0};" +
                                                     "DATABASE={1};" +
                                                     "UID={2};" +
                                                     "PASSWORD={3};",
                    Project.SqlWebUrl, Project.SqlDatabaseName,
                    Project.SqlUsername,
                    SqlPassword.ConvertToUnsecureString());

                var deleteConnection = new MySqlConnection(connectionString);
                try
                {
                    deleteConnection.Open();
                }
                catch (MySqlException ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "An MySQL-exception occured when trying to undo the SQL-insertions...",
                                    ex, PopupButtons.Ok)));
                    deleteConnection.Close();
                    return;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while connecting to the database when trying to undo the SQL-insertions...",
                                    ex, PopupButtons.Ok)));
                    deleteConnection.Close();
                    SetUiState(true);
                    return;
                }

                var command = deleteConnection.CreateCommand();
                command.CommandText =
                    String.Format("DELETE FROM `Version` WHERE `Version` = \"{0}\"", packageVersion);

                try
                {
                    command.ExecuteNonQuery();
                    _commandsExecuted = false;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while executing the commands when trying to undo the SQL-insertions...",
                                    ex, PopupButtons.Ok)));
                    deleteConnection.Close();
                }
            }

            if (_packageExisting)
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Undoing package upload..."));
                try
                {
                    _ftp.DeleteDirectory(String.Format("{0}/{1}", _ftp.Directory, packageVersion));
                    _packageExisting = false;
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("No such file or directory"))
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error,
                                        "Error while undoing the package upload.",
                                        ex,
                                        PopupButtons.Ok)));
                        SetUiState(true);
                        return;
                    }
                    _packageExisting = false;
                }
            }

            if (_configurationUploaded)
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Uploading old configuration..."));

                string updateConfigurationFilePath = Path.Combine(Program.Path, "updates.json");
                try
                {
                    File.WriteAllText(updateConfigurationFilePath, Serializer.Serialize(_backupConfiguration));
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the old configuration.", ex,
                                    PopupButtons.Ok)));
                    SetUiState(true);
                    return;
                }

                try
                {
                    _ftp.UploadFile(updateConfigurationFilePath);
                    _configurationUploaded = true;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the configuration.", ex,
                                    PopupButtons.Ok)));
                    SetUiState(true);
                    return;
                }

                try
                {
                    File.WriteAllText(updateConfigurationFilePath, String.Empty);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the local configuration.", ex,
                                    PopupButtons.Ok)));
                }
            }

            SetUiState(true);
        }

        /// <summary>
        ///     Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        public void SetUiState(bool enabled)
        {
            Invoke(new Action(() =>
            {
                foreach (var c in from Control c in Controls where c.Visible select c)
                {
                    c.Enabled = enabled;
                }

                if (!enabled)
                {
                    _allowCancel = false;
                    loadingPanel.Visible = true;
                    loadingPanel.Location = new Point(179, 135);
                    loadingPanel.BringToFront();
                }
                else
                {
                    _allowCancel = true;
                    loadingPanel.Visible = false;
                }
            }));
        }

        ///// <summary>
        /////     Sets the language.
        ///// </summary>
        //public void SetLanguage()
        //{
        //    string languageFilePath = Path.Combine(Program.LanguagesDirectory,
        //        String.Format("{0}.json", Settings.Default.Language.Name));
        //    LocalizationProperties ls;
        //    if (File.Exists(languageFilePath))
        //        ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
        //    else
        //    {
        //        string resourceName = "nUpdate.Administration.Core.Localization.en.json";
        //        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        //        {
        //            ls = Serializer.Deserialize<LocalizationProperties>(stream);
        //        }
        //    }

        //    Text = String.Format("{0} - {1}", Project.Name, ls.ProductTitle);
        //    overviewHeader.Text = ls.ProjectDialogOverviewText;
        //    overviewTabPage.Text = ls.ProjectDialogOverviewTabText;
        //    packagesTabPage.Text = ls.ProjectDialogPackagesTabText;
        //    nameLabel.Text = ls.ProjectDialogNameLabelText;
        //    updateUrlLabel.Text = ls.ProjectDialogUpdateUrlLabelText;
        //    ftpHostLabel.Text = ls.ProjectDialogFtpHostLabelText;
        //    ftpDirectoryLabel.Text = ls.ProjectDialogFtpDirectoryLabelText;
        //    releasedPackagesAmountLabel.Text = ls.ProjectDialogPackagesAmountLabelText;
        //    newestPackageReleasedLabel.Text = ls.ProjectDialogNewestPackageLabelText;
        //    infoFileloadingLabel.Text = ls.ProjectDialogInfoFileloadingLabelText;
        //    checkUpdateConfigurationLinkLabel.Text = ls.ProjectDialogCheckInfoFileStatusLinkLabelText;
        //    projectDataHeader.Text = ls.ProjectDialogProjectDataText;
        //    publicKeyLabel.Text = ls.ProjectDialogPublicKeyLabelText;
        //    projectIdLabel.Text = ls.ProjectDialogProjectIdLabelText;
        //    stepTwoLabel.Text = String.Format(stepTwoLabel.Text, copySourceButton.Text);

        //    addButton.Text = ls.ProjectDialogAddButtonText;
        //    editButton.Text = ls.ProjectDialogEditButtonText;
        //    deleteButton.Text = ls.ProjectDialogDeleteButtonText;
        //    uploadButton.Text = ls.ProjectDialogUploadButtonText;
        //    historyButton.Text = ls.ProjectDialogHistoryButtonText;

        //    packagesList.Columns[0].Text = ls.ProjectDialogVersionText;
        //    packagesList.Columns[1].Text = ls.ProjectDialogReleasedText;
        //    packagesList.Columns[2].Text = ls.ProjectDialogSizeText;
        //    packagesList.Columns[3].Text = ls.ProjectDialogDescriptionText;

        //    searchTextBox.Cue = ls.ProjectDialogSearchText;
        //}

        /// <summary>
        ///     Initializes the dialog with the data of the project.
        /// </summary>
        /// <returns>Returns whether the operation was successful or not.</returns>
        private bool InitializeProjectData()
        {
            try
            {
                Invoke(new Action(() =>
                {
                    nameTextBox.Text = Project.Name;
                    updateUrlTextBox.Text = Project.UpdateUrl;
                    ftpHostTextBox.Text = Project.FtpHost;
                    ftpDirectoryTextBox.Text = Project.FtpDirectory;
                    amountLabel.Text = Project.ReleasedPackages.ToString(CultureInfo.InvariantCulture);
                }));
                
                if (!String.IsNullOrEmpty(Project.AssemblyVersionPath))
                {
                    Invoke(new Action(() =>
                    {
                        loadFromAssemblyRadioButton.Checked = true;
                        assemblyPathTextBox.Text = Project.AssemblyVersionPath;
                    }));
                }
                else
                {
                    Invoke(new Action(() => enterVersionManuallyRadioButton.Checked = true));
                }

                Invoke(new Action(() =>
                {
                    newestPackageLabel.Text = Project.NewestPackage ?? "-";

                    projectIdTextBox.Text = Project.Guid;
                    publicKeyTextBox.Text = Project.PublicKey;
                }));
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while loading project-data.", ex,
                                PopupButtons.Ok)));
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Adds the package items to the dialog's package listview.
        /// </summary>
        private void InitializePackageItems()
        {
            Invoke(new Action(() =>
            {
                if (packagesList.Items.Count > 0)
                    packagesList.Items.Clear();
            }));

            if (Project.Packages == null || Project.Packages.Count == 0)
                return;

            foreach (var package in Project.Packages)
            {
                try
                {
                    var packageListViewItem = new ListViewItem(package.Version.FullText);
                    var packageDirectoryInfo = new DirectoryInfo(package.LocalPackagePath);
                    var packageFileInfo = new FileInfo(package.LocalPackagePath);

                    packageListViewItem.SubItems.Add(packageDirectoryInfo.CreationTime.ToString());
                    var sizeInBytes = packageFileInfo.Length;
                    string sizeText = null;

                    if (sizeInBytes >= 107374182.4) // 0,1 GB
                        sizeText = String.Format("{0} GB", (float) Math.Round(sizeInBytes/GB, 1));
                    else if (sizeInBytes >= 104857.6) // 0,1 MB
                        sizeText = String.Format("{0} MB", (float) Math.Round(sizeInBytes/MB, 1));
                    else if (sizeInBytes >= 102.4) // 0,1 KB
                        sizeText = String.Format("{0} KB", (float) Math.Round(sizeInBytes/KB, 1));
                    else if (sizeInBytes >= 1) // 1 B
                        sizeText = String.Format("{0} B", sizeInBytes);

                    packageListViewItem.SubItems.Add(sizeText);
                    packageListViewItem.SubItems.Add(package.Description);
                    packageListViewItem.Group = package.IsReleased ? packagesList.Groups[0] : packagesList.Groups[1];
                    packageListViewItem.Tag = package.Version;
                    Invoke(
                        new Action(
                            () =>
                                packagesList.Items.Add(packageListViewItem)));
                }
                catch (Exception ex)
                {
                    var packagePlaceholder = package;
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    String.Format("Error while loading the package \"{0}\".", packagePlaceholder.Version),
                                    ex,
                                    PopupButtons.Ok)));
                }
            }
        }

        private void ProjectDialog_Shown(object sender, EventArgs e)
        {
            if (!InitializeProjectData())
            {
                Close();
                return;
            }

            try
            {
                _ftp.Host = Project.FtpHost;
                _ftp.Port = Project.FtpPort;
                _ftp.Username = Project.FtpUsername;
                _ftp.Password = FtpPassword;
                _ftp.Protocol = (FtpSecurityProtocol)Project.FtpProtocol;
                _ftp.UsePassiveMode = Project.FtpUsePassiveMode;
                _ftp.Directory = Project.FtpDirectory;
                _ftp.Proxy = Project.Proxy;
                _ftp.ProgressChanged += ProgressChanged;
                _ftp.CancellationFinished += CancellationFinished;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.Ok);
                Close();
                return;
            }

            InitializePackageItems();
            //SetLanguage();

            _updateLog.Project = Project;
            _configurationFileUrl = UriConnector.ConnectUri(Project.UpdateUrl, "updates.json");

            Text = String.Format(Text, Project.Name);
            programmingLanguageComboBox.DataSource = Enum.GetValues(typeof(ProgrammingLanguage));
            programmingLanguageComboBox.SelectedIndex = 0;
            cancelToolTip.SetToolTip(cancelLabel, "Click here to cancel the package upload.");
            updateStatisticsButtonToolTip.SetToolTip(updateStatisticsButton, "Update the statistics.");

            var values = Enum.GetValues(typeof(DevelopmentalStage));
            Array.Reverse(values);
            developmentalStageComboBox.DataSource = values;
            developmentalStageComboBox.SelectedIndex = 2;

            packagesList.DoubleBuffer();
            projectDataPartsTabControl.DoubleBuffer();
            packagesList.MakeCollapsable();
            statisticsDataGridView.RowHeadersVisible = false;

            if (!ConnectionChecker.IsConnectionAvailable())
            {
                checkUpdateConfigurationLinkLabel.Enabled = false;
                addButton.Enabled = false;
                deleteButton.Enabled = false;
                noStatisticsLabel.Text = "Statistics couldn't be loaded.\nNo network connection available.";

                foreach (
                    var c in
                        from Control c in statisticsTabPage.Controls where c.GetType() != typeof(Panel) select c)
                {
                    c.Visible = false;
                }

                Popup.ShowPopup(this, SystemIcons.Error, "No network connection available.",
                    "No active network connection could be found. Most functions require a network connection in order to connect to services on the internet and have been deactivated for now. Just open this dialog again if you again gained access to the internet.",
                    PopupButtons.Ok);
                _isSetByUser = true;
                return;
            }

            BeginUpdateConfigurationCheck();
            if (Project.UseStatistics)
            {
                ThreadPool.QueueUserWorkItem(arg => InitializeStatisticsData());
            }
            else
            {
                foreach (
                    var c in
                        from Control c in statisticsTabPage.Controls where c.GetType() != typeof(Panel) select c)
                {
                    c.Visible = false;
                }

                noStatisticsPanel.Visible = true;
                _isSetByUser = true;
            } 
        }

        private void ProjectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        private void InitializeStatisticsData()
        {
            if (_dataGridViewRowTags.Count > 0)
                _dataGridViewRowTags.Clear();

            Invoke(
                    new Action(
                        () =>
                        {
                            statisticsDataGridView.Visible = false;
                            gatheringStatisticsPictureBox.Visible = true;
                            statisticsStatusLabel.Visible = true;
                            statisticsStatusLabel.Text = "Gathering statistics...";
                        }));

            var connectionString = String.Format("SERVER={0};" +
                                                 "DATABASE={1};" +
                                                 "UID={2};" +
                                                 "PASSWORD={3};",
                Project.SqlWebUrl, Project.SqlDatabaseName,
                Project.SqlUsername,
                SqlPassword.ConvertToUnsecureString());

            _queryConnection = new MySqlConnection(connectionString);
            try
            {
                _queryConnection.Open();
            }
            catch (MySqlException ex)
            {
                Invoke(new Action(() =>
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
                        ex, PopupButtons.Ok);
                    statisticsStatusLabel.Visible = true;
                    statisticsStatusLabel.Text = "No downloads.";
                    gatheringStatisticsPictureBox.Visible = false;
                }));
                _queryConnection.Close();
                return;
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while connecting to the database.",
                        ex, PopupButtons.Ok); 
                    statisticsStatusLabel.Visible = true;
                    statisticsStatusLabel.Text = "No downloads.";
                    gatheringStatisticsPictureBox.Visible = false;
                }));
                _queryConnection.Close();
                return;
            }

            var dataSet = new DataSet();
            using (var dataAdapter =
                new MySqlDataAdapter(
                    String.Format(
                        "SELECT v.Version, COUNT(*) AS 'Downloads' FROM Download LEFT JOIN Version v ON (v.ID = Version_ID) WHERE `Application_ID` = {0} GROUP BY Version_ID;",
                        Project.ApplicationId),
                    _queryConnection))
            {
                try
                {
                    dataAdapter.Fill(dataSet);
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        row[0] = new UpdateVersion(row.ItemArray[0].ToString()).FullText;
                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        Popup.ShowPopup(this, SystemIcons.Error,
                            "Error while gathering the table entries of the database.", ex, PopupButtons.Ok);
                        statisticsStatusLabel.Visible = true;
                        statisticsStatusLabel.Text = "No downloads.";
                        gatheringStatisticsPictureBox.Visible = false;
                    }));
                    _queryConnection.Close();
                    return;
                }
            }

            IEnumerable<UpdateConfiguration> updateConfiguration;
            try
            {
                 updateConfiguration = UpdateConfiguration.Download(_configurationFileUrl, Project.Proxy) ?? Enumerable.Empty<UpdateConfiguration>();
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    Popup.ShowPopup(this, SystemIcons.Error,
                        "Error while downloading the configuration.", ex, PopupButtons.Ok);
                    statisticsStatusLabel.Visible = true;
                    statisticsStatusLabel.Text = "No downloads.";
                    gatheringStatisticsPictureBox.Visible = false;
                }));
                _queryConnection.Close();
                return;
            }

            string[] operatingSystemStrings = {"Windows Vista", "Windows 7", "Windows 8", "Windows 8.1", "Windows 10"};
            const string commandString = "SELECT ((SELECT COUNT(OperatingSystem) FROM Download WHERE `Version_ID` = {0} AND `OperatingSystem` = \"{1}\") / (SELECT COUNT(OperatingSystem) FROM Download WHERE `Version_ID` = {0})*100)";
            
            var updateConfigurationArray = updateConfiguration as UpdateConfiguration[] ?? updateConfiguration.ToArray();
            foreach (var configuration in updateConfigurationArray)
            {
                var version = configuration.LiteralVersion;
                var statisticsChart = new StatisticsChart {Version = new UpdateVersion(configuration.LiteralVersion)};
                foreach (var operatingSystemString in operatingSystemStrings)
                {
                    string adjustedCommandString = String.Format(commandString, updateConfigurationArray.First(item => item.LiteralVersion == version.ToString()).VersionId, operatingSystemString);
                    var command = _queryConnection.CreateCommand();
                    command.CommandText = adjustedCommandString;

                    MySqlDataReader reader = null;
                    try
                    {
                        reader = command.ExecuteReader();
                        if (!reader.Read())
                            continue;
                        var value = reader.GetValue(0);
                        if (Convert.IsDBNull(value))
                            continue;
                        var percentage = Convert.ToInt32(value, CultureInfo.InvariantCulture);

                        switch (operatingSystemString)
                        {
                            case "Windows Vista":
                                statisticsChart.WindowsVistaPercentage = percentage;
                                break;
                            case "Windows 7":
                                statisticsChart.WindowsSevenPercentage = percentage;
                                break;
                            case "Windows 8":
                                statisticsChart.WindowsEightPercentage = percentage;
                                break;
                            case "Windows 8.1":
                                statisticsChart.WindowsEightPointOnePercentage = percentage;
                                break;
                            case "Windows 10":
                                statisticsChart.WindowsTenPercentage = percentage;
                                break;
                        }
                    }
                    catch (MySqlException
                        ex)
                    {
                        Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.", ex, PopupButtons.Ok)));
                        _queryConnection.Close();
                        return;
                    }
                    catch (Exception
                        ex)
                    {
                        Invoke(new Action(() => Popup.ShowPopup(this, SystemIcons.Error, "Error while reading the SQL-data.", ex, PopupButtons.Ok)));
                        _queryConnection.Close();
                        return;
                    }
                    finally
                    {
                        if (reader != null)
                            reader.Close();
                    }
                }

                try
                {
                    _dataGridViewRowTags.Add(new UpdateVersion(version), statisticsChart);
                }
                catch (InvalidOperationException)
                {
                    // "continue"-statement would be unnecessary
                }
            }

            Invoke(new Action(() =>
            {
                statisticsDataGridView.DataSource = dataSet.Tables[0];
                statisticsDataGridView.Columns[0].Width = 278;
                statisticsDataGridView.Columns[1].Width = 278; 
                lastUpdatedLabel.Text = String.Format("Last updated: {0}", DateTime.Now);
                gatheringStatisticsPictureBox.Visible = false;
                statisticsDataGridView.Visible = true;

                if (statisticsDataGridView.Rows.Count == 0)
                {
                    statisticsStatusLabel.Visible = true;
                    statisticsStatusLabel.Text = "No downloads.";
                }
                else
                    statisticsStatusLabel.Visible = false;
            }));

            _queryConnection.Close();
            _isSetByUser = true;
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
            if (!ConnectionChecker.IsConnectionAvailable())
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No network connection available.",
                    "No active network connection was found. In order to add a package a network connection is required because the update configuration must be downloaded from the server.",
                    PopupButtons.Ok);
                return;
            }

            var packageAddDialog = new PackageAddDialog
            {
                FtpPassword = FtpPassword.Copy(),
                SqlPassword = SqlPassword.Copy(),
                ProxyPassword = ProxyPassword.Copy()
            };

            var existingUpdateVersions =
                (from ListViewItem lvi in packagesList.Items select new UpdateVersion(lvi.Tag.ToString())).ToList();
            packageAddDialog.ExistingVersions = existingUpdateVersions;
            packageAddDialog.Project = Project;

            if (packageAddDialog.ShowDialog() != DialogResult.OK)
                return;

            packagesList.Items.Clear();
            InitializePackageItems();
            InitializeProjectData();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            InitializeEditing();
        }

        private async void InitializeEditing()
        {
            if (packagesList.SelectedItems.Count == 0)
                return;

            var packageVersion = (UpdateVersion)packagesList.SelectedItems[0].Tag;
            UpdatePackage correspondingPackage;

            try
            {
                correspondingPackage = Project.Packages.First(item => item.Version == packageVersion);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while selecting the corresponding package.", ex,
                    PopupButtons.Ok);
                return;
            }

            if (!ConnectionChecker.IsConnectionAvailable() && correspondingPackage.IsReleased)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No network connection available.",
                    "No active network connection was found. In order to edit a package, which is already existing on the server, a network connection is required because the update configuration must be downloaded from the server.",
                    PopupButtons.Ok);
                return;
            }

            var packageEditDialog = new PackageEditDialog
            {
                Project = Project,
                PackageVersion = packageVersion,
                FtpPassword = FtpPassword.Copy(),
                SqlPassword = SqlPassword.Copy(),
                ProxyPassword = ProxyPassword.Copy()
            };

            if (correspondingPackage.IsReleased)
            {
                bool loadingSuccessful = await Task.Factory.StartNew(() => LoadConfiguration());
                if (loadingSuccessful)
                {
                    packageEditDialog.IsReleased = true;
                    packageEditDialog.UpdateConfiguration = _editingUpdateConfiguration == null
                        ? null
                        : _editingUpdateConfiguration.ToList();
                }
                else
                    return;
            }
            else
            {
                packageEditDialog.IsReleased = false;

                try
                {
                    packageEditDialog.UpdateConfiguration =
                        UpdateConfiguration.FromFile(Path.Combine(Directory.GetParent(
                            Project.Packages.First(item => item.Version == packageVersion).LocalPackagePath).FullName,
                            "updates.json")).ToList();
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the configuration.", ex,
                        PopupButtons.Ok);
                    return;
                }
            }

            packageEditDialog.ConfigurationFileUrl = _configurationFileUrl;
            if (packageEditDialog.ShowDialog() != DialogResult.OK)
                return;
            ThreadPool.QueueUserWorkItem(arg => InitializeStatisticsData());
            InitializePackageItems();
        }

        /// <summary>
        ///     Loads the update configuration for editing a package on the server.
        /// </summary>
        /// <returns>Returns 'true' if the operation was successful, otherwise 'false'.</returns>
        private bool LoadConfiguration()
        {
            SetUiState(false);
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = "Initializing..."));

            try
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Getting current configuration..."));
                _editingUpdateConfiguration = UpdateConfiguration.Download(_configurationFileUrl, Project.Proxy);
                SetUiState(true);
                return true;
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () => Popup.ShowPopup(this, SystemIcons.Error, "Error while downloading the configuration.", ex,
                            PopupButtons.Ok)));
                SetUiState(true);
                return false;
            }
        }

        private void copySourceButton_Click(object sender, EventArgs e)
        {
            var versionString = (developmentalStageComboBox.SelectedIndex == 2)
                ? new UpdateVersion((int) majorNumericUpDown.Value, (int) minorNumericUpDown.Value,
                    (int) buildNumericUpDown.Value,
                    (int) revisionNumericUpDown.Value).ToString()
                : new UpdateVersion((int) majorNumericUpDown.Value, (int) minorNumericUpDown.Value,
                    (int) buildNumericUpDown.Value,
                    (int) revisionNumericUpDown.Value,
                    (DevelopmentalStage)
                        Enum.Parse(typeof (DevelopmentalStage),
                            developmentalStageComboBox.GetItemText(developmentalStageComboBox.SelectedItem)),
                    (int) developmentBuildNumericUpDown.Value).ToString();

            var vbSource =
                String.Format(
                    "Dim manager As New UpdateManager(New Uri(\"{0}\"), \"{1}\", New UpdateVersion(\"{2}\"), New CultureInfo(\"en\"))",
                    UriConnector.ConnectUri(updateUrlTextBox.Text, "updates.json"), publicKeyTextBox.Text, versionString);
            var cSharpSource =
                String.Format(
                    "UpdateManager manager = new UpdateManager(new Uri(\"{0}\"), \"{1}\", new UpdateVersion(\"{2}\"), new CultureInfo(\"en\"));",
                    UriConnector.ConnectUri(updateUrlTextBox.Text, "updates.json"), publicKeyTextBox.Text, versionString);

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
            var historyDialog = new HistoryDialog {Project = Project};
            historyDialog.ShowDialog();
        }

        private void packagesList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (packagesList.SelectedItems.Count > 1)
            {
                editButton.Enabled = false;
                uploadButton.Enabled = false;
                deleteButton.Enabled = true;
            }
            else switch (packagesList.SelectedItems.Count)
            {
                case 0:
                    editButton.Enabled = false;
                    uploadButton.Enabled = false;
                    deleteButton.Enabled = false;
                    break;
                case 1:
                    editButton.Enabled = true;
                    deleteButton.Enabled = true;
                    if (packagesList.SelectedItems[0].Group == packagesList.Groups[1])
                        uploadButton.Enabled = true;
                    break;
            }
        }

        private void browseAssemblyButton_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Multiselect = false;
                fileDialog.SupportMultiDottedExtensions = false;
                fileDialog.Filter = "Executable files (*.exe)|*.exe|Dynamic link libraries (*.dll)|*.dll";

                if (fileDialog.ShowDialog() != DialogResult.OK) return;
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
                Project.AssemblyVersionPath = assemblyPathTextBox.Text;

                try
                {
                    UpdateProject.SaveProject(Project.Path, Project);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving new project info.", ex,
                                    PopupButtons.Ok)));
                    return;
                }

                InitializeProjectData();
            }
        }

        private void updateStatisticsButton_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(arg => InitializeStatisticsData());
        }

        private void loadFromAssemblyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!loadFromAssemblyRadioButton.Checked)
                return;

            assemblyPathTextBox.Enabled = true;
            browseAssemblyButton.Enabled = true;
        }

        private void enterVersionManuallyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!enterVersionManuallyRadioButton.Checked)
                return;
            assemblyPathTextBox.Enabled = false;
            browseAssemblyButton.Enabled = false;

            if (!_isSetByUser)
                return;
            Project.AssemblyVersionPath = null;

            try
            {
                UpdateProject.SaveProject(Project.Path, Project);
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while saving new project info.", ex,
                                PopupButtons.Ok)));
            }

            assemblyPathTextBox.Clear();
            InitializeProjectData();
        }

        private void developmentalStageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            developmentBuildNumericUpDown.Enabled = developmentalStageComboBox.SelectedIndex != 2;
        }

        #region "Upload"

        private void uploadButton_Click(object sender, EventArgs e)
        {
            if (packagesList.SelectedItems.Count == 0)
                return;

            var version = (UpdateVersion) packagesList.SelectedItems[0].Tag;
            ThreadPool.QueueUserWorkItem(arg => UploadPackage(version));
        }

        /// <summary>
        ///     Provides a new thread that uploads the package.
        /// </summary>
        private void UploadPackage(UpdateVersion packageVersion)
        {
            var updateConfigurationFilePath = Path.Combine(Program.Path, "Projects", Project.Name,
                   packageVersion.ToString(), "updates.json");

            SetUiState(false);
            Invoke(new Action(() => loadingLabel.Text = "Getting old configuration..."));
            try
            {
                var updateConfiguration = UpdateConfiguration.Download(_configurationFileUrl, Project.Proxy) ?? Enumerable.Empty<UpdateConfiguration>();
                _backupConfiguration = updateConfiguration.ToList();
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () => Popup.ShowPopup(this, SystemIcons.Error,
                            "Error while downloading the old configuration.", ex, PopupButtons.Ok)));
                SetUiState(true);
                return;
            }

            if (Project.UseStatistics)
            {
                int versionId;
                try
                {
                    versionId =
                        UpdateConfiguration.FromFile(updateConfigurationFilePath)
                            .First(item => new UpdateVersion(item.LiteralVersion) == packageVersion)
                            .VersionId;
                }
                catch (InvalidOperationException)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while preparing the SQL-connection.",
                                    "The update configuration of package \"{0}\" doesn't contain any entries for that version.", PopupButtons.Ok)));
                    Reset();
                    return;
                }

                Invoke(new Action(() => loadingLabel.Text = "Connecting to SQL-server..."));

                var connectionString = String.Format("SERVER={0};" +
                                                     "DATABASE={1};" +
                                                     "UID={2};" +
                                                     "PASSWORD={3};",
                    Project.SqlWebUrl, Project.SqlDatabaseName,
                    Project.SqlUsername,
                    SqlPassword.ConvertToUnsecureString());

                var insertConnection = new MySqlConnection(connectionString);
                try
                {
                    insertConnection.Open();
                }
                catch (MySqlException ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
                                    ex, PopupButtons.Ok)));
                    insertConnection.Close();
                    SetUiState(true);
                    return;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while connecting to the database.",
                                    ex, PopupButtons.Ok)));
                    insertConnection.Close();
                    SetUiState(true);
                    return;
                }

                Invoke(new Action(() => loadingLabel.Text = "Executing SQL-commands..."));

                var command = insertConnection.CreateCommand();
                command.CommandText =
                    String.Format("INSERT INTO `Version` (`ID`, `Version`, `Application_ID`) VALUES ({0}, \"{1}\", {2});",
                        versionId, packageVersion, Project.ApplicationId);

                try
                {
                    command.ExecuteNonQuery();
                    _commandsExecuted = true;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while executing the commands.",
                                    ex, PopupButtons.Ok)));
                    SetUiState(true);
                    return;
                }
                finally
                {
                    insertConnection.Close();
                    command.Dispose();
                }
            }

            Invoke(new Action(() =>
            {
                loadingLabel.Text = String.Format("Uploading... {0}", "0%");
                cancelLabel.Visible = true;
            }));

            try
            {
                var packagePath = Project.Packages.First(x => x.Version == packageVersion).LocalPackagePath;
                _ftp.UploadPackage(packagePath, packageVersion.ToString());
            }
            catch (InvalidOperationException)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the package.",
                                "The project's package data doesn't contain any entries for version \"{0}\".", PopupButtons.Ok)));
                Reset();
                return;
            }
            catch (Exception ex) // Errors that were thrown directly relate to the directory
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the package directory.",
                                ex, PopupButtons.Ok)));
                Reset();
                return;
            }

            if (_uploadCancelled)
                return;

            if (_ftp.PackageUploadException != null)
            {
                var ex = _ftp.PackageUploadException.InnerException ?? _ftp.PackageUploadException;
                Invoke(
                    new Action(
                        () => Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the package.", ex,
                            PopupButtons.Ok)));

                Reset();
                return;
            }

            Invoke(new Action(() =>
            {
                loadingLabel.Text = "Uploading new configuration...";
                cancelLabel.Visible = false;
            }));

            try
            {
                _ftp.UploadFile(updateConfigurationFilePath);
                _configurationUploaded = true;
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the configuration.", ex,
                                PopupButtons.Ok)));
                Reset();
                return;
            }

            _updateLog.Write(LogEntry.Upload, packageVersion.FullText);

            try
            {
                Project.Packages.First(x => x.Version == packageVersion).IsReleased = true;
                Project.NewestPackage = packageVersion.FullText;
                Project.ReleasedPackages += 1;
                UpdateProject.SaveProject(Project.Path, Project);
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the new project data.", ex,
                                PopupButtons.Ok)));
                Reset();
                return;
            }

            SetUiState(true);
            InitializeProjectData();
            InitializePackageItems();
        }

        private void ProgressChanged(object sender, TransferProgressEventArgs e)
        {
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text =
                            String.Format("Uploading... {0}",
                                String.Format("{0}% | {1}KB/s", Math.Round(e.Percentage, 1), e.BytesPerSecond/1024))));

            if (_uploadCancelled)
                Invoke(new Action(() => { loadingLabel.Text = "Cancelling upload..."; }));
        }

        private void cancelLabel_Click(object sender, EventArgs e)
        {
            _uploadCancelled = true;
            
            Invoke(new Action(() =>
            {
                loadingLabel.Text = "Cancelling upload...";
                cancelLabel.Visible = false;
            }));

            _ftp.CancelPackageUpload();
        }

        private void CancellationFinished(object sender, EventArgs e)
        {
            UpdateVersion packageVersion = null;
            try
            {
                Invoke(
                    new Action(
                        () =>
                            packageVersion = (UpdateVersion) packagesList.SelectedItems[0].Tag));
                _ftp.DeleteDirectory(String.Format("{0}/{1}", _ftp.Directory, packageVersion));
            }
            catch (Exception deletingEx)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while undoing the package upload.",
                                deletingEx, PopupButtons.Ok)));
            }

            Reset();
            _uploadCancelled = false;
        }

        #endregion

        #region "Configuration"

        private bool _foundWithFtp;
        private bool _foundWithUrl;
        private bool _hasFinishedCheck;

        private async void BeginUpdateConfigurationCheck()
        {
            Invoke(new Action(() =>
            {
                checkingUrlPictureBox.Visible = true;
                checkUpdateConfigurationLinkLabel.Enabled = false;
            }));
            await Task.Factory.StartNew(() => CheckUpdateConfigurationStatus(_configurationFileUrl));
        }

        private void checkUpdateConfigurationLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            BeginUpdateConfigurationCheck();
        }

        private void CheckUpdateConfigurationStatus(Uri configFileUrl)
        {
            if (!ConnectionChecker.IsConnectionAvailable())
            {
                Invoke(
                    new Action(
                        () =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "No network connection available.",
                                String.Format(
                                    "Checking the update configuration failed because there is no network connection avilable."),
                                PopupButtons.Ok);

                            checkingUrlPictureBox.Visible = false;
                            checkUpdateConfigurationLinkLabel.Enabled = true;
                        }));
                return;
            }

            using (var client = new WebClientWrapper(5000))
            {
                ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };
                try
                {
                    var stream = client.OpenRead(configFileUrl);
                    if (stream == null)
                    {
                        _foundWithUrl = false;
                        return;
                    }
                    _foundWithUrl = true;
                }
                catch (Exception)
                {
                    _foundWithUrl = false;
                }
            }

            try
            {
                if (_ftp.IsExisting("updates.json"))
                    _foundWithFtp = true;
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while checking if the configuration file exists.",
                        ex, PopupButtons.Ok);
                    checkingUrlPictureBox.Visible = false;
                    checkUpdateConfigurationLinkLabel.Enabled = true;
                }));
                return;
            }

            if (_foundWithUrl && _foundWithFtp)
            {
                Invoke(new Action(() =>
                {
                    tickPictureBox.Visible = true;
                    checkingUrlPictureBox.Visible = false;
                    checkUpdateConfigurationLinkLabel.Enabled = true;
                }));
            }
            else if (_foundWithFtp && !_foundWithUrl)
            {
                Invoke(
                    new Action(
                        () =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "HTTP(S)-access of configuration file failed.",
                                "The configuration file was found on the FTP-server but it couldn't be accessed via HTTP(S). Please check if the update url is correct and if your server is reachable.",
                                PopupButtons.Ok);

                            checkingUrlPictureBox.Visible = false;
                            checkUpdateConfigurationLinkLabel.Enabled = true;
                            tickPictureBox.Visible = false;
                        }));
            }
            else if (!_foundWithFtp && _foundWithUrl)
            {
                Invoke(
                    new Action(
                        () =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error,
                                "Configuration file was not found in the directory set.",
                                "The configuration file was found at the update url's destination but it couldn't be found in the given FTP-directory.",
                                PopupButtons.Ok);

                            checkingUrlPictureBox.Visible = false;
                            checkUpdateConfigurationLinkLabel.Enabled = true;
                            tickPictureBox.Visible = false;
                        }));
            }
            else if (!_foundWithFtp && !_foundWithUrl)
            {
                SetUiState(false);
                Invoke(new Action(() =>
                {
                    loadingLabel.Text = "Updating configuration file...";

                    checkUpdateConfigurationLinkLabel.Enabled = false;
                    checkingUrlPictureBox.Visible = false;
                    tickPictureBox.Visible = false;
                }));

                var temporaryConfigurationFile = Path.Combine(Program.Path, "updates.json");
                try
                {
                    if (!File.Exists(temporaryConfigurationFile))
                    {
                        using (File.Create(temporaryConfigurationFile))
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                            {
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while creating the new configuration file.", ex,
                                    PopupButtons.Ok);

                                checkingUrlPictureBox.Visible = false;
                                checkUpdateConfigurationLinkLabel.Enabled = true;
                            }));
                    SetUiState(true);
                    return;
                }

                try
                {
                    _ftp.UploadFile(temporaryConfigurationFile);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                            {
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while uploading the new configuration file.", ex,
                                    PopupButtons.Ok);

                                checkingUrlPictureBox.Visible = false;
                                checkUpdateConfigurationLinkLabel.Enabled = true;
                            }));
                    SetUiState(true);
                    return;
                }

                if (_ftp.FileUploadException != null)
                {
                    Invoke(
                        new Action(
                            () =>
                            {
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while uploading the new configuration file.",
                                    _ftp.FileUploadException,
                                    PopupButtons.Ok);

                                checkingUrlPictureBox.Visible = false;
                                checkUpdateConfigurationLinkLabel.Enabled = true;
                            }));
                    SetUiState(true);
                    return;
                }

                Invoke(
                    new Action(
                        () =>
                            checkUpdateConfigurationLinkLabel.Enabled = true));
                SetUiState(true);

                if (_hasFinishedCheck)
                {
                    _hasFinishedCheck = false;
                    return;
                }

                _hasFinishedCheck = true;
                BeginUpdateConfigurationCheck();
            }
        }

        #endregion

        #region "Deleting"

        private bool _shouldKeepErrorsSecret;

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (packagesList.SelectedItems.Count == 0)
                return;

            var answer = Popup.ShowPopup(this, SystemIcons.Question,
                "Delete the selected update packages?", "Are you sure that you want to delete this/these package(s)?",
                PopupButtons.YesNo);
            if (answer != DialogResult.Yes)
                return;

            ThreadPool.QueueUserWorkItem(delegate { DeletePackage(); }, null);
        }

        /// <summary>
        ///     Initializes a new thread for deleting the package.
        /// </summary>
        private void DeletePackage()
        {
            SetUiState(false);
            Invoke(new Action(() => loadingLabel.Text = "Getting old configuration..."));

            List<UpdateConfiguration> updateConfig;
            try
            {
                var rawUpdateConfiguration = UpdateConfiguration.Download(_configurationFileUrl, Project.Proxy);
                updateConfig = rawUpdateConfiguration != null
                    ? rawUpdateConfiguration.ToList()
                    : Enumerable.Empty<UpdateConfiguration>().ToList();
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () => Popup.ShowPopup(this, SystemIcons.Error,
                            "Error while downloading the old configuration.", ex, PopupButtons.Ok)));
                SetUiState(true);
                return;
            }

            IEnumerator enumerator = null;
            Invoke(
                new Action(
                    () => { enumerator = packagesList.SelectedItems.GetEnumerator(); }));

            if (updateConfig.Count != 0)
            {
                while (enumerator.MoveNext())
                {
                    try
                    {
                        updateConfig.Remove(
                            updateConfig.First(item => new UpdateVersion(item.LiteralVersion) ==
                                                       (UpdateVersion) ((ListViewItem) enumerator.Current).Tag));
                    }
                    catch
                    {
                        // Ignored
                    }
                }
                enumerator.Reset();

                var configurationFilePath = Path.Combine(Program.Path, "updates.json");

                try
                {
                    File.WriteAllText(configurationFilePath, Serializer.Serialize(updateConfig));
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while writing to local configuration file.", ex,
                                    PopupButtons.Ok)));
                    SetUiState(true);
                    return;
                }

                Invoke(new Action(() => loadingLabel.Text = "Uploading new configuration..."));

                try
                {
                    _ftp.UploadFile(configurationFilePath);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while uploading the new configuration file.", ex, PopupButtons.Ok)));
                    SetUiState(true);
                    return;
                }

                try
                {
                    File.WriteAllText(configurationFilePath, String.Empty);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while writing to local configuration file.", ex,
                                    PopupButtons.Ok)));
                    SetUiState(true);
                    return;
                }
            }

            while (enumerator.MoveNext())
            {
                var selectedItem = (ListViewItem) enumerator.Current;
                ListViewGroup releasedGroup = null;
                Invoke(new Action(() => releasedGroup = packagesList.Groups[0]));
                if (selectedItem.Group == releasedGroup) // Must be deleted online, too.
                {
                    Invoke(
                        new Action(
                            () =>
                                loadingLabel.Text =
                                    String.Format("Deleting package {0} on the server...", selectedItem.Text)));

                    try
                    {
                        _ftp.DeleteDirectory(String.Format("{0}/{1}", _ftp.Directory, selectedItem.Tag));
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("No such file or directory"))
                        {
                            _shouldKeepErrorsSecret = true;
                        }
                        else
                        {
                            Invoke(
                                new Action(
                                    () =>
                                        Popup.ShowPopup(this, SystemIcons.Error,
                                            "Error while deleting the package directory.", ex, PopupButtons.Ok)));
                            SetUiState(true);
                            return;
                        }

                        if (!_shouldKeepErrorsSecret)
                        {
                            SetUiState(true);
                            return;
                        }
                    }
                }

                Invoke(
                    new Action(
                        () => loadingLabel.Text = "Deleting local package directory..."));

                try
                {
                    Directory.Delete(Path.Combine(Program.Path, "Projects", Project.Name, selectedItem.Tag.ToString()),
                        true);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof (DirectoryNotFoundException))
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error,
                                        "Error while deleting local package directory.",
                                        ex, PopupButtons.Ok)));
                        SetUiState(true);
                        return;
                    }
                }

                Invoke(
                        new Action(
                            () => loadingLabel.Text = "Editing and saving project-data..."));

                try
                {
                    // Remove current package entry and save the edited project
                    Project.Packages.RemoveAll(x => x.Version == (UpdateVersion)selectedItem.Tag);
                    if (Project.ReleasedPackages != 0) // To prevent that the number becomes negative
                        Project.ReleasedPackages -= 1;

                    // The newest package
                    if (Project.Packages != null && Project.Packages.Count != 0)
                        Project.NewestPackage = Project.Packages.Last().Version.FullText;
                    else
                        Project.NewestPackage = null;

                    // The version-id must be adjusted, too
                    if (Project.UseStatistics)
                    {
                        Settings.Default.VersionID -= 1;
                        Settings.Default.Save();
                        Settings.Default.Reload();
                    }

                    UpdateProject.SaveProject(Project.Path, Project);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving new project info.", ex,
                                    PopupButtons.Ok)));
                    SetUiState(true);
                    return;
                }

                if (Project.UseStatistics)
                {
                    Invoke(
                        new Action(
                            () => loadingLabel.Text = "Connecting to SQL-server..."));

                    var connectionString = String.Format("SERVER={0};" +
                                                         "DATABASE={1};" +
                                                         "UID={2};" +
                                                         "PASSWORD={3};",
                        Project.SqlWebUrl, Project.SqlDatabaseName,
                        Project.SqlUsername,
                        SqlPassword.ConvertToUnsecureString());

                    var deleteConnection = new MySqlConnection(connectionString);

                    try
                    {
                        deleteConnection.Open();
                    }
                    catch (MySqlException ex)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
                                        ex, PopupButtons.Ok)));
                        deleteConnection.Close();
                        SetUiState(true);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while connecting to the database.",
                                        ex, PopupButtons.Ok)));
                        deleteConnection.Close();
                        SetUiState(true);
                        return;
                    }

                    Invoke(
                        new Action(
                            () => loadingLabel.Text = "Executing SQL-commands..."));

                    var queryCommand = deleteConnection.CreateCommand();
                    queryCommand.CommandText = String.Format("SELECT `ID` FROM `Version` WHERE `Version` = \"{0}\"",
                        selectedItem.Tag);

                    int versionId;
                    MySqlDataReader dataReader = null;
                    try
                    {
                        dataReader = queryCommand.ExecuteReader();
                        dataReader.Read();
                        versionId = (int) dataReader.GetValue(0);
                    }
                    catch (Exception ex)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while executing the commands.",
                                        ex, PopupButtons.Ok)));
                        deleteConnection.Close();
                        SetUiState(true);
                        return;
                    }
                    finally
                    {
                        dataReader.Close();
                    }

                    var deleteCommand = deleteConnection.CreateCommand();
                    deleteCommand.CommandText = String.Format(@"DELETE FROM Download WHERE `Version_ID`= {0};
DELETE FROM Version WHERE `ID` = {0};", versionId);

                    try
                    {
                        deleteCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while executing the commands.",
                                        ex, PopupButtons.Ok)));
                        SetUiState(true);
                        return;
                    }
                    finally
                    {
                        deleteConnection.Close();
                        deleteCommand.Dispose();
                    }
                }

                _updateLog.Write(LogEntry.Delete, ((UpdateVersion)selectedItem.Tag).FullText);
            }

            SetUiState(true);
            ThreadPool.QueueUserWorkItem(arg => InitializeStatisticsData());
            InitializePackageItems();
            InitializeProjectData();
        }

        #endregion

        private void statisticsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            updateStatisticsButton.Enabled = false;
            chartPanel.Visible = true;
            var chart = _dataGridViewRowTags.First(item => item.Key == UpdateVersion.FromFullText((string)statisticsDataGridView.Rows[e.RowIndex].Cells[0].Value)).Value;
            chart.TotalDownloadCount = Convert.ToInt32(statisticsDataGridView.Rows[e.RowIndex].Cells[1].Value);
            chart.StatisticsChartClosed += CurrentChartClosed;
            chart.Dock = DockStyle.Fill;
            chartPanel.Controls.Add(chart);
            chart.BringToFront();
        }

        private void CurrentChartClosed(object sender, EventArgs e)
        {
            chartPanel.Controls.Remove((StatisticsChart)sender);
            chartPanel.Visible = false;
            updateStatisticsButton.Enabled = true;
        }

        private void readOnlyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control || (e.KeyCode != Keys.A)) 
                return;
            if (sender != null)
                ((TextBox)sender).SelectAll();
            e.Handled = true;
        }
    }
}