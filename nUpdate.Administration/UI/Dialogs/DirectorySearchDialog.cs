// Copyright © Dominic Beger 2018

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
using Starksoft.Aspen.Ftps;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class DirectorySearchDialog : BaseDialog, IAsyncSupportable
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private readonly List<TreeNode> _handledNodes = new List<TreeNode>();
        private readonly Navigator<TreeNode> _nav = new Navigator<TreeNode>();
        private bool _allowCancel;
        private FtpManager _ftp;
        private Margins _margins;
        private bool _nodeSelectedByUser = true;

        public DirectorySearchDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the path of the assembly containing custom transfer handlers.
        /// </summary>
        public string FtpAssemblyPath { get; set; }

        /// <summary>
        ///     The host of the FTP-server.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     Gets or sets the network version.
        /// </summary>
        public NetworkVersion NetworkVersion { get; set; }

        /// <summary>
        ///     The password for the credentials.
        /// </summary>
        public SecureString Password { get; set; }

        /// <summary>
        ///     The port of the FTP-server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     The name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        ///     Sets the protocol to use.
        /// </summary>
        public int Protocol { get; set; }

        /// <summary>
        ///     The directory selected.
        /// </summary>
        public string SelectedDirectory { get; set; }

        /// <summary>
        ///     Sets if passive mode should be used.
        /// </summary>
        public bool UsePassiveMode { get; set; }

        /// <summary>
        ///     The username for the credentials.
        /// </summary>
        public string Username { get; set; }

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
                        c => c.GetType() != typeof(ExplorerNavigationButton.ExplorerNavigationButton)))
                    c.Enabled = enabled;

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

        private void addDirectoryButton_Click(object sender, EventArgs e)
        {
            var node = new TreeNode("Name", 1, 1);
            if (serverDataTreeView.SelectedNode != null)
            {
                serverDataTreeView.SelectedNode.Nodes.Add(node);
                serverDataTreeView.SelectedNode.Expand();
            }
            else
            {
                serverDataTreeView.Nodes[0].Nodes.Add(node);
            }

            serverDataTreeView.LabelEdit = true;
            node.BeginEdit();
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

        private void continueButton_Click(object sender, EventArgs e)
        {
            SelectedDirectory = directoryTextBox.Text;
            DialogResult = DialogResult.OK;
        }

        private async Task CreateDirectory(string path)
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

        private void DirectorySearchDialog_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            NativeMethods.ReleaseCapture();
            NativeMethods.SendMessage(Handle, WM_NCLBUTTONDOWN, new IntPtr(HT_CAPTION), new IntPtr(0));
        }

        private async void DirectorySearchDialog_Shown(object sender, EventArgs e)
        {
            Text = string.Format(Text, ProjectName, Program.VersionString);
            if (NativeMethods.DwmIsCompositionEnabled())
            {
                _margins = new Margins {Top = 38};
                NativeMethods.DwmExtendFrameIntoClientArea(Handle, ref _margins);
            }

            _ftp =
                new FtpManager(Host, Port, null, Username,
                    Password, null, UsePassiveMode, FtpAssemblyPath, Protocol, NetworkVersion);

            var node = new TreeNode("Server", 0, 0);
            serverDataTreeView.Nodes.Add(node);
            await LoadListAsync("/", node);
            node.Expand();
        }

        private void DirectorySearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
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

        private async Task LoadListAsync(string path, TreeNode node)
        {
            await Task.Factory.StartNew(() =>
            {
                SetUiState(false);
                Invoke(
                    new Action(
                        () =>
                            loadingLabel.Text = "Loading directories..."));

                List<ServerItem> directoryItems;
                try
                {
                    directoryItems = _ftp.ListDirectoriesAndFiles(path, false).ToList();
                }
                catch (Exception ex)
                {
                    Invoke(
                        new Action(
                            () =>
                            {
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while listing the directories.", ex,
                                    PopupButtons.Ok);
                                SetUiState(true);
                            }));

                    SetUiState(true);
                    DialogResult = DialogResult.OK;
                    return;
                }

                foreach (var serverItem in directoryItems.Where(i => i.ItemType == ServerItemType.Directory))
                    Invoke(new Action(() =>
                        node.Nodes.Add(new TreeNode(serverItem.Name, 1, 1))));

                SetUiState(true);
            });
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

        private async Task RemoveDirectory(string path)
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

        private async void removeDirectoryButton_Click(object sender, EventArgs e)
        {
            if (serverDataTreeView.SelectedNode == null ||
                serverDataTreeView.SelectedNode == serverDataTreeView.Nodes[0])
                return;

            await RemoveDirectory(directoryTextBox.Text);
        }

        private async void serverDataTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                    "Please enter a name for the directory to create.", PopupButtons.Ok);
                e.Node.BeginEdit();
                return;
            }

            serverDataTreeView.LabelEdit = false;
            await CreateDirectory($"{directoryTextBox.Text}/{e.Label}");
            _handledNodes.Add(e.Node);
        }

        private async void serverDataTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == serverDataTreeView.Nodes[0])
            {
                directoryTextBox.Text = "/";
                return;
            }

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

            if (!_handledNodes.Contains(e.Node))
            {
                await LoadListAsync(directory, e.Node);
                _handledNodes.Add(e.Node);
            }

            serverDataTreeView.SelectedNode = e.Node;
            e.Node.Expand();
        }
    }
}