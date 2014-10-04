// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Application.History;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;
using Starksoft.Net.Ftp;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectDialog : BaseDialog
    {
        private const float KB = 1024;
        private const float MB = 1048577;

        private const int COR_E_ENDOFSTREAM = unchecked((int) 0x80070026);
        private const int COR_E_FILELOAD = unchecked((int) 0x80131621);
        private const int COR_E_FILENOTFOUND = unchecked((int) 0x80070002);
        private const int COR_E_DIRECTORYNOTFOUND = unchecked((int) 0x80070003);
        private const int COR_E_PATHTOOLONG = unchecked((int) 0x800700CE);
        private const int COR_E_IO = unchecked((int) 0x80131620);
        private readonly FtpManager _ftp = new FtpManager();
        private readonly Log _updateLog = new Log();
        private bool _allowCancel = true;
        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private Uri _configurationFileUrl;
        private MySqlConnection _deleteConnection;
        private int _groupIndex;
        private MySqlConnection _insertConnection;
        private bool _isSetByUser;
        private bool _isNetworkAvailable = true;
        private UpdateVersion _packageVersion;
        private MySqlConnection _queryConnection;

        public ProjectDialog()
        {
            InitializeComponent();
        }

        #region "Localization"

        /*
        private string ftpDataLoadErrorCaption = "Failed to load FTP-data.";
        private string gettingUrlErrorCaption = "Error while getting url.";
        private string invalidArgumentCaption = "Invalid argument found.";
        private string invalidArgumentText = "The entry {0} can't be parsed to {1}.";
        private string invalidServerDirectoryErrorCaption = "Invalid server directory.";

        private string invalidServerDirectoryErrorText =
            "The directory for the update files on the server is not valid. Please edit it.";

        private string readingPackageBytesErrorCaption = "Reading package bytes failed.";

        private string relativeUriErrorText = "The server-directory can't be set as a relative uri.";
        private string savingInformationErrorCaption = "Saving package information failed.";
        private string uploadFailedErrorCaption = "Upload failed.";

        private string uploadingConfigInfoText = "Uploading configuration...";*/
        private string uploadingPackageInfoText = "Uploading package - {0}";

        /// <summary>
        ///     Sets the language.
        /// </summary>
        public void SetLanguage()
        {
            string languageFilePath = Path.Combine(Program.LanguagesDirectory,
                String.Format("{0}.json", Settings.Default.Language.Name));
            LocalizationProperties ls;
            if (File.Exists(languageFilePath))
                ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            Text = String.Format("{0} - {1}", Project.Name, ls.ProductTitle);
            overviewHeader.Text = ls.ProjectDialogOverviewText;
            overviewTabPage.Text = ls.ProjectDialogOverviewTabText;
            packagesTabPage.Text = ls.ProjectDialogPackagesTabText;
            nameLabel.Text = ls.ProjectDialogNameLabelText;
            updateUrlLabel.Text = ls.ProjectDialogUpdateUrlLabelText;
            ftpHostLabel.Text = ls.ProjectDialogFtpHostLabelText;
            ftpDirectoryLabel.Text = ls.ProjectDialogFtpDirectoryLabelText;
            releasedPackagesAmountLabel.Text = ls.ProjectDialogPackagesAmountLabelText;
            newestPackageReleasedLabel.Text = ls.ProjectDialogNewestPackageLabelText;
            infoFileloadingLabel.Text = ls.ProjectDialogInfoFileloadingLabelText;
            checkUpdateConfigurationLinkLabel.Text = ls.ProjectDialogCheckInfoFileStatusLinkLabelText;
            projectDataHeader.Text = ls.ProjectDialogProjectDataText;
            publicKeyLabel.Text = ls.ProjectDialogPublicKeyLabelText;
            projectIdLabel.Text = ls.ProjectDialogProjectIdLabelText;
            stepTwoLabel.Text = String.Format(stepTwoLabel.Text, copySourceButton.Text);

            addButton.Text = ls.ProjectDialogAddButtonText;
            editButton.Text = ls.ProjectDialogEditButtonText;
            deleteButton.Text = ls.ProjectDialogDeleteButtonText;
            uploadButton.Text = ls.ProjectDialogUploadButtonText;
            historyButton.Text = ls.ProjectDialogHistoryButtonText;

            packagesList.Columns[0].Text = ls.ProjectDialogVersionText;
            packagesList.Columns[1].Text = ls.ProjectDialogReleasedText;
            packagesList.Columns[2].Text = ls.ProjectDialogSizeText;
            packagesList.Columns[3].Text = ls.ProjectDialogDescriptionText;

            searchTextBox.Cue = ls.ProjectDialogSearchText;
        }

        #endregion

        private void ProjectDialog_Load(object sender, EventArgs e)
        {
            _ftp.ProgressChanged += ProgressChanged;

            if (!InitializeProjectDetails())
            {
                Close();
                return;
            }

            if (!InitializeFtpData())
            {
                Close();
                return;
            }

            InitializePackageItems();
            SetLanguage();

            _updateLog.Project = Project;

            programmingLanguageComboBox.DataSource = Enum.GetValues(typeof (ProgrammingLanguage));
            programmingLanguageComboBox.SelectedIndex = 0;

            if (!Project.UpdateUrl.EndsWith("/"))
                Project.UpdateUrl += "/";
            _configurationFileUrl = UriConnector.ConnectUri(Project.UpdateUrl, "updates.json");

            // Control position- and draw settings
            checkingUrlPictureBox.Location =
                new Point(
                    checkUpdateConfigurationLinkLabel.Location.X + checkUpdateConfigurationLinkLabel.Size.Width + 2,
                    checkingUrlPictureBox.Location.Y);
            tickPictureBox.Location =
                new Point(
                    checkUpdateConfigurationLinkLabel.Location.X + checkUpdateConfigurationLinkLabel.Size.Width + 2,
                    tickPictureBox.Location.Y);
            packagesList.DoubleBuffer();
            tabControl1.DoubleBuffer();

            statisticsDataGridView.RowHeadersVisible = false;

            // Check for network
            if (!ConnectionChecker.IsConnectionAvailable())
            {
                _isNetworkAvailable = false;
                checkUpdateConfigurationLinkLabel.Enabled = false;
                addButton.Enabled = false;
                deleteButton.Enabled = false;
                doNotUseStatisticsServerRadioButton.Enabled = false;
                useStatisticsServerRadioButton.Enabled = false;
                statisticsServerPanel.Enabled = false;
                label5.Text = "Statistics couldn't be loaded.\nNo network connection available.";

                foreach (
                    Control c in
                        from Control c in statisticsTabPage.Controls where c.GetType() != typeof (Panel) select c)
                {
                    c.Visible = false;
                }

                Popup.ShowPopup(this, SystemIcons.Error, "No network connection.",
                    "Some functions aren't usable because no network connection is available.", PopupButtons.Ok);
                _isSetByUser = true;
                return;
            }

            StartCheckingUpdateInfo();

            if (Project.UseStatistics)
            {
                string databaseNameString = String.Format("Database: {0}", Project.SqlSettings.DatabaseName);
                dataBaseLabel.Text = databaseNameString;

                string lastUpdatedInfoString = String.Format("Last updated on {0} at {1}",
                    DateTime.Now.ToString("dd.MM.yyy"), DateTime.Now.ToString("HH:mm:ss tt"));
                lastUpdatedLabel.Text = lastUpdatedInfoString; // TODO: Label

                useStatisticsServerRadioButton.Checked = true;

                try
                {
                    string connectionString = String.Format("SERVER={0};" +
                                                            "DATABASE={1};" +
                                                            "UID={2};" +
                                                            "PASSWORD={3};",
                        Project.SqlSettings.WebUrl, Project.SqlSettings.DatabaseName,
                        Project.SqlSettings.Username,
                        Program.SqlPassword.ConvertToUnsecureString());

                    _queryConnection = new MySqlConnection(connectionString);
                    _queryConnection.Open();
                }
                catch (MySqlException ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
                                    ex, PopupButtons.Ok)));
                    _queryConnection.Close();
                    return;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while connecting to the database.",
                                    ex, PopupButtons.Ok)));
                    _queryConnection.Close();
                    return;
                }

                var dataAdapter =
                    new MySqlDataAdapter(
                        String.Format(
                            "SELECT v.Version, COUNT(*) AS 'Downloads' FROM Download LEFT JOIN Version v ON (v.ID = Version_ID) WHERE `Applicaton_ID` = {0} GROUP BY Version_ID;",
                            Project.ApplicationId),
                        _queryConnection);
                var dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                statisticsDataGridView.DataSource = dataSet.Tables[0];
                _queryConnection.Close();

                statisticsDataGridView.Columns[0].Width = 278;
                statisticsDataGridView.Columns[1].Width = 278;
            }
            else
            {
                foreach (
                    Control c in
                        from Control c in statisticsTabPage.Controls where c.GetType() != typeof (Panel) select c)
                {
                    c.Visible = false;
                }
            }

            // All actions now are done by the user ...
            _isSetByUser = true;
        }

        private void ProjectDialog_Shown(object sender, EventArgs e)
        {
            packagesList.MakeCollapsable();
        }

        private void ProjectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            ListViewItem matchingItem = packagesList.FindItemWithText(searchTextBox.Text, true, 0);

            if (matchingItem != null)
            {
                int index = matchingItem.Index;
                packagesList.Items[index].Selected = true;
            }
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
                    "No package can be created because no network connection is available.", PopupButtons.Ok);
                return;
            }

            var packageAddDialog = new PackageAddDialog();
            var existingUpdateVersions =
                (from ListViewItem lvi in packagesList.Items select new UpdateVersion(lvi.Tag.ToString())).ToList();
            packageAddDialog.NewestVersion = UpdateVersion.GetHighestUpdateVersion(existingUpdateVersions);
            packageAddDialog.Project = Project;

            if (packageAddDialog.ShowDialog() != DialogResult.OK) return;

            packagesList.Items.Clear();
            InitializePackageItems();
            InitializeProjectDetails();
        }

        private void copySourceButton_Click(object sender, EventArgs e)
        {
            string updateUrl = updateUrlTextBox.Text;
            if (!updateUrl.EndsWith("/"))
                updateUrl += "/";

            string vbSource =
                String.Format(
                    "Dim manager As New UpdateManager(New Uri(\"{0}\"), \"{1}\", New UpdateVersion(\"0.0.0.0\"))",
                    UriConnector.ConnectUri(updateUrl, "updates.json"), publicKeyTextBox.Text);
            string cSharpSource =
                String.Format(
                    "UpdateManager manager = new UpdateManager(new Uri(\"{0}\"), \"{1}\", new UpdateVersion(\"0.0.0.0\"));",
                    UriConnector.ConnectUri(updateUrl, "updates.json"), publicKeyTextBox.Text);

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

        private void editButton_Click(object sender, EventArgs e)
        {
            var packageEditDialog = new PackageEditDialog {Project = Project};
            try
            {
                List<UpdateConfiguration> allConfigurations =
                    UpdateConfiguration.LoadFromFile(Path.Combine(Program.Path, "Projects", Project.Name,
                        _packageVersion.ToString(), "updates.json"));
                packageEditDialog.UpdateConfigurations = allConfigurations;
                packageEditDialog.PackageConfiguration =
                    allConfigurations.First(item => item.Version == _packageVersion.ToString());
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the package configuration.", ex,
                    PopupButtons.Ok);
                return;
            }
            packageEditDialog.ShowDialog();
        }

        private void editFtpButton_Click(object sender, EventArgs e)
        {
            var ftpEditDialog = new FtpEditDialog {Project = Project};
            if (ftpEditDialog.ShowDialog() != DialogResult.OK) return;
            InitializeFtpData();
            InitializeProjectDetails();
        }

        private void editProjectButton_Click(object sender, EventArgs e)
        {
            var projectEditDialog = new ProjectEditDialog {Project = Project};
            if (projectEditDialog.ShowDialog() != DialogResult.OK) return;
            string projectFilePath = Project.Path;
            Project = projectEditDialog.Project;

            try
            {
                ApplicationInstance.SaveProject(projectFilePath, Project);
                if (projectFilePath != Project.Path)
                    File.Move(projectFilePath, Project.Path);

                InitializeProjectDetails();
                _configurationFileUrl = UriConnector.ConnectUri(updateUrlTextBox.Text, "updates.json");
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving project data.", ex, PopupButtons.Ok);
            }
        }

        private void historyButton_Click(object sender, EventArgs e)
        {
            var historyDialog = new HistoryDialog {Project = Project};
            historyDialog.ShowDialog();
        }

        private void packagesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (packagesList.FocusedItem == null) return;
            if (!_isNetworkAvailable) return;
            _packageVersion = (UpdateVersion) packagesList.FocusedItem.Tag;

            if (packagesList.FocusedItem.Group == packagesList.Groups[0])
            {
                _groupIndex = 0;
                editButton.Enabled = false;
                uploadButton.Enabled = false;
            }
            else
            {
                _groupIndex = 1;
                editButton.Enabled = true;
                uploadButton.Enabled = true;
            }
        }

        /// <summary>
        ///     Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        private void SetUiState(bool enabled)
        {
            Invoke(new Action(() =>
            {
                foreach (Control c in from Control c in Controls where c.Visible select c)
                {
                    c.Enabled = enabled;
                }

                if (!enabled)
                {
                    _allowCancel = false;
                    loadingPanel.Visible = true;
                    loadingPanel.Location = new Point(179, 135);
                    loadingPanel.BringToFront();

                    editButton.Enabled = false;
                    uploadButton.Enabled = false;
                }
                else
                {
                    _allowCancel = true;
                    loadingPanel.Visible = false;
                }
            }));
        }

        private void browseAssemblyButton_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Multiselect = false;
                fileDialog.SupportMultiDottedExtensions = false;
                fileDialog.Filter = "Executable files (*.exe)|*.exe|Executable extension files (*.dll)|*.dll";

                if (fileDialog.ShowDialog() != DialogResult.OK) return;
                try
                {
                    Assembly projectAssembly = Assembly.LoadFile(fileDialog.FileName);
                    FileVersionInfo.GetVersionInfo(projectAssembly.Location);
                }
                catch
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid assembly found.",
                        "The version of the assembly for the selected executable (extension) file could not be read.",
                        PopupButtons.Ok);
                    enterVersionManuallyRadioButton.Checked = true;
                    return;
                }

                assemblyPathTextBox.Text = fileDialog.FileName;
                Project.AssemblyVersionPath = assemblyPathTextBox.Text;

                try
                {
                    ApplicationInstance.SaveProject(Project.Path, Project);
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

                InitializeProjectDetails();
            }
        }

        private void loadFromAssemblyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!loadFromAssemblyRadioButton.Checked) return;
            assemblyPathTextBox.Enabled = true;
            browseAssemblyButton.Enabled = true;
        }

        private void enterVersionManuallyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!enterVersionManuallyRadioButton.Checked) return;
            assemblyPathTextBox.Enabled = false;
            browseAssemblyButton.Enabled = false;

            if (!_isSetByUser) return;
            Project.AssemblyVersionPath = null;

            try
            {
                ApplicationInstance.SaveProject(Project.Path, Project);
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
            InitializeProjectDetails();
        }

        #region "Initializing"

        /// <summary>
        /// Initializes the FTP-data.
        /// </summary>
        /// <returns>Returns whether the operation was successful or not.</returns>
        private bool InitializeFtpData()
        {
            try
            {
                _ftp.Host = Project.FtpHost;
                _ftp.Port = Project.FtpPort;
                _ftp.UserName = Project.FtpUsername;
                _ftp.Password = Program.FtpPassword;
                _ftp.Protocol = (FtpSecurityProtocol) Project.FtpProtocol;
                _ftp.UsePassiveMode = Project.FtpUsePassiveMode;
                _ftp.Directory = Project.FtpDirectory;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.Ok);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Sets all the details for the project.
        /// </summary>
        /// <returns>Returns whether the operation was successful or not.</returns>
        private bool InitializeProjectDetails()
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
        ///     Adds the package items to the listview.
        /// </summary>
        private void InitializePackageItems()
        {
            Invoke(new Action(() =>
            {
                if (packagesList.Items.Count != 0)
                    packagesList.Items.Clear();
            }));

            if (Project.Packages == null) return;
            foreach (UpdatePackage package in Project.Packages)
            {
                try
                {
                    var lviPackage = new ListViewItem(package.Version.FullText);
                    var diPackage = new DirectoryInfo(package.LocalPackagePath);
                    lviPackage.SubItems.Add(diPackage.CreationTime.ToString());

                    // Get the size of the package
                    var fiPackageFile = new FileInfo(package.LocalPackagePath);
                    long sizeInBytes = fiPackageFile.Length;
                    float size;
                    string sizeText;

                    if (sizeInBytes > 104857.6)
                    {
                        size = (float) Math.Round(sizeInBytes/MB, 1);
                        sizeText = String.Format("{0} MB", size);
                    }
                    else
                    {
                        size = (float) Math.Round(sizeInBytes/KB, 1);
                        sizeText = String.Format("{0} KB", size);
                    }

                    lviPackage.SubItems.Add(sizeText);
                    lviPackage.SubItems.Add(package.Description);
                    lviPackage.Group = package.IsReleased ? packagesList.Groups[0] : packagesList.Groups[1];
                    lviPackage.Tag = package.Version;
                    Invoke(new Action(() => packagesList.Items.Add(lviPackage)));
                }
                catch (IOException ex)
                {
                    var dialogResult = DialogResult.None;
                    UpdatePackage packagePlaceholder = package;
                    Invoke(
                        new Action(
                            () =>
                                dialogResult =
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the package.",
                                        String.Format(
                                            "{0} - Should the entry for package {1} in the project be deleted in order to hide this error the next time?",
                                            GetNameOfExceptionType(ex), packagePlaceholder.Version), PopupButtons.YesNo)));

                    if (dialogResult != DialogResult.Yes) continue;
                    // Remove the package info from the project file and stop the loading
                    Project.Packages.RemoveAll(item => item.Version == package.Version);

                    if (Project.ReleasedPackages != 0)
                        // Set the released packages again with subtracting 1 for the project that was just removed
                        Project.ReleasedPackages -= 1;

                    // The released packages
                    if (Project.Packages != null)
                    {
                        if (Project.Packages.Count != 0)
                            Project.NewestPackage = Project.Packages.Last().Version.FullText;
                    }
                    else
                        Project.NewestPackage = null;

                    try
                    {
                        ApplicationInstance.SaveProject(Project.Path, Project);
                    }
                    catch (Exception)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while saving new project info.", ex,
                                        PopupButtons.Ok)));
                    }
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the package.", ex,
                                    PopupButtons.Ok)));
                }
            }

            InitializeProjectDetails();
        }

        /// <summary>
        ///     Returns the name/description of the current exception.
        /// </summary>
        private string GetNameOfExceptionType(Exception ex)
        {
            int hrEx = Marshal.GetHRForException(ex);
            switch (hrEx)
            {
                case COR_E_DIRECTORYNOTFOUND:
                    return "DirectoryNotFound";
                case COR_E_ENDOFSTREAM:
                    return "EndOfStream";
                case COR_E_FILELOAD:
                    return "FileLoadException";
                case COR_E_FILENOTFOUND:
                    return "FileNotFound";
            }
            return "Unknown Exception";
        }

        #endregion

        // TODO: Manage upload

        #region "Upload"

        /// <summary>
        /// Undoes the MySQL-insertion.
        /// </summary>
        private void UndoSqlInsertion()
        {
            Invoke(new Action(() => loadingLabel.Text = "Connecting to MySQL-server..."));

            string connectionString = String.Format("SERVER={0};" +
                                                    "DATABASE={1};" +
                                                    "UID={2};" +
                                                    "PASSWORD={3};",
                Project.SqlSettings.WebUrl, Project.SqlSettings.DatabaseName,
                Project.SqlSettings.Username,
                Program.SqlPassword.ConvertToUnsecureString());

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
                return;
            }

            MySqlCommand command = deleteConnection.CreateCommand();
            command.CommandText =
                String.Format("DELETE FROM `Version` WHERE `Version` = \"{0}\"", _packageVersion);

            try
            {
                command.ExecuteNonQuery();
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

        private void uploadButton_Click(object sender, EventArgs e)
        {
            if (packagesList.SelectedItems.Count == 0) return;
            ThreadPool.QueueUserWorkItem(delegate { UploadPackage(); }, null);
        }

        /// <summary>
        ///     Provides a new thread that uploads the package.
        /// </summary>
        private void UploadPackage()
        {
            try
            {
                SetUiState(false);

                /* -------------- MySQL Initializing -------------*/
                if (Project.UseStatistics)
                {
                    Invoke(new Action(() => loadingLabel.Text = "Connecting to MySQL-server..."));

                    try
                    {
                        string connectionString = String.Format("SERVER={0};" +
                                                                "DATABASE={1};" +
                                                                "UID={2};" +
                                                                "PASSWORD={3};",
                            Project.SqlSettings.WebUrl, Project.SqlSettings.DatabaseName,
                            Project.SqlSettings.Username,
                            Program.SqlPassword.ConvertToUnsecureString());

                        _insertConnection = new MySqlConnection(connectionString);
                        _insertConnection.Open();
                    }
                    catch (MySqlException ex)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
                                        ex, PopupButtons.Ok)));
                        _insertConnection.Close();
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
                        _insertConnection.Close();
                        SetUiState(true);
                        return;
                    }

                    MySqlCommand command = _insertConnection.CreateCommand();
                    command.CommandText =
                        String.Format("INSERT INTO `Version` (`Version`, `Application_ID`) VALUES (\"{0}\", {1});",
                            _packageVersion, Project.ApplicationId);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while executing the commands.",
                                        ex, PopupButtons.Ok)));
                        _insertConnection.Close();
                        SetUiState(true);
                        return;
                    }
                }

                string updateConfigurationFilePath = Path.Combine(Program.Path, "Projects", Project.Name,
                    _packageVersion.ToString(), "updates.json");
                string packagePath = Project.Packages.First(x => x.Version == _packageVersion).LocalPackagePath;
                try
                {
                    _ftp.UploadPackage(packagePath, _packageVersion.ToString());
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the package directory.",
                                    ex, PopupButtons.Ok)));
                    SetUiState(true);
                    return;
                }

                if (_ftp.PackageUploadException != null)
                {
                    if (_ftp.PackageUploadException.InnerException != null)
                    {
                        if (_ftp.PackageUploadException.InnerException.GetType() == typeof (WebException))
                        {
                            switch (
                                ((int)
                                    ((FtpWebResponse) ((WebException) _ftp.PackageUploadException).Response).StatusCode)
                                )
                            {
                                case 550:
                                    Invoke(
                                        new Action(
                                            () =>
                                                Popup.ShowPopup(this, SystemIcons.Error,
                                                    "Error while creating new config file.",
                                                    String.Format(
                                                        "The server returned 550. Make sure the given FTP-directory exists and then try again. - {0}",
                                                        ftpDirectoryTextBox.Text), PopupButtons.Ok)));
                                    SetUiState(true);
                                    break;
                                case 530:
                                    Invoke(
                                        new Action(
                                            () =>
                                                Popup.ShowPopup(this, SystemIcons.Error,
                                                    "Error while creating new config file.",
                                                    "The server login failed. Make sure the login credentials are correct and then try again.",
                                                    PopupButtons.Ok)));
                                    SetUiState(true);
                                    break;
                                default:
                                    Invoke(
                                        new Action(
                                            () =>
                                                Popup.ShowPopup(this, SystemIcons.Error,
                                                    "Error while creating new config file.",
                                                    _ftp.PackageUploadException.InnerException, PopupButtons.Ok)));
                                    SetUiState(true);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Exception ex = _ftp.PackageUploadException.InnerException ?? _ftp.PackageUploadException;

                        // Just handle it normally
                        Invoke(new Action(() =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ex,
                                PopupButtons.Ok);
                            SetUiState(true);
                        }));
                    }
                }

                try
                {
                    _ftp.DeleteFile("updates.json"); // Configuration
                    _ftp.UploadFile(updateConfigurationFilePath);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ex,
                                    PopupButtons.Ok)));
                    _cancellationToken.Cancel();
                }

                if (_cancellationToken.IsCancellationRequested)
                    return;

                // Write to log
                _updateLog.Write(LogEntry.Upload, _packageVersion.ToString());

                Project.Packages.First(x => x.Version == _packageVersion).IsReleased = true;
                ApplicationInstance.SaveProject(Project.Path, Project);
            }
            finally
            {
                Project.Packages.First(x => x.Version == _packageVersion).IsReleased = true;
                ApplicationInstance.SaveProject(Project.Path, Project);

                SetUiState(true);
                InitializeProjectDetails();
                InitializePackageItems();
            }
        }

        /// <summary>
        ///     Called when the progress changes.
        /// </summary>
        private void ProgressChanged(object sender, TransferProgressEventArgs e)
        {
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text =
                            String.Format(uploadingPackageInfoText, String.Format("{0}%", e.Percentage))));
        }

        #endregion // TODO: Manage upload

        #region "Configuration"

        /// <summary>
        ///     Starts checking if the update configuration exists.
        /// </summary>
        private void StartCheckingUpdateInfo()
        {
            checkingUrlPictureBox.Visible = true;
            ThreadPool.QueueUserWorkItem(delegate { CheckUpdateConfigurationStatus(_configurationFileUrl); }, null);
        }

        private void checkUpdateConfigurationLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StartCheckingUpdateInfo();
        }

        private bool _hasFinishedCheck;
        private bool _isExisting = true;
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

                            checkUpdateConfigurationLinkLabel.Enabled = true;
                        }));

                return;
            }

            using (var client = new WebClientWrapper(5000))
            {
                ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };
                try
                {
                    Stream stream = client.OpenRead(configFileUrl);
                    if (stream == null)
                    {
                        _isExisting = false;
                        return;
                    }
                    _isExisting = true;
                }
                catch (Exception)
                {
                    _isExisting = false;
                }
            }

            if (_isExisting)
            {
                Invoke(new Action(() =>
                {
                    tickPictureBox.Visible = true;
                    checkingUrlPictureBox.Visible = false;
                    checkUpdateConfigurationLinkLabel.Enabled = true;
                }));
                return;
            }

            if (_hasFinishedCheck)
            {
                _hasFinishedCheck = false;
                Invoke(
                    new Action(
                        () =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "HTTP(S)-access of configuration file failed.",
                                String.Format(
                                    "The configuration file was successfully updated on the FTP-server, but it couldn't be accessed via HTTP(S). Please check if it is correct."),
                                PopupButtons.Ok);

                            checkUpdateConfigurationLinkLabel.Enabled = true;
                        }));

                return;
            }

            SetUiState(false);
            Invoke(new Action(() =>
            {
                loadingLabel.Text = "Updating configuration file...";

                checkUpdateConfigurationLinkLabel.Enabled = false;
                checkingUrlPictureBox.Visible = false;
                tickPictureBox.Visible = false;
            }));

            string temporaryConfigurationFile = Path.Combine(Program.Path, "updates.json");
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
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the new configuration file.", ex,
                                PopupButtons.Ok);

                            checkUpdateConfigurationLinkLabel.Enabled = true;
                        }));
                SetUiState(true);
                return;
            }

            try
            {
                // Upload the file now
                _ftp.UploadFile(temporaryConfigurationFile);
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the new configuration file.", ex,
                                PopupButtons.Ok);

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
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the new configuration file.",
                                _ftp.FileUploadException,
                                PopupButtons.Ok);

                            checkUpdateConfigurationLinkLabel.Enabled = true;
                        }));
                SetUiState(true);
                return;
            }

            _hasFinishedCheck = true;
            SetUiState(true);
            CheckUpdateConfigurationStatus(_configurationFileUrl);
        }

        #endregion

        #region "Deleting"

        private bool _shouldKeepErrorsSecret;

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (packagesList.SelectedItems.Count == 0) return;
            DialogResult answer = Popup.ShowPopup(this, SystemIcons.Question,
                String.Format("Delete the update package {0}?", _packageVersion),
                "Are you sure that you want to delete this package?", PopupButtons.YesNo);
            if (answer == DialogResult.Yes)
            {
                ThreadPool.QueueUserWorkItem(delegate { DeletePackage(); }, null);
            }
        }

        /// <summary>
        ///     Initializes a new thread for deleting the package.
        /// </summary>
        private void DeletePackage()
        {
            try
            {
                SetUiState(false);
                if (Equals(_groupIndex, 0)) // Must be deleted online, too.
                {
                    Invoke(new Action(() => loadingLabel.Text = "Deleting package..."));
                    
                    try
                    {
                        // Delete package folder
                        _ftp.DeleteDirectory(_packageVersion.ToString());
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("No such file or directory"))
                        {
                            // Just handle it normally
                            Invoke(
                                new Action(
                                    () =>
                                        Popup.ShowPopup(this, SystemIcons.Error,
                                            "Error while deleting the package directory.", ex, PopupButtons.Ok)));
                        }
                        else
                        {
                            _shouldKeepErrorsSecret = true;
                        }

                        if (!_shouldKeepErrorsSecret)
                        {
                            SetUiState(true);
                            return;
                        }
                    }

                    Invoke(new Action(() => loadingLabel.Text = "Getting old configuration..."));

                    List<UpdateConfiguration> updateConfig = null;
                    try
                    {
                        updateConfig = UpdateConfiguration.DownloadUpdateConfiguration(_configurationFileUrl,
                            Project.Proxy);
                    }
                    catch (Exception ex)
                    {
                        Invoke(
                            new Action(
                                () => Popup.ShowPopup(this, SystemIcons.Error,
                                    "Error while downloading the old configuration.", ex, PopupButtons.Ok)));
                    }

                    // The path to the temporary new update config
                    string configurationFilePath = Path.Combine(Program.Path, "updates.json");

                    if (updateConfig != null)
                    {
                        if (updateConfig.Count != 0)
                        {
                            if (updateConfig.Any(item => new UpdateVersion(item.Version) == 
                                _packageVersion))
                            {
                                updateConfig.Remove(
                                    updateConfig.First(item => new UpdateVersion(item.Version) ==
                                                               _packageVersion));
                                string content = Serializer.Serialize(updateConfig);

                                try
                                {
                                    File.WriteAllText(configurationFilePath, content);
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
                        }

                        Invoke(new Action(() => loadingLabel.Text = "Uploading new configuration..."));

                        try
                        {
                            // Upload new configuration file
                            _ftp.UploadFile(configurationFilePath);
                        }
                        catch (Exception ex)
                        {
                            Invoke(
                                new Action(
                                    () =>
                                        Popup.ShowPopup(this, SystemIcons.Error,
                                            "Error while uploading new configuration file.", ex, PopupButtons.Ok)));
                           SetUiState(true);
                           return;
                        }

                        // A try-catch is not really necessary as the writing above has completed successfully
                        File.WriteAllText(configurationFilePath, String.Empty);
                    }
                }

                Invoke(new Action(() => loadingLabel.Text = "Deleting local directory..."));

                try
                {
                    // Delete local folder
                    Directory.Delete(Path.Combine(Program.Path, "Projects", Project.Name, _packageVersion.ToString()),
                        true);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while deleting local package directory.",
                                    ex, PopupButtons.Ok)));
                    return;
                }

                try
                {
                    // Remove current package entry and save the edited project
                    Project.Packages.RemoveAll(x => x.Version == _packageVersion);
                    if (Project.ReleasedPackages != 0) // The amount of released packages
                        Project.ReleasedPackages -= 1;

                    // The newest package
                    if (Project.Packages != null && Project.Packages.Count != 0)
                        Project.NewestPackage = Project.Packages.Last().Version.FullText;
                    else
                        Project.NewestPackage = null;

                    ApplicationInstance.SaveProject(Project.Path, Project);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving new project info.", ex,
                                    PopupButtons.Ok)));
                }
            }
            finally
            {
                _updateLog.Write(LogEntry.Delete, _packageVersion.ToString());

                SetUiState(true);
                Invoke(new Action(() =>
                {
                    packagesList.Items.Clear();
                    InitializePackageItems();
                    InitializeProjectDetails();
                }));
            }
        }

        #endregion

        #region "Statistics"

        /// <summary>
        ///     Saves the settings for the statistics.
        /// </summary>
        private void SaveStatisticsSettings(Sql sqlSettings, bool useStatistics)
        {
            if (useStatistics)
            {
                Project.SqlSettings = sqlSettings;
                Project.UseStatistics = true;
            }
            else
            {
                Project.SqlSettings = null;
                Project.UseStatistics = false;
            }

            try
            {
                ApplicationInstance.SaveProject(Project.Path, Project);
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error,
                                "Error while saving the new project-data.", ex, PopupButtons.Ok)));
            }
        }

        private void selectStatisticsServerButton_Click(object sender, EventArgs e)
        {
            var statisticsServerDialog = new StatisticsServerDialog();
            statisticsServerDialog.ReactsOnKeyDown = true;
            if (statisticsServerDialog.ShowDialog() == DialogResult.OK)
            {
                if (Project.Packages != null)
                {
                    if (Project.Packages.Count != 0)
                    {
                        var packagesToAffectDialog = new PackagesToAffectDialog();
                        foreach (ListViewItem item in packagesList.Items)
                        {
                            if (item.Group == packagesList.Groups[0])
                                packagesToAffectDialog.PackageVersionLiterals.Add(item.Text);
                        }

                        if (packagesToAffectDialog.ShowDialog() == DialogResult.OK)
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                InitializeStatisticsOnConfigurations(packagesToAffectDialog.PackageVersionsToAffect,
                                    statisticsServerDialog.SqlSettings, true);
                            });
                        }
                    }
                    else
                        SaveStatisticsSettings(statisticsServerDialog.SqlSettings, true);
                }
                else
                    SaveStatisticsSettings(statisticsServerDialog.SqlSettings, true);
            }
        }

        /// <summary>
        ///     Provides a new thread that initializes the "UseStatistics"-bool-property on the configs of the existing packages.
        /// </summary>
        private void InitializeStatisticsOnConfigurations(IEnumerable<string> packageVersionsToAffect, Sql sqlSettings,
            bool useStatistics)
        {
            SetUiState(false);

            #region "SQL-Setup"

            string connectionString = String.Format("SERVER={0};" +
                                                    "DATABASE={1};" +
                                                    "UID={2};" +
                                                    "PASSWORD={3};",
                sqlSettings.WebUrl, sqlSettings.DatabaseName,
                sqlSettings.Username,
                Program.SqlPassword.ConvertToUnsecureString());

            var packageVersionToAffects = packageVersionsToAffect as string[] ?? packageVersionsToAffect.ToArray();
            if (useStatistics) // Insert the data into the table for the MySQL-setup...
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Connecting to MySQL-server..."));

                try
                {
                    _insertConnection = new MySqlConnection(connectionString);
                    _insertConnection.Open();
                }
                catch (MySqlException ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
                                    ex, PopupButtons.Ok)));
                    _insertConnection.Close();
                    SetUiState(true);

                    _isSetByUser = false;

                    Invoke(
                        new Action(
                            () =>
                                doNotUseStatisticsServerRadioButton.Checked = true));

                    _isSetByUser = true;
                    return;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while connecting to the database.",
                                    ex, PopupButtons.Ok)));
                    _insertConnection.Close();
                    SetUiState(true);

                    _isSetByUser = false;

                    Invoke(
                        new Action(
                            () =>
                                doNotUseStatisticsServerRadioButton.Checked = true));

                    _isSetByUser = true;
                    return;
                }

                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Inserting table entries..."));

                var builder = new StringBuilder();
                foreach (string packageVersionToAffect in packageVersionToAffects)
                {
                    if (String.IsNullOrEmpty(builder.ToString()))
                        builder.Append(
                            String.Format("INSERT INTO `Version` (`Version`, `Application_ID`) VALUES (\"{0}\", {1});",
                                packageVersionToAffect, Settings.Default.ApplicationID));
                    else
                        builder.AppendLine(
                            String.Format("INSERT INTO `Version` (`Version`, `Application_ID`) VALUES (\"{0}\", {1});",
                                packageVersionToAffect, Settings.Default.ApplicationID));
                }

                // TODO: Go on...
                MySqlCommand command = _insertConnection.CreateCommand();
                command.CommandText = builder.ToString();

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while executing the commands.",
                                    ex, PopupButtons.Ok)));
                    _insertConnection.Close();
                    SetUiState(true);

                    _isSetByUser = false;

                    Invoke(
                        new Action(
                            () =>
                                doNotUseStatisticsServerRadioButton.Checked = true));

                    _isSetByUser = true;
                    return;
                }

                _insertConnection.Close();
            }

            #endregion

            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = "Downloading old config..."));

            if (!Project.UpdateUrl.EndsWith("/"))
                Project.UpdateUrl += "/";

            List<UpdateConfiguration> configurations = null;

            try
            {
                configurations =
                    UpdateConfiguration.DownloadUpdateConfiguration(
                        UriConnector.ConnectUri(Project.UpdateUrl, "updates.json"),
                        Project.Proxy);
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    Popup.ShowPopup(this, SystemIcons.Error,
                        "Error while downloading the update configuration.", ex, PopupButtons.Ok);
                    SetUiState(true);
                    _isSetByUser = false;

                    if (!useStatistics)
                        Invoke(
                            new Action(
                                () =>
                                    useStatisticsServerRadioButton.Checked = true));
                    else
                        Invoke(
                            new Action(
                                () =>
                                    doNotUseStatisticsServerRadioButton.Checked = true));

                    _isSetByUser = true;
                }));
            }

            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = "Initializing configurations..."));

            var editedConfigurations = new List<UpdateConfiguration>();

            if (useStatistics) // Should use them, so get the packages which should be affected...
            {
                if (configurations != null)
                    foreach (UpdateConfiguration configuration in configurations)
                    {
                        configuration.UseStatistics = packageVersionToAffects.Contains(configuration.Version);
                        editedConfigurations.Add(configuration);
                    }
            }
            else // All packages should be excluded as no statistics are used
            {
                if (configurations != null)
                    foreach (UpdateConfiguration configuration in configurations)
                    {
                        configuration.UseStatistics = false;
                        editedConfigurations.Add(configuration);
                    }
            }

            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = "Uploading new configuration..."));

            string path = Path.Combine(Program.Path, "updates.json");
            try
            {
                File.WriteAllText(path,
                    Serializer.Serialize(editedConfigurations));
                _ftp.DeleteFile("updates.json");
                _ftp.UploadFile(path);
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    Popup.ShowPopup(this, SystemIcons.Error,
                        "Error while uploading the new configuration.", ex, PopupButtons.Ok);
                    SetUiState(true);
                    _isSetByUser = false;

                    if (!useStatistics)
                        Invoke(
                            new Action(
                                () =>
                                    useStatisticsServerRadioButton.Checked = true));
                    else
                        Invoke(
                            new Action(
                                () =>
                                    doNotUseStatisticsServerRadioButton.Checked = true));

                    _isSetByUser = true;
                }));
            }
            finally
            {
                File.WriteAllText(path, String.Empty);
            }

            #region "SQL-setup"

            if (!useStatistics) // Delete the entries to prevent problems
            {
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Connecting to MySQL-server..."));

                try
                {
                    _deleteConnection = new MySqlConnection(connectionString);
                    _deleteConnection.Open();
                }
                catch (MySqlException ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
                                    ex, PopupButtons.Ok)));
                    _deleteConnection.Close();
                    SetUiState(true);

                    _isSetByUser = false;

                    Invoke(
                        new Action(
                            () =>
                                useStatisticsServerRadioButton.Checked = true));

                    _isSetByUser = true;
                    return;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while connecting to the database.",
                                    ex, PopupButtons.Ok)));
                    _deleteConnection.Close();
                    SetUiState(true);

                    _isSetByUser = false;

                    Invoke(
                        new Action(
                            () =>
                                useStatisticsServerRadioButton.Checked = true));

                    _isSetByUser = true;
                    return;
                }

                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Inserting table entries..."));

                string plainCommand = @"DELETE FROM Application WHERE `Application_ID` = {0}
