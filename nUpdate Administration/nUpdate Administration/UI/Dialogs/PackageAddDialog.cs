// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ionic.Zip;
using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Application.History;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Core.Update.Operations;
using nUpdate.Administration.Core.Update.Operations.Panels;
using nUpdate.Administration.Core.Win32;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;
using Starksoft.Net.Ftp;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageAddDialog : BaseDialog, IAsyncSupportable, IResettable
    {
        private bool _allowCancel = true;
        private int _architectureIndex = 2;
        private Uri _configurationFileUrl;
        private DevelopmentalStage _developmentalStage;
        private bool _includeIntoStatistics;
        private MySqlConnection _insertConnection;
        private bool _mustUpdate;
        private bool _nodeInitializingFailed;
        private string _packageFolder;
        private bool _packageUploaded;
        private UpdateVersion _packageVersion;
        private bool _publishUpdate;
        private string _updateConfigFile;
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

        private readonly UpdateConfiguration _configuration = new UpdateConfiguration();

        private readonly TreeNode _createRegistrySubKeyNode = new TreeNode("Create registry sub key", 14, 14)
        {
            Tag = "CreateRegistrySubKey"
        };

        private readonly List<CultureInfo> _cultures = new List<CultureInfo>();
        private readonly TreeNode _deleteNode = new TreeNode("Delete file", 9, 9) {Tag = "DeleteFile"};

        private readonly TreeNode _deleteRegistrySubKeyNode = new TreeNode("Delete registry sub key", 12, 12)
        {
            Tag = "DeleteRegistrySubKey"
        };

        private readonly TreeNode _deleteRegistryValueNode = new TreeNode("Delete registry value", 12, 12)
        {
            Tag = "DeleteRegistryValue"
        };

        private readonly FtpManager _ftp = new FtpManager();
        private readonly UpdatePackage _package = new UpdatePackage();
        private readonly TreeNode _renameNode = new TreeNode("Rename file", 10, 10) {Tag = "RenameFile"};
        private readonly TreeNode _replaceNode = new TreeNode("Replace file/folder", 11, 11) {Tag = "ReplaceFile"};

        private readonly TreeNode _setRegistryValueNode = new TreeNode("Set registry key value", 13, 13)
        {
            Tag = "SetRegistryValue"
        };

        private readonly TreeNode _startProcessNode = new TreeNode("Start process", 8, 8) {Tag = "StartProcess"};
        private readonly TreeNode _startServiceNode = new TreeNode("Start service", 5, 5) {Tag = "StartService"};
        private readonly TreeNode _stopServiceNode = new TreeNode("Stop service", 6, 6) {Tag = "StopService"};
        private readonly TreeNode _terminateProcessNode = new TreeNode("Terminate process", 7, 7) {Tag = "StopProcess"};
        private readonly BindingList<string> _unsupportedVersionLiteralsBindingList = new BindingList<string>();
        private readonly Log _updateLog = new Log();
        private readonly ZipFile _zip = new ZipFile();

        public PackageAddDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The existing package versions.
        /// </summary>
        public IEnumerable<UpdateVersion> ExistingVersions { get; set; }

        /// <summary>
        ///     Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        public void SetUiState(bool enabled)
        {
            if (enabled)
                _allowCancel = true;

            Invoke(new Action(() =>
            {
                foreach (var c in from Control c in Controls where c.Visible select c)
                {
                    c.Enabled = enabled;
                }

                loadingPanel.Visible = !enabled;
            }));
        }

        /// <summary>
        ///     Resets the data set.
        /// </summary>
        public void Reset()
        {
            if (_packageUploaded)
            {
                try
                {
                    Invoke(new Action(() => loadingLabel.Text = "Undoing package upload..."));
                    _ftp.DeleteDirectory(String.Format("{0}/{1}", _ftp.Directory, _packageVersion));
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
            if (Project.Packages != null)
            {
                Project.Packages.Remove(_package); // Remove the saved package again
            }

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
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while saving project data.", ex,
                                PopupButtons.Ok)));
            }
            finally
            {
                DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        ///     Initializes the FTP-data.
        /// </summary>
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

                return true;
            }
            catch (IOException ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.Ok);
                return false;
            }
            catch (NullReferenceException)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.",
                    "The project file is corrupt and does not have the necessary arguments.", PopupButtons.Ok);
                return false;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading FTP-data.", ex, PopupButtons.Ok);
                return false;
            }
        }

        private void PackageAddDialog_Load(object sender, EventArgs e)
        {
            _ftp.ProgressChanged += ProgressChanged;
            _ftp.CancellationFinished += CancellationFinished;

            _updateLog.Project = Project;

            if (!InitializeFtpData())
            {
                Close();
                return;
            }

            //SetLanguage();

            categoryTreeView.Nodes[3].Nodes.Add(_replaceNode);
            categoryTreeView.Nodes[3].Toggle();

            unsupportedVersionsListBox.DataSource = _unsupportedVersionLiteralsBindingList;
            var devStages = Enum.GetValues(typeof (DevelopmentalStage));
            Array.Reverse(devStages);
            developmentalStageComboBox.DataSource = devStages;
            var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            foreach (var info in cultureInfos)
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

            majorNumericUpDown.Maximum = Decimal.MaxValue;
            minorNumericUpDown.Maximum = Decimal.MaxValue;
            buildNumericUpDown.Maximum = Decimal.MaxValue;
            revisionNumericUpDown.Maximum = Decimal.MaxValue;

            if (!String.IsNullOrEmpty(Project.AssemblyVersionPath))
            {
                var projectAssembly = Assembly.LoadFile(Project.AssemblyVersionPath);
                var info = FileVersionInfo.GetVersionInfo(projectAssembly.Location);
                var assemblyVersion = new UpdateVersion(info.FileVersion);

                majorNumericUpDown.Value = assemblyVersion.Major;
                minorNumericUpDown.Value = assemblyVersion.Minor;
                buildNumericUpDown.Value = assemblyVersion.Build;
                revisionNumericUpDown.Value = assemblyVersion.Revision;
            }

            generalTabPage.DoubleBuffer();
            changelogTabPage.DoubleBuffer();
            cancelToolTip.SetToolTip(cancelLabel, "Click here to cancel the package upload.");
        }

        private void PackageAddDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        private void createPackageButton_Click(object sender, EventArgs e)
        {
            if (_developmentalStage == DevelopmentalStage.Release)
            {
                _packageVersion = new UpdateVersion((int) majorNumericUpDown.Value, (int) minorNumericUpDown.Value,
                    (int) buildNumericUpDown.Value, (int) revisionNumericUpDown.Value);
            }
            else
            {
                _packageVersion = new UpdateVersion((int) majorNumericUpDown.Value, (int) minorNumericUpDown.Value,
                    (int) buildNumericUpDown.Value, (int) revisionNumericUpDown.Value, _developmentalStage,
                    (int) developmentBuildNumericUpDown.Value);
            }

            if (_packageVersion.BasicVersion == "0.0.0.0")
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Invalid version set.",
                    "Version \"0.0.0.0\" is not a valid version.", PopupButtons.Ok);
                generalPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
                return;
            }

            if (Project.Packages != null && Project.Packages.Count != 0)
            {
                if (ExistingVersions.Any(item => item == _packageVersion))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid version set.",
                        String.Format(
                            "Version \"{0}\" is already existing.",
                            _packageVersion.FullText), PopupButtons.Ok);
                    generalPanel.BringToFront();
                    categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
                    return;
                }
            }

            if (String.IsNullOrEmpty(englishChangelogTextBox.Text))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No changelog set.",
                    "Please specify a changelog for the package.", PopupButtons.Ok);
                changelogPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[1];
                return;
            }

            if (!filesDataTreeView.Nodes.Cast<TreeNode>().Any(node => node.Nodes.Count > 0))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No files and/or folders set.",
                    "Please specify some files and/or folders that should be included into the package.",
                    PopupButtons.Ok);
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
                var isDirectory = false;
                if (node.Tag != null)
                {
                    var attributes = File.GetAttributes(node.Tag.ToString());
                    if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        isDirectory = true;
                }
                else
                {
                    isDirectory = true;
                }

                if (isDirectory)
                {
                    var tmpDir = string.Format("{0}/{1}", currentDirectory, node.Text);
                    try
                    {
                        _zip.AddDirectoryByName(tmpDir);
                        InitializeArchiveContents(node, tmpDir);
                    }
                    catch (ArgumentException)
                    {
                        var nodePlaceHolder = node;
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
                        var nodePlaceHolder = node;
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

            Invoke(new Action(() => loadingLabel.Text = "Initializing archive..."));

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
                Reset();
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

            var packageFile = String.Format("{0}.zip", Project.Guid);
            _zip.Save(Path.Combine(_packageFolder, packageFile));

            _updateLog.Write(LogEntry.Create, _packageVersion.ToString());
            Invoke(new Action(() => loadingLabel.Text = "Preparing update..."));

            // Initialize the package itself
            // -----------------------------
            string[] unsupportedVersionLiterals = null;

            if (unsupportedVersionsListBox.Items.Count == 0)
                allVersionsRadioButton.Checked = true;
            else if (unsupportedVersionsListBox.Items.Count > 0 && someVersionsRadioButton.Checked)
            {
                unsupportedVersionLiterals = _unsupportedVersionLiteralsBindingList.ToArray();
            }

            var changelog = new Dictionary<CultureInfo, string> {{new CultureInfo("en"), englishChangelogTextBox.Text}};
            foreach (
                var tabPage in
                    changelogContentTabControl.TabPages.Cast<TabPage>().Where(tabPage => tabPage.Text != "English"))
            {
                var panel = (ChangelogPanel) tabPage.Controls[0];
                if (String.IsNullOrEmpty(panel.Changelog)) continue;
                changelog.Add((CultureInfo) tabPage.Tag, panel.Changelog);
            }

            // Create a new package configuration
            _configuration.Changelog = changelog;
            _configuration.MustUpdate = _mustUpdate;
            _configuration.Architecture = (Architecture) _architectureIndex;

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

            Invoke(new Action(() => loadingLabel.Text = "Signing package..."));

            try
            {
                byte[] data;
                using (var reader =
                    new BinaryReader(File.Open(Path.Combine(_packageFolder, String.Format("{0}.zip", Project.Guid)),
                        FileMode.Open)))
                {
                    data = reader.ReadBytes((int) reader.BaseStream.Length);
                }
                _configuration.Signature = Convert.ToBase64String(new RsaSignature(Project.PrivateKey).SignData(data));
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while signing the package.", ex,
                                PopupButtons.Ok)));
                Reset();
                return;
            }

            _configuration.UnsupportedVersions = unsupportedVersionLiterals;
            _configuration.UpdatePhpFileUrl = UriConnector.ConnectUri(Project.UpdateUrl, "getfile.php");
            _configuration.UpdatePackageUrl = UriConnector.ConnectUri(Project.UpdateUrl,
                String.Format("{0}/{1}.zip", _packageVersion, Project.Guid));
            _configuration.LiteralVersion = _packageVersion.ToString();
            _configuration.UseStatistics = _includeIntoStatistics;

            if (Project.UseStatistics)
                _configuration.VersionId = Project.VersionId + 1;

            /* -------- Configuration initializing ------------*/
            Invoke(new Action(() => loadingLabel.Text = "Initializing configuration..."));

            var configurationList = new List<UpdateConfiguration>();

            // Load the configuration
            try
            {
                var configurationEnumerable = UpdateConfiguration.Download(_configurationFileUrl, Project.Proxy);
                if (configurationEnumerable != null)
                    configurationList = configurationEnumerable.ToList();
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the old configuration.", ex,
                                PopupButtons.Ok)));
                Reset();
                return;
            }

            configurationList.Add(_configuration);

            try
            {
                File.WriteAllText(_updateConfigFile, Serializer.Serialize(configurationList));
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the new configuration.", ex,
                                PopupButtons.Ok)));
                Reset();
                return;
            }

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
                        Reset();
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
                        Reset();
                        return;
                    }

                    var command = _insertConnection.CreateCommand();
                    command.CommandText =
                        String.Format("INSERT INTO `Version` (`Version`, `Application_ID`) VALUES (\"{0}\", {1});",
                            _packageVersion, Project.ApplicationId);
                    // SQL-injections are impossible as conversions to the relating datatype would already fail if any injection statements were attached (would have to be a string then)

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
                        Reset();
                        return;
                    }
                }

                /* -------------- Package upload -----------------*/
                Invoke(new Action(() =>
                {
                    loadingLabel.Text = String.Format("Uploading package... {0}", "0%");
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
                    Reset();
                    return;
                }

                if (_uploadCancelled)
                    return;

                _packageUploaded = true;

                if (_ftp.PackageUploadException != null)
                {
                    if (_ftp.PackageUploadException.InnerException != null)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the package.",
                                        _ftp.PackageUploadException.InnerException, PopupButtons.Ok)));
                    }
                    else
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the package.",
                                        _ftp.PackageUploadException, PopupButtons.Ok)));
                    }

                    Reset();
                    return;
                }

                Invoke(new Action(() =>
                {
                    loadingLabel.Text = "Uploading configuration...";
                    cancelLabel.Visible = false;
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
                        _ftp.DeleteDirectory(String.Format("{0}/{1}", _ftp.Directory, _packageVersion));
                        _updateLog.Write(LogEntry.Delete, _packageVersion.ToString());
                    }
                    catch (Exception deletingEx)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while undoing the package upload.",
                                        deletingEx, PopupButtons.Ok)));
                        Reset();
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
                Reset();
                return;
            }

            DialogResult = DialogResult.OK;
        }

        /// <summary>
        ///     Fired when the uploading progress changes.
        /// </summary>
        private void ProgressChanged(object sender, TransferProgressEventArgs e)
        {
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text =
                            String.Format("Uploading package... {0}% | {1}KiB/s", Math.Round(e.Percentage, 1),
                                e.BytesPerSecond/1024)));
            if (_uploadCancelled)
            {
                Invoke(new Action(() => { loadingLabel.Text = "Cancelling upload..."; }));
            }
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
            if (changelogLanguageComboBox.SelectedIndex == changelogLanguageComboBox.FindStringExact("English - en"))
            {
                ((TextBox) changelogContentTabControl.SelectedTab.Controls[0]).Clear();
            }
            else
            {
                var currentChangelogPanel = (ChangelogPanel) changelogContentTabControl.SelectedTab.Controls[0];
                ((TextBox) currentChangelogPanel.Controls[0]).Clear();
            }
        }

        /// <summary>
        ///     Lists the directory content recursively.
        /// </summary>
        private void ListDirectoryContent(string path)
        {
            var rootDirectoryInfo = new DirectoryInfo(path);
            Invoke(new Action(() =>
            {
                var directoryNode = CreateDirectoryNode(rootDirectoryInfo);
                if (directoryNode == null)
                    return;

                filesDataTreeView.SelectedNode.Nodes.Add(directoryNode);
                if (!filesDataTreeView.SelectedNode.IsExpanded)
                    filesDataTreeView.SelectedNode.Toggle();
            }));
        }

        /// <summary>
        ///     Creates a new subnode for the corresponding directory info.
        /// </summary>
        private TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name, 0, 0) {Tag = directoryInfo.FullName};
            try
            {
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    if (_nodeInitializingFailed)
                    {
                        _nodeInitializingFailed = false;
                        break;
                    }

                    var node = CreateDirectoryNode(directory);
                    if (node != null)
                        directoryNode.Nodes.Add(node);
                }

                foreach (var file in directoryInfo.GetFiles())
                {
                    if (_nodeInitializingFailed)
                    {
                        _nodeInitializingFailed = false;
                        break;
                    }

                    TreeNode fileNode;
                    if (filesImageList.Images.ContainsKey(file.Extension))
                    {
                        var index = filesImageList.Images.IndexOfKey(file.Extension);
                        fileNode = new TreeNode(file.Name, index, index) {Tag = file.FullName};
                    }
                    else
                    {
                        var icon = IconReader.GetFileIcon(file.Extension);
                        if (icon != null)
                        {
                            var index = 0;
                            var file1 = file;
                            Invoke(new Action(() =>
                            {
                                filesImageList.Images.Add(file1.Extension, icon.ToBitmap());
                                index = filesImageList.Images.IndexOfKey(file1.Extension);
                            }));
                            fileNode = new TreeNode(file.Name, index, index) {Tag = file.FullName};
                        }
                        else
                        {
                            fileNode = new TreeNode(file.Name, 1, 1) {Tag = file.FullName};
                        }
                    }

                    Invoke(new Action(() => directoryNode.Nodes.Add(fileNode)));
                }
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while adding a directory recursively.", ex,
                                PopupButtons.Ok)));
                _nodeInitializingFailed = true;
                directoryNode = null;
            }

            return directoryNode;
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode == null)
                return;

            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() != DialogResult.OK)
                    return;
                if (filesDataTreeView.SelectedNode == null)
                    return;

                ThreadPool.QueueUserWorkItem(arg => ListDirectoryContent(folderDialog.SelectedPath));
            }
        }

        private void addFilesButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode == null)
                return;

            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.SupportMultiDottedExtensions = true;
                fileDialog.Multiselect = true;
                fileDialog.Filter = "All Files (*.*)| *.*";

                if (fileDialog.ShowDialog() != DialogResult.OK) return;
                foreach (var fileName in fileDialog.FileNames)
                {
                    TreeNode fileNode;
                    var fileInfo = new FileInfo(fileName);
                    if (filesImageList.Images.ContainsKey(fileInfo.Extension))
                    {
                        var index = filesImageList.Images.IndexOfKey(fileInfo.Extension);
                        fileNode = new TreeNode(fileInfo.Name, index, index) {Tag = fileInfo.FullName};
                    }
                    else
                    {
                        var icon = IconReader.GetFileIcon(fileInfo.Extension);
                        if (icon != null)
                        {
                            filesImageList.Images.Add(fileInfo.Extension, icon.ToBitmap());
                            var index = filesImageList.Images.IndexOfKey(fileInfo.Extension);
                            fileNode = new TreeNode(fileInfo.Name, index, index) {Tag = fileInfo.FullName};
                        }
                        else
                        {
                            fileNode = new TreeNode(fileInfo.Name, 1, 1) {Tag = fileInfo.FullName};
                        }
                    }

                    filesDataTreeView.SelectedNode.Nodes.Add(fileNode);
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
            if (
                unsupportedMajorNumericUpDown.Value == 0 && unsupportedMinorNumericUpDown.Value == 0 &&
                unsupportedBuildNumericUpDown.Value == 0 && unsupportedRevisionNumericUpDown.Value == 0)
            {
                Popup.ShowPopup(this, SystemIcons.Warning, "Invalid version.",
                    "You can't add version \"0.0.0.0\" to the unsupported versions. Please specify a minimum version of \"0.1.0.0\"",
                    PopupButtons.Ok);
                return;
            }

            var version = new UpdateVersion((int) unsupportedMajorNumericUpDown.Value,
                (int) unsupportedMinorNumericUpDown.Value, (int) unsupportedBuildNumericUpDown.Value,
                (int) unsupportedRevisionNumericUpDown.Value);
            _unsupportedVersionLiteralsBindingList.Add(version.ToString());
        }

        private void removeVersionButton_Click(object sender, EventArgs e)
        {
            _unsupportedVersionLiteralsBindingList.Remove(unsupportedVersionsListBox.SelectedItem.ToString());
        }

        private void developmentalStageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _developmentalStage =
                (DevelopmentalStage)
                    Enum.Parse(typeof (DevelopmentalStage),
                        developmentalStageComboBox.GetItemText(developmentalStageComboBox.SelectedItem));
            if (_developmentalStage == DevelopmentalStage.Alpha || _developmentalStage == DevelopmentalStage.Beta)
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
                var aimPage = changelogContentTabControl.TabPages.Cast<TabPage>()
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
            if (categoryTreeView.SelectedNode == null) 
                return;
            if ((e.KeyCode != Keys.Delete && e.KeyCode != Keys.Back) || categoryTreeView.SelectedNode.Parent == null ||
                categoryTreeView.SelectedNode.Text == "Replace file/folder") 
                return;
            categoryTabControl.TabPages.Remove(
                categoryTabControl.TabPages[4 + categoryTreeView.SelectedNode.Index]);
            categoryTreeView.SelectedNode.Remove();
        }

        private void categoryTreeView_DragDrop(object sender, DragEventArgs e)
        {
            var nodeToDropIn = categoryTreeView.GetNodeAt(categoryTreeView.PointToClient(new Point(e.X, e.Y)));
            if (nodeToDropIn == null || nodeToDropIn.Index != 3) // Operations-node
                return;

            var data = e.Data.GetData(typeof (string));
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

                case "CreateRegistrySubKey":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _createRegistrySubKeyNode.Clone());

                    var createRegistrySubKeyPage = new TabPage("Create registry subkey")
                    {
                        BackColor = SystemColors.Window
                    };
                    createRegistrySubKeyPage.Controls.Add(new RegistrySubKeyCreateOperationPanel());
                    categoryTabControl.TabPages.Add(createRegistrySubKeyPage);
                    break;

                case "DeleteRegistrySubKey":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _deleteRegistrySubKeyNode.Clone());

                    var deleteRegistrySubKeyPage = new TabPage("Delete registry subkey")
                    {
                        BackColor = SystemColors.Window
                    };
                    deleteRegistrySubKeyPage.Controls.Add(new RegistrySubKeyDeleteOperationPanel());
                    categoryTabControl.TabPages.Add(deleteRegistrySubKeyPage);
                    break;

                case "SetRegistryValue":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _setRegistryValueNode.Clone());

                    var setRegistryValuePage = new TabPage("Set registry value") {BackColor = SystemColors.Window};
                    setRegistryValuePage.Controls.Add(new RegistrySetValueOperationPanel());
                    categoryTabControl.TabPages.Add(setRegistryValuePage);
                    break;

                case "DeleteRegistryValue":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode) _deleteRegistryValueNode.Clone());

                    var deleteRegistryValuePage = new TabPage("Delete registry value") {BackColor = SystemColors.Window};
                    deleteRegistryValuePage.Controls.Add(new RegistryDeleteValueOperationPanel());
                    categoryTabControl.TabPages.Add(deleteRegistryValuePage);
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
            if (!_packageUploaded)
                _packageUploaded = true;

            Reset();
        }

        private void bulletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = changelogContentTabControl.SelectedTab;
            if (page.Text != "English")
            {
                var panel = (ChangelogPanel) page.Controls[0];
                panel.Paste("•");
            }
            else
            {
                englishChangelogTextBox.Paste("•");
            }
        }

        private void insideQuotationMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = changelogContentTabControl.SelectedTab;
            if (page.Text != "English")
            {
                var panel = (ChangelogPanel) page.Controls[0];
                panel.Paste("» «");
            }
            else
            {
                englishChangelogTextBox.Paste("» «");
            }
        }

        private void classicQuotationMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = changelogContentTabControl.SelectedTab;
            if (page.Text != "English")
            {
                var panel = (ChangelogPanel) page.Controls[0];
                panel.Paste("„ “");
            }
            else
            {
                englishChangelogTextBox.Paste("„  “");
            }
        }

        private void outsideQuotationMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = changelogContentTabControl.SelectedTab;
            if (page.Text != "English")
            {
                var panel = (ChangelogPanel) page.Controls[0];
                panel.Paste("« »");
            }
            else
            {
                englishChangelogTextBox.Paste("« »");
            }
        }

        private void apostropheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = changelogContentTabControl.SelectedTab;
            if (page.Text != "English")
            {
                var panel = (ChangelogPanel) page.Controls[0];
                panel.Paste("'");
            }
            else
            {
                englishChangelogTextBox.Paste("'");
            }
        }

        private void copyrightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = changelogContentTabControl.SelectedTab;
            if (page.Text != "English")
            {
                var panel = (ChangelogPanel) page.Controls[0];
                panel.Paste("©");
            }
            else
            {
                englishChangelogTextBox.Paste("©");
            }
        }

        private void allRightsReservedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = changelogContentTabControl.SelectedTab;
            if (page.Text != "English")
            {
                var panel = (ChangelogPanel) page.Controls[0];
                panel.Paste("®");
            }
            else
            {
                englishChangelogTextBox.Paste("®");
            }
        }

        private void soundRecordingCopyrightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = changelogContentTabControl.SelectedTab;
            if (page.Text != "English")
            {
                var panel = (ChangelogPanel) page.Controls[0];
                panel.Paste("℗");
            }
            else
            {
                englishChangelogTextBox.Paste("℗");
            }
        }

        private void unregisteredTrademarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = changelogContentTabControl.SelectedTab;
            if (page.Text != "English")
            {
                var panel = (ChangelogPanel) page.Controls[0];
                panel.Paste("™");
            }
            else
            {
                englishChangelogTextBox.Paste("™");
            }
        }

        private void serviceMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = changelogContentTabControl.SelectedTab;
            if (page.Text != "English")
            {
                var panel = (ChangelogPanel) page.Controls[0];
                panel.Paste("℠");
            }
            else
            {
                englishChangelogTextBox.Paste("℠");
            }
        }

        private void englishChangelogTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.A)
                englishChangelogTextBox.SelectAll();
            else if (e.Control & e.KeyCode == Keys.Back)
                SendKeys.SendWait("^+{LEFT}{BACKSPACE}");
        }

        private void addExistingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addFolderButton_Click(sender, e);
        }

        private void addVirtualFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode == null)
                return;

            var folderNode = new TreeNode("Folder name", 0, 0);
            filesDataTreeView.SelectedNode.Nodes.Add(folderNode);
            if (!filesDataTreeView.SelectedNode.IsExpanded)
                filesDataTreeView.SelectedNode.Toggle();

            folderNode.BeginEdit();
        }

        private void filesDataTreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Parent == null)
                e.Node.EndEdit(true);
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            var updatingInfoDialog = new UpdatingInfoDialog();
            updatingInfoDialog.ShowDialog();
        }

        #region "Localization"

        //private string configDownloadErrorCaption;
        //private string creatingPackageDataErrorCaption;
        //private string ftpDataLoadErrorCaption;
        //private string gettingUrlErrorCaption;
        //private string initializingArchiveInfoText;
        //private string initializingConfigInfoText;
        //private string invalidArgumentCaption;
        //private string invalidArgumentText;
        //private string invalidServerDirectoryErrorCaption;
        //private string invalidServerDirectoryErrorText;
        //private string invalidVersionCaption;
        //private string invalidVersionText;
        //private string loadingProjectDataErrorCaption;
        //private string noChangelogCaption;
        //private string noChangelogText;
        //private string noFilesCaption;
        //private string noFilesText;
        //private string noNetworkCaption;
        //private string noNetworkText;
        //private string preparingUpdateInfoText;
        //private string readingPackageBytesErrorCaption;
        //private string relativeUriErrorText;
        //private string savingInformationErrorCaption;
        //private string serializingDataErrorCaption;
        //private string signingPackageInfoText;
        //private string unsupportedArchiveCaption;
        //private string unsupportedArchiveText;
        //private string uploadFailedErrorCaption;
        //private string uploadingConfigInfoText;
        //private string uploadingPackageInfoText;

        //private void SetLanguage()
        //{
        //    //string languageFilePath = Path.Combine(Program.LanguagesDirectory,
        //    //    String.Format("{0}.json", Settings.Default.Language.Name));
        //    //var ls = new LocalizationProperties();
        //    //if (File.Exists(languageFilePath))
        //    //    ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
        //    //else
        //    //{
        //    //    string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
        //    //    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        //    //    {
        //    //        ls = Serializer.Deserialize<LocalizationProperties>(stream);
        //    //    }
        //    //}

        //    //noNetworkCaption = ls.PackageAddDialogNoInternetWarningCaption;
        //    //noNetworkText = ls.PackageAddDialogNoInternetWarningText;
        //    //noFilesCaption = ls.PackageAddDialogNoFilesSpecifiedWarningCaption;
        //    //noFilesText = ls.PackageAddDialogNoFilesSpecifiedWarningText;
        //    //unsupportedArchiveCaption = ls.PackageAddDialogUnsupportedArchiveWarningCaption;
        //    //unsupportedArchiveText = ls.PackageAddDialogUnsupportedArchiveWarningText;
        //    //invalidVersionCaption = ls.PackageAddDialogVersionInvalidWarningCaption;
        //    //invalidVersionText = ls.PackageAddDialogVersionInvalidWarningText;
        //    //noChangelogCaption = ls.PackageAddDialogNoChangelogWarningCaption;
        //    //noChangelogText = ls.PackageAddDialogNoChangelogWarningText;
        //    //invalidArgumentCaption = ls.InvalidArgumentErrorCaption;
        //    //invalidArgumentText = ls.InvalidArgumentErrorText;
        //    //creatingPackageDataErrorCaption = ls.PackageAddDialogPackageDataCreationErrorCaption;
        //    //loadingProjectDataErrorCaption = ls.PackageAddDialogProjectDataLoadingErrorCaption;
        //    //gettingUrlErrorCaption = ls.PackageAddDialogGettingUrlErrorCaption;
        //    //readingPackageBytesErrorCaption = ls.PackageAddDialogReadingPackageBytesErrorCaption;
        //    //invalidServerDirectoryErrorCaption = ls.PackageAddDialogInvalidServerDirectoryErrorCaption;
        //    //invalidServerDirectoryErrorText = ls.PackageAddDialogInvalidServerDirectoryErrorText;
        //    //ftpDataLoadErrorCaption = ls.PackageAddDialogLoadingFtpDataErrorCaption;
        //    //configDownloadErrorCaption = ls.PackageAddDialogConfigurationDownloadErrorCaption;
        //    //serializingDataErrorCaption = ls.PackageAddDialogSerializingDataErrorCaption;
        //    //relativeUriErrorText = ls.PackageAddDialogRelativeUriErrorText;
        //    //savingInformationErrorCaption = ls.PackageAddDialogPackageInformationSavingErrorCaption;
        //    //uploadFailedErrorCaption = ls.PackageAddDialogUploadFailedErrorCaption;

        //    //initializingArchiveInfoText = ls.PackageAddDialogArchiveInitializerInfoText;
        //    //preparingUpdateInfoText = ls.PackageAddDialogPrepareInfoText;
        //    //signingPackageInfoText = ls.PackageAddDialogSigningInfoText;
        //    //initializingConfigInfoText = ls.PackageAddDialogConfigInitializerInfoText;
        //    //uploadingPackageInfoText = ls.PackageAddDialogUploadingPackageInfoText;
        //    //uploadingConfigInfoText = ls.PackageAddDialogUploadingConfigInfoText;

        //    //Text = String.Format(ls.PackageAddDialogTitle, Project.Name, ls.ProductTitle);
        //    //cancelButton.Text = ls.CancelButtonText;
        //    //createButton.Text = ls.CreatePackageButtonText;

        //    //devStageLabel.Text = ls.PackageAddDialogDevelopmentalStageLabelText;
        //    //versionLabel.Text = ls.PackageAddDialogVersionLabelText;
        //    //descriptionLabel.Text = ls.PackageAddDialogDescriptionLabelText;
        //    //publishCheckBox.Text = ls.PackageAddDialogPublishCheckBoxText;
        //    //publishInfoLabel.Text = ls.PackageAddDialogPublishInfoLabelText;
        //    //environmentLabel.Text = ls.PackageAddDialogEnvironmentLabelText;
        //    //architectureInfoLabel.Text = ls.PackageAddDialogEnvironmentInfoLabelText;

        //    //changelogLoadButton.Text = ls.PackageAddDialogLoadButtonText;
        //    //changelogClearButton.Text = ls.PackageAddDialogClearButtonText;

        //    //addFilesButton.Text = ls.PackageAddDialogAddFileButtonText;
        //    //removeEntryButton.Text = ls.PackageAddDialogRemoveFileButtonText;
        //    //filesList.Columns[0].Text = ls.PackageAddDialogNameHeaderText;
        //    //filesList.Columns[1].Text = ls.PackageAddDialogSizeHeaderText;

        //    //allVersionsRadioButton.Text = ls.PackageAddDialogAvailableForAllRadioButtonText;
        //    //someVersionsRadioButton.Text = ls.PackageAddDialogAvailableForSomeRadioButtonText;
        //    //allVersionsInfoLabel.Text = ls.PackageAddDialogAvailableForAllInfoText;
        //    //someVersionsInfoLabel.Text = ls.PackageAddDialogAvailableForSomeInfoText;
        //}

        #endregion
    }
}