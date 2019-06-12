// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.History;
using nUpdate.Administration.Core.Operations.Panels;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core;
using nUpdate.Internal.Core.Operations;
using nUpdate.Updating;
using Newtonsoft.Json.Linq;
using Starksoft.Aspen.Ftps;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using nUpdate.Administration.Core.Win32;
using Ionic.Zip;
using System.Threading;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageEditDialog : BaseDialog, IAsyncSupportable, IResettable
    {
        private readonly TreeNode _createRegistrySubKeyNode = new TreeNode("Create registry subkey", 14, 14)
        {
            Tag = "CreateRegistrySubKey"
        };

        private readonly List<CultureInfo> _cultures = new List<CultureInfo>();
        private readonly TreeNode _deleteNode = new TreeNode("Delete file", 9, 9) {Tag = "DeleteFile"};

        private readonly TreeNode _deleteRegistrySubKeyNode = new TreeNode("Delete registry subkey", 12, 12)
        {
            Tag = "DeleteRegistrySubKey"
        };

        private readonly TreeNode _deleteRegistryValueNode = new TreeNode("Delete registry value", 12, 12)
        {
            Tag = "DeleteRegistryValue"
        };

        private readonly TreeNode _executeScriptNode = new TreeNode("Execute Script", 15, 15) {Tag = "ExecuteScript"};
        private readonly TreeNode _renameNode = new TreeNode("Rename file", 10, 10) {Tag = "RenameFile"};
        private readonly TreeNode _replaceNode = new TreeNode("Replace file/folder", 11, 11) { Tag = "ReplaceFile" };

        private readonly TreeNode _setRegistryValueNode = new TreeNode("Set registry value", 13, 13)
        {
            Tag = "SetRegistryValue"
        };

        private readonly TreeNode _startProcessNode = new TreeNode("Start process", 8, 8) {Tag = "StartProcess"};
        private readonly TreeNode _startServiceNode = new TreeNode("Start service", 5, 5) {Tag = "StartService"};
        private readonly TreeNode _stopServiceNode = new TreeNode("Stop service", 6, 6) {Tag = "StopService"};

        private readonly TreeNode _terminateProcessNode = new TreeNode("Terminate process", 7, 7)
            {Tag = "StopProcess"};

        private readonly BindingList<string> _unsupportedVersionLiteralsBindingList = new BindingList<string>();
        private bool _nodeInitializingFailed;
        private bool _allowCancel = true;
        private bool _commandsExecuted;
        private bool _configurationUploaded;
        private string _existingVersionString;
        private ZipFile _zip;
        private FtpManager _ftp;
        private string _newPackageDirectory;
        private UpdateVersion _newVersion;
        private string _oldPackageDirectoryPath;
        private UpdateConfiguration _packageConfiguration;
        private readonly Log _updateLog = new Log();
        private String _packagePath;
        private String _extractedPackagePath;

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
        private bool _uploadCancelled;
        private bool _packageUploaded;

        public PackageEditDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The url of the configuration file.
        /// </summary>
        public Uri ConfigurationFileUrl { get; set; }

        /// <summary>
        ///     Gets or sets if the package is released.
        /// </summary>
        public bool IsReleased { get; set; }

        /// <summary>
        ///     The version of the package to edit.
        /// </summary>
        public UpdateVersion PackageVersion { get; set; }

        /// <summary>
        ///     Gets or sets the rollout conditions.
        /// </summary>
        public List<RolloutCondition> Conditions { get; set; }

        /// <summary>
        ///     The configurations available in the file.
        /// </summary>
        public List<UpdateConfiguration> UpdateConfiguration { get; set; }

        public void Reset()
        {
            if (_packageUploaded)
                try
                {
                    Invoke(new Action(() => loadingLabel.Text = "Undoing package upload..."));
                    _ftp.DeleteDirectory($"{_ftp.Directory}/{_packageConfiguration.LiteralVersion}");
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("No such file or directory"))
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while undoing the package upload.",
                                        ex,
                                        PopupButtons.Ok)));
                }

            if (_existingVersionString != _newVersion.ToString())
                try
                {
                    Directory.Move(_newPackageDirectory, _oldPackageDirectoryPath);
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Error while renaming the package directory.", ex,
                            PopupButtons.Ok);
                        Close();
                    }));
                    return;
                }

            if (_commandsExecuted)
            {
                Invoke(new Action(() => loadingLabel.Text = "Connecting to SQL-server..."));
                var connectionString = $"SERVER='{Project.SqlWebUrl}';" + $"DATABASE='{Project.SqlDatabaseName}';" +
                                       $"UID='{Project.SqlUsername}';" +
                                       $"PASSWORD='{SqlPassword.ConvertToInsecureString()}';";

                MySqlConnection myConnection = null;
                try
                {
                    myConnection = new MySqlConnection(connectionString);
                    myConnection.Open();
                }
                catch (MySqlException ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
                                    ex, PopupButtons.Ok)));
                    myConnection?.Close();
                    Invoke(new Action(Close));
                    return;
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while connecting to the database.",
                                    ex, PopupButtons.Ok)));
                    myConnection?.Close();
                    Invoke(new Action(Close));
                    return;
                }

                Invoke(new Action(() => loadingLabel.Text = "Executing SQL-commands..."));

                var command = myConnection.CreateCommand();
                command.CommandText =
                    $"UPDATE Version SET `Version` = \"{PackageVersion}\" WHERE `ID` = {_packageConfiguration.VersionId};";

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
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while executing the commands.",
                                    ex, PopupButtons.Ok)));
                    Invoke(new Action(Close));
                    return;
                }
                finally
                {
                    myConnection.Close();
                    command.Dispose();
                }
            }

            if (!_configurationUploaded)
                return;

            try
            {
                var configurationFilePath = Path.Combine(_newPackageDirectory, "updates.json");
                File.WriteAllText(configurationFilePath, Serializer.Serialize(UpdateConfiguration));
                _ftp.UploadFile(configurationFilePath);
                _configurationUploaded = false;
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while undoing the changes.", ex,
                        PopupButtons.Ok);
                    Close();
                }));
            }
        }

        /// <summary>
        ///     Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        public void SetUiState(bool enabled)
        {
            _allowCancel = enabled;

            Invoke(new Action(() =>
            {
                foreach (var c in from Control c in Controls where c.Visible select c) c.Enabled = enabled;

                loadingPanel.Visible = !enabled;
            }));
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

        private void allVersionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            unsupportedVersionsPanel.Enabled = false;
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

        private void categoryTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (categoryTreeView.SelectedNode.Parent == null) // Check whether the selected node is an operation or not
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
                        categoryTabControl.SelectedTab = conditionsTabPage;
                        break;
                    case 4:
                        categoryTabControl.SelectedTab = operationsTabPage;
                        break;
                }
            else
                switch (categoryTreeView.SelectedNode.Tag.ToString())
                {
                    case "ReplaceFile":
                        categoryTabControl.SelectedTab = replaceFilesTabPage;
                        break;
                    default:
                        categoryTabControl.SelectedTab =
                            categoryTabControl.TabPages[(int)categoryTreeView.SelectedNode.Tag];
                        break;
                }
        }

        private void categoryTreeView_DragDrop(object sender, DragEventArgs e)
        {
            var nodeToDropIn = categoryTreeView.GetNodeAt(categoryTreeView.PointToClient(new Point(e.X, e.Y)));
            if (nodeToDropIn == null || nodeToDropIn.Index != 4) // Operations-node
                return;

            var data = e.Data.GetData(typeof(string));
            if (data == null)
                return;

            TreeNode node;
            switch (data.ToString())
            {
                case "DeleteFile":
                    node = (TreeNode)_deleteNode.Clone();

                    var deletePage = new TabPage("Delete file") { BackColor = SystemColors.Window };
                    deletePage.Controls.Add(new FileDeleteOperationPanel());
                    categoryTabControl.TabPages.Add(deletePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(deletePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "RenameFile":
                    node = (TreeNode)_renameNode.Clone();

                    var renamePage = new TabPage("Rename file") { BackColor = SystemColors.Window };
                    renamePage.Controls.Add(new FileRenameOperationPanel());
                    categoryTabControl.TabPages.Add(renamePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(renamePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "CreateRegistrySubKey":
                    node = (TreeNode)_createRegistrySubKeyNode.Clone();

                    var createRegistrySubKeyPage = new TabPage("Create registry subkey")
                    {
                        BackColor = SystemColors.Window
                    };
                    createRegistrySubKeyPage.Controls.Add(new RegistrySubKeyCreateOperationPanel());
                    categoryTabControl.TabPages.Add(createRegistrySubKeyPage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(createRegistrySubKeyPage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "DeleteRegistrySubKey":
                    node = (TreeNode)_deleteRegistrySubKeyNode.Clone();

                    var deleteRegistrySubKeyPage = new TabPage("Delete registry subkey")
                    {
                        BackColor = SystemColors.Window
                    };
                    deleteRegistrySubKeyPage.Controls.Add(new RegistrySubKeyDeleteOperationPanel());
                    categoryTabControl.TabPages.Add(deleteRegistrySubKeyPage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(deleteRegistrySubKeyPage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "SetRegistryValue":
                    node = (TreeNode)_setRegistryValueNode.Clone();

                    var setRegistryValuePage = new TabPage("Set registry value") { BackColor = SystemColors.Window };
                    setRegistryValuePage.Controls.Add(new RegistrySetValueOperationPanel());
                    categoryTabControl.TabPages.Add(setRegistryValuePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(setRegistryValuePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "DeleteRegistryValue":
                    node = (TreeNode)_deleteRegistryValueNode.Clone();

                    var deleteRegistryValuePage =
                        new TabPage("Delete registry value") { BackColor = SystemColors.Window };
                    deleteRegistryValuePage.Controls.Add(new RegistryDeleteValueOperationPanel());
                    categoryTabControl.TabPages.Add(deleteRegistryValuePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(deleteRegistryValuePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "StartProcess":
                    node = (TreeNode)_startProcessNode.Clone();

                    var startProcessPage = new TabPage("Start process") { BackColor = SystemColors.Window };
                    startProcessPage.Controls.Add(new ProcessStartOperationPanel());
                    categoryTabControl.TabPages.Add(startProcessPage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(startProcessPage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;
                case "TerminateProcess":
                    node = (TreeNode)_terminateProcessNode.Clone();

                    var terminateProcessPage = new TabPage("Terminate process") { BackColor = SystemColors.Window };
                    terminateProcessPage.Controls.Add(new ProcessStopOperationPanel());
                    categoryTabControl.TabPages.Add(terminateProcessPage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(terminateProcessPage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;
                case "StartService":
                    node = (TreeNode)_startServiceNode.Clone();

                    var startServicePage = new TabPage("Start service") { BackColor = SystemColors.Window };
                    startServicePage.Controls.Add(new ServiceStartOperationPanel());
                    categoryTabControl.TabPages.Add(startServicePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(startServicePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;
                case "StopService":
                    node = (TreeNode)_stopServiceNode.Clone();

                    var stopServicePage = new TabPage("Stop service") { BackColor = SystemColors.Window };
                    stopServicePage.Controls.Add(new ServiceStopOperationPanel());
                    categoryTabControl.TabPages.Add(stopServicePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(stopServicePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;
                case "ExecuteScript":
                    node = (TreeNode)_executeScriptNode.Clone();

                    var executeScriptPage = new TabPage("Stop service") { BackColor = SystemColors.Window };
                    executeScriptPage.Controls.Add(new ScriptExecuteOperationPanel());
                    categoryTabControl.TabPages.Add(executeScriptPage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(executeScriptPage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;
            }
        }

        private void categoryTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void categoryTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (categoryTreeView.SelectedNode?.Parent == null)
                return;

            TreeNode selectedNode = categoryTreeView.SelectedNode;
            if (e.Control && e.KeyCode == Keys.Up)
            {
                selectedNode.MoveUp();
                categoryTreeView.SelectedNode = selectedNode;
                return;
            }
            else if (e.Control && e.KeyCode == Keys.Down)
            {
                selectedNode.MoveDown();
                categoryTreeView.SelectedNode = selectedNode;
                return;
            }

            if (e.KeyCode != Keys.Delete && e.KeyCode != Keys.Back ||
                categoryTreeView.SelectedNode.Equals(_replaceNode))
                return;

            int index = (int)categoryTreeView.SelectedNode.Tag;
            foreach (var node in categoryTreeView.Nodes[4].Nodes.Cast<TreeNode>().Where(n => !n.Equals(_replaceNode) && (int)n.Tag > index))
                node.Tag = (int)node.Tag - 1;

            categoryTreeView.SelectedNode.Remove();
            categoryTabControl.TabPages.Remove(
                categoryTabControl.TabPages[index]);
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

        private void developmentalStageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            developmentBuildNumericUpDown.Enabled = developmentalStageComboBox.SelectedIndex != 3;
        }

        private void englishChangelogTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & (e.KeyCode == Keys.A))
                englishChangelogTextBox.SelectAll();
            else if (e.Control & (e.KeyCode == Keys.Back))
                SendKeys.SendWait("^+{LEFT}{BACKSPACE}");
        }

        private async void InitializePackage()
        {
            await Task.Factory.StartNew(() =>
            {
                SetUiState(false);
                if (Project.UseStatistics)
                {
                    Invoke(new Action(() => loadingLabel.Text = "Connecting to SQL-server..."));

                    var connectionString = $"SERVER='{Project.SqlWebUrl}';" + $"DATABASE='{Project.SqlDatabaseName}';" +
                                           $"UID='{Project.SqlUsername}';" +
                                           $"PASSWORD='{SqlPassword.ConvertToInsecureString()}';";

                    MySqlConnection myConnection = null;
                    try
                    {
                        myConnection = new MySqlConnection(connectionString);
                        myConnection.Open();
                    }
                    catch (MySqlException ex)
                    {
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "An MySQL-exception occured.",
                                        ex, PopupButtons.Ok)));
                        myConnection?.Close();
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
                        myConnection?.Close();
                        Reset();
                        return;
                    }

                    Invoke(new Action(() => loadingLabel.Text = "Executing SQL-commands..."));

                    var command = myConnection.CreateCommand();
                    command.CommandText =
                        $"UPDATE Version SET `Version` = \"{_newVersion}\" WHERE `ID` = {_packageConfiguration.VersionId};";

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
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while executing the commands.",
                                        ex, PopupButtons.Ok)));
                        Reset();
                        return;
                    }
                    finally
                    {
                        myConnection.Close();
                        command.Dispose();
                    }
                }

                Invoke(new Action(() => loadingLabel.Text = "Saving package archive..."));
                try
                {
                    _zip.Dispose();
                    File.Delete(_packagePath);

                    _zip = new ZipFile();
                    _zip.AddDirectoryByName("Program");
                    _zip.AddDirectoryByName("AppData");
                    _zip.AddDirectoryByName("Temp");
                    _zip.AddDirectoryByName("Desktop");

                    InitializeArchiveContents(filesDataTreeView.Nodes[0], "Program");
                    InitializeArchiveContents(filesDataTreeView.Nodes[1], "AppData");
                    InitializeArchiveContents(filesDataTreeView.Nodes[2], "Temp");
                    InitializeArchiveContents(filesDataTreeView.Nodes[3], "Desktop");
                    _zip.Save(_packagePath);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the package archive.", ex,
                                    PopupButtons.Ok)));
                    Reset();
                    return;
                }

                /* -------------- Package upload -----------------*/
                Invoke(new Action(() =>
                {
                    loadingLabel.Text = "Uploading package... 0%";
                    cancelLabel.Visible = true;
                }));

                try
                {
                    _ftp.UploadPackage(_packagePath, _packageConfiguration.LiteralVersion.ToString());
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
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the package.",
                                        _ftp.PackageUploadException.InnerException, PopupButtons.Ok)));
                    else
                        Invoke(
                            new Action(
                                () =>
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while uploading the package.",
                                        _ftp.PackageUploadException, PopupButtons.Ok)));

                    Reset();
                    return;
                }

                Invoke(new Action(() => loadingLabel.Text = "Uploading new configuration..."));

                try
                {
                    _ftp.UploadFile(Path.Combine(_newPackageDirectory, "updates.json"));
                    if (_newVersion != new UpdateVersion(_existingVersionString))
                        _ftp.RenameDirectory(_existingVersionString, _packageConfiguration.LiteralVersion);
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
                    Project.Packages.First(
                            item => new UpdateVersion(item.Version) == new UpdateVersion(_existingVersionString))
                        .Description = description;
                    if (_newVersion != new UpdateVersion(_existingVersionString))
                    {
                        Project.Packages.First(
                                    item => new UpdateVersion(item.Version) ==
                                            new UpdateVersion(_existingVersionString))
                                .LocalPackagePath
                            = $"{_newPackageDirectory}\\{Project.Guid}.zip";
                        Project.Packages.First(item => item.Version == _existingVersionString)
                            .Version = _packageConfiguration.LiteralVersion;
                    }

                    _updateLog.Write(LogEntry.Edit, _newVersion.FullText);

                    UpdateProject.SaveProject(Project.Path, Project);
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
            });
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

        private void operationsListView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void operationsListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (operationsListView.SelectedItems.Count > 0)
                operationsListView.DoDragDrop(operationsListView.SelectedItems[0].Tag, DragDropEffects.Move);
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

        private void PackageEditDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        private void PackageEditDialog_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, PackageVersion.FullText, Program.VersionString);

            try
            {
                _ftp =
                    new FtpManager(Project.FtpHost, Project.FtpPort, Project.FtpDirectory, Project.FtpUsername,
                        FtpPassword,
                        Project.Proxy, Project.FtpUsePassiveMode, Project.FtpTransferAssemblyFilePath,
                        Project.FtpProtocol, Project.FtpNetworkVersion);

                _ftp.ProgressChanged += ProgressChanged;
                _ftp.CancellationFinished += CancellationFinished;
                if (!string.IsNullOrWhiteSpace(Project.FtpTransferAssemblyFilePath))
                    _ftp.TransferAssemblyPath = Project.FtpTransferAssemblyFilePath;
                else
                    _ftp.Protocol = (FtpsSecurityProtocol)Project.FtpProtocol;
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while loading the FTP-data.", ex, PopupButtons.Ok);
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
                    UpdateConfiguration.First(item => item.LiteralVersion == PackageVersion.ToString());
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
            majorNumericUpDown.Maximum = decimal.MaxValue;
            minorNumericUpDown.Maximum = decimal.MaxValue;
            buildNumericUpDown.Maximum = decimal.MaxValue;
            revisionNumericUpDown.Maximum = decimal.MaxValue;

            majorNumericUpDown.Value = packageVersion.Major;
            minorNumericUpDown.Value = packageVersion.Minor;
            buildNumericUpDown.Value = packageVersion.Build;
            revisionNumericUpDown.Value = packageVersion.Revision;

            _existingVersionString = _packageConfiguration.LiteralVersion;

            var devStages = Enum.GetValues(typeof(DevelopmentalStage));
            Array.Reverse(devStages);
            developmentalStageComboBox.DataSource = devStages;
            developmentalStageComboBox.SelectedIndex =
                developmentalStageComboBox.FindStringExact(packageVersion.DevelopmentalStage.ToString());
            developmentBuildNumericUpDown.Value = packageVersion.DevelopmentBuild;
            developmentBuildNumericUpDown.Enabled = packageVersion.DevelopmentalStage != DevelopmentalStage.Release;
            architectureComboBox.SelectedIndex = (int)_packageConfiguration.Architecture;
            necessaryUpdateCheckBox.Checked = _packageConfiguration.NecessaryUpdate;
            includeIntoStatisticsCheckBox.Enabled = Project.UseStatistics;
            includeIntoStatisticsCheckBox.Checked = _packageConfiguration.UseStatistics;
            foreach (
                var package in Project.Packages.Where(package => new UpdateVersion(package.Version) == packageVersion))
                descriptionTextBox.Text = package.Description;

            unsupportedVersionsListBox.DataSource = _unsupportedVersionLiteralsBindingList;
            var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            foreach (var info in cultureInfos)
            {
                changelogLanguageComboBox.Items.Add($"{info.EnglishName} - {info.Name}");
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
                        Tag = culture
                    };
                    page.Controls.Add(new ChangelogPanel { Changelog = changelogDictionaryEntry.Value });
                    changelogContentTabControl.TabPages.Add(page);
                }
                else
                {
                    englishChangelogTextBox.Text = changelogDictionaryEntry.Value;
                }
            }

            categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
            if (_packageConfiguration.UnsupportedVersions != null &&
                _packageConfiguration.UnsupportedVersions.Length != 0)
            {
                someVersionsRadioButton.Checked = true;
                unsupportedVersionsPanel.Enabled = true;
                foreach (var unsupportedVersionLiteral in _packageConfiguration.UnsupportedVersions)
                    _unsupportedVersionLiteralsBindingList.Add(unsupportedVersionLiteral);
            }
            else
            {
                unsupportedVersionsPanel.Enabled = false;
            }

            if (_packageConfiguration.RolloutConditions == null)
                _packageConfiguration.RolloutConditions = new List<RolloutCondition>();

            Conditions = _packageConfiguration.RolloutConditions;
            conditionsDataGridView.AutoGenerateColumns = false;
            var source = new BindingSource(new BindingList<RolloutCondition>(Conditions) { AllowNew = true }, null);
            conditionsDataGridView.DataSource = source;
            rolloutConditionModeComboBox.SelectedIndex = (int)_packageConfiguration.RolloutConditionMode;

            _updateLog.Project = Project;

            var packageFolder = Path.Combine(Program.Path, "Projects", Project.Name, _packageConfiguration.LiteralVersion.ToString());
            var packageFile = $"{Project.Guid}.zip";
            _packagePath = Path.Combine(packageFolder, packageFile);
            _extractedPackagePath = Path.Combine(packageFolder, Project.Guid.ToString());

            if (!File.Exists(_packagePath))
            {
                Invoke(new Action(() =>
                {
                    if (Popup.ShowPopup(this, SystemIcons.Warning, "Package archive not found locally.",
                        "The archive of the package file does not exist in your local project data. Should it be downloaded from the server? Without the package archive, the package cannot be edited.",
                        PopupButtons.YesNo) == DialogResult.No)
                    {
                        Close();
                        return;
                    }
                }));

                var mre = new ManualResetEvent(false);
                var wc = new WebClientWrapper();
                wc.DownloadDataCompleted += (o, dce) =>
                {
                    mre.Set();
                };
                wc.DownloadFileAsync(_packageConfiguration.UpdatePackageUri, packageFolder);
                mre.WaitOne();
            }

            _zip.ParallelDeflateThreshold = -1;
            _zip.UseZip64WhenSaving = Zip64Option.AsNecessary;

            using (var stream = File.OpenRead(_packagePath))
            {
                _zip = ZipFile.Read(stream);
                _zip.ExtractAll(_extractedPackagePath);
            }

            var operations = Serializer.Deserialize<IEnumerable<Operation>>(File.ReadAllText(Path.Combine(_extractedPackagePath, "operations.json")));
            foreach (var operation in operations.Where(o => o.ExecuteBeforeReplacingFiles))
                AddOperation(operation);
            categoryTreeView.Nodes[4].Nodes.Add(_replaceNode);
            foreach (var operation in operations.Where(o => !o.ExecuteBeforeReplacingFiles))
                AddOperation(operation);
        }

        private void ProgressChanged(object sender, TransferInterface.TransferProgressEventArgs e)
        {
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text =
                            $"Uploading package... {e.PercentComplete}% | {e.BytesPerSecond / 1024}KiB/s"));
            if (_uploadCancelled) Invoke(new Action(() => { loadingLabel.Text = "Cancelling upload..."; }));
        }

        private void AddOperation(Operation operation)
        {
            TreeNode node;
            switch (Operation.GetOperationTag(operation))
            {
                case "DeleteFile":
                    node = (TreeNode)_deleteNode.Clone();

                    var deletePage = new TabPage("Delete file") { BackColor = SystemColors.Window };
                    deletePage.Controls.Add(new FileDeleteOperationPanel
                    {
                        Path = operation.Value,
                        ItemList =
                            ((JArray)operation.Value2).ToObject<BindingList<string>>()
                    });
                    categoryTabControl.TabPages.Add(deletePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(deletePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "RenameFile":
                    node = (TreeNode)_renameNode.Clone();

                    var renamePage = new TabPage("Rename file") { BackColor = SystemColors.Window };
                    renamePage.Controls.Add(new FileRenameOperationPanel
                    {
                        Path = operation.Value,
                        NewName = operation.Value2.ToString()
                    });
                    categoryTabControl.TabPages.Add(renamePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(renamePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "CreateRegistrySubKey":
                    node = (TreeNode)_createRegistrySubKeyNode.Clone();

                    var createRegistrySubKeyPage = new TabPage("Create registry subkey")
                    {
                        BackColor = SystemColors.Window
                    };
                    createRegistrySubKeyPage.Controls.Add(new RegistrySubKeyCreateOperationPanel
                    {
                        KeyPath = operation.Value,
                        ItemList =
                            ((JArray)operation.Value2).ToObject<BindingList<string>>()
                    });
                    categoryTabControl.TabPages.Add(createRegistrySubKeyPage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(createRegistrySubKeyPage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "DeleteRegistrySubKey":
                    node = (TreeNode)_deleteRegistrySubKeyNode.Clone();

                    var deleteRegistrySubKeyPage = new TabPage("Delete registry subkey")
                    {
                        BackColor = SystemColors.Window
                    };
                    deleteRegistrySubKeyPage.Controls.Add(new RegistrySubKeyDeleteOperationPanel
                    {
                        KeyPath = operation.Value,
                        ItemList =
                            ((JArray)operation.Value2).ToObject<BindingList<string>>()
                    });
                    categoryTabControl.TabPages.Add(deleteRegistrySubKeyPage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(deleteRegistrySubKeyPage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "SetRegistryValue":
                    node = (TreeNode)_setRegistryValueNode.Clone();

                    var setRegistryValuePage = new TabPage("Set registry value") { BackColor = SystemColors.Window };
                    setRegistryValuePage.Controls.Add(new RegistrySetValueOperationPanel
                    {
                        KeyPath = operation.Value,
                        NameValuePairs =
                            ((JArray)operation.Value2).ToObject<List<Tuple<string, object, RegistryValueKind>>>()
                    });
                    categoryTabControl.TabPages.Add(setRegistryValuePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(setRegistryValuePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "DeleteRegistryValue":
                    node = (TreeNode)_deleteRegistryValueNode.Clone();

                    var deleteRegistryValuePage =
                        new TabPage("Delete registry value") { BackColor = SystemColors.Window };
                    deleteRegistryValuePage.Controls.Add(new RegistryDeleteValueOperationPanel
                    {
                        KeyPath = operation.Value,
                        ItemList = ((JArray)operation.Value2).ToObject<BindingList<string>>()
                    });
                    categoryTabControl.TabPages.Add(deleteRegistryValuePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(deleteRegistryValuePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;

                case "StartProcess":
                    node = (TreeNode)_startProcessNode.Clone();

                    var startProcessPage = new TabPage("Start process") { BackColor = SystemColors.Window };
                    startProcessPage.Controls.Add(new ProcessStartOperationPanel
                    {
                        Path = operation.Value,
                        Arguments = operation.Value2.ToString()
                    });
                    categoryTabControl.TabPages.Add(startProcessPage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(startProcessPage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;
                case "TerminateProcess":
                    node = (TreeNode)_terminateProcessNode.Clone();

                    var terminateProcessPage = new TabPage("Terminate process") { BackColor = SystemColors.Window };
                    terminateProcessPage.Controls.Add(new ProcessStopOperationPanel
                    {
                        ProcessName = operation.Value
                    });
                    categoryTabControl.TabPages.Add(terminateProcessPage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(terminateProcessPage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;
                case "StartService":
                    node = (TreeNode)_startServiceNode.Clone();

                    var startServicePage = new TabPage("Start service") { BackColor = SystemColors.Window };
                    startServicePage.Controls.Add(new ServiceStartOperationPanel
                    {
                        ServiceName = operation.Value,
                        Arguments = ((JArray)operation.Value2).ToObject<IEnumerable<string>>()
                    });
                    categoryTabControl.TabPages.Add(startServicePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(startServicePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;
                case "StopService":
                    node = (TreeNode)_stopServiceNode.Clone();

                    var stopServicePage = new TabPage("Stop service") { BackColor = SystemColors.Window };
                    stopServicePage.Controls.Add(new ServiceStopOperationPanel
                    {
                        ServiceName = operation.Value
                    });
                    categoryTabControl.TabPages.Add(stopServicePage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(stopServicePage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;
                case "ExecuteScript":
                    node = (TreeNode)_executeScriptNode.Clone();

                    var executeScriptPage = new TabPage("Stop service") { BackColor = SystemColors.Window };
                    executeScriptPage.Controls.Add(new ScriptExecuteOperationPanel
                    {
                        Code = operation.Value
                    });
                    categoryTabControl.TabPages.Add(executeScriptPage);

                    node.Tag = categoryTabControl.TabPages.IndexOf(executeScriptPage);
                    categoryTreeView.Nodes[4].Nodes.Add(node);
                    break;
            }
        }

        private void LoadPackageContent(string packagePath)
        {
            TaskEx.Run(() => {
                ListDirectoryContent(Path.Combine(packagePath, "Program"), filesDataTreeView.Nodes[0], true);
                ListDirectoryContent(Path.Combine(packagePath, "AppData"), filesDataTreeView.Nodes[1], true);
                ListDirectoryContent(Path.Combine(packagePath, "Temp"), filesDataTreeView.Nodes[2], true);
                ListDirectoryContent(Path.Combine(packagePath, "Desktop"), filesDataTreeView.Nodes[3], true);
            });
        }

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
                    var tmpDir = $"{currentDirectory}/{node.Text}";
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
                                        $"The file/folder \"{nodePlaceHolder.Text}\" was removed from the collection because it is already existing in the current directory.",
                                        PopupButtons.Ok)));
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
                                        $"The file/folder \"{nodePlaceHolder.Text}\" was removed from the collection because it is already existing in the current directory.",
                                        PopupButtons.Ok)));
                    }
                }
            }
        }

        private void removeVersionButton_Click(object sender, EventArgs e)
        {
            _unsupportedVersionLiteralsBindingList.RemoveAt(unsupportedVersionsListBox.SelectedIndex);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _newVersion = new UpdateVersion((int)majorNumericUpDown.Value, (int)minorNumericUpDown.Value,
                (int)buildNumericUpDown.Value, (int)revisionNumericUpDown.Value, (DevelopmentalStage)
                Enum.Parse(typeof(DevelopmentalStage),
                    developmentalStageComboBox.GetItemText(developmentalStageComboBox.SelectedItem)),
                (int)developmentBuildNumericUpDown.Value);
            if (_newVersion.BasicVersion == "0.0.0.0")
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Invalid version set.",
                    "Version \"0.0.0.0\" is not a valid version.", PopupButtons.Ok);
                generalPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
                return;
            }

            if (Project.Packages != null && Project.Packages.Count != 0)
                if (PackageVersion != _newVersion &&
                    Project.Packages.Any(item => new UpdateVersion(item.Version) == _newVersion))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid version set.",
                        $"Version \"{_newVersion.FullText}\" is already existing.", PopupButtons.Ok);
                    generalPanel.BringToFront();
                    categoryTreeView.SelectedNode = categoryTreeView.Nodes[0];
                    return;
                }

            if (string.IsNullOrEmpty(englishChangelogTextBox.Text))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No changelog set.",
                    "Please specify a changelog for the package. If you have already set a changelog in another language, you still need to specify one for \"English - en\" to support client's that don't use your specified culture on their computer.",
                    PopupButtons.Ok);
                changelogPanel.BringToFront();
                categoryTreeView.SelectedNode = categoryTreeView.Nodes[1];
                return;
            }

            if (!filesDataTreeView.Nodes.Cast<TreeNode>().Any(node => node.Nodes.Count > 0) &&
                categoryTreeView.Nodes[4].Nodes.Count <= 1)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "No files and/or folders or operations set.",
                    "Please specify some files and/or folders that should be included or operations into the package.",
                    PopupButtons.Ok);
                filesPanel.BringToFront();
                categoryTreeView.SelectedNode = _replaceNode;
                return;
            }

            foreach (
                var tabPage in
                from tabPage in categoryTabControl.TabPages.Cast<TabPage>()
                let operationPanel = tabPage.Controls[0] as IOperationPanel
                where operationPanel != null && !operationPanel.IsValid
                select tabPage)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "An added operation isn't valid.",
                    "Please make sure to fill out all required fields correctly.",
                    PopupButtons.Ok);
                categoryTreeView.SelectedNode =
                    categoryTreeView.Nodes[4].Nodes.Cast<TreeNode>()
                        .First(item => (int)item.Tag == categoryTabControl.TabPages.IndexOf(tabPage));
                return;
            }

            var changelog = new Dictionary<CultureInfo, string>
            {
                {new CultureInfo("en"), englishChangelogTextBox.Text}
            };
            foreach (
                var tabPage in
                changelogContentTabControl.TabPages.Cast<TabPage>().Where(tabPage => tabPage.Text != "English"))
            {
                var panel = (ChangelogPanel)tabPage.Controls[0];
                if (string.IsNullOrEmpty(panel.Changelog)) continue;
                changelog.Add((CultureInfo)tabPage.Tag, panel.Changelog);
            }

            _packageConfiguration.NecessaryUpdate = necessaryUpdateCheckBox.Checked;
            _packageConfiguration.Architecture = (Architecture)architectureComboBox.SelectedIndex;
            _packageConfiguration.Changelog = changelog;
            _packageConfiguration.RolloutConditionMode =
                (RolloutConditionMode)rolloutConditionModeComboBox.SelectedIndex;

            if (unsupportedVersionsListBox.Items.Count == 0)
                allVersionsRadioButton.Checked = true;
            else if (unsupportedVersionsListBox.Items.Count > 0 && someVersionsRadioButton.Checked)
                _packageConfiguration.UnsupportedVersions =
                    unsupportedVersionsListBox.Items.Cast<string>().ToArray();

            var operations = new List<Operation>();
            foreach (TreeNode node in categoryTreeView.Nodes[4].Nodes)
            {
                if (node.Equals(_replaceNode))
                    continue;

                bool execBefore = categoryTreeView.Nodes[4].Nodes.IndexOf(_replaceNode) > node.Index;
                var panel = (IOperationPanel)categoryTabControl.TabPages[(int)node.Tag].Controls[0];
                Operation op = panel.Operation;
                op.ExecuteBeforeReplacingFiles = execBefore;
                operations.Add(op);
            }

            File.WriteAllText(Path.Combine(_extractedPackagePath, "operations.json"), Serializer.Serialize(operations));

            _packageConfiguration.UseStatistics = includeIntoStatisticsCheckBox.Checked;

            string[] unsupportedVersionLiterals = null;

            if (unsupportedVersionsListBox.Items.Count == 0)
                allVersionsRadioButton.Checked = true;
            else if (unsupportedVersionsListBox.Items.Count > 0 && someVersionsRadioButton.Checked)
                unsupportedVersionLiterals = _unsupportedVersionLiteralsBindingList.ToArray();

            _packageConfiguration.UnsupportedVersions = unsupportedVersionLiterals;
            _packageConfiguration.LiteralVersion = _newVersion.ToString();
            _packageConfiguration.UpdatePackageUri = new Uri(
                $"{UriConnector.ConnectUri(Project.UpdateUrl, _packageConfiguration.LiteralVersion)}/{Project.Guid}.zip");

            _newPackageDirectory = Path.Combine(Program.Path, "Projects", Project.Name,
                _newVersion.ToString());

            if (_existingVersionString != _newVersion.ToString())
            {
                _oldPackageDirectoryPath = Path.Combine(Program.Path, "Projects", Project.Name,
                    _existingVersionString);
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
            }

            UpdateConfiguration[
                    UpdateConfiguration.IndexOf(
                        UpdateConfiguration.First(item => item.LiteralVersion == PackageVersion.ToString()))] =
                _packageConfiguration;
            var configurationFilePath = Path.Combine(_newPackageDirectory, "updates.json");
            try
            {
                File.WriteAllText(configurationFilePath, Serializer.Serialize(UpdateConfiguration));
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the new configuration.", ex,
                    PopupButtons.Ok);
                return;
            }

            loadingPanel.Location = new Point(180, 91);
            loadingPanel.BringToFront();

            if (IsReleased)
                InitializePackage();
            else
                DialogResult = DialogResult.OK;
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

        private void someVersionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            unsupportedVersionsPanel.Enabled = true;
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

        private void FilesDataTreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Parent == null || e.Node.ImageIndex != 0)
                e.Node.EndEdit(true);
        }

        private void AddFolderButton_Click(object sender, EventArgs e)
        {
            InitializeDirectoryListing(false);
        }

        private void AddFolderContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitializeDirectoryListing(true);
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

        private void ListDirectoryContent(string path, bool onlyContent)
        {
            ListDirectoryContent(path, filesDataTreeView.SelectedNode, onlyContent);
        }

        private void ListDirectoryContent(string path, TreeNode selectedNode, bool onlyContent)
        {
            BeginInvoke(new Action(() =>
            {
                if (!onlyContent)
                {
                    var folderNode = new TreeNode(new DirectoryInfo(path).Name, 0, 0);
                    selectedNode.Nodes.Add(folderNode);
                    selectedNode = folderNode;
                }

                var rootDirectoryInfo = new DirectoryInfo(path);
                CreateFilesNode(rootDirectoryInfo, selectedNode);
                foreach (
                    var node in
                    rootDirectoryInfo.GetDirectories().Select(CreateDirectoryNode).Where(node => node != null))
                    selectedNode.Nodes.Add(node);

                if (!selectedNode.IsExpanded)
                    selectedNode.Toggle();
            }));
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
                {
                    fileNode = new TreeNode(file.Name, 1, 1) { Tag = file.FullName };
                }
                else
                {
                    if (string.IsNullOrEmpty(file.Extension))
                    {
                        fileNode = new TreeNode(file.Name, 1, 1) { Tag = file.FullName };
                    }
                    else if (filesImageList.Images.ContainsKey(file.Extension))
                    {
                        var index = filesImageList.Images.IndexOfKey(file.Extension);
                        fileNode = new TreeNode(file.Name, index, index) { Tag = file.FullName };
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
                            fileNode = new TreeNode(file.Name, index, index) { Tag = file.FullName };
                        }
                        else
                        {
                            fileNode = new TreeNode(file.Name, 1, 1) { Tag = file.FullName };
                        }
                    }
                }

                var node = directoryNode;
                Invoke(new Action(() => node.Nodes.Add(fileNode)));
            }
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

        private void AddVirtualFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode == null)
                return;

            var folderNode = new TreeNode("Folder name", 0, 0);
            filesDataTreeView.SelectedNode.Nodes.Add(folderNode);
            if (!filesDataTreeView.SelectedNode.IsExpanded)
                filesDataTreeView.SelectedNode.Toggle();

            folderNode.BeginEdit();
        }

        private void AddFilesButton_Click(object sender, EventArgs e)
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
                    if (string.IsNullOrEmpty(fileInfo.Extension))
                    {
                        fileNode = new TreeNode(fileInfo.Name, 1, 1) { Tag = fileInfo.FullName };
                    }
                    else if (filesImageList.Images.ContainsKey(fileInfo.Extension))
                    {
                        var index = filesImageList.Images.IndexOfKey(fileInfo.Extension);
                        fileNode = new TreeNode(fileInfo.Name, index, index) { Tag = fileInfo.FullName };
                    }
                    else
                    {
                        var icon = IconReader.GetFileIcon(fileInfo.Extension);
                        if (icon != null)
                        {
                            filesImageList.Images.Add(fileInfo.Extension, icon.ToBitmap());
                            var index = filesImageList.Images.IndexOfKey(fileInfo.Extension);
                            fileNode = new TreeNode(fileInfo.Name, index, index) { Tag = fileInfo.FullName };
                        }
                        else
                        {
                            fileNode = new TreeNode(fileInfo.Name, 1, 1) { Tag = fileInfo.FullName };
                        }
                    }

                    filesDataTreeView.SelectedNode.Nodes.Add(fileNode);
                    if (!filesDataTreeView.SelectedNode.IsExpanded)
                        filesDataTreeView.SelectedNode.Toggle();
                }
            }
        }

        private void RemoveEntryButton_Click(object sender, EventArgs e)
        {
            if (filesDataTreeView.SelectedNode?.Parent != null)
                filesDataTreeView.SelectedNode.Remove();
        }

        private void InfoButton_Click(object sender, EventArgs e)
        {
            var updatingInfoDialog = new UpdatingInfoDialog();
            updatingInfoDialog.ShowDialog();
        }
    }
}