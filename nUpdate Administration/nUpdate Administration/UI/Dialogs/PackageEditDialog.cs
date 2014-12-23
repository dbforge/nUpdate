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
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Core.Update.Operations;
using nUpdate.Administration.Core.Update.Operations.Panels;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;
using Starksoft.Net.Ftp;
using System.Security;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageEditDialog : BaseDialog, IAsyncSupportable, IResettable
    {
        private string _newPackageDirectory;
        private string _oldPackageDirectoryPath;
        private string _existingVersion;
        private readonly List<CultureInfo> _cultures = new List<CultureInfo>();
        private readonly TreeNode _deleteNode = new TreeNode("Delete file", 9, 9) { Tag = "DeleteFile" };
        private readonly TreeNode _renameNode = new TreeNode("Rename file", 10, 10) { Tag = "RenameFile" };
        private readonly TreeNode _createRegistryEntryNode = new TreeNode("Create registry entry", 14, 14) { Tag = "CreateRegistryEntry" };
        private readonly TreeNode _deleteRegistryEntryNode = new TreeNode("Delete registry entry", 12, 12) { Tag = "DeleteRegistryEntry" };
        private readonly TreeNode _setRegistryKeyValueNode = new TreeNode("Set registry key value", 13, 13) { Tag = "SetRegistryKeyValue" };
        private readonly TreeNode _startProcessNode = new TreeNode("Start process", 8, 8) { Tag = "StartProcess"};
        private readonly TreeNode _terminateProcessNode = new TreeNode("Terminate process", 7, 7)
          {Tag = "StopProcess"};
        private readonly TreeNode _startServiceNode = new TreeNode("Start service", 5, 5) { Tag = "StartService"};
        private readonly TreeNode _stopServiceNode = new TreeNode("Stop service", 6, 6) { Tag = "StopService" };
        private bool _allowCancel = true;
        private bool _configurationUploaded;
        private List<UpdateConfiguration> _newConfiguration = new List<UpdateConfiguration>(); 
        private UpdateConfiguration _packageConfiguration;
        private readonly BindingList<string> _unsupportedVersionLiteralsBindingList = new BindingList<string>(); 
        private readonly FtpManager _ftp = new FtpManager();

        public PackageEditDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The configurations available in the file.
        /// </summary>
        public List<UpdateConfiguration> UpdateConfiguration { get; set; }

        /// <summary>
        ///     Gets or sets if the package is released.
        /// </summary>
        public bool IsReleased { get; set; }

        /// <summary>
        ///     The version of the package to edit.
        /// </summary>
        public UpdateVersion PackageVersion { get; set; }

        /// <summary>
        ///     The url of the configuration file.
        /// </summary>
        public Uri ConfigurationFileUrl { get; set; }

        /// <summary>
        ///     The FTP-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public SecureString FtpPassword = new SecureString();

        /// <summary>
        ///     The MySQL-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public SecureString SqlPassword = new SecureString();

        /// <summary>
        ///     The proxy-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public SecureString ProxyPassword = new SecureString();

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
                _ftp.Protocol = (FtpSecurityProtocol)Project.FtpProtocol;
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
        ///     Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        public void SetUiState(bool enabled)
        {
            if (enabled)
                _allowCancel = true;

            BeginInvoke(new Action(() =>
            {
                foreach (var c in from Control c in Controls where c.Visible select c)
                {
                    c.Enabled = enabled;
                }

                loadingPanel.Visible = !enabled;
            }));
        }

        public void Reset()
        {
            Invoke(new Action(() => loadingLabel.Text = "Undoing package upload..."));
            try
            {
                Directory.Move(_newPackageDirectory, _oldPackageDirectoryPath);
                if (_configurationUploaded)
                {
                    string configurationFilePath = Path.Combine(_newPackageDirectory, "updates.json");
                    File.WriteAllText(configurationFilePath, Serializer.Serialize(UpdateConfiguration));
                    _ftp.UploadFile(configurationFilePath);
                    _configurationUploaded = false;
                }
            }
            catch (Exception undoException)
            {
                Invoke(new Action(() =>
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while undoing the changes.", undoException,
                        PopupButtons.Ok);
                    Close();
                }));
            }
        }

        private void PackageEditDialog_Load(object sender, EventArgs e)
        {
            if (!InitializeFtpData())
            {
                Close();
                return;
            }

            if (UpdateConfiguration == null)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the configuration.",
                    "There are no entries available in the configuration.",
                    PopupButtons.Ok);
                Close();
                return;
            }

            try
            {
                _packageConfiguration =
                    UpdateConfiguration.First(item => item.LiteralVersion == PackageVersion.ToString()).DeepCopy();
            }
            catch (InvalidOperationException)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the configuration.",
                    "There are no entries available for the current package in the configuration.",
                    PopupButtons.Ok);
                Close();
                return;
            }

            var packageVersion = new UpdateVersion(_packageConfiguration.LiteralVersion);
            majorNumericUpDown.Maximum = Decimal.MaxValue;
            minorNumericUpDown.Maximum = Decimal.MaxValue;
            buildNumericUpDown.Maximum = Decimal.MaxValue;
            revisionNumericUpDown.Maximum = Decimal.MaxValue;

            majorNumericUpDown.Value = packageVersion.Major;
            minorNumericUpDown.Value = packageVersion.Minor;
            buildNumericUpDown.Value = packageVersion.Build;
            revisionNumericUpDown.Value = packageVersion.Revision;

            _existingVersion = _packageConfiguration.LiteralVersion;

            developmentalStageComboBox.SelectedValue = packageVersion.DevelopmentalStage;
            developmentBuildNumericUpDown.Value = (packageVersion.DevelopmentBuild > 0)
                ? packageVersion.DevelopmentBuild
                : 1;
            developmentBuildNumericUpDown.Enabled = (packageVersion.DevelopmentalStage != DevelopmentalStage.Release);
            mustUpdateCheckBox.Checked = _packageConfiguration.MustUpdate;
            foreach (UpdatePackage package in Project.Packages.Where(package => package.Version == packageVersion))
            {
                descriptionTextBox.Text = package.Description;
            }

            unsupportedVersionsListBox.DataSource = _unsupportedVersionLiteralsBindingList;
            Array devStages = Enum.GetValues(typeof(DevelopmentalStage));
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

            foreach (var changelogDictionaryEntry in _packageConfiguration.Changelog)
            {
                var culture = changelogDictionaryEntry.Key;
                if (culture.Name != "en")
                {
                    var page = new TabPage("Changelog")
                    {
                        BackColor = SystemColors.Window,
                        Tag = culture,
                    };
                    page.Controls.Add(new ChangelogPanel { Changelog = changelogDictionaryEntry.Value });
                    changelogContentTabControl.TabPages.Add(page);
                }
                else
                {
                    englishChangelogTextBox.Text = changelogDictionaryEntry.Value;
                }
            }

            architectureComboBox.SelectedIndex = 2;
            categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
            developmentalStageComboBox.SelectedIndex = 2;

            if (_packageConfiguration.UnsupportedVersions != null && _packageConfiguration.UnsupportedVersions.Count() != 0)
            {
                someVersionsRadioButton.Checked = true;
                unsupportedVersionsPanel.Enabled = true;
                foreach (var unsupportedVersionLiteral in _packageConfiguration.UnsupportedVersions)
                {
                    _unsupportedVersionLiteralsBindingList.Add(unsupportedVersionLiteral);
                }
            }
            else
            {
                unsupportedVersionsPanel.Enabled = false;
            }

            foreach (var operation in _packageConfiguration.Operations)
            {
                switch (Operation.GetOperationTag(operation))
                {
                    case "DeleteFile":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_deleteNode.Clone());

                        var deletePage = new TabPage("Delete file") { BackColor = SystemColors.Window };
                        deletePage.Controls.Add(new FileDeleteOperationPanel
                        {
                            Path = operation.Value.ToString(),
                            ItemList = new BindingList<string>(((JArray)operation.Value2).ToObject<List<string>>()),
                        });
                        categoryTabControl.TabPages.Add(deletePage);
                        break;

                    case "RenameFile":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_renameNode.Clone());

                        var renamePage = new TabPage("Rename file") { BackColor = SystemColors.Window };
                        renamePage.Controls.Add(new FileRenameOperationPanel
                        {
                            Path = operation.Value.ToString(),
                            NewName = operation.Value2.ToString()
                        });
                        categoryTabControl.TabPages.Add(renamePage);
                        break;

                    case "CreateRegistryEntry":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_createRegistryEntryNode.Clone());

                        var createRegistryEntryPage = new TabPage("Create registry entry")
                        {
                            BackColor = SystemColors.Window
                        };
                        createRegistryEntryPage.Controls.Add(new RegistryEntryCreateOperationPanel
                        {
                            KeyPath = operation.Value.ToString(),
                            ItemList = new BindingList<string>(((JArray)operation.Value2).ToObject<List<string>>()),
                        });
                        categoryTabControl.TabPages.Add(createRegistryEntryPage);
                        break;

                    case "DeleteRegistryEntry":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_deleteRegistryEntryNode.Clone());

                        var deleteRegistryEntryPage = new TabPage("Delete registry entry")
                        {
                            BackColor = SystemColors.Window
                        };
                        deleteRegistryEntryPage.Controls.Add(new RegistryEntryDeleteOperationPanel
                        {
                            KeyPath = operation.Value.ToString(),
                            ItemList = new BindingList<string>(((JArray)operation.Value2).ToObject<List<string>>()),
                        });
                        categoryTabControl.TabPages.Add(deleteRegistryEntryPage);
                        break;

                    case "SetRegistryKeyValue":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_setRegistryKeyValueNode.Clone());

                        var setRegistryEntryValuePage = new TabPage("Set registry entry value")
                        {
                            BackColor = SystemColors.Window
                        };
                        setRegistryEntryValuePage.Controls.Add(new RegistryEntrySetValueOperationPanel
                        {
                            KeyPath = operation.Value.ToString(),
                            Value = ((JObject)operation.Value2).ToObject<Tuple<string, string>>(),
                        });
                        categoryTabControl.TabPages.Add(setRegistryEntryValuePage);
                        break;
                    case "StartProcess":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_startProcessNode.Clone());

                        var startProcessPage = new TabPage("Start process") { BackColor = SystemColors.Window };
                        startProcessPage.Controls.Add(new ProcessStartOperationPanel
                        {
                            Path = operation.Value.ToString(),
                            Arguments = ((JArray)operation.Value2).ToObject<string[]>()
                        });
                        categoryTabControl.TabPages.Add(startProcessPage);
                        break;
                    case "TerminateProcess":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_terminateProcessNode.Clone());

                        var terminateProcessPage = new TabPage("Terminate process") { BackColor = SystemColors.Window };
                        terminateProcessPage.Controls.Add(new ProcessStopOperationPanel
                        {
                            ProcessName = operation.Value.ToString()
                        });
                        categoryTabControl.TabPages.Add(terminateProcessPage);
                        break;
                    case "StartService":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_startServiceNode.Clone());

                        var startServicePage = new TabPage("Start service") { BackColor = SystemColors.Window };
                        startServicePage.Controls.Add(new ServiceStartOperationPanel
                        {
                            ServiceName = operation.Value.ToString()
                        });
                        categoryTabControl.TabPages.Add(startServicePage);
                        break;
                    case "StopService":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_stopServiceNode.Clone());

                        var stopServicePage = new TabPage("Stop service") { BackColor = SystemColors.Window };
                        stopServicePage.Controls.Add(new ServiceStopOperationPanel
                        {
                            ServiceName = operation.Value.ToString()
                        });
                        categoryTabControl.TabPages.Add(stopServicePage);
                        break;
                }
            }
        }

        private void PackageEditDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
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
                categoryTabControl.SelectedTab =
                    categoryTabControl.TabPages[4 + categoryTreeView.SelectedNode.Index];
            }
        }

        private void someVersionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            unsupportedVersionsPanel.Enabled = true;
        }

        private void allVersionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            unsupportedVersionsPanel.Enabled = false;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _newConfiguration = UpdateConfiguration;

            var changelog = new Dictionary<CultureInfo, string>
            {
                {new CultureInfo("en"), englishChangelogTextBox.Text}
            };
            foreach (
                TabPage tabPage in
                    changelogContentTabControl.TabPages.Cast<TabPage>().Where(tabPage => tabPage.Text != "English"))
            {
                var panel = (ChangelogPanel) tabPage.Controls[0];
                if (String.IsNullOrEmpty(panel.Changelog)) continue;
                changelog.Add((CultureInfo) tabPage.Tag, panel.Changelog);
            }

            _packageConfiguration.MustUpdate = mustUpdateCheckBox.Checked;
            _packageConfiguration.Architecture = (Architecture) architectureComboBox.SelectedIndex;
            _packageConfiguration.Changelog = changelog;

            if (unsupportedVersionsListBox.Items.Count == 0)
                allVersionsRadioButton.Checked = true;
            else if (unsupportedVersionsListBox.Items.Count > 0 && someVersionsRadioButton.Checked)
            {
                _packageConfiguration.UnsupportedVersions =
                    unsupportedVersionsListBox.Items.Cast<string>().ToArray();
            }

            _packageConfiguration.Operations.Clear();
            foreach (var operationPanel in from TreeNode node in categoryTreeView.Nodes[3].Nodes
                select (IOperationPanel) categoryTabControl.TabPages[4 + node.Index].Controls[0])
            {
                _packageConfiguration.Operations.Add(operationPanel.Operation);
            }

            _packageConfiguration.UseStatistics = includeIntoStatisticsCheckBox.Checked;


            string[] unsupportedVersionLiterals = null;

            if (unsupportedVersionsListBox.Items.Count == 0)
                allVersionsRadioButton.Checked = true;
            else if (unsupportedVersionsListBox.Items.Count > 0 && someVersionsRadioButton.Checked)
            {
                unsupportedVersionLiterals = _unsupportedVersionLiteralsBindingList.ToArray();
            }

            _packageConfiguration.UnsupportedVersions = unsupportedVersionLiterals;

            if (developmentalStageComboBox.SelectedIndex == 2)
                _packageConfiguration.LiteralVersion = new UpdateVersion((int) majorNumericUpDown.Value,
                    (int) minorNumericUpDown.Value, (int) buildNumericUpDown.Value,
                    (int) revisionNumericUpDown.Value)
                    .ToString();
            else
            {
                _packageConfiguration.LiteralVersion = new UpdateVersion((int) majorNumericUpDown.Value,
                    (int) minorNumericUpDown.Value, (int) buildNumericUpDown.Value,
                    (int) revisionNumericUpDown.Value,
                    (DevelopmentalStage)
                        Enum.Parse(typeof (DevelopmentalStage),
                            developmentalStageComboBox.GetItemText(developmentalStageComboBox.SelectedItem)),
                    (int) developmentBuildNumericUpDown.Value).ToString();
            }

            _packageConfiguration.UpdatePackageUrl = new Uri(String.Format("{0}/{1}.zip",
                UriConnector.ConnectUri(Project.UpdateUrl, _packageConfiguration.LiteralVersion), Project.Guid));

            var index =
                _newConfiguration.IndexOf(_newConfiguration.First(item => item.LiteralVersion == _existingVersion));
            _newConfiguration[index] = _packageConfiguration;

            _newPackageDirectory = Path.Combine(Program.Path, "Projects", Project.Name,
                _packageConfiguration.LiteralVersion);

            if (_existingVersion == _packageConfiguration.LiteralVersion)
            {
                string configurationFilePath = Path.Combine(_newPackageDirectory, "updates.json");
                try
                {
                    File.WriteAllText(configurationFilePath, Serializer.Serialize(_newConfiguration));
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the new configuration.", ex,
                        PopupButtons.Ok);
                    return;
                }
            }
            else
            {
                _oldPackageDirectoryPath = Path.Combine(Program.Path, "Projects", Project.Name,
                    _existingVersion);
                string configurationFilePath = Path.Combine(_newPackageDirectory, "updates.json");
                try
                {
                    Directory.Move(_oldPackageDirectoryPath, _newPackageDirectory);
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error,
                        "Error while changing the version of the package directory.", ex,
                        PopupButtons.Ok);
                    return;
                }

                try
                {
                    File.WriteAllText(configurationFilePath, Serializer.Serialize(UpdateConfiguration));
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the new configuration.", ex,
                        PopupButtons.Ok);
                    Reset();
                    return;
                }
            }

            loadingPanel.Location = new Point(180, 91);
            loadingPanel.BringToFront();

            if (IsReleased)
                ThreadPool.QueueUserWorkItem(arg => InitializePackage());
            else
                DialogResult = DialogResult.OK;
        }

        private void InitializePackage()
        {
            SetUiState(false);
            Invoke(new Action(() => loadingLabel.Text = "Uploading new configuration..."));

            try
            {
                _ftp.UploadFile(Path.Combine(_newPackageDirectory, "updates.json"));
                _ftp.RenameDirectory(_existingVersion, _packageConfiguration.LiteralVersion);
                _configurationUploaded = true;
            }
            catch (Exception ex)
            {
                Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the new configuration.",
                                    ex, PopupButtons.Ok)));
                Reset();
                return;
            }

            try
            {
                string description = null;
                Invoke(new Action(() => description = descriptionTextBox.Text));
                Project.Packages.First(item => item.Version == new UpdateVersion(_existingVersion)).
                    Description = description;
                if (new UpdateVersion(_packageConfiguration.LiteralVersion) != new UpdateVersion(_existingVersion))
                {
                    Project.Packages.First(item => item.Version == new UpdateVersion(_existingVersion)).LocalPackagePath
                        = String.Format("{0}\\{1}.zip", _newPackageDirectory, Project.Guid);
                    Project.Packages.First(item => item.Version == new UpdateVersion(_existingVersion)).Version = new UpdateVersion(_packageConfiguration.LiteralVersion);
                }

                ApplicationInstance.SaveProject(Project.Path, Project);
            }
            catch (Exception ex)
            {
                Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the project.",
                                    ex, PopupButtons.Ok)));
                Reset();
                return;
            }

            SetUiState(true);
            DialogResult = DialogResult.OK;
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

                    var setRegistryEntryValuePage = new TabPage("Set registry entry value")
                    {
                        BackColor = SystemColors.Window
                    };
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

            categoryTreeView.Nodes[0].Toggle();
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

        private void developmentalStageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var developmentalStage = (DevelopmentalStage) Enum.Parse(typeof (DevelopmentalStage),
                developmentalStageComboBox.GetItemText(developmentalStageComboBox.SelectedItem));

            if (developmentalStage == DevelopmentalStage.Alpha || developmentalStage == DevelopmentalStage.Beta)
                developmentBuildNumericUpDown.Enabled = true;
            else
                developmentBuildNumericUpDown.Enabled = false;
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
            _unsupportedVersionLiteralsBindingList.RemoveAt(unsupportedVersionsListBox.SelectedIndex);
        }

        private void categoryTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (categoryTreeView.SelectedNode == null) return;
            if ((e.KeyCode != Keys.Delete && e.KeyCode != Keys.Back) || categoryTreeView.SelectedNode.Parent == null)
                categoryTabControl.TabPages.Remove(
                    categoryTabControl.TabPages[3 + categoryTreeView.SelectedNode.Index]);
            categoryTreeView.SelectedNode.Remove();
        }

        private void bulletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = changelogContentTabControl.SelectedTab;
            if (page.Text != "English")
            {
                var panel = (ChangelogPanel)page.Controls[0];
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
                var panel = (ChangelogPanel)page.Controls[0];
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
                var panel = (ChangelogPanel)page.Controls[0];
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
                var panel = (ChangelogPanel)page.Controls[0];
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
                var panel = (ChangelogPanel)page.Controls[0];
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
                var panel = (ChangelogPanel)page.Controls[0];
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
                var panel = (ChangelogPanel)page.Controls[0];
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
                var panel = (ChangelogPanel)page.Controls[0];
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
                var panel = (ChangelogPanel)page.Controls[0];
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
                var panel = (ChangelogPanel)page.Controls[0];
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
    }
}