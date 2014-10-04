// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class DirectorySearchDialog : BaseDialog
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        private readonly List<TreeNode> _listedNodes = new List<TreeNode>();
        private readonly Navigator<TreeNode> _nav = new Navigator<TreeNode>();
        private bool _allowCancel;
        private bool _isGettingRootData = true;
        private bool _isSetByUser;
        private MARGINS _margins;
        private bool mustCancel = false;

        private FtpWebRequest _request;
        private FtpWebResponse _response;

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
        ///     The username for the FTP's credentials.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     The password for the FTP's credentials.
        /// </summary>
        public SecureString Password { get; set; }

        /// <summary>
        ///     Sets if passive mode should be used.
        /// </summary>
        public bool UsePassiveMode { get; set; }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (DwmIsCompositionEnabled())
            {
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
        }

        private void DirectorySearchForm_Load(object sender, EventArgs e)
        {
            Text = String.Format("Set directory - {0} - nUpdate Administration 1.1.0.0", ProjectName);

            if (DwmIsCompositionEnabled())
            {
                _margins = new MARGINS();
                _margins.Top = 40;
                DwmExtendFrameIntoClientArea(Handle, ref _margins);
            }
        }

        private void DirectorySearchForm_Shown(object sender, EventArgs e)
        {
            var thread = new Thread(() => LoadListAsync());
            thread.Start();
        }

        private void DirectorySearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
            int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void DirectorySearchDialog_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void serverDataTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (serverDataTreeView.SelectedNode.Text.Equals("Server") && serverDataTreeView.SelectedNode.Index.Equals(0))
                directoryTextBox.Text = "/";
            else
            {
                TreeNode node = serverDataTreeView.SelectedNode;
                if (!_listedNodes.Contains(node))
                {
                    var thread = new Thread(() => LoadListAsync());
                    thread.Start();
                }

                directoryTextBox.Text = String.Empty;

                IList<TreeNode> ancestorList = GetAncestors(node, x => x.Parent).ToList();
                var parents = new Stack<TreeNode>();
                foreach (TreeNode nodeParent in ancestorList)
                {
                    parents.Push(nodeParent);
                }

                foreach (TreeNode parent in parents)
                {
                    if (parent.Index.Equals(0) && parent.Text.Equals("Server"))
                        directoryTextBox.Text = String.Format("/{0}/", node.Text);
                    else
                    {
                        directoryTextBox.Text = String.Format("/{0}/{1}/", parent.Text,
                            node.Text.Split(new[] {'/'}).Last());
                    }
                }
            }

            if (_isSetByUser)
                _nav.Set(e.Node);
            else
                _isSetByUser = true;

            if (!_nav.CanGoBack)
                backButton.Enabled = false;
            if (_nav.CanGoBack)
                backButton.Enabled = true;
            if (!_nav.CanGoForward)
                forwardButton.Enabled = false;
            if (_nav.CanGoForward)
                forwardButton.Enabled = true;
        }

        /// <summary>
        ///     Iterates through the parents of a given child node and returns them.
        /// </summary>
        private IEnumerable<TItem> GetAncestors<TItem>(TItem item, Func<TItem, TItem> getParentFunc)
        {
            if (ReferenceEquals(item, null))
                yield break;

            for (TItem curItem = getParentFunc(item); !ReferenceEquals(curItem, null); curItem = getParentFunc(curItem))
            {
                yield return curItem;
            }
        }

        private void LoadListAsync()
        {
            Invoke(new Action(() =>
            {
                serverDataTreeView.Enabled = false;
                loadPictureBox.Visible = true;
                cancelButton.Enabled = false;
                continueButton.Enabled = false;
            }));

            var ftp = new FtpManager {Host = Host, Port = Port, UserName = Username, Password = Password};
            ftp.ListDirectoriesAndFilesRecursively();

            TreeNode currentNode = null;
            foreach (var item in ftp.ListedFtpItems)
            {
                if (item.ParentPath.Length < 2) // Has no parent-directory
                {
                    var itemPlaceholder = item;
                    Invoke(new Action(() =>
                    {
                        serverDataTreeView.Nodes[0].Nodes.Add(itemPlaceholder.FullPath);
                        currentNode = serverDataTreeView.Nodes[0].LastNode;
                    }));
                }
                else
                {
                    var itemPlaceholder = item;
                    Invoke(new Action(
                        () => { if (currentNode != null) currentNode.Nodes.Add(itemPlaceholder.FullPath); }));
                    //if (itemPlaceholder.FullPath.Split(new char[] {'/'}).Length > 2)
                }
            }

            Invoke(new Action(() =>
            {
                serverDataTreeView.Enabled = true;
                loadPictureBox.Visible = false;
                cancelButton.Enabled = true;
                continueButton.Enabled = true;
            }));

            _isGettingRootData = false;
            _allowCancel = true;
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            SelectedDirectory = directoryTextBox.Text;
            DialogResult = DialogResult.OK;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            _isSetByUser = false;

            if (!_nav.CanGoBack)
                backButton.Enabled = false;
            if (_nav.CanGoBack)
                backButton.Enabled = true;
            if (!_nav.CanGoForward)
                forwardButton.Enabled = false;
            if (_nav.CanGoForward)
                forwardButton.Enabled = true;

            _nav.Back();
            if (_nav.Current != null)
                serverDataTreeView.SelectedNode = _nav.Current;
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            _isSetByUser = false;

            if (!_nav.CanGoBack)
                backButton.Enabled = false;
            if (_nav.CanGoBack)
                backButton.Enabled = true;
            if (!_nav.CanGoForward)
                forwardButton.Enabled = false;
            if (_nav.CanGoForward)
                forwardButton.Enabled = true;

            _nav.Forward();
            if (_nav.Current != null)
                serverDataTreeView.SelectedNode = _nav.Current;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {

        }
    }
}