// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.UserInterface.Popups;
using nUpdate.Administration.Win32;

namespace nUpdate.Administration.UserInterface.Dialogs
{
    internal partial class DirectorySearchDialog : BaseDialog
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private TransferManager _transferManager;
        private readonly Navigator<TreeNode> _navigator = new Navigator<TreeNode>();
        private List<IServerItem> _listedFtpItems = new List<IServerItem>();
        private Margins _margins;
        private bool _nodeSelectedByUser = true;

        public DirectorySearchDialog(TransferManager transferManager, string projectName)
        {
            InitializeComponent();
            LoadingPanel = loadingPanel;
            _transferManager = transferManager;
            ProjectName = projectName;
        }

        public string ProjectName { get; set; }

        /// <summary>
        ///     Gets or sets the selected directory.
        /// </summary>
        public string SelectedDirectory { get; set; }
        
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (!NativeMethods.DwmIsCompositionEnabled())
                return;

            _margins.Top = 38;
            e.Graphics.Clear(Color.Black);

            var clientArea = new Rectangle(
                _margins.Left,
                _margins.Top,
                ClientRectangle.Width - _margins.Left - _margins.Right,
                ClientRectangle.Height - _margins.Top - _margins.Bottom
                );
            Brush b = new SolidBrush(BackColor);
            e.Graphics.FillRectangle(b, clientArea);
        }

        private async void DirectorySearchDialog_Shown(object sender, EventArgs e)
        {
            Text = string.Format(Text, ProjectName, Program.VersionString);
            if (NativeMethods.DwmIsCompositionEnabled())
            {
                _margins = new Margins {Top = 38};
                NativeMethods.DwmExtendFrameIntoClientArea(Handle, ref _margins);
            }

            await LoadListAsync();
        }

        private void DirectorySearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AllowCancel)
                e.Cancel = true;
        }

        private void DirectorySearchDialog_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            NativeMethods.ReleaseCapture();
            NativeMethods.SendMessage(Handle, WM_NCLBUTTONDOWN, new IntPtr(HT_CAPTION), new IntPtr(0));
        }

        private void serverDataTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == serverDataTreeView.Nodes[0])
                return;

            if (_nodeSelectedByUser)
                _navigator.Add(e.Node);
            if (!backButton.Enabled && _navigator.CanGoBack)
                backButton.Enabled = true;

            var directories = new Stack<string>();
            var currentNode = e.Node;
            while (currentNode.Parent != null && currentNode.Parent != serverDataTreeView.Nodes[0])
            {
                directories.Push(currentNode.Parent.Text);
                currentNode = currentNode.Parent;
            }

            var directory = $"/{string.Join("/", directories)}/{e.Node.Text}";
            directoryTextBox.Text = directory.StartsWith("//") ? directory.Remove(0, 1) : directory;
        }

        private async Task LoadListAsync()
        {
            DisableControls(true);
            Invoke(
                new Action(
                    () =>
                        loadingLabel.Text = "Loading server directories..."));

            try
            {
                _listedFtpItems = (await _transferManager.List("/", true)).ToList();
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while listing the server data.", ex,
                                PopupButtons.Ok);
                            EnableControls();
                        }));

                DialogResult = DialogResult.OK;
                return;
            }

            /*if (_listedFtpItems != null)
            {
                var root = ConvertToListingItem(_listedFtpItems, "/");
                var rootNode = new TreeNode("Server", 0, 0);
                Invoke(new Action(() =>
                {
                    RecursiveAdd(root, rootNode);
                    serverDataTreeView.Nodes.Add(rootNode);
                    serverDataTreeView.Nodes[0].Expand();
                    serverDataTreeView.SelectedNode = rootNode;
                }));
            }*/

            EnableControls();
        }

        private void RecursiveAdd(ListingItem item, TreeNode target)
        {
            foreach (var child in item.Children)
            {
                if (!child.IsDirectory)
                    continue;

                var childNode = new TreeNode(child.Text, 1, 1);
                target.Nodes.Add(childNode);
                RecursiveAdd(child, childNode);
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            SelectedDirectory = directoryTextBox.Text;
            DialogResult = DialogResult.OK;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            _navigator.GoBack();
            _nodeSelectedByUser = false;
            serverDataTreeView.SelectedNode = _navigator.Current;
            _nodeSelectedByUser = true;
            if (!_navigator.CanGoBack)
                backButton.Enabled = false;
            if (_navigator.CanGoForward)
                forwardButton.Enabled = true;
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            _navigator.GoForward();
            _nodeSelectedByUser = false;
            serverDataTreeView.SelectedNode = _navigator.Current;
            _nodeSelectedByUser = true;
            if (!_navigator.CanGoForward)
                forwardButton.Enabled = false;
            if (_navigator.CanGoBack)
                backButton.Enabled = true;
        }

        private void addDirectoryButton_Click(object sender, EventArgs e)
        {
            var node = new TreeNode("Name", 1, 1);
            if (serverDataTreeView.SelectedNode != null)
            {
                serverDataTreeView.SelectedNode.Nodes.Add(node);
                serverDataTreeView.SelectedNode.Expand();
            }
            else
                serverDataTreeView.Nodes[0].Nodes.Add(node);

            serverDataTreeView.LabelEdit = true;
            node.BeginEdit();
        }

        private void serverDataTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                    "Please enter a name for the directory to create.", PopupButtons.Ok);
                e.Node.BeginEdit();
                return;
            }

            serverDataTreeView.LabelEdit = false;
#pragma warning disable 4014
            CreateDirectory($"{directoryTextBox.Text}/{e.Label}");
#pragma warning restore 4014
        }

        private async void CreateDirectory(string path)
        {
            await Task.Factory.StartNew(async () =>
            {
                DisableControls(true);
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = $"Creating directory \"{path}\"..."));
                try
                {
                    await _transferManager.MakeDirectory(path);
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
            });
        }

        private void removeDirectoryButton_Click(object sender, EventArgs e)
        {
            if (serverDataTreeView.SelectedNode == null ||
                serverDataTreeView.SelectedNode == serverDataTreeView.Nodes[0])
                return;

            RemoveDirectory(directoryTextBox.Text);
        }

        private async void RemoveDirectory(string path)
        {
            await Task.Factory.StartNew(() =>
            {
                DisableControls(true);
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = $"Deleting directory \"{path}\"..."));
                try
                {
                    //_ftp.DeleteDirectory(path);
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
                            serverDataTreeView.SelectedNode.Remove()));
                EnableControls();
            });
        }
    }
}