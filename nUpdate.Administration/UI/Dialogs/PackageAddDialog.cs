// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ionic.Zip;
using MySql.Data.MySqlClient;
using nUpdate.Administration.Application;
using nUpdate.Administration.Ftp;
using nUpdate.Administration.History;
using nUpdate.Administration.Operations.Panels;
using nUpdate.Administration.Properties;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;
using nUpdate.Administration.Win32;
using nUpdate.Operations;
using nUpdate.Updating;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageAddDialog : BaseDialog, IAsyncSupportable, IResettable
    {
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

        private readonly TreeNode _executeScriptNode = new TreeNode("Execute Script", 16, 16) {Tag = "ExecuteScript"};

        private readonly UpdatePackage _package = new UpdatePackage();
        private readonly TreeNode _renameNode = new TreeNode("Rename file", 10, 10) {Tag = "RenameFile"};
        private readonly TreeNode _replaceNode = new TreeNode("Replace file/folder", 11, 11) {Tag = "ReplaceFile"};

        private readonly PackageItem _rootPackageItem = new PackageItem(string.Empty, "%root%", Guid.Empty, true)
        {
            IsRoot = true,
            Children = (new[]
            {
                new PackageItem(string.Empty, "Program", Guid.Empty, true),
                new PackageItem(string.Empty, "AppData", Guid.Empty, true),
                new PackageItem(string.Empty, "Temp", Guid.Empty, true),
                new PackageItem(string.Empty, "Desktop", Guid.Empty, true)
            }.ToList())
        };

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
        private bool _allowCancel = true;
        private int _architectureIndex = 2;
        private Uri _configurationFileUrl;
        private DevelopmentalStage _developmentalStage;
        private FTPManager _ftp;
        private string _hashFile;
        private bool _includeIntoStatistics;
        private MySqlConnection _insertConnection;
        private bool _necessaryUpdate;
        private bool _nodeInitializingFailed;
        private string _packageFolder;
        private bool _packageInitialized;
        private bool _packageUploaded;
        private UpdateVersion _packageVersion;
        private PackageItem _previousDataPackageItem;
        private bool _publishUpdate;
        private string _updateConfigFile;
        private bool _uploadCancelled;

        private List<UpdateRequirement> _updateRequirements;
        private Dictionary<string, Version> _osVersions;

        public PackageAddDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        public void SetUIState(bool enabled)
        {
            _allowCancel = enabled;

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
                    _ftp.DeleteDirectory($"{_ftp.Directory}/{_packageVersion}");
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

            SetUIState(true);
            if (_packageInitialized)
            {
                if (Project.Packages != null)
                    Project.Packages.First(item => item == _package).IsReleased = false;

                try
                {
                    UpdateProject.SaveProject(Project.Path, Project);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving project data.", ex,
                                    PopupButtons.Ok)));
                }
            }

            DialogResult = DialogResult.OK;
        }

        private void PackageAddDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, Project.Name, Program.VersionString);

            try
            {
                _ftp =
                    new FTPManager(Project.FtpHost, Project.FtpPort, Project.FtpDirectory, Project.FtpUsername,
                        Project.RuntimeFtpPassword,
                        Project.Proxy, Project.FtpUsePassiveMode, Project.FtpTransferAssemblyFilePath,
                        Project.FtpProtocol);
                _ftp.ProgressChanged += ProgressChanged;
                _ftp.CancellationFinished += CancellationFinished;
                if (!string.IsNullOrWhiteSpace(Project.FtpTransferAssemblyFilePath))
                    _ftp.TransferAssemblyPath = Project.FtpTransferAssemblyFilePath;
                else
                    _ftp.Protocol = (FtpSecurityProtocol) Project.FtpProtocol;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the FTP-data.", ex, PopupButtons.Ok);
                Close();
                return;
            }

            _zip.ParallelDeflateThreshold = -1;
            _updateLog.Project = Project;
            //SetLanguage();

            categoryTreeView.Nodes[4].Nodes.Add(_replaceNode);
            categoryTreeView.Nodes[4].Toggle();

            unsupportedVersionsListBox.DataSource = _unsupportedVersionLiteralsBindingList;
            var devStages = Enum.GetValues(typeof (DevelopmentalStage));
            Array.Reverse(devStages);
            developmentalStageComboBox.DataSource = devStages;
            var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            foreach (var info in cultureInfos)
            {
                changelogLanguageComboBox.Items.Add($"{info.EnglishName} - {info.Name}");
                _cultures.Add(info);
            }

            changelogContentTabControl.TabPages[0].Tag = _cultures.Where(x => x.Name == "en");
            changelogLanguageComboBox.SelectedIndex = changelogLanguageComboBox.FindStringExact("English - en");

            architectureComboBox.SelectedIndex = 2;
            categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
            developmentalStageComboBox.SelectedIndex = 3;
            unsupportedVersionsPanel.Enabled = false;

            _publishUpdate = publishCheckBox.Checked;
            _necessaryUpdate = necessaryUpdateCheckBox.Checked;
            includeIntoStatisticsCheckBox.Enabled = Project.UseStatistics;

            majorNumericUpDown.Maximum = decimal.MaxValue;
            minorNumericUpDown.Maximum = decimal.MaxValue;
            buildNumericUpDown.Maximum = decimal.MaxValue;
            revisionNumericUpDown.Maximum = decimal.MaxValue;

            if (!string.IsNullOrEmpty(Project.AssemblyVersionPath))
            {
                var projectAssembly = Assembly.GetCallingAssembly();
                var nUpateVersionAttribute =
                    projectAssembly.GetCustomAttributes(false).OfType<nUpdateVersionAttribute>().SingleOrDefault();

                if (nUpateVersionAttribute == null)
                    return;
                var assemblyVersion = new UpdateVersion(nUpateVersionAttribute.VersionString);

                majorNumericUpDown.Value = assemblyVersion.Major;
                minorNumericUpDown.Value = assemblyVersion.Minor;
                buildNumericUpDown.Value = assemblyVersion.Build;
                revisionNumericUpDown.Value = assemblyVersion.Revision;
            }

            generalTabPage.DoubleBuffer();
            changelogTabPage.DoubleBuffer();
            cancelToolTip.SetToolTip(cancelLabel, "Click here to cancel the package upload.");


            _updateRequirements = new List<UpdateRequirement>();
            requiredOSComboBox.SelectedIndex = 0;
            requiredFrameworkComboBox.SelectedIndex = 0;
            requirementsTypeComboBox.SelectedIndex = 0;

            _osVersions = new Dictionary<string, Version>();
            _osVersions.Add("Windows Vista", new Version("6.0.6000.0"));
            _osVersions.Add("Windows Vista Service Pack 1", new Version("6.0.6001.0"));
            _osVersions.Add("Windows Vista Service Pack 2", new Version("6.0.6002.0"));
            _osVersions.Add("Windows 7", new Version("6.1.7600.0"));
            _osVersions.Add("Windows 7 Service Pack 1", new Version("6.1.7601.0"));
            _osVersions.Add("Windows 8", new Version("6.2.9200.0"));
            _osVersions.Add("Windows 8.1", new Version("6.3.9600.0"));
            _osVersions.Add("Windows 10", new Version("10.0.10240.0"));
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
                if (Project.Packages.Select(item => new UpdateVersion(item.Version)).Any(item => item == _packageVersion))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid version set.",
                        $"Version \"{_packageVersion.FullText}\" is already existing.", PopupButtons.Ok);
                    generalPanel.BringToFront();
                    categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
                    return;
                }
            }

            if (string.IsNullOrEmpty(englishChangelogTextBox.Text))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No changelog set.",
                    "Please specify a changelog for the package. If you have already set a changelog in another language you still need to specify one for \"English - en\" to support client's that don't use your specified culture on their computer.",
                    PopupButtons.Ok);
                changelogPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[1];
                return;
            }

            if (!filesDataTreeView.Nodes.Cast<TreeNode>().Any(node => node.Nodes.Count > 0) &&
                categoryTreeView.Nodes[3].Nodes.Count <= 1)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No files and/or folders or operations set.",
                    "Please specify some files and/or folders that should be included or operations into the package.",
                    PopupButtons.Ok);
                filesPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[3].Nodes[0];
                return;
            }

            foreach (
                var tabPage in
                    from tabPage in categoryTabControl.TabPages.Cast<TabPage>().Where(item => item.TabIndex > 4)
                    let operationPanel = tabPage.Controls[0] as IOperationPanel
                    where operationPanel != null && !operationPanel.IsValid
                    select tabPage)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "An added operation isn't valid.",
                    "Please make sure to fill out all required fields correctly.",
                    PopupButtons.Ok);
                categoryTreeView.SelectedNode =
                    categoryTreeView.Nodes[3].Nodes.Cast<TreeNode>()
                        .First(item => item.Index == tabPage.TabIndex - 4);
                return;
            }

            SetUIState(false);

            loadingPanel.Location = new Point(180, 91);
            loadingPanel.BringToFront();
            loadingPanel.Visible = true;

            InitializePackage();
        }

        private void InitializeArchiveContents(TreeNode treeNode, string currentDirectory, PackageItem currentItem)
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
                    var tmpDir = $"{currentDirectory}/{node.Text}";
                    try
                    {
                        _zip.AddDirectoryByName(tmpDir);
                        var packageItem = new PackageItem(string.Empty, node.Text, currentItem.Guid, true);
                        currentItem.Children.Add(packageItem);
                        InitializeArchiveContents(node, tmpDir, packageItem);
                    }
                    catch (ArgumentException)
                    {
                        var nodePlaceHolder = node;
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Information, "The element was removed.",
                                        $"The file/folder \"{nodePlaceHolder.Text}\" was removed from the collection because it is already existing in the current directory.", PopupButtons.Ok)));
                    }
                }
                else
                {
                    try
                    {
                        string hash = BitConverter.ToString(SHAManager.HashFile(node.Tag.ToString()))
                            .Replace("-", string.Empty);
                        string previousHash = null;
                        bool shouldNotBeAdded = false;
                        if (_previousDataPackageItem != null &&
                            (differentialUpdateCheckBox.Checked || deltaPatchingCheckBox.Checked))
                        {
                            var node1 = node;
                            var relatingItems = SelectRecursive(_previousDataPackageItem,
                                packageItem =>
                                    (from child in packageItem.Children
                                        where packageItem.Name == node1.Text
                                        select packageItem).ToList()).Where(item => CheckHierarchy(node1, item));

                            foreach (var previousItem in relatingItems)
                            {
                                previousHash = previousItem.Hash;
                            }

                            shouldNotBeAdded = (previousHash != null && hash == previousHash);
                        }

                        if (!shouldNotBeAdded)
                            _zip.AddFile(node.Tag.ToString(), currentDirectory);
                        currentItem.Children.Add(new PackageItem(hash, node.Text, currentItem.Guid, false));
                    }
                    catch (ArgumentException)
                    {
                        var nodePlaceHolder = node;
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Information, "The element was removed.",
                                        $"The file/folder \"{nodePlaceHolder.Text}\" was removed from the collection because it is already existing in the current directory.", PopupButtons.Ok)));
                    }
                }
            }
        }

        public static IEnumerable<PackageItem> SelectRecursive(PackageItem packageItem,
            Func<PackageItem, IEnumerable<PackageItem>> selector)
        {
            var stillToProcess = new Queue<PackageItem>(packageItem.Children);
            while (stillToProcess.Count > 0)
            {
                PackageItem item = stillToProcess.Dequeue();
                yield return item;
                foreach (var child in selector(item))
                {
                    stillToProcess.Enqueue(child);
                }
            }
        }

        private bool CheckHierarchy(TreeNode node, PackageItem item)
        {
            TreeNode[] currentNode = {node};
            var currentItem = item;

            var nodeTextHierarchy = new Stack<string>();
            var rootNodes = filesDataTreeView.Nodes.Cast<TreeNode>().Where(x => x.Parent == null).ToList();
            while (rootNodes.All(x => x != currentNode[0]))
            {
                currentNode[0] = currentNode[0].Parent;
                nodeTextHierarchy.Push(currentNode[0].Text);
            }

            var packageItemParentHierarchies = new Stack<string>();
            while (!currentItem.IsRoot)
            {
                var item1 = currentItem;
                var relatingItems = SelectRecursive(currentItem,
                    packageItem =>
                        (from child in packageItem.Children
                            where packageItem.Guid == item1.ParentGuid
                            select packageItem).ToList());
                currentItem = relatingItems.First();
                if (!currentItem.IsRoot)
                    packageItemParentHierarchies.Push(currentItem.Name);
            }

            return nodeTextHierarchy.SequenceEqual(packageItemParentHierarchies);
        }

        /// <summary>
        ///     Initializes the package.
        /// </summary>
        private async void InitializePackage()
        {
            await (Task.Factory.StartNew(() =>
            {
                if (!Project.UpdateUrl.EndsWith("/"))
                    Project.UpdateUrl += "/";

                _configurationFileUrl = UriConnector.ConnectUri(Project.UpdateUrl, "updates.json");
                _packageFolder = Path.Combine(Program.Path, "Projects", Project.Name, _packageVersion.ToString());
                _updateConfigFile = Path.Combine(Program.Path, "Projects", Project.Name, _packageVersion.ToString(),
                    "updates.json");
                _hashFile = Path.Combine(Program.Path, "Projects", Project.Name, _packageVersion.ToString(),
                    "hashes.json");

                if (differentialUpdateCheckBox.Checked || deltaPatchingCheckBox.Checked) // TODO: Variable
                {
                    var previousVersion =
                        UpdateVersion.GetHighestUpdateVersion(
                            Project.Packages.Select(item => new UpdateVersion(item.Version)));
                    string previousPackagePath = Path.Combine(Program.Path, "Projects", Project.Name,
                        previousVersion.ToString(), "hashes.json");
                    if (File.Exists(previousPackagePath))
                    {
                        Invoke(new Action(() => loadingLabel.Text = "Loading previous hash data..."));

                        try
                        {
                            _previousDataPackageItem =
                                Serializer.Deserialize<PackageItem>(File.ReadAllText(previousPackagePath));
                        }
                        catch (Exception ex)
                        {
                            Invoke(
                                new Action(
                                    () =>
                                        Popup.ShowPopup(this, SystemIcons.Error,
                                            "Error while loading the previous hashes.", ex,
                                            PopupButtons.Ok)));
                            Reset();
                            return;
                        }
                    }
                    else
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Information,
                                        "Previous hash file not existing.",
                                        "The file containing the hash data of the previous version doesn't exist and differential updates/delta patching won't work for this package. This can be a result of upgrading to a newer version of nUpdate Administration. However, a hash file for this package will be generated and available for the next package.",
                                        PopupButtons.Ok)));
                    }
                }

                Invoke(new Action(() => loadingLabel.Text = "Initializing archive..."));

                try
                {
                    Directory.CreateDirectory(_packageFolder); // Create the content folder
                    using (File.Create(_updateConfigFile))
                    {
                    }

                    using (File.Create(_hashFile))
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

                InitializeArchiveContents(filesDataTreeView.Nodes[0], "Program", _rootPackageItem.Children[0]);
                InitializeArchiveContents(filesDataTreeView.Nodes[1], "AppData", _rootPackageItem.Children[1]);
                InitializeArchiveContents(filesDataTreeView.Nodes[2], "Temp", _rootPackageItem.Children[2]);
                InitializeArchiveContents(filesDataTreeView.Nodes[3], "Desktop", _rootPackageItem.Children[3]);

                Invoke(new Action(() => loadingLabel.Text = "Initializing hash file..."));

                try
                {
                    File.WriteAllText(_hashFile, Serializer.SerializeWithIgnoringLoops(_rootPackageItem));
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the file for the hashes.",
                                    ex,
                                    PopupButtons.Ok)));
                    Reset();
                    return;
                }

                var packageFile = $"{Project.Guid}.zip";
                _zip.Save(Path.Combine(_packageFolder, packageFile));

                _updateLog.Write(LogEntry.Create, _packageVersion.FullText);
                Invoke(new Action(() => loadingLabel.Text = "Preparing update..."));

                string[] unsupportedVersionLiterals = null;

                if (unsupportedVersionsListBox.Items.Count == 0)
                    allVersionsRadioButton.Checked = true;
                else if (unsupportedVersionsListBox.Items.Count > 0 && someVersionsRadioButton.Checked)
                {
                    unsupportedVersionLiterals = _unsupportedVersionLiteralsBindingList.ToArray();
                }

                var changelog = new Dictionary<CultureInfo, string>
                {
                    {new CultureInfo("en"), englishChangelogTextBox.Text}
                };
                foreach (
                    var tabPage in
                        changelogContentTabControl.TabPages.Cast<TabPage>().Where(tabPage => tabPage.Text != "English"))
                {
                    var panel = (ChangelogPanel) tabPage.Controls[0];
                    if (string.IsNullOrEmpty(panel.Changelog)) continue;
                    changelog.Add((CultureInfo) tabPage.Tag, panel.Changelog);
                }

                // Create a new package configuration
                _configuration.Changelog = changelog;
                _configuration.NecessaryUpdate = _necessaryUpdate;
                _configuration.Architecture = (Architecture) _architectureIndex;
                _configuration.UpdateRequirements = _updateRequirements;

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
                        new BinaryReader(File.Open(Path.Combine(_packageFolder, $"{Project.Guid}.zip"),
                            FileMode.Open)))
                    {
                        data = reader.ReadBytes((int) reader.BaseStream.Length);
                    }
                    _configuration.Signature = Convert.ToBase64String(new RsaManager(Project.PrivateKey).SignData(data));
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
                _configuration.UpdatePhpFileUri = UriConnector.ConnectUri(Project.UpdateUrl, "statistics.php");
                _configuration.UpdatePackageUri = UriConnector.ConnectUri(Project.UpdateUrl,
                    $"{_packageVersion}/{Project.Guid}.zip");
                _configuration.LiteralVersion = _packageVersion.ToString();
                _configuration.UseStatistics = _includeIntoStatistics;

                if (Project.UseStatistics)
                {
                    Settings.Default.VersionID += 1;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                    _configuration.VersionId = Settings.Default.VersionID;
                }

                Invoke(new Action(() => loadingLabel.Text = "Initializing configuration..."));

                List<UpdateConfiguration> configurationList;

                // Load the configuration
                try
                {
                    var configurationEnumerable = UpdateConfiguration.Download(_configurationFileUrl, Project.Proxy);
                    configurationList = configurationEnumerable?.ToList() ?? Enumerable.Empty<UpdateConfiguration>().ToList();
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the old configuration.",
                                    ex,
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

                Invoke(
                    new Action(
                        () =>
                            _package.Description = descriptionTextBox.Text));

                _package.IsReleased = _publishUpdate;
                _package.Version = _packageVersion.ToString();
                _packageInitialized = true;

                if (Project.Packages == null)
                    Project.Packages = new List<UpdatePackage>();
                Project.Packages.Add(_package);

                if (_publishUpdate)
                {
                    if (Project.UseStatistics)
                    {
                        Invoke(new Action(() => loadingLabel.Text = "Connecting to SQL-server..."));
                        try
                        {
                            var connectionString = $"SERVER={Project.SqlWebUrl};" +
                                                   $"DATABASE={Project.SqlDatabaseName};" +
                                                   $"UID={Project.SqlUsername};" +
                                                   $"PASSWORD={Project.RuntimeSqlPassword.ConvertToInsecureString()};";

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
                                        Popup.ShowPopup(this, SystemIcons.Error,
                                            "Error while connecting to the database.",
                                            ex, PopupButtons.Ok)));
                            _insertConnection.Close();
                            Reset();
                            return;
                        }

                        Invoke(new Action(() => loadingLabel.Text = "Executing SQL-commands..."));

                        var command = _insertConnection.CreateCommand();
                        command.CommandText =
                            $"INSERT INTO `Version` (`ID`, `Version`, `Application_ID`) VALUES ({Settings.Default.VersionID}, \"{_packageVersion}\", {Project.ApplicationId});";
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
                        loadingLabel.Text = $"Uploading package... {"0%"}";
                        cancelLabel.Visible = true;
                    }));

                    try
                    {
                        _ftp.UploadPackage(
                            Path.Combine(Program.Path, "Projects", Project.Name, _packageVersion.ToString(),
                                $"{Project.Guid}.zip"), _packageVersion.ToString());
                    }
                    catch (Exception ex) // Upload-method is async, it's true, but directory creation can fail.
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error,
                                        "Error while creating the package directory.",
                                        ex, PopupButtons.Ok)));
                        Reset();
                        return;
                    }

                    if (_uploadCancelled)
                    {
                        Reset();
                        return;
                    }

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
                        _updateLog.Write(LogEntry.Upload, _packageVersion.FullText);
                    }
                    catch (Exception ex)
                    {
                        Reset();
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the configuration.",
                                        ex, PopupButtons.Ok)));
                    }
                }

                try
                {
                    UpdateProject.SaveProject(Project.Path, Project);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the project data.", ex,
                                    PopupButtons.Ok)));
                    Reset();
                }

                SetUIState(true);
                DialogResult = DialogResult.OK;
            }));
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
                            $"Uploading package... {Math.Round(e.Percentage, 1)}% | {e.BytesPerSecond/1024}KB/s"));
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

        private void ListDirectoryContent(string path, bool onlyContent)
        {
            BeginInvoke(new Action(() =>
            {
                if (!onlyContent)
                {
                    var folderNode = new TreeNode(new DirectoryInfo(path).Name, 0, 0);
                    filesDataTreeView.SelectedNode.Nodes.Add(folderNode);
                    filesDataTreeView.SelectedNode = folderNode;
                }

                var rootDirectoryInfo = new DirectoryInfo(path);
                CreateFilesNode(rootDirectoryInfo, filesDataTreeView.SelectedNode);
                foreach (
                    var node in
                        rootDirectoryInfo.GetDirectories().Select(CreateDirectoryNode).Where(node => node != null))
                {
                    filesDataTreeView.SelectedNode.Nodes.Add(node);
                }

                if (!filesDataTreeView.SelectedNode.IsExpanded)
                    filesDataTreeView.SelectedNode.Toggle();
            }));
        }

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

                CreateFilesNode(directoryInfo, directoryNode);
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

        private void CreateFilesNode(DirectoryInfo directoryInfo, TreeNode directoryNode)
        {
            foreach (var file in directoryInfo.GetFiles())
            {
                if (_nodeInitializingFailed)
                {
                    _nodeInitializingFailed = false;
                    break;
                }

                TreeNode fileNode;
                if (string.IsNullOrWhiteSpace(file.Extension))
                    fileNode = new TreeNode(file.Name, 1, 1) {Tag = file.FullName};
                else
                {
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
                }

                var node = directoryNode;
                Invoke(new Action(() => node.Nodes.Add(fileNode)));
            }
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            InitializeDirectoryListing(false);
        }

        private void InitializeDirectoryListing(bool onlyContent)
        {
            if (filesDataTreeView.SelectedNode == null)
                return;

            string selectedPath;
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() != DialogResult.OK)
                    return;

                selectedPath = folderDialog.SelectedPath;
            }

            TaskEx.Run(() => ListDirectoryContent(selectedPath, onlyContent));
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
            if (filesDataTreeView.SelectedNode?.Parent != null)
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
            if (_developmentalStage == DevelopmentalStage.Alpha || _developmentalStage == DevelopmentalStage.Beta ||
                _developmentalStage == DevelopmentalStage.ReleaseCandidate)
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
            _necessaryUpdate = necessaryUpdateCheckBox.Checked;
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
                        categoryTabControl.SelectedTab = requirementsTabPage;
                        break;
                    case 4:
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
                            categoryTabControl.TabPages[5 + categoryTreeView.SelectedNode.Index];
                        break;
                }
            }
        }

        private void categoryTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (categoryTreeView.SelectedNode == null)
                return;

            if (e.Control && e.KeyCode == Keys.Up)
            {
                if (categoryTreeView.SelectedNode.Text != "Replace file/folder" &&
                    categoryTreeView.SelectedNode.Index != 1)
                    categoryTreeView.SelectedNode.MoveUp();
            }
            else if (e.Control && e.KeyCode == Keys.Down)
            {
                if (categoryTreeView.SelectedNode.Text != "Replace file/folder")
                    categoryTreeView.SelectedNode.MoveDown();
            }

            if ((e.KeyCode != Keys.Delete && e.KeyCode != Keys.Back) || categoryTreeView.SelectedNode.Parent == null ||
                categoryTreeView.SelectedNode.Text == "Replace file/folder")
                return;
            categoryTabControl.TabPages.Remove(
                categoryTabControl.TabPages[5 + categoryTreeView.SelectedNode.Index]);
            categoryTreeView.SelectedNode.Remove();
        }

        private void categoryTreeView_DragDrop(object sender, DragEventArgs e)
        {
            var nodeToDropIn = categoryTreeView.GetNodeAt(categoryTreeView.PointToClient(new Point(e.X, e.Y)));
            if (nodeToDropIn == null || nodeToDropIn.Index != 4) // Operations-node
                return;

            var data = e.Data.GetData(typeof (string));
            if (data == null)
                return;

            switch (data.ToString())
            {
                case "DeleteFile":
                    categoryTreeView.Nodes[4].Nodes.Add((TreeNode) _deleteNode.Clone());

                    var deletePage = new TabPage("Delete file") {BackColor = SystemColors.Window};
                    deletePage.Controls.Add(new FileDeleteOperationPanel());
                    categoryTabControl.TabPages.Add(deletePage);
                    break;

                case "RenameFile":
                    categoryTreeView.Nodes[4].Nodes.Add((TreeNode) _renameNode.Clone());

                    var renamePage = new TabPage("Rename file") {BackColor = SystemColors.Window};
                    renamePage.Controls.Add(new FileRenameOperationPanel());
                    categoryTabControl.TabPages.Add(renamePage);
                    break;

                case "CreateRegistrySubKey":
                    categoryTreeView.Nodes[4].Nodes.Add((TreeNode) _createRegistrySubKeyNode.Clone());

                    var createRegistrySubKeyPage = new TabPage("Create registry subkey")
                    {
                        BackColor = SystemColors.Window
                    };
                    createRegistrySubKeyPage.Controls.Add(new RegistrySubKeyCreateOperationPanel());
                    categoryTabControl.TabPages.Add(createRegistrySubKeyPage);
                    break;

                case "DeleteRegistrySubKey":
                    categoryTreeView.Nodes[4].Nodes.Add((TreeNode) _deleteRegistrySubKeyNode.Clone());

                    var deleteRegistrySubKeyPage = new TabPage("Delete registry subkey")
                    {
                        BackColor = SystemColors.Window
                    };
                    deleteRegistrySubKeyPage.Controls.Add(new RegistrySubKeyDeleteOperationPanel());
                    categoryTabControl.TabPages.Add(deleteRegistrySubKeyPage);
                    break;

                case "SetRegistryValue":
                    categoryTreeView.Nodes[4].Nodes.Add((TreeNode) _setRegistryValueNode.Clone());

                    var setRegistryValuePage = new TabPage("Set registry value") {BackColor = SystemColors.Window};
                    setRegistryValuePage.Controls.Add(new RegistrySetValueOperationPanel());
                    categoryTabControl.TabPages.Add(setRegistryValuePage);
                    break;

                case "DeleteRegistryValue":
                    categoryTreeView.Nodes[4].Nodes.Add((TreeNode) _deleteRegistryValueNode.Clone());

                    var deleteRegistryValuePage = new TabPage("Delete registry value") {BackColor = SystemColors.Window};
                    deleteRegistryValuePage.Controls.Add(new RegistryDeleteValueOperationPanel());
                    categoryTabControl.TabPages.Add(deleteRegistryValuePage);
                    break;

                case "StartProcess":
                    categoryTreeView.Nodes[4].Nodes.Add((TreeNode) _startProcessNode.Clone());

                    var startProcessPage = new TabPage("Start process") {BackColor = SystemColors.Window};
                    startProcessPage.Controls.Add(new ProcessStartOperationPanel());
                    categoryTabControl.TabPages.Add(startProcessPage);
                    break;
                case "TerminateProcess":
                    categoryTreeView.Nodes[4].Nodes.Add((TreeNode) _terminateProcessNode.Clone());

                    var terminateProcessPage = new TabPage("Terminate process") {BackColor = SystemColors.Window};
                    terminateProcessPage.Controls.Add(new ProcessStopOperationPanel());
                    categoryTabControl.TabPages.Add(terminateProcessPage);
                    break;
                case "StartService":
                    categoryTreeView.Nodes[4].Nodes.Add((TreeNode) _startServiceNode.Clone());

                    var startServicePage = new TabPage("Start service") {BackColor = SystemColors.Window};
                    startServicePage.Controls.Add(new ServiceStartOperationPanel());
                    categoryTabControl.TabPages.Add(startServicePage);
                    break;
                case "StopService":
                    categoryTreeView.Nodes[4].Nodes.Add((TreeNode) _stopServiceNode.Clone());

                    var stopServicePage = new TabPage("Stop service") {BackColor = SystemColors.Window};
                    stopServicePage.Controls.Add(new ServiceStopOperationPanel());
                    categoryTabControl.TabPages.Add(stopServicePage);
                    break;
                case "ExecuteScript":
                    categoryTreeView.Nodes[4].Nodes.Add((TreeNode) _executeScriptNode.Clone());

                    var executeScriptPage = new TabPage("Stop service") {BackColor = SystemColors.Window};
                    executeScriptPage.Controls.Add(new ScriptExecuteOperationPanel());
                    categoryTabControl.TabPages.Add(executeScriptPage);
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

        private void addFolderContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitializeDirectoryListing(true);
        }

        private void filesDataTreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Parent == null || e.Node.ImageIndex != 0)
                e.Node.EndEdit(true);
        }

        private void PackageAddDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            var updatingInfoDialog = new UpdatingInfoDialog();
            updatingInfoDialog.ShowDialog();
        }

        private void requirementsTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            requirementsTypeTabControl.SelectedIndex = requirementsTypeComboBox.SelectedIndex;
        }

        private void addRequirementButton_Click(object sender, EventArgs e)
        {
            UpdateRequirement _updateRequirement = null;
            switch (requirementsTypeComboBox.SelectedIndex)
            {
                case 0:
                    _updateRequirement = new UpdateRequirement(
                        UpdateRequirement.RequirementType.OSVersion,
                        _osVersions[requiredOSComboBox.Text]);
                    break;

                case 1:
                    _updateRequirement = new UpdateRequirement(
                            UpdateRequirement.RequirementType.DotNetFramework,
                            Version.Parse(requiredFrameworkComboBox.Text.Replace(".NET Framework ", "")));
                    break;
                default:
                    _updateRequirement = null;
                    break;
            }
            if (_updateRequirement != null)
            {
                _updateRequirements.Add(_updateRequirement);
                requirementsListBox.Items.Add(_updateRequirement);
            }
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
        //    //    string resourceName = "nUpdate.Administration.Localization.en.xml";
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