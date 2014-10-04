// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Core.Update.Operations;
using nUpdate.Administration.Core.Update.Operations.Panels;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class PackageEditDialog : BaseDialog
    {
        private readonly List<CultureInfo> _cultures = new List<CultureInfo>();
        private readonly TreeNode _createRegistryEntryNode = new TreeNode("Create registry entry", 14, 14);
        private readonly TreeNode _deleteNode = new TreeNode("Delete file", 9, 9);
        private readonly TreeNode _deleteRegistryEntryNode = new TreeNode("Delete registry entry", 12, 12);
        private readonly TreeNode _renameNode = new TreeNode("Rename file", 10, 10);
        private readonly TreeNode _setRegistryKeyValueNode = new TreeNode("Set registry key value", 13, 13);
        private readonly TreeNode _startProcessNode = new TreeNode("Start process", 8, 8);
        private readonly TreeNode _startServiceNode = new TreeNode("Start service", 5, 5);
        private readonly TreeNode _stopServiceNode = new TreeNode("Stop service", 6, 6);
        private readonly TreeNode _terminateProcessNode = new TreeNode("Terminate process", 7, 7);
        private readonly BindingList<UpdateVersion> _unsupportedVersionsBindingsList = new BindingList<UpdateVersion>();

        public PackageEditDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The configurations available in the file.
        /// </summary>
        internal List<UpdateConfiguration> UpdateConfigurations { get; set; } 

        /// <summary>
        ///     The configuration of the package itself.
        /// </summary>
        internal UpdateConfiguration PackageConfiguration { get; set; }

        private void PackageEditDialog_Load(object sender, EventArgs e)
        {
            var packageVersion = new UpdateVersion(PackageConfiguration.Version);
            majorNumericUpDown.Maximum = Decimal.MaxValue;
            minorNumericUpDown.Maximum = Decimal.MaxValue;
            buildNumericUpDown.Maximum = Decimal.MaxValue;
            revisionNumericUpDown.Maximum = Decimal.MaxValue;

            majorNumericUpDown.Value = packageVersion.Major;
            majorNumericUpDown.Minimum = packageVersion.Major;
            minorNumericUpDown.Value = packageVersion.Minor;
            buildNumericUpDown.Value = packageVersion.Build;
            revisionNumericUpDown.Value = packageVersion.Revision;

            developmentalStageComboBox.SelectedValue = packageVersion.DevelopmentalStage;
            developmentBuildNumericUpDown.Value = (packageVersion.DevelopmentBuild > 0)
                ? packageVersion.DevelopmentBuild
                : 1;
            developmentBuildNumericUpDown.Enabled = (packageVersion.DevelopmentalStage != DevelopmentalStage.Release);
            mustUpdateCheckBox.Checked = PackageConfiguration.MustUpdate;
            foreach (UpdatePackage package in Project.Packages.Where(package => package.Version == packageVersion))
            {
                descriptionTextBox.Text = package.Description;
            }

            unsupportedVersionsListBox.DataSource = _unsupportedVersionsBindingsList;
            Array devStages = Enum.GetValues(typeof(DevelopmentalStage));
            Array.Reverse(devStages);
            developmentalStageComboBox.DataSource = devStages;
            unsupportedDevelopmentalStageComboBox.DataSource = devStages;
            List<CultureInfo> cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            foreach (CultureInfo info in cultureInfos)
            {
                changelogLanguageComboBox.Items.Add(String.Format("{0} - {1}", info.EnglishName, info.Name));
                _cultures.Add(info);
            }

            changelogContentTabControl.TabPages[0].Tag = _cultures.Where(x => x.Name == "en");
            changelogLanguageComboBox.SelectedIndex = changelogLanguageComboBox.FindStringExact("English - en");

            foreach (var changelogDictionaryEntry in PackageConfiguration.Changelog)
            {
                var culture = changelogDictionaryEntry.Key;
                if (culture.Name != "en")
                {
                    var page = new TabPage("Changelog")
                    {
                        BackColor = SystemColors.Window,
                        Tag = culture,
                    };
                    page.Controls.Add(new ChangelogPanel() { Changelog = changelogDictionaryEntry.Value });
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
            unsupportedVersionsPanel.Enabled = false;

            InitializeOperationNodes();
            foreach (var operation in PackageConfiguration.Operations)
            {
                switch (Operation.GetOperationTag(operation))
                {
                    case "DeleteFile":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_deleteNode.Clone());
                        
                        var deletePage = new TabPage("Delete file") { BackColor = SystemColors.Window };
                        deletePage.Controls.Add(new FileDeleteOperationPanel {Path = operation.Value.ToString(), ItemList = new BindingList<string>(((JArray)operation.Value2).ToObject<List<string>>()), });
                        categoryTabControl.TabPages.Add(deletePage);
                        break;

                    case "RenameFile":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_renameNode.Clone());

                        var renamePage = new TabPage("Rename file") { BackColor = SystemColors.Window };
                        renamePage.Controls.Add(new FileRenameOperationPanel {Path = operation.Value.ToString(), NewName = operation.Value2.ToString()});
                        categoryTabControl.TabPages.Add(renamePage);
                        break;

                    case "CreateRegistryEntry":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_createRegistryEntryNode.Clone());

                        var createRegistryEntryPage = new TabPage("Create registry entry") { BackColor = SystemColors.Window };
                        createRegistryEntryPage.Controls.Add(new RegistryEntryCreateOperationPanel { KeyPath = operation.Value.ToString(), ItemList = new BindingList<string>(((JArray)operation.Value2).ToObject<List<string>>()), });
                        categoryTabControl.TabPages.Add(createRegistryEntryPage);
                        break;

                    case "DeleteRegistryEntry":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_deleteRegistryEntryNode.Clone());

                        var deleteRegistryEntryPage = new TabPage("Delete registry entry") { BackColor = SystemColors.Window };
                        deleteRegistryEntryPage.Controls.Add(new RegistryEntryDeleteOperationPanel { KeyPath = operation.Value.ToString(), ItemList = new BindingList<string>(((JArray)operation.Value2).ToObject<List<string>>()), });
                        categoryTabControl.TabPages.Add(deleteRegistryEntryPage);
                        break;

                    case "SetRegistryKeyValue":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_setRegistryKeyValueNode.Clone());

                        var setRegistryEntryValuePage = new TabPage("Set registry entry value") { BackColor = SystemColors.Window };
                        setRegistryEntryValuePage.Controls.Add(new RegistryEntrySetValueOperationPanel {KeyPath = operation.Value.ToString(), Value = ((JObject)operation.Value2).ToObject<Tuple<string, string>>(), });
                        categoryTabControl.TabPages.Add(setRegistryEntryValuePage);
                        break;
                    case "StartProcess":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_startProcessNode.Clone());

                        var startProcessPage = new TabPage("Start process") { BackColor = SystemColors.Window };
                        startProcessPage.Controls.Add(new ProcessStartOperationPanel {Path = operation.Value.ToString(), Arguments = ((JArray)operation.Value2).ToObject<string[]>()});
                        categoryTabControl.TabPages.Add(startProcessPage);
                        break;
                    case "TerminateProcess":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_terminateProcessNode.Clone());

                        var terminateProcessPage = new TabPage("Terminate process") { BackColor = SystemColors.Window };
                        terminateProcessPage.Controls.Add(new ProcessStopOperationPanel {ProcessName = operation.Value.ToString()});
                        categoryTabControl.TabPages.Add(terminateProcessPage);
                        break;
                    case "StartService":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_startServiceNode.Clone());

                        var startServicePage = new TabPage("Start service") { BackColor = SystemColors.Window };
                        startServicePage.Controls.Add(new ServiceStartOperationPanel {ServiceName = operation.Value.ToString()});
                        categoryTabControl.TabPages.Add(startServicePage);
                        break;
                    case "StopService":
                        categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_stopServiceNode.Clone());

                        var stopServicePage = new TabPage("Stop service") { BackColor = SystemColors.Window };
                        stopServicePage.Controls.Add(new ServiceStopOperationPanel {ServiceName = operation.Value.ToString()});
                        categoryTabControl.TabPages.Add(stopServicePage);
                        break;
                }
            }
        }

        /// <summary>
        ///     Initializes the tree nodes for the update operations.
        /// </summary>
        private void InitializeOperationNodes()
        {
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
            var changelog = new Dictionary<CultureInfo, string> {{new CultureInfo("en"), englishChangelogTextBox.Text}};
            foreach (
                TabPage tabPage in
                    changelogContentTabControl.TabPages.Cast<TabPage>().Where(tabPage => tabPage.Text != "English"))
            {
                var panel = (ChangelogPanel) tabPage.Controls[0];
                if (String.IsNullOrEmpty(panel.Changelog)) continue;
                changelog.Add((CultureInfo) tabPage.Tag, panel.Changelog);
            }

            PackageConfiguration.MustUpdate = mustUpdateCheckBox.Checked;
            PackageConfiguration.Architecture = (Architecture) architectureComboBox.SelectedIndex;
            PackageConfiguration.Changelog = changelog;

            if (unsupportedVersionsListBox.Items.Count == 0)
                allVersionsRadioButton.Checked = true;
            else if (unsupportedVersionsListBox.Items.Count > 0 && someVersionsRadioButton.Checked)
            {
                PackageConfiguration.UnsupportedVersions = unsupportedVersionsListBox.Items.Cast<string>().ToArray();
            }

            PackageConfiguration.Operations.Clear();
            foreach (var operationPanel in from TreeNode node in categoryTreeView.Nodes[3].Nodes
                where node.Index != 0
                select (IOperationPanel) categoryTabControl.TabPages[4 + node.Index].Controls[0])
            {
                PackageConfiguration.Operations.Add(operationPanel.Operation);
            }

            PackageConfiguration.UseStatistics = includeIntoStatisticsCheckBox.Checked;

            string existingVersion = PackageConfiguration.Version;
            if (developmentalStageComboBox.SelectedIndex == 2)
                PackageConfiguration.Version = new UpdateVersion((int)majorNumericUpDown.Value,
                    (int)minorNumericUpDown.Value, (int)buildNumericUpDown.Value, (int)revisionNumericUpDown.Value).ToString();
            else
            {
                PackageConfiguration.Version = new UpdateVersion((int)majorNumericUpDown.Value,
                    (int)minorNumericUpDown.Value, (int)buildNumericUpDown.Value, (int)revisionNumericUpDown.Value, (DevelopmentalStage)Enum.Parse(typeof(DevelopmentalStage), developmentalStageComboBox.GetItemText(developmentalStageComboBox.SelectedItem)), (int)developmentBuildNumericUpDown.Value).ToString();
            }

            UpdateConfigurations[
                UpdateConfigurations.IndexOf(UpdateConfigurations.First(item => item.Version == existingVersion))] =
                PackageConfiguration;

            if (existingVersion == PackageConfiguration.Version)
            {
                string configurationFilePath = Path.Combine(Program.Path, "Projects", Project.Name, PackageConfiguration.Version, "updates.json");
                try
                {
                    File.WriteAllText(configurationFilePath, Serializer.Serialize(UpdateConfigurations));
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the new configuration.", ex,
                        PopupButtons.Ok);
                }
            }
            else
            {
                string oldPackageDirectoryPath = Path.Combine(Program.Path, "Projects", existingVersion);
                string newPackageDirectoryPath = Path.Combine(Program.Path, "Projects", PackageConfiguration.Version);
                string configurationFilePath = Path.Combine(newPackageDirectoryPath, "updates.json");
                try
                {
                    Directory.Move(oldPackageDirectoryPath, newPackageDirectoryPath);
                    File.WriteAllText(configurationFilePath, Serializer.Serialize(UpdateConfigurations));
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while saving the new configuration.", ex,
                        PopupButtons.Ok);
                }
            }
        }

        private void categoryTreeView_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode nodeToDropIn = categoryTreeView.GetNodeAt(categoryTreeView.PointToClient(new Point(e.X, e.Y)));
            if (nodeToDropIn == null || nodeToDropIn.Index != 3) // Operations-node
                return;

            object data = e.Data.GetData(typeof(string));
            if (data == null)
                return;

            switch (data.ToString())
            {
                case "DeleteFile":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_deleteNode.Clone());

                    var deletePage = new TabPage("Delete file") { BackColor = SystemColors.Window };
                    deletePage.Controls.Add(new FileDeleteOperationPanel());
                    categoryTabControl.TabPages.Add(deletePage);
                    break;

                case "RenameFile":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_renameNode.Clone());

                    var renamePage = new TabPage("Rename file") { BackColor = SystemColors.Window };
                    renamePage.Controls.Add(new FileRenameOperationPanel());
                    categoryTabControl.TabPages.Add(renamePage);
                    break;

                case "CreateRegistryEntry":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_createRegistryEntryNode.Clone());

                    var createRegistryEntryPage = new TabPage("Create registry entry") { BackColor = SystemColors.Window };
                    createRegistryEntryPage.Controls.Add(new RegistryEntryCreateOperationPanel());
                    categoryTabControl.TabPages.Add(createRegistryEntryPage);
                    break;

                case "DeleteRegistryEntry":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_deleteRegistryEntryNode.Clone());

                    var deleteRegistryEntryPage = new TabPage("Delete registry entry") { BackColor = SystemColors.Window };
                    deleteRegistryEntryPage.Controls.Add(new RegistryEntryDeleteOperationPanel());
                    categoryTabControl.TabPages.Add(deleteRegistryEntryPage);
                    break;

                case "SetRegistryKeyValue":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_setRegistryKeyValueNode.Clone());

                    var setRegistryEntryValuePage = new TabPage("Set registry entry value") { BackColor = SystemColors.Window };
                    setRegistryEntryValuePage.Controls.Add(new RegistryEntrySetValueOperationPanel());
                    categoryTabControl.TabPages.Add(setRegistryEntryValuePage);
                    break;
                case "StartProcess":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_startProcessNode.Clone());

                    var startProcessPage = new TabPage("Start process") { BackColor = SystemColors.Window };
                    startProcessPage.Controls.Add(new ProcessStartOperationPanel());
                    categoryTabControl.TabPages.Add(startProcessPage);
                    break;
                case "TerminateProcess":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_terminateProcessNode.Clone());

                    var terminateProcessPage = new TabPage("Terminate process") { BackColor = SystemColors.Window };
                    terminateProcessPage.Controls.Add(new ProcessStopOperationPanel());
                    categoryTabControl.TabPages.Add(terminateProcessPage);
                    break;
                case "StartService":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_startServiceNode.Clone());

                    var startServicePage = new TabPage("Start service") { BackColor = SystemColors.Window };
                    startServicePage.Controls.Add(new ServiceStartOperationPanel());
                    categoryTabControl.TabPages.Add(startServicePage);
                    break;
                case "StopService":
                    categoryTreeView.Nodes[3].Nodes.Add((TreeNode)_stopServiceNode.Clone());

                    var stopServicePage = new TabPage("Stop service") { BackColor = SystemColors.Window };
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
            var developmentalStage = (DevelopmentalStage)Enum.Parse(typeof (DevelopmentalStage),
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

            var developmentalStage = (DevelopmentalStage)Enum.Parse(typeof(DevelopmentalStage),
                developmentalStageComboBox.GetItemText(unsupportedDevelopmentalStageComboBox.SelectedItem));

            if (developmentalStage == DevelopmentalStage.Alpha || developmentalStage == DevelopmentalStage.Beta)
            {
                var version = new UpdateVersion((int) unsupportedMajorNumericUpDown.Value,
                    (int) unsupportedMinorNumericUpDown.Value, (int) unsupportedBuildNumericUpDown.Value,
                    (int) unsupportedRevisionNumericUpDown.Value, developmentalStage, (int)unsupportedDevelopmentBuildNumericUpDown.Value);
                _unsupportedVersionsBindingsList.Add(version);
            }
            else
            {
                var version = new UpdateVersion((int)unsupportedMajorNumericUpDown.Value,
                    (int)unsupportedMinorNumericUpDown.Value, (int)unsupportedBuildNumericUpDown.Value,
                    (int)unsupportedRevisionNumericUpDown.Value);
                _unsupportedVersionsBindingsList.Add(version);
            }
        }

        private void removeVersionButton_Click(object sender, EventArgs e)
        {
            _unsupportedVersionsBindingsList.RemoveAt(unsupportedVersionsListBox.SelectedIndex);
        }

        private void unsupportedDevelopmentalStageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var developmentalStage = (DevelopmentalStage)Enum.Parse(typeof(DevelopmentalStage),
                developmentalStageComboBox.GetItemText(unsupportedDevelopmentalStageComboBox.SelectedItem));

            if (developmentalStage == DevelopmentalStage.Alpha || developmentalStage == DevelopmentalStage.Beta)
                unsupportedDevelopmentBuildNumericUpDown.Enabled = true;
            else
                unsupportedDevelopmentBuildNumericUpDown.Enabled = false;
        }

        private void categoryTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (categoryTreeView.SelectedNode == null) return;
            if ((e.KeyCode != Keys.Delete && e.KeyCode != Keys.Back) || categoryTreeView.SelectedNode.Parent == null)
            categoryTabControl.TabPages.Remove(
                categoryTabControl.TabPages[3 + categoryTreeView.SelectedNode.Index]);
            categoryTreeView.SelectedNode.Remove();
        }
    }
}