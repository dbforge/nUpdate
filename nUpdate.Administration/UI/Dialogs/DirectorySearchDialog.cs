// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Win32;
using nUpdate.Administration.TransferInterface;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class DirectorySearchDialog : BaseDialog, IAsyncSupportable
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private readonly Navigator<TreeNode> _nav = new Navigator<TreeNode>();
        private bool _allowCancel;
        private FtpManager _ftp;
        private List<ServerItem> _listedFtpItems = new List<ServerItem>();
        private Margins _margins;
        private bool _nodeSelectedByUser = true;

        public DirectorySearchDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The directory selected.
        /// </summary>
        public string SelectedDirectory { get; set; }

        /// <summary>
        ///     The name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        ///     The host of the FTP-server.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     The port of the FTP-server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     The username for the credentials.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     The password for the credentials.
        /// </summary>
        public SecureString Password { get; set; }

        /// <summary>
        ///     Sets if passive mode should be used.
        /// </summary>
        public bool UsePassiveMode { get; set; }

        /// <summary>
        ///     Sets the protocol to use.
        /// </summary>
        public int Protocol { get; set; }

        /// <summary>
        ///     Gets or sets the path of the assembly containing custom transfer handlers.
        /// </summary>
        public string FtpAssemblyPath { get; set; }

        /// <summary>
        ///     Enables or disables the UI controls.
        /// </summary>
        /// <param name="enabled">Sets the activation state.</param>
        public void SetUiState(bool enabled)
        {
            BeginInvoke(new Action(() =>
            {
                foreach (
                    var c in
                        (from Control c in Controls where c.Visible select c).Where(
                            c => c.GetType() != typeof (ExplorerNavigationButton.ExplorerNavigationButton)))
                {
                    c.Enabled = enabled;
                }

                if (!enabled)
                {
                    _allowCancel = false;
                    loadingPanel.Visible = true;
                }
                else
                {
                    _allowCancel = true;
                    loadingPanel.Visible = false;
                }
            }));
        }

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

        private void DirectorySearchDialog_Shown(object sender, EventArgs e)
        {
            Text = string.Format(Text, ProjectName, Program.VersionString);
            if (NativeMethods.DwmIsCompositionEnabled())
            {
                _margins = new Margins {Top = 38};
                NativeMethods.DwmExtendFrameIntoClientArea(Handle, ref _margins);
            }

            _ftp =
                new FtpManager(Host, Port, null, Username,
                    Password, null, UsePassiveMode, FtpAssemblyPath, Protocol);

            LoadListAsync();
        }

        private void DirectorySearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
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
                _nav.Add(e.Node);
            if (!backButton.Enabled && _nav.CanGoBack)
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

        private async void LoadListAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                SetUiState(false);
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Loading server directories..."));

                try
                {
                    _listedFtpItems = _ftp.ListDirectoriesAndFiles("/", true).ToList();
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                            {
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while listing the server data.", ex,
                                    PopupButtons.Ok);
                                SetUiState(true);
                            }));

                    DialogResult = DialogResult.OK;
                    return;
                }

                if (_listedFtpItems != null)
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
                }

                SetUiState(true);
            });
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

        public static ListingItem ConvertToListingItem(IEnumerable<ServerItem> inputItems, string separator)
        {
            var root = new ListingItem("Root", false);
            foreach (var item in inputItems)
            {
                var currentParent = root;
                foreach (
                    var pathSegment in item.FullPath.Remove(0, 1).Split(new[] {separator}, StringSplitOptions.None))
                {
                    var child = currentParent.Children.FirstOrDefault(t => t.Text == pathSegment);
                    if (child == null)
                    {
                        child = new ListingItem(pathSegment, item.ItemType == ServerItemType.Directory);
                        currentParent.Children.Add(child);
                    }
                    currentParent = child;
                }
            }

            return root;
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            SelectedDirectory = directoryTextBox.Text;
            DialogResult = DialogResult.OK;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            _nav.GoBack();
            _nodeSelectedByUser = false;
            serverDataTreeView.SelectedNode = _nav.Current;
            _nodeSelectedByUser = true;
            if (!_nav.CanGoBack)
                backButton.Enabled = false;
            if (_nav.CanGoForward)
                forwardButton.Enabled = true;
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            _nav.GoForward();
            _nodeSelectedByUser = false;
            serverDataTreeView.SelectedNode = _nav.Current;
            _nodeSelectedByUser = true;
            if (!_nav.CanGoForward)
                forwardButton.Enabled = false;
            if (_nav.CanGoBack)
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
            await Task.Factory.StartNew(() =>
            {
                SetUiState(false);
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = $"Creating directory \"{path}\"..."));
                try
                {
                    _ftp.MakeDirectory(path);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while creating the directory.", ex,
                                    PopupButtons.Ok)));
                }
                SetUiState(true);
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
                SetUiState(false);
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = $"Deleting directory \"{path}\"..."));
                try
                {
                    _ftp.DeleteDirectory(path);
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while deleting the directory.", ex,
                                    PopupButtons.Ok)));
                    SetUiState(true);
                    return;
                }

                Invoke(
                    new Action(
                        () =>
                            serverDataTreeView.SelectedNode.Remove()));
                SetUiState(true);
            });
        }
    }
}