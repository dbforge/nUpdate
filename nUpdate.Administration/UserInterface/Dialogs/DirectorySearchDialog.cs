// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.UserInterface.Popups;

namespace nUpdate.Administration.UserInterface.Dialogs
{
    internal partial class DirectorySearchDialog : BaseDialog
    {
        private readonly List<TreeNode> _handledNodes = new List<TreeNode>();
        private readonly string _projectName;
        private readonly TransferManager _transferManager;

        public DirectorySearchDialog(TransferManager transferManager, string projectName)
        {
            InitializeComponent();
            LoadingPanel = loadingPanel;
            _transferManager = transferManager;
            _projectName = projectName;
        }

        public string SelectedDirectory { get; set; }

        private async void DirectorySearchDialog_Shown(object sender, EventArgs e)
        {
            Text = string.Format(Text, _projectName, Program.VersionString);

            var node = new TreeNode("Server", 0, 0);
            serverDataTreeView.Nodes.Add(node);
            await LoadListAsync("/", node); // TODO: Certificate check
            node.Expand();
        }

        private void DirectorySearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AllowCancel)
                e.Cancel = true;
        }

        private string ForgeDirectoryPath(TreeNode currentNode)
        {
            var directories = new Stack<string>();
            while (currentNode.Parent != null && currentNode.Parent != serverDataTreeView.Nodes[0])
            {
                directories.Push(currentNode.Parent.Text);
                currentNode = currentNode.Parent;
            }

            return
                $"{(directories.Count > 0 ? "/" : string.Empty)}{string.Join("/", directories)}/{serverDataTreeView.SelectedNode.Text}";
        }

        private async void serverDataTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == serverDataTreeView.Nodes[0])
            {
                directoryTextBox.Text = "/";
                return;
            }

            directoryTextBox.Text = ForgeDirectoryPath(e.Node);
            if (!_handledNodes.Contains(e.Node))
            {
                await LoadListAsync(directoryTextBox.Text, e.Node);
                _handledNodes.Add(e.Node);
            }

            serverDataTreeView.SelectedNode = e.Node;
            e.Node.Expand();
        }

        private async Task LoadListAsync(string path, TreeNode node)
        {
            DisableControls(true);
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = "Loading directories..."));

            IEnumerable<IServerItem> directoryItems;
            try
            {
                directoryItems = await _transferManager.List(path, false);
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while listing the directories.", ex,
                                PopupButtons.Ok);
                        }));

                EnableControls();
                DialogResult = DialogResult.OK;
                return;
            }

            foreach (
                var serverItem in directoryItems.Where(i => i.ItemType == ServerItemType.Directory).OrderBy(i => i.Name)
                )
            {
                Invoke(new Action(() =>
                    node.Nodes.Add(new TreeNode(serverItem.Name, 1, 1))));
            }

            EnableControls();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            SelectedDirectory = directoryTextBox.Text;
            DialogResult = DialogResult.OK;
        }

        private void addDirectoryButton_Click(object sender, EventArgs e)
        {
            var node = new TreeNode("Name", 1, 1);
            if (serverDataTreeView.SelectedNode == null)
                return;

            serverDataTreeView.SelectedNode.Nodes.Add(node);
            serverDataTreeView.SelectedNode.Expand();
            serverDataTreeView.LabelEdit = true;
            node.BeginEdit();
        }

        private async void serverDataTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                    "Please enter a name for the directory to create.", PopupButtons.Ok);
                e.Node.Remove();
                return;
            }

            serverDataTreeView.LabelEdit = false;
            directoryTextBox.Text = ForgeDirectoryPath(serverDataTreeView.SelectedNode);
            await CreateDirectory($"{directoryTextBox.Text}/{e.Label}");
            _handledNodes.Add(e.Node);
        }

        private async Task CreateDirectory(string path)
        {
            DisableControls(true);
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = $"Creating directory \"{path}\"..."));
            try
            {
                await _transferManager.MakeDirectoryWithPath(path);
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the directory.", ex,
                                PopupButtons.Ok)));
            }
            EnableControls();
        }

        private async void removeDirectoryButton_Click(object sender, EventArgs e)
        {
            if (serverDataTreeView.SelectedNode == null ||
                serverDataTreeView.SelectedNode == serverDataTreeView.Nodes[0])
                return;

            if (
                Popup.ShowPopup(this, SystemIcons.Warning, "Delete the selected directory?",
                    "Are you sure that you want to delete the selected directory? It will be deleted unrecoverably.",
                    PopupButtons.YesNo) == TaskDialogResult.No)
                return;
            
            directoryTextBox.Text = ForgeDirectoryPath(serverDataTreeView.SelectedNode);
            await RemoveDirectory(directoryTextBox.Text);
        }

        private async Task RemoveDirectory(string path)
        {
            DisableControls(true);
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = $"Deleting directory \"{path}\"..."));
            try
            {
                await _transferManager.DeleteDirectoryWithPath(path);
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while deleting the directory.", ex,
                                PopupButtons.Ok)));
                EnableControls();
                return;
            }

            Invoke(
                new Action(
                    () =>
                    {
                        var parent = serverDataTreeView.SelectedNode.Parent;
                        serverDataTreeView.SelectedNode.Remove();
                        directoryTextBox.Text = ForgeDirectoryPath(parent);
                    }));
            EnableControls();
        }
    }
}