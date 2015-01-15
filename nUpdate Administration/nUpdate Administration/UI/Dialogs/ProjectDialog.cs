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
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Application.History;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Popups;
using Starksoft.Net.Ftp;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class ProjectDialog : BaseDialog, IAsyncSupportable, IResettable
    {
        private const float KB = 1024;
        private const float MB = 1048577;
        private const int COR_E_ENDOFSTREAM = unchecked((int) 0x80070026);
        private const int COR_E_FILELOAD = unchecked((int) 0x80131621);
        private const int COR_E_FILENOTFOUND = unchecked((int) 0x80070002);
        private const int COR_E_DIRECTORYNOTFOUND = unchecked((int) 0x80070003);
        private bool _allowCancel = true;
        private Uri _configurationFileUrl;
        private MySqlConnection _deleteConnection;
        private IEnumerable<UpdateConfiguration> _editingUpdateConfiguration;
        private MySqlConnection _insertConnection;
        private bool _isNetworkAvailable = true;
        private bool _isSetByUser;
        private bool _packageExisting;
        private MySqlConnection _queryConnection;
        private bool _uploadCancelled;

        /// <summary>
        ///     The FTP-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public SecureString FtpPassword = new SecureString();

        /// <summary>
        ///     The proxy-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public SecureString ProxyPassword = new SecureString();

        /// <summary>
        ///     The MySQL-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public SecureString SqlPassword = new SecureString();

        private readonly FtpManager _ftp = new FtpManager();
        private readonly ManualResetEvent _loadConfigurationResetEvent = new ManualResetEvent(false);
        private readonly Log _updateLog = new Log();

        public ProjectDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        public void SetUiState(bool enabled)
        {
            BeginInvoke(new Action(() =>
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

                    //editButton.Enabled = false;
                    //uploadButton.Enabled = false;
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

        private void ProjectDialog_Load(object sender, EventArgs e)
        {
            _ftp.ProgressChanged += ProgressChanged;
            _ftp.CancellationFinished += CancellationFinished;

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
            //SetLanguage();

            _updateLog.Project = Project;

            programmingLanguageComboBox.DataSource = Enum.GetValues(typeof (ProgrammingLanguage));
            programmingLanguageComboBox.SelectedIndex = 0;

            var values = Enum.GetValues(typeof (DevelopmentalStage));
            Array.Reverse(values);
            developmentalStageComboBox.DataSource = values;
            developmentalStageComboBox.SelectedIndex = 2;

            if (!Project.UpdateUrl.EndsWith("/"))
                Project.UpdateUrl += "/";
            _configurationFileUrl = UriConnector.ConnectUri(Project.UpdateUrl, "updates.json");

            packagesList.DoubleBuffer();
            tabControl1.DoubleBuffer();

            statisticsDataGridView.RowHeadersVisible = false;

            if (!ConnectionChecker.IsConnectionAvailable())
            {
                _isNetworkAvailable = false;
                checkUpdateConfigurationLinkLabel.Enabled = false;
                addButton.Enabled = false;
                deleteButton.Enabled = false;
                label5.Text = "Statistics couldn't be loaded.\nNo network connection available.";

                foreach (
                    var c in
                        from Control c in statisticsTabPage.Controls where c.GetType() != typeof (Panel) select c)
                {
                    c.Visible = false;
                }

                Popup.ShowPopup(this, SystemIcons.Error, "No network connection.",
                    "Some functions aren't usable because no network connection is available.", PopupButtons.Ok);
                _isSetByUser = true;
                return;
            }

            StartCheckingUpdateConfiguration();

            if (Project.UseStatistics)
            {
                try
                {
                    var connectionString = String.Format("SERVER={0};" +
                                                         "DATABASE={1};" +
                                                         "UID={2};" +
                                                         "PASSWORD={3};",
                        Project.SqlWebUrl, Project.SqlDatabaseName,
                        Project.SqlUsername,
                        SqlPassword.ConvertToUnsecureString());

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
                            "SELECT v.Version, COUNT(*) AS 'Downloads' FROM Download LEFT JOIN Version v ON (v.ID = Version_ID) WHERE `Application_ID` = {0} GROUP BY Version_ID;",
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
                    var c in
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
                    "No package can be created because no network connection is available.", PopupButtons.Ok);
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
            InitializeProjectDetails();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            var packageVersion = (UpdateVersion) packagesList.SelectedItems[0].Tag;
            var packageEditDialog = new PackageEditDialog
            {
                Project = Project,
                PackageVersion = packageVersion,
                FtpPassword = FtpPassword.Copy(),
                SqlPassword = SqlPassword.Copy(),
                ProxyPassword = ProxyPassword.Copy()
            };
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

            if (correspondingPackage.IsReleased)
            {
                ThreadPool.QueueUserWorkItem(arg => LoadConfiguration());
                _loadConfigurationResetEvent.WaitOne();
                _loadConfigurationResetEvent.Reset();

                packageEditDialog.IsReleased = true;
                packageEditDialog.UpdateConfiguration = _editingUpdateConfiguration.ToList();
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
            if (packageEditDialog.ShowDialog() == DialogResult.OK)
                InitializePackageItems();
        }

        /// <summary>
        ///     Loads the configration for editing a package on the server.
        /// </summary>
        private void LoadConfiguration()
        {
            SetUiState(false);
            BeginInvoke(new Action(() => loadingLabel.Text = "Initializing..."));

            try
            {
                BeginInvoke(new Action(() => loadingLabel.Text = "Downloading update configuration..."));
                _editingUpdateConfiguration = UpdateConfiguration.Download(_configurationFileUrl, null);
            }
            catch (Exception ex)
            {
                BeginInvoke(
                    new Action(
                        () => Popup.ShowPopup(this, SystemIcons.Error, "Error while downloading the configuration.", ex,
                            PopupButtons.Ok)));
            }
            finally
            {
                SetUiState(true);
                _loadConfigurationResetEvent.Set();
            }
        }

        private void copySourceButton_Click(object sender, EventArgs e)
        {
            if (!updateUrlTextBox.Text.EndsWith("/"))
                updateUrlTextBox.Text += "/";

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
                    "Dim manager As New UpdateManager(New Uri(\"{0}\"), \"{1}\", New UpdateVersion(\"{2}\"), New CultureInfo(\"en-US\"))",
                    UriConnector.ConnectUri(updateUrlTextBox.Text, "updates.json"), publicKeyTextBox.Text, versionString);
            var cSharpSource =
                String.Format(
                    "UpdateManager manager = new UpdateManager(new Uri(\"{0}\"), \"{1}\", new UpdateVersion(\"{2}\"), new CultureInfo(\"en-US\"));",
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
            if (!_isNetworkAvailable) return;

            if (packagesList.SelectedItems.Count > 1 || packagesList.SelectedItems.Count == 0)
            {
                editButton.Enabled = false;
                uploadButton.Enabled = false;
                deleteButton.Enabled = false;
            }
            else if (packagesList.SelectedItems.Count == 1)
            {
                editButton.Enabled = true;
                deleteButton.Enabled = true;
                if (packagesList.SelectedItems[0].Group == packagesList.Groups[1])
                    uploadButton.Enabled = true;
            }
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
                    var projectAssembly = Assembly.LoadFile(fileDialog.FileName);
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

        private void developmentalStageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            developmentBuildNumericUpDown.Enabled = developmentalStageComboBox.SelectedIndex != 2;
        }

        #region "Initializing"

        /// <summary>
        ///     Initializes the FTP-data.
        /// </summary>
        /// <returns>Returns whether the operation was successful or not.</returns>
        private bool InitializeFtpData()
        {
            try
            {
                _ftp.Host = Project.FtpHost;
                _ftp.Port = Project.FtpPort;
                _ftp.Username = Project.FtpUsername;
                _ftp.Password = FtpPassword;
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

            if (Project.Packages == null || Project.Packages.Count == 0) return;
            foreach (var package in Project.Packages)
            {
                try
                {
                    var packageListViewItem = new ListViewItem(package.Version.FullText);
                    var packageDirectoryInfo = new DirectoryInfo(package.LocalPackagePath);
                    packageListViewItem.SubItems.Add(packageDirectoryInfo.CreationTime.ToString());

                    var packageFileInfo = new FileInfo(package.LocalPackagePath);
                    var sizeInBytes = packageFileInfo.Length;
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

                    packageListViewItem.SubItems.Add(sizeText);
                    packageListViewItem.SubItems.Add(package.Description);
                    packageListViewItem.Group = package.IsReleased ? packagesList.Groups[0] : packagesList.Groups[1];
                    packageListViewItem.Tag = package.Version;
                    Invoke(new Action(() => packagesList.Items.Add(packageListViewItem)));
                }
                catch (IOException ex)
                {
                    var dialogResult = DialogResult.None;
                    var packagePlaceholder = package;
                    Invoke(
                        new Action(
                            () =>
                                dialogResult =
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the package.",
                                        String.Format(
                                            "{0} - Should the entry for package {1} in the project be deleted in order to hide this error the next time?",
                                            GetNameOfExceptionType(ex), packagePlaceholder.Version), PopupButtons.YesNo)));

                    if (dialogResult != DialogResult.Yes) continue;
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


                    InitializeProjectDetails();
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
        }

        /// <summary>
        ///     Returns the name/description of the current exception.
        /// </summary>
        private string GetNameOfExceptionType(Exception ex)
        {
            var hrEx = Marshal.GetHRForException(ex);
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

        #region "Upload"

        /// <summary>
        ///     Resets the MySQL-data and undoes the package upload.
        /// </summary>
        public void Reset()
        {
            // TODO: MySQL
            if (_packageExisting)
            {
                try
                {
                    UpdateVersion packageVersion = null;
                    Invoke(new Action(() => packageVersion = (UpdateVersion) packagesList.SelectedItems[0].Tag));

                    Invoke(new Action(() => loadingLabel.Text = "Undoing package upload..."));
                    _ftp.DeleteDirectory(String.Format("{0}/{1}", _ftp.Directory, packageVersion));
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("No such file or directory"))
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while undoing the package upload.",
                                        ex,
                                        PopupButtons.Ok)));
                    }
                }
            }

            SetUiState(true);
        }

        /// <summary>
        ///     Undoes the MySQL-insertion.
        /// </summary>
        private void UndoSqlInsertion(UpdateVersion packageVersion)
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
                return;
            }

            var command = deleteConnection.CreateCommand();
            command.CommandText =
                String.Format("DELETE FROM `Version` WHERE `Version` = \"{0}\"", packageVersion);

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
            var version = (UpdateVersion) packagesList.SelectedItems[0].Tag;
            ThreadPool.QueueUserWorkItem(delegate { UploadPackage(version); }, null);
        }

        /// <summary>
        ///     Provides a new thread that uploads the package.
        /// </summary>
        private void UploadPackage(UpdateVersion packageVersion)
        {
            SetUiState(false);

            /* -------------- MySQL Initializing -------------*/
            if (Project.UseStatistics)
            {
                Invoke(new Action(() => loadingLabel.Text = "Connecting to MySQL-server..."));

                try
                {
                    var connectionString = String.Format("SERVER={0};" +
                                                         "DATABASE={1};" +
                                                         "UID={2};" +
                                                         "PASSWORD={3};",
                        Project.SqlWebUrl, Project.SqlDatabaseName,
                        Project.SqlUsername,
                        SqlPassword.ConvertToUnsecureString());

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
                    SetUiState(true); // Single call is faster than "Reset"-method
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

                var command = _insertConnection.CreateCommand();
                command.CommandText =
                    String.Format("INSERT INTO `Version` (`Version`, `Application_ID`) VALUES (\"{0}\", {1});",
                        packageVersion, Project.ApplicationId);

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

            /* -------------- Package upload -----------------*/
            Invoke(new Action(() =>
            {
                loadingLabel.Text = String.Format("Uploading... {0}", "0%");
                cancelLabel.Visible = true;
            }));

            var updateConfigurationFilePath = Path.Combine(Program.Path, "Projects", Project.Name,
                packageVersion.ToString(), "updates.json");
            var packagePath = Project.Packages.First(x => x.Version == packageVersion).LocalPackagePath;
            try
            {
                _ftp.UploadPackage(packagePath, packageVersion.ToString());
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

            if (_uploadCancelled)
                return;

            if (_ftp.PackageUploadException != null)
            {
                var ex = _ftp.PackageUploadException.InnerException ?? _ftp.PackageUploadException;
                Invoke(
                    new Action(
                        () => Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ex,
                            PopupButtons.Ok)));

                SetUiState(true);
                return;
            }

            _packageExisting = true;

            Invoke(new Action(() =>
            {
                loadingLabel.Text = "Uploading new configuration...";
                cancelLabel.Visible = false;
            }));

            try
            {
                _ftp.UploadFile(updateConfigurationFilePath);
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while creating new config file.", ex,
                                PopupButtons.Ok)));
                Reset();
                return;
            }

            _updateLog.Write(LogEntry.Upload, packageVersion.ToString());

            try
            {
                Project.Packages.First(x => x.Version == packageVersion).IsReleased = true;
                Project.NewestPackage = packageVersion.FullText;
                Project.ReleasedPackages += 1;
                ApplicationInstance.SaveProject(Project.Path, Project);
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
            InitializeProjectDetails();
            InitializePackageItems();
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
                            String.Format("Uploading... {0}",
                                String.Format("{0}% | {1}KiB/s", Math.Round(e.Percentage, 1), e.BytesPerSecond/1024))));

            if (_uploadCancelled)
            {
                Invoke(new Action(() => { loadingLabel.Text = "Cancelling upload..."; }));
            }
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
                Invoke(new Action(() => packageVersion = (UpdateVersion) packagesList.SelectedItems[0].Tag));
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
        }

        #endregion

        #region "Configuration"

        private bool _foundWithFtp;
        private bool _foundWithUrl;
        private bool _hasFinishedCheck;

        /// <summary>
        ///     Starts checking if the update configuration exists.
        /// </summary>
        private void StartCheckingUpdateConfiguration()
        {
            Invoke(new Action(() =>
            {
                checkingUrlPictureBox.Visible = true;
                checkUpdateConfigurationLinkLabel.Enabled = false;
            }));
            ThreadPool.QueueUserWorkItem(delegate { CheckUpdateConfigurationStatus(_configurationFileUrl); }, null);
        }

        private void checkUpdateConfigurationLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StartCheckingUpdateConfiguration();
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
                                "The configuration file was found on the FTP-server but it couldn't be accessed via HTTP(S). Please check if the update url is correct.",
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
                StartCheckingUpdateConfiguration();
            }
        }

        #endregion

        #region "Deleting"

        private bool _shouldKeepErrorsSecret;

        private void deleteButton_Click(object sender, EventArgs e)
        {
            var answer = Popup.ShowPopup(this, SystemIcons.Question,
                "Delete the selected update packages?", "Are you sure that you want to delete this package?",
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

            var updateConfig = new List<UpdateConfiguration>();
            try
            {
                var rawUpdateConfiguration = UpdateConfiguration.Download(_configurationFileUrl, Project.Proxy);
                if (rawUpdateConfiguration != null)
                    updateConfig = rawUpdateConfiguration.ToList();
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
            Invoke(new Action(() => { enumerator = packagesList.SelectedItems.GetEnumerator(); }));

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
                                    String.Format("Deleting package {0} on server...", selectedItem.Text)));

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

                    if (updateConfig.Count != 0)
                    {
                        if (updateConfig.Any(item => new UpdateVersion(item.LiteralVersion) ==
                                                     (UpdateVersion) selectedItem.Tag))
                        {
                            updateConfig.Remove(
                                updateConfig.First(item => new UpdateVersion(item.LiteralVersion) ==
                                                           (UpdateVersion) selectedItem.Tag));
                        }
                    }
                }

                Invoke(new Action(() => loadingLabel.Text = "Deleting local package directory..."));

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

                try
                {
                    // Remove current package entry and save the edited project
                    Project.Packages.RemoveAll(x => x.Version == (UpdateVersion) selectedItem.Tag);
                    if (Project.ReleasedPackages != 0) // To prevent that the number becomes negative
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
                    SetUiState(true);
                    return;
                }

                _updateLog.Write(LogEntry.Delete, selectedItem.Tag.ToString());
            }


            var configurationFilePath = Path.Combine(Program.Path, "updates.json");
            var content = Serializer.Serialize(updateConfig);

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
                                "Error while uploading new configuration file.", ex, PopupButtons.Ok)));
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

            SetUiState(true);
            InitializePackageItems();
            InitializeProjectDetails();
        }

        #endregion
    }
}