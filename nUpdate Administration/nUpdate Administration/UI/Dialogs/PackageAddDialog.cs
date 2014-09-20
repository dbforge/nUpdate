// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ionic.Zip;
using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Application.History;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Core.Update.Operations;
using nUpdate.Administration.Core.Update.Operations.Panels;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;
using Starksoft.Net.Ftp;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageAddDialog : BaseDialog
    {
        private readonly TreeNode _createRegistryEntryNode = new TreeNode("Create registry entry", 14, 14);
        private readonly List<CultureInfo> _cultures = new List<CultureInfo>();
        private readonly TreeNode _deleteNode = new TreeNode("Delete file", 9, 9);
        private readonly TreeNode _deleteRegistryEntryNode = new TreeNode("Delete registry entry", 12, 12);
        private readonly FtpManager _ftp = new FtpManager();
        private readonly UpdatePackage _package = new UpdatePackage();
        private readonly TreeNode _renameNode = new TreeNode("Rename file", 10, 10);
        private readonly TreeNode _replaceNode = new TreeNode("Replace file/folder", 11, 11);
        private readonly TreeNode _setRegistryKeyValueNode = new TreeNode("Set registry key value", 13, 13);
        private readonly TreeNode _startProcessNode = new TreeNode("Start process", 8, 8);
        private readonly TreeNode _startServiceNode = new TreeNode("Start service", 5, 5);
        private readonly TreeNode _stopServiceNode = new TreeNode("Stop service", 6, 6);
        private readonly TreeNode _terminateProcessNode = new TreeNode("Terminate process", 7, 7);
        private readonly BindingList<string> _unsupportedVersionsBindingsList = new BindingList<string>();
        private readonly Log _updateLog = new Log();
        private bool _allowCancel = true;
        private int _architectureIndex = 2;
        private UpdateConfiguration _configuration = new UpdateConfiguration();
        private bool _hasFailedOnLoading;
        private bool _includeIntoStatistics;
        private MySqlConnection _insertConnection;
        private bool _mustUpdate;

        private string _packageFolder;
        private UpdateVersion _packageVersion;
        private bool _publishUpdate;
        private readonly bool _savePackages = Settings.Default.SavePackages; 
        private Uri _configurationFileUrl;
        private string _updateConfigFile;
        private ZipFile _zip = new ZipFile();

        public PackageAddDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The newest package version.
        /// </summary>
        internal UpdateVersion NewestVersion { get; set; }

        /// <summary>
        ///     Sets the developmental stage of the package.
        /// </summary>
        public DevelopmentalStage DevStage { get; set; }

        /// <summary>
        ///     Returns all operations set.
        /// </summary>
        internal List<Operation> Operations { get; set; }

        #region "Localization"

        private string configDownloadErrorCaption;
        private string creatingPackageDataErrorCaption;
        private string ftpDataLoadErrorCaption;
        private string gettingUrlErrorCaption;
        private string initializingArchiveInfoText;
        private string initializingConfigInfoText;
        private string invalidArgumentCaption;
        private string invalidArgumentText;
        private string invalidServerDirectoryErrorCaption;
        private string invalidServerDirectoryErrorText;
        private string invalidVersionCaption;
        private string invalidVersionText;
        private string loadingProjectDataErrorCaption;
        private string noChangelogCaption;
        private string noChangelogText;
        private string noFilesCaption;
        private string noFilesText;
        private string noNetworkCaption;
        private string noNetworkText;
        private string preparingUpdateInfoText;
        private string readingPackageBytesErrorCaption;
        private string relativeUriErrorText;
        private string savingInformationErrorCaption;
        private string serializingDataErrorCaption;
        private string signingPackageInfoText;
        private string unsupportedArchiveCaption;
        private string unsupportedArchiveText;
        private string uploadFailedErrorCaption;
        private string uploadingConfigInfoText;
        private string uploadingPackageInfoText;

        private void SetLanguage()
        {
            string languageFilePath = Path.Combine(Program.LanguagesDirectory,
                String.Format("{0}.json", Settings.Default.Language.Name));
            var ls = new LocalizationProperties();
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

            noNetworkCaption = ls.PackageAddDialogNoInternetWarningCaption;
            noNetworkText = ls.PackageAddDialogNoInternetWarningText;
            noFilesCaption = ls.PackageAddDialogNoFilesSpecifiedWarningCaption;
            noFilesText = ls.PackageAddDialogNoFilesSpecifiedWarningText;
            unsupportedArchiveCaption = ls.PackageAddDialogUnsupportedArchiveWarningCaption;
            unsupportedArchiveText = ls.PackageAddDialogUnsupportedArchiveWarningText;
            invalidVersionCaption = ls.PackageAddDialogVersionInvalidWarningCaption;
            invalidVersionText = ls.PackageAddDialogVersionInvalidWarningText;
            noChangelogCaption = ls.PackageAddDialogNoChangelogWarningCaption;
            noChangelogText = ls.PackageAddDialogNoChangelogWarningText;
            invalidArgumentCaption = ls.InvalidArgumentErrorCaption;
            invalidArgumentText = ls.InvalidArgumentErrorText;
            creatingPackageDataErrorCaption = ls.PackageAddDialogPackageDataCreationErrorCaption;
            loadingProjectDataErrorCaption = ls.PackageAddDialogProjectDataLoadingErrorCaption;
            gettingUrlErrorCaption = ls.PackageAddDialogGettingUrlErrorCaption;
            readingPackageBytesErrorCaption = ls.PackageAddDialogReadingPackageBytesErrorCaption;
            invalidServerDirectoryErrorCaption = ls.PackageAddDialogInvalidServerDirectoryErrorCaption;
            invalidServerDirectoryErrorText = ls.PackageAddDialogInvalidServerDirectoryErrorText;
            ftpDataLoadErrorCaption = ls.PackageAddDialogLoadingFtpDataErrorCaption;
            configDownloadErrorCaption = ls.PackageAddDialogConfigurationDownloadErrorCaption;
            serializingDataErrorCaption = ls.PackageAddDialogSerializingDataErrorCaption;
            relativeUriErrorText = ls.PackageAddDialogRelativeUriErrorText;
            savingInformationErrorCaption = ls.PackageAddDialogPackageInformationSavingErrorCaption;
            uploadFailedErrorCaption = ls.PackageAddDialogUploadFailedErrorCaption;

            initializingArchiveInfoText = ls.PackageAddDialogArchiveInitializerInfoText;
            preparingUpdateInfoText = ls.PackageAddDialogPrepareInfoText;
            signingPackageInfoText = ls.PackageAddDialogSigningInfoText;
            initializingConfigInfoText = ls.PackageAddDialogConfigInitializerInfoText;
            uploadingPackageInfoText = ls.PackageAddDialogUploadingPackageInfoText;
            uploadingConfigInfoText = ls.PackageAddDialogUploadingConfigInfoText;

            Text = String.Format(ls.PackageAddDialogTitle, Project.Name, ls.ProductTitle);
            cancelButton.Text = ls.CancelButtonText;
            createButton.Text = ls.CreatePackageButtonText;

            devStageLabel.Text = ls.PackageAddDialogDevelopmentalStageLabelText;
            versionLabel.Text = ls.PackageAddDialogVersionLabelText;
            descriptionLabel.Text = ls.PackageAddDialogDescriptionLabelText;
            publishCheckBox.Text = ls.PackageAddDialogPublishCheckBoxText;
            publishInfoLabel.Text = ls.PackageAddDialogPublishInfoLabelText;
            environmentLabel.Text = ls.PackageAddDialogEnvironmentLabelText;
            architectureInfoLabel.Text = ls.PackageAddDialogEnvironmentInfoLabelText;

            changelogLoadButton.Text = ls.PackageAddDialogLoadButtonText;
            changelogClearButton.Text = ls.PackageAddDialogClearButtonText;

            addFilesButton.Text = ls.PackageAddDialogAddFileButtonText;
            removeEntryButton.Text = ls.PackageAddDialogRemoveFileButtonText;
            filesList.Columns[0].Text = ls.PackageAddDialogNameHeaderText;
            filesList.Columns[1].Text = ls.PackageAddDialogSizeHeaderText;

            allVersionsRadioButton.Text = ls.PackageAddDialogAvailableForAllRadioButtonText;
            someVersionsRadioButton.Text = ls.PackageAddDialogAvailableForSomeRadioButtonText;
            allVersionsInfoLabel.Text = ls.PackageAddDialogAvailableForAllInfoText;
            someVersionsInfoLabel.Text = ls.PackageAddDialogAvailableForSomeInfoText;
        }

        #endregion

        /// <summary>
        ///     Initializes the tree nodes for the update operations.
        /// </summary>
        private void InitializeOperationNodes()
        {
            _replaceNode.Tag = "ReplaceFile";
            _deleteNode.Tag = "DeleteFile";
            _renameNode.Tag = "RenameFile";
            _createRegistryEntryNode.Tag = "CreateRegistryEntry";
            _deleteRegistryEntryNode.Tag = "DeleteRegistryEntry";
            _setRegistryKeyValueNode.Tag = "SetRegistryKeyValue";
            _startProcessNode.Tag = "StartProcess";
            _terminateProcessNode.Tag = "StopProcess";
            _startServiceNode.Tag = "StartService";
            _stopServiceNode.Tag = "StopService";
        }

        /// <summary>
        ///     Initializes the FTP-data.
        /// </summary>
        private void InitializeFtpData()
        {
            try
            {
                _ftp.Host = Project.FtpHost;
                _ftp.Port = Project.FtpPort;
                _ftp.UserName = Project.FtpUsername;
                _ftp.Password = Program.FtpPassword;
                _ftp.Protocol = (FtpSecurityProtocol)Project.FtpProtocol;
                _ftp.UsePassiveMode = Project.FtpUsePassiveMode;
                _ftp.Directory = Project.FtpDirectory;
            }
            catch (IOException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.Ok);
                _hasFailedOnLoading = true;
            }
            catch (NullReferenceException)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.",
                    "The project file is corrupt and does not have the necessary arguments.", PopupButtons.Ok);
                _hasFailedOnLoading = true;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.Ok);
                _hasFailedOnLoading = true;
            }
        }

        private void PackageAddDialog_Load(object sender, EventArgs e)
        {
            _ftp.ProgressChanged += ProgressChanged;

            _updateLog.Project = Project;
            Operations = new List<Operation>();

            InitializeFtpData();
            if (_hasFailedOnLoading)
            {
                // TODO: Failed to load FTP-data. *Take the code from ProjectDialog*
                Close();
                return;
            }

            SetLanguage();
            InitializeOperationNodes();

            categoryTreeView.Nodes[3].Nodes.Add(_replaceNode);
            categoryTreeView.Nodes[3].Toggle();

            unsupportedVersionsListBox.DataSource = _unsupportedVersionsBindingsList;
            Array devStages = Enum.GetValues(typeof (DevelopmentalStage));
            Array.Reverse(devStages);
            developmentalStageComboBox.DataSource = devStages;
            List<CultureInfo> cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            foreach (CultureInfo info in cultureInfos)
            {
                changelogLanguageComboBox.Items.Add(String.Format("{0} - {1}", info.EnglishName, info.Name));
                _cultures.Add(info);
            }

            changelogContentTabControl.TabPages[0].Tag = _cultures.Where(x => x.Name == "en");
            changelogLanguageComboBox.SelectedIndex = changelogLanguageComboBox.FindStringExact("English - en");

            architectureComboBox.SelectedIndex = 2;
            categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
            developmentalStageComboBox.SelectedIndex = 2;
            unsupportedVersionsPanel.Enabled = false;

            _publishUpdate = publishCheckBox.Checked;
            _mustUpdate = mustUpdateCheckBox.Checked;
            includeIntoStatisticsCheckBox.Enabled = Project.UseStatistics;
            majorNumericUpDown.Minimum = NewestVersion.Major;
            minorNumericUpDown.Value = NewestVersion.Minor;
            buildNumericUpDown.Value = NewestVersion.Build;
            revisionNumericUpDown.Value = NewestVersion.Revision;

            majorNumericUpDown.Maximum = Decimal.MaxValue;
            minorNumericUpDown.Maximum = Decimal.MaxValue;
            buildNumericUpDown.Maximum = Decimal.MaxValue;
            revisionNumericUpDown.Maximum = Decimal.MaxValue;

            if (!String.IsNullOrEmpty(Project.AssemblyVersionPath))
            {
                Assembly projectAssembly = Assembly.LoadFile(Project.AssemblyVersionPath);
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(projectAssembly.Location);
                var assemblyVersion = new UpdateVersion(info.FileVersion);

                majorNumericUpDown.Value = assemblyVersion.Major;
                minorNumericUpDown.Value = assemblyVersion.Minor;
                buildNumericUpDown.Value = assemblyVersion.Build;
                revisionNumericUpDown.Value = assemblyVersion.Revision;
            }

            generalTabPage.DoubleBuffer();
            cancelToolTip.SetToolTip(cancelLabel, "Click here to cancel the package upload.");
        }

        private void PackageAddDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        /// <summary>
        ///     Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        private void SetUiState(bool enabled)
        {
            if (enabled)
                _allowCancel = true;

            Invoke(new Action(() =>
            {
                foreach (Control c in from Control c in Controls where c.Visible select c)
                {
                    c.Enabled = enabled;
                }

                loadingPanel.Visible = !enabled;
            }));
        }

        /// <summary>
        /// Resets the data set.
        /// </summary>
        /// <param name="savePackage">Sets if the package should be saved.</param>
        private void Reset(bool savePackage)
        {
            if (Project.Packages != null)
            {
                Project.Packages.Remove(_package); // Remove the saved package again
            }

            if (!savePackage)
            {
                Directory.Delete(Path.Combine(Program.Path, "Projects", Project.Name, _packageVersion.ToString()), true);
                // Directory

                _zip.Dispose(); // Zip-file
                _zip = new ZipFile();

                _configuration = null; // Configuration
                _configuration = new UpdateConfiguration();
            }
            else
            {
                _package.IsReleased = false;

                if (Project.Packages == null)
                    Project.Packages = new List<UpdatePackage>();
                Project.Packages.Add(_package);

                try
                {
                    ApplicationInstance.SaveProject(Project.Path, Project);
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while saving project data.", ex, PopupButtons.Ok);
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (DevStage == DevelopmentalStage.Release)
            {
                _packageVersion = new UpdateVersion((int) majorNumericUpDown.Value, (int) minorNumericUpDown.Value,
                    (int) buildNumericUpDown.Value, (int) revisionNumericUpDown.Value);
            }
            else
            {
                _packageVersion = new UpdateVersion((int) majorNumericUpDown.Value, (int) minorNumericUpDown.Value,
                    (int) buildNumericUpDown.Value, (int) revisionNumericUpDown.Value, DevStage,
                    (int) developmentBuildNumericUpDown.Value);
            }

            if (_packageVersion.LiteralUpdateVersion == "0.0.0.0")
            {
                Popup.ShowPopup(this, SystemIcons.Error, invalidVersionCaption, "Version \"0.0.0.0\" is not a valid version.", PopupButtons.Ok);
                generalPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
                return;
            }

            if (Project.Packages != null && Project.Packages.Count != 0)
            {
                var equalItems = new List<UpdateVersion>();
                Project.Packages.ForEach(item => equalItems.Add(item.Version));
                if (_packageVersion <= UpdateVersion.GetHighestUpdateVersion(equalItems))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, invalidVersionCaption,
                        String.Format(
                            "Version \"{0}\" is whether already existing or older than the newest one released.",
                            _packageVersion.FullText), PopupButtons.Ok);
                    generalPanel.BringToFront();
                    categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
                    return;
                }
            }

            if (String.IsNullOrEmpty(englishChangelogTextBox.Text))
            {
                Popup.ShowPopup(this, SystemIcons.Error, noChangelogCaption, noChangelogText, PopupButtons.Ok);
                changelogPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[1];
                return;
            }

            if (filesDataTreeView.Nodes[0].Nodes.Count == 0)
            {
                Popup.ShowPopup(this, SystemIcons.Error, noFilesCaption, noFilesText, PopupButtons.Ok);
                filesPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[3].Nodes[0];
                return;
            }

            _allowCancel = false;
            SetUiState(false);

            loadingPanel.Location = new Point(180, 91);
            loadingPanel.BringToFront();
            loadingPanel.Visible = true;

            ThreadPool.QueueUserWorkItem(delegate { InitializePackage(); }, null);
        }

        /// <summary>
        ///     Initializes the contents for the archive.
        /// </summary>
        /// <param name="treeNode">The current node to use.</param>
        /// <param name="currentDirectory">The current directory in the archive to paste the entries.</param>
        private void InitializeArchiveContents(TreeNode treeNode, string currentDirectory)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                FileAttributes attr = File.GetAttributes(node.Tag.ToString());
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    string tmpDir = string.Format("{0}/{1}", currentDirectory, node.Text);
                    try
                    {
                        _zip.AddDirectoryByName(tmpDir);
                        InitializeArchiveContents(node, tmpDir);
                    }
                    catch (ArgumentException)
                    {
                        TreeNode nodePlaceHolder = node;
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Information, "The element was removed.",
                                        String.Format(
                                            "The file/folder \"{0}\" was removed from the collection because it is already existing in the current directory.",
                                            nodePlaceHolder.Text), PopupButtons.Ok)));
                    }
                }
                else
                {
                    try
                    {
                        _zip.AddFile(node.Tag.ToString(), currentDirectory);
                    }
                    catch (ArgumentException)
                    {
                        TreeNode nodePlaceHolder = node;
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Information, "The element was removed.",
                                        String.Format(
                                            "The file/folder \"{0}\" was removed from the collection because it is already existing in the current directory.",
                                            nodePlaceHolder.Text), PopupButtons.Ok)));
                    }
                }
            }
        }

        /// <summary>
        ///     Initializes the update package and uploads it, if set.
        /// </summary>
        private void InitializePackage()
        {
            if (!Project.UpdateUrl.EndsWith("/"))
                Project.UpdateUrl += "/";

            _configurationFileUrl = UriConnector.ConnectUri(Project.UpdateUrl, "updates.json");
            _packageFolder = Path.Combine(Program.Path, "Projects", Project.Name, _packageVersion.ToString());
            _updateConfigFile = Path.Combine(Program.Path, "Projects", Project.Name, _packageVersion.ToString(),
                "updates.json");

            Invoke(new Action(() => loadingLabel.Text = initializingArchiveInfoText));

            // Save the package first
            // ----------------------

            try
            {
                Directory.CreateDirectory(_packageFolder); // Create the content folder
                using (File.Create(_updateConfigFile))
                {
                }
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while creating local package data.", ex,
                                PopupButtons.Ok)));
                SetUiState(true);
                Reset(false);
                return;
            }

            _zip.AddDirectoryByName("Program");
            _zip.AddDirectoryByName("AppData");
            _zip.AddDirectoryByName("Temp");
            _zip.AddDirectoryByName("Desktop");

            InitializeArchiveContents(filesDataTreeView.Nodes[0], "Program");
            InitializeArchiveContents(filesDataTreeView.Nodes[1], "AppData");
            InitializeArchiveContents(filesDataTreeView.Nodes[2], "Temp");
            InitializeArchiveContents(filesDataTreeView.Nodes[3], "Desktop");

            string packageFile = String.Format("{0}.zip", Project.Guid);
            _zip.Save(Path.Combine(_packageFolder, packageFile));

            _updateLog.Write(LogEntry.Create, _packageVersion.ToString());
            Invoke(new Action(() => loadingLabel.Text = preparingUpdateInfoText));

            // Initialize the package itself
            // -----------------------------
            string[] unsupportedVersions = null;

            if (unsupportedVersionsListBox.Items.Count == 0)
                allVersionsRadioButton.Checked = true;
            else if (unsupportedVersionsListBox.Items.Count > 0 && someVersionsRadioButton.Checked)
            {
                unsupportedVersions = unsupportedVersionsListBox.Items.Cast<string>().ToArray();
            }

            var changelog = new Dictionary<CultureInfo, string> {{new CultureInfo("en"), englishChangelogTextBox.Text}};
            foreach (
                TabPage tabPage in
                    changelogContentTabControl.TabPages.Cast<TabPage>().Where(tabPage => tabPage.Text != "English"))
            {
                var panel = (ChangelogPanel) tabPage.Controls[0];
                if (String.IsNullOrEmpty(panel.Changelog)) continue;
                changelog.Add((CultureInfo) tabPage.Tag, panel.Changelog);
            }

            // Create a new package configuration
            _configuration.Changelog = changelog;
            _configuration.MustUpdate = _mustUpdate;

            switch (_architectureIndex)
            {
                case 0:
                    _configuration.Architecture = "x86";
                    break;
                case 1:
                    _configuration.Architecture = "x64";
                    break;
                case 2:
                    _configuration.Architecture = "AnyCPU";
                    break;
            }

            _configuration.Operations = new List<Operation>();
            Invoke(new Action(() =>
            {
                foreach (var operationPanel in from TreeNode node in categoryTreeView.Nodes[3].Nodes
                    where node.Index != 0
                    select (IOperationPanel) categoryTabControl.TabPages[4 + node.Index].Controls[0])
                {
                    _configuration.Operations.Add(operationPanel.Operation);
                }
            }));

            Invoke(new Action(() => loadingLabel.Text = signingPackageInfoText));

            try
            {
                byte[] data = File.ReadAllBytes(Path.Combine(_packageFolder, String.Format("{0}.zip", Project.Guid)));
                _configuration.Signature = Convert.ToBase64String(new RsaSignature(Project.PrivateKey).SignData(data));
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while signing the package.", ex,
                                PopupButtons.Ok)));
                SetUiState(true);
                Reset(false);
                return;
            }

            _configuration.UnsupportedVersions = unsupportedVersions;
            _configuration.UpdatePhpFileUrl = UriConnector.ConnectUri(Project.UpdateUrl, "getfile.php");
            _configuration.UpdatePackageUrl = UriConnector.ConnectUri(Project.UpdateUrl,
                String.Format("{0}/{1}.zip", _packageVersion, Project.Guid));
            _configuration.Version = _packageVersion.ToString();
            _configuration.UseStatistics = _includeIntoStatistics;

            if (Project.UseStatistics)
                _configuration.VersionId = Project.VersionId + 1;

            /* -------- Configuration initializing ------------*/
            Invoke(new Action(() => loadingLabel.Text = initializingConfigInfoText));

            List<UpdateConfiguration> configurationList = null;

            // Load the configuration
            try
            {
                configurationList = UpdateConfiguration.DownloadUpdateConfiguration(_configurationFileUrl, Project.Proxy);
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the old configuration.", ex,
                                PopupButtons.Ok)));
                SetUiState(true);
                Reset(false);
                return;
            }

            if (configurationList == null) // If nethod returned null
                configurationList = new List<UpdateConfiguration>();

            configurationList.Add(_configuration);
            File.WriteAllText(_updateConfigFile, Serializer.Serialize(configurationList));

            /* ------------- Save package info  ------------- */
            Invoke(new Action(() => _package.Description = descriptionTextBox.Text));
            _package.IsReleased = _publishUpdate;
            _package.LocalPackagePath = Path.Combine(Program.Path, "Projects", Project.Name, _packageVersion.ToString(),
                String.Format("{0}.zip", Project.Guid));
            _package.Version = _packageVersion;

            if (Project.Packages == null)
                Project.Packages = new List<UpdatePackage>();
            Project.Packages.Add(_package);

            if (_publishUpdate)
            {
                /* -------------- MySQL Initializing -------------*/
                if (Project.UseStatistics)
                {
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
                        Reset(_savePackages);
                        if (!_savePackages) return;

                        DialogResult = DialogResult.OK;
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
                        Reset(_savePackages);
                        if (!_savePackages) return;

                        DialogResult = DialogResult.OK;
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
                        Reset(_savePackages);
                        if (!_savePackages) return;

                        DialogResult = DialogResult.OK;
                        return;
                    }
                }

                /* -------------- Package upload -----------------*/
                Invoke(new Action(() =>
                {
                    loadingLabel.Text = String.Format(uploadingPackageInfoText, "0%");
                    cancelLabel.Visible = true;
                }));

                try
                {
                    _ftp.UploadPackage(
                        Path.Combine(Program.Path, "Projects", Project.Name, _packageVersion.ToString(),
                            String.Format("{0}.zip", Project.Guid)), _packageVersion.ToString());
                }
                catch (Exception ex) // Upload-method is async, it's true, but directory creation can fail.
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the package directory.",
                                    ex, PopupButtons.Ok)));
                    SetUiState(true);
                    Reset(_savePackages);
                    if (!_savePackages) return;

                    DialogResult = DialogResult.OK;
                    return;
                }

                if (_ftp.PackageUploadException != null)
                {
                    if (_ftp.PackageUploadException.InnerException != null)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the package.",
                                        _ftp.PackageUploadException.InnerException, PopupButtons.Ok)));
                        SetUiState(true);
                        Reset(_savePackages);
                        if (!_savePackages) return;

                        DialogResult = DialogResult.OK;
                        return;
                    }
                }

                Invoke(new Action(() =>
                {
                    loadingLabel.Text = uploadingConfigInfoText;
                    cancelLabel.Visible = true;
                }));

                try
                {
                    _ftp.UploadFile(_updateConfigFile);
                    _updateLog.Write(LogEntry.Upload, _packageVersion.ToString());
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the configuration.", ex,
                            PopupButtons.Ok);
                        loadingLabel.Text = "Undoing changes...";
                    }));

                    try
                    {
                        _ftp.DeleteDirectory(_packageVersion.ToString());
                        _updateLog.Write(LogEntry.Delete, _packageVersion.ToString());
                    }
                    catch (Exception deletingEx)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while undoing the package upload.",
                                        deletingEx, PopupButtons.Ok)));
                        SetUiState(true);
                        Reset(_savePackages);
                        if (!_savePackages) return;
                        
                        DialogResult = DialogResult.OK;
                        return;
                    }
                }
            }

            if (_publishUpdate)
            {
                Project.NewestPackage = _packageVersion.FullText;
                Project.ReleasedPackages += 1;
            }

            SetUiState(true);

            try
            {
                ApplicationInstance.SaveProject(Project.Path, Project);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving project data.", ex, PopupButtons.Ok);
            }
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Fired when the uploading progress changes.
        /// </summary>
        private void ProgressChanged(object sender, TransferProgressEventArgs e)
        {
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text =
                            String.Format(uploadingPackageInfoText,
                                String.Format("{0}% | {1}KiB/s", Math.Round(e.Percentage, 1), e.BytesPerSecond/1024))));
        }

        private void changelogLoadButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.SupportMultiDottedExtensions = false;
                ofd.Multiselect = false;

                ofd.Filter = "Textdocument (*.txt)|*.txt|RTF-Document (*.rtf)|*.rtf";

                if (ofd.ShowDialog() == DialogResult.OK)
                    englishChangelogTextBox.Text = File.ReadAllText(ofd.FileName, Encoding.Default);
            }
        }

        private void changelogClearButton_Click(object sender, EventArgs e)
        {
            englishChangelogTextBox.Clear();
        }

        /// <summary>
        ///     Lists the directory content recursively.
        /// </summary>
        private void ListDirectory(string path)
        {
            var rootDirectoryInfo = new DirectoryInfo(path);
            TreeNode selectedNode = filesDataTreeView.SelectedNode;
            selectedNode.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        /// <summary>
        ///     Creates a new subnode for the corresponding directory info.
        /// </summary>
        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name, 2, 2);
            directoryNode.Tag = directoryInfo.FullName;
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            }
            foreach (var fileNode in directoryInfo.GetFiles().Select(file => new TreeNode(file.Name, 1, 1) {Tag = file.FullName}))
            {
                directoryNode.Nodes.Add(fileNode);
            }

            return directoryNode;
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode == null) return;
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() != DialogResult.OK) return;
                if (filesDataTreeView.SelectedNode == null) return;
                ListDirectory(folderDialog.SelectedPath);
                if (!filesDataTreeView.SelectedNode.IsExpanded)
                    filesDataTreeView.SelectedNode.Toggle();
            }
        }

        private void addFilesButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode == null) return;
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.SupportMultiDottedExtensions = true;
                fileDialog.Multiselect = true;
                fileDialog.Filter = "All Files (*.*)| *.*";

                if (fileDialog.ShowDialog() != DialogResult.OK) return;
                foreach (var node in fileDialog.FileNames.Select(path => new TreeNode(Path.GetFileName(path)) {Tag = path, ImageIndex = 1, SelectedImageIndex = 1}))
                {
                    filesDataTreeView.SelectedNode.Nodes.Add(node);

                    if (!filesDataTreeView.SelectedNode.IsExpanded)
                        filesDataTreeView.SelectedNode.Toggle();
                }
            }
        }

        private void removeEntryButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode != null && filesDataTreeView.SelectedNode.Parent != null)
                filesDataTreeView.SelectedNode.Remove();
        }

        private void someVersionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            unsupportedVersionsPanel.Enabled = true;
        }

        private void allVersionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            unsupportedVersionsPanel.Enabled = false;
        }

        private void addVersionButton_Click(object sender, EventArgs e)
        {
            string version = unsupportedMajorNumericUpDown.Value + "." + unsupportedMinorNumericUpDown.Value + "." +
                             unsupportedBuildNumericUpDown.Value + "." + unsupportedRevisionNumericUpDown.Value;
            _unsupportedVersionsBindingsList.Add(version);
        }

        private void removeVersionButton_Click(object sender, EventArgs e)
        {
            _unsupportedVersionsBindingsList.Remove(unsupportedVersionsListBox.SelectedItem.ToString());
        }

        private void developmentalStageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DevStage =
                (DevelopmentalStage)
                    Enum.Parse(typeof (DevelopmentalStage),
                        developmentalStageComboBox.GetItemText(developmentalStageComboBox.SelectedItem));
            if (DevStage == DevelopmentalStage.Alpha || DevStage == DevelopmentalStage.Beta)
                developmentBuildNumericUpDown.Enabled = true;
            else
                developmentBuildNumericUpDown.Enabled = false;
        }

        private void publishCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _publishUpdate = publishCheckBox.Checked;
        }

        private void mustUpdateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _mustUpdate = mustUpdateCheckBox.Checked;
        }

        private void includeIntoStatisticsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _includeIntoStatistics = includeIntoStatisticsCheckBox.Checked;
        }

        private void architectureComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _architectureIndex = architectureComboBox.SelectedIndex;
        }

        private void changelogLanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (changelogLanguageComboBox.SelectedIndex == changelogLanguageComboBox.FindStringExact("English - en"))
            {
                changelogContentTabControl.SelectTab(changelogContentTabControl.TabPages[0]);
                return;
            }

            if (
                changelogContentTabControl.TabPages.Cast<TabPage>()
                    .Any(item => item.Tag.Equals(_cultures[changelogLanguageComboBox.SelectedIndex])))
            {
                TabPage aimPage = changelogContentTabControl.TabPages.Cast<TabPage>()
                    .First(item => item.Tag.Equals(_cultures[changelogLanguageComboBox.SelectedIndex]));
                changelogContentTabControl.SelectTab(aimPage);
            }
            else
            {
                var page = new TabPage("Changelog")
                {
                    BackColor = SystemColors.Window,
                    Tag = _cultures[changelogLanguageComboBox.SelectedIndex]
                };
                page.Controls.Add(new ChangelogPanel());
                changelogContentTabControl.TabPages.Add(page);
                changelogContentTabControl.SelectTab(page);
            }
        }

        private void categoryTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (categoryTreeView.SelectedNode.Parent == null) // Check whether the selected node is an operation or not
            {
                switch (categoryTreeView.SelectedNode.Index)
                {
                    case 0:
                        categoryTabControl.SelectedTab = generalTabPage;
                        break;
                    case 1:
                        categoryTabControl.SelectedTab = changelogTabPage;
                        break;
                    case 2:
                        categoryTabControl.SelectedTab = availabilityTabPage;
                        break;
                    case 3:
                        categoryTabControl.SelectedTab = operationsTabPage;
                        break;
                }
            }
            else
            {
                switch (categoryTreeView.SelectedNode.Tag.ToString())
                {
                    case "ReplaceFile":
                        categoryTabControl.SelectedTab = replaceFilesTabPage;
                        break;
                    default:
                        categoryTabControl.SelectedTab =
                            categoryTabControl.TabPages[4 + categoryTreeView.SelectedNode.Index];
                        break;
                }
            }
        }

        private void categoryTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (categoryTreeView.SelectedNode == null) return;
            if ((e.KeyCode != Keys.Delete && e.KeyCode != Keys.Back) || categoryTreeView.SelectedNode.Parent == null ||
                categoryTreeView.SelectedNode.Text == "Replace file/folder") return;
            categoryTabControl.TabPages.Remove(
                categoryTabControl.TabPages[4 + categoryTreeView.SelectedNode.Index]);
            categoryTreeView.SelectedNode.Remove();
        }

        private void categoryTreeView_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode nodeToDropIn = categoryTreeView.GetNodeAt(categoryTreeView.PointToClient(new Point(e.X, e.Y)));
            if (nodeToDropIn == null || nodeToDropIn.Index != 3) // Operations-node
                return;

            object data = e.Data.GetData(typeof (string));
            if (data == null)
                return;

            switch (data.ToString())
            {
                case "DeleteFile":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _deleteNode.Clone());

                    var deletePage = new TabPage("Delete file") {BackColor = SystemColors.Window};
                    deletePage.Controls.Add(new FileDeleteOperationPanel());
                    categoryTabControl.TabPages.Add(deletePage);
                    break;

                case "RenameFile":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _renameNode.Clone());

                    var renamePage = new TabPage("Rename file") {BackColor = SystemColors.Window};
                    renamePage.Controls.Add(new FileRenameOperationPanel());
                    categoryTabControl.TabPages.Add(renamePage);
                    break;

                case "CreateRegistryEntry":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _createRegistryEntryNode.Clone());

                    var createRegistryEntryPage = new TabPage("Create registry entry") {BackColor = SystemColors.Window};
                    createRegistryEntryPage.Controls.Add(new RegistryEntryCreateOperationPanel());
                    categoryTabControl.TabPages.Add(createRegistryEntryPage);
                    break;

                case "DeleteRegistryEntry":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _deleteRegistryEntryNode.Clone());

                    var deleteRegistryEntryPage = new TabPage("Delete registry entry") {BackColor = SystemColors.Window};
                    deleteRegistryEntryPage.Controls.Add(new RegistryEntryDeleteOperationPanel());
                    categoryTabControl.TabPages.Add(deleteRegistryEntryPage);
                    break;

                case "SetRegistryKeyValue":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _setRegistryKeyValueNode.Clone());

                    var setRegistryEntryValuePage = new TabPage("Set registry entry value") {BackColor = SystemColors.Window};
                    setRegistryEntryValuePage.Controls.Add(new RegistryEntrySetValueOperationPanel());
                    categoryTabControl.TabPages.Add(setRegistryEntryValuePage);
                    break;
                case "StartProcess":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _startProcessNode.Clone());

                    var startProcessPage = new TabPage("Start process") {BackColor = SystemColors.Window};
                    startProcessPage.Controls.Add(new ProcessStartOperationPanel());
                    categoryTabControl.TabPages.Add(startProcessPage);
                    break;
                case "TerminateProcess":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _terminateProcessNode.Clone());

                    var terminateProcessPage = new TabPage("Terminate process") {BackColor = SystemColors.Window};
                    terminateProcessPage.Controls.Add(new ProcessStopOperationPanel());
                    categoryTabControl.TabPages.Add(terminateProcessPage);
                    break;
                case "StartService":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _startServiceNode.Clone());

                    var startServicePage = new TabPage("Start service") {BackColor = SystemColors.Window};
                    startServicePage.Controls.Add(new ServiceStartOperationPanel());
                    categoryTabControl.TabPages.Add(startServicePage);
                    break;
                case "StopService":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _stopServiceNode.Clone());

                    var stopServicePage = new TabPage("Stop service") {BackColor = SystemColors.Window};
                    stopServicePage.Controls.Add(new ServiceStopOperationPanel());
                    categoryTabControl.TabPages.Add(stopServicePage);
                    break;
            }
        }

        private void categoryTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void operationsListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (operationsListView.SelectedItems.Count > 0)
                operationsListView.DoDragDrop(operationsListView.SelectedItems[0].Tag, DragDropEffects.Move);
        }

        private void operationsListView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Error, "", "Not implemented.", PopupButtons.Ok);
        }

        private void cancelLabel_Click(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                loadingLabel.Text = "Cancelling upload...";
                cancelLabel.Visible = false;
            }));

            _ftp.CancelPackageUpload();

            try
            {
                _ftp.DeleteDirectory(_packageVersion.ToString());
            }
            catch (Exception deletingEx)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while undoing the package upload.",
                                deletingEx, PopupButtons.Ok)));
            }

            SetUiState(true);
            Reset(_savePackages);
            if (!_savePackages) return;

            DialogResult = DialogResult.OK;
        }
    }
}