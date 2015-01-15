// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Win32;
using nUpdate.Administration.UI.Popups;
using Starksoft.Net.Ftp;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class DirectorySearchDialog : BaseDialog, IAsyncSupportable
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        private bool _allowCancel;
        private FtpManager _ftp;
        private List<FtpItem> _listedFtpItems = new List<FtpItem>();
        private Margins _margins;
        private bool _nodeSelectedByUser = true;
        private readonly Navigator<TreeNode> _nav = new Navigator<TreeNode>();

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
            Text = String.Format("Set directory - {0} - nUpdate Administration 0.1.0.0", ProjectName);

            if (NativeMethods.DwmIsCompositionEnabled())
            {
                _margins = new Margins {Top = 38};
                NativeMethods.DwmExtendFrameIntoClientArea(Handle, ref _margins);
            }

            _ftp = new FtpManager
            {
                Host = Host,
                Port = Port,
                Username = Username,
                Password = Password,
                UsePassiveMode = UsePassiveMode,
                Protocol = (FtpSecurityProtocol) Protocol
            };

            ThreadPool.QueueUserWorkItem(arg => LoadListAsync());
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

            var directory = String.Format("/{0}/{1}", String.Join("/", directories), e.Node.Text);
            directoryTextBox.Text = directory.StartsWith("//") ? directory.Remove(0, 1) : directory;
        }

        private void LoadListAsync()
        {
            SetUiState(false);
            BeginInvoke(
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
                            Close();
                        }));
            }

            if (_listedFtpItems != null)
            {
                var root = ConvertToListingItem(_listedFtpItems, "/");
                var rootNode = new TreeNode("Server", 0, 0);
                BeginInvoke(new Action(() =>
                {
                    RecursiveAdd(root, rootNode);
                    serverDataTreeView.Nodes.Add(rootNode);
                    serverDataTreeView.Nodes[0].Expand();
                    serverDataTreeView.SelectedNode = rootNode;
                }));
            }

            SetUiState(true);
            _allowCancel = true;
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

        public static ListingItem ConvertToListingItem(IEnumerable<FtpItem> inputItems, string seperator)
        {
            var root = new ListingItem("Root", false);
            foreach (var item in inputItems)
            {
                var currentParent = root;
                foreach (
                    var pathSegment in item.FullPath.Remove(0, 1).Split(new[] {seperator}, StringSplitOptions.None))
                {
                    if (currentParent.Children.All(t => t.Text != pathSegment))
                    {
                        var nodeToken = new ListingItem(pathSegment, item.ItemType == FtpItemType.Directory);
                        currentParent.Children.Add(nodeToken);
                        currentParent = nodeToken;
                    }
                    else
                    {
                        currentParent = currentParent.Children.First(t => t.Text == pathSegment);
                    }
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

        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }
    }
}