DELETE FROM `Version` WHERE `Application_ID` = {0}";
                string command = String.Format(plainCommand,
                    Project.ApplicationId);

                // TODO: Go on...
                MySqlCommand deleteCommand = _insertConnection.CreateCommand();
                deleteCommand.CommandText = command;

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
                    _insertConnection.Close();
                    SetUiState(true);

                    _isSetByUser = false;

                    Invoke(
                        new Action(
                            () =>
                                useStatisticsServerRadioButton.Checked = true));

                    _isSetByUser = true;
                    return;
                }

                _insertConnection.Close();

                Invoke(
                    new Action(()
                        => dataBaseLabel.Text = "Database: -"));
            }

            #endregion

            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = "Saving project..."));
            SaveStatisticsSettings(sqlSettings, useStatistics);
            SetUiState(true);
        }

        private void doNotUseStatisticsServerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (doNotUseStatisticsServerRadioButton.Checked && _isSetByUser)
            {
                statisticsServerPanel.Enabled = false;
                if (Project.SqlSettings != null && Project.UseStatistics)
                {
                    ThreadPool.QueueUserWorkItem(delegate { InitializeStatisticsOnConfigurations(null, null, false); });
                }
            }
            else if (doNotUseStatisticsServerRadioButton.Checked && !_isSetByUser)
                statisticsServerPanel.Enabled = false;
        }

        private void useStatisticsServerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            statisticsServerPanel.Enabled = true;
        }

        #endregion
    }
}