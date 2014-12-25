// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

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
using Starksoft.Net.Ftp;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class DirectorySearchDialog : BaseDialog, IAsyncSupportable
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        private readonly Navigator<TreeNode> _nav = new Navigator<TreeNode>();
        private FtpManager _ftp;

        private bool _allowCancel;
        private bool _isGettingRootData = true;
        private bool _isSetByUser;
        private Margins _margins;
        private bool _mustCancel = false;

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

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (!NativeMethods.DwmIsCompositionEnabled()) return;

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

        private void DirectorySearchForm_Load(object sender, EventArgs e)
        {
            Text = String.Format("Set directory - {0} - nUpdate Administration 0.1.0.0", ProjectName);

            if (!NativeMethods.DwmIsCompositionEnabled())
                return;

            _margins = new Margins {Top = 38};
            NativeMethods.DwmExtendFrameIntoClientArea(Handle, ref _margins);

            _ftp = new FtpManager
            {
                Host = Host,
                Port = Port,
                Username = Username,
                Password = Password,
                UsePassiveMode = UsePassiveMode,
                Protocol = (FtpSecurityProtocol) Protocol
            };
        }

        private void DirectorySearchForm_Shown(object sender, EventArgs e)
        {
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
            //if (serverDataTreeView.SelectedNode.Text.Equals("Server") && serverDataTreeView.SelectedNode.Index.Equals(0))
            //    directoryTextBox.Text = "/";
            //else
            //{
            //    if (parent.Index.Equals(0) && parent.Text.Equals("Server"))
            //        directoryTextBox.Text = String.Format("/{0}/", node.Text);
            //    else
            //    {
            //        directoryTextBox.Text = String.Format("/{0}/{1}/", parent.Text,
            //            node.Text.Split('/').Last());
            //    }
            //}
        }

        private void LoadListAsync()
        {
            SetUiState(false);
            var items = _ftp.ListDirectoriesAndFiles("/", true);
            
            foreach (var item in items)
            {
                if (item.ParentPath.Length < 2) // Has no parent-directory
                {
                    var itemPlaceholder = item;
                    Invoke(new Action(() =>
                    {
                        serverDataTreeView.Nodes[0].Nodes.Add(String.Format("/{0}", itemPlaceholder.Name));
                    }));
                }
                else
                {
                    var itemPlaceholder = item;
                    string[] pathParts = itemPlaceholder.ParentPath.Split('/');
                    BeginInvoke(
                            new Action(
                                () =>
                                    serverDataTreeView.SelectedNode =
                                        serverDataTreeView.Nodes[0].Nodes.Cast<TreeNode>()
                                            .First(node => node.Name == itemPlaceholder.Name)));

                    for (int i = 0; i <= pathParts.Length - 1; ++i)
                    {
                        var i1 = i;
                        var newNode = new TreeNode(pathParts[i1]);
                        BeginInvoke(
                            new Action(
                                () =>
                                {
                                    serverDataTreeView.SelectedNode.Nodes.Add(newNode);
                                    serverDataTreeView.SelectedNode = newNode;
                                }));
                    }
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
        public struct Margins
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {

        }

        public void SetUiState(bool enabled)
        {
            Invoke(new Action(() =>
            {
                serverDataTreeView.Enabled = enabled;
                loadPictureBox.Visible = !enabled;
                cancelButton.Enabled = enabled;
                continueButton.Enabled = enabled;
            }));
        }
    }
}