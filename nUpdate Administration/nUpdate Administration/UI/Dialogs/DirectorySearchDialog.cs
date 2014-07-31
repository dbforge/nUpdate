using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Popups;
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

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class DirectorySearchDialog : BaseDialog
    {
        private bool mustCancel = false;
        private bool allowCancel = false;
        private bool isGettingRootData = true;
        private bool isSetByUser = false;

        private List<TreeNode> listedNodes = new List<TreeNode>();
        private Navigator<TreeNode> nav = new Navigator<TreeNode>();
        private FtpWebRequest request = null;

        public DirectorySearchDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The directory selected.
        /// </summary>
        public string SelectedDirectory { get; set; }

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// The host of the FTP-server.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The port of the FTP-server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The username for the FTP's credentials.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password for the FTP's credentials.
        /// </summary>
        public SecureString Password { get; set; }

        /// <summary>
        /// Sets if passive mode should be used.
        /// </summary>
        public bool UsePassiveMode { get; set; }

        /// <summary>
        /// Sets the protocol for the request.
        /// </summary>
        public FTPProtocol Protocol { get; set; }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll")]
        public extern static int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margin);

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (DwmIsCompositionEnabled())
            {

                e.Graphics.Clear(Color.Black);

                Rectangle clientArea = new Rectangle(
                        this.margins.Left,
                        this.margins.Top,
                        this.ClientRectangle.Width - this.margins.Left - this.margins.Right,
                        this.ClientRectangle.Height - this.margins.Top - this.margins.Bottom
                    );
                Brush b = new SolidBrush(this.BackColor);
                e.Graphics.FillRectangle(b, clientArea);
            }
        }

        private MARGINS margins;
        private void DirectorySearchForm_Load(object sender, EventArgs e)
        {
            this.Text = String.Format("Set directory - {0} - nUpdate Administration 1.1.0.0", ProjectName);

            if (DwmIsCompositionEnabled())
            {
                this.margins = new MARGINS();
                this.margins.Top = 40;
                DwmExtendFrameIntoClientArea(this.Handle, ref this.margins);
            }
        }

        private void DirectorySearchForm_Shown(object sender, EventArgs e)
        {
            var node = serverDataTreeView.Nodes[0];
            Thread thread = new Thread(() => this.LoadListAsync(node));
            thread.Start();
        }

        private void DirectorySearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.allowCancel)
            {
                e.Cancel = true;
            }
        }

        private void serverDataTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.serverDataTreeView.SelectedNode.Text.Equals("Server") && this.serverDataTreeView.SelectedNode.Index.Equals(0))
            {
                this.directoryTextBox.Text = "/";
            }
            else
            {
                var node = this.serverDataTreeView.SelectedNode;
                if (!this.listedNodes.Contains(node))
                {
                    Thread thread = new Thread(() => this.LoadListAsync(node));
                    thread.Start();
                }

                this.directoryTextBox.Text = String.Empty;

                IList<TreeNode> ancestorList = this.GetAncestors(node, x => x.Parent).ToList();
                Stack<TreeNode> parents = new Stack<TreeNode>();
                foreach (TreeNode nodeParent in ancestorList)
                {
                    parents.Push(nodeParent);
                }

                foreach (TreeNode parent in parents)
                {
                    if (parent.Index.Equals(0) && parent.Text.Equals("Server"))
                    {
                        this.directoryTextBox.Text = String.Format("/{0}/", node.Text);
                    }
                    else
                    {
                        this.directoryTextBox.Text = String.Format("/{0}/{1}/", parent.Text, node.Text.Split(new char[] { '/' }).Last());
                    }
                }
            }

            if (this.isSetByUser)
            {
                this.nav.Set(e.Node);
            }
            else
            {
                this.isSetByUser = true;
            }

            if (!this.nav.CanGoBack)
            {
                this.backButton.Enabled = false;
            }
            if (this.nav.CanGoBack)
            {
                this.backButton.Enabled = true;
            }
            if (!this.nav.CanGoForward)
            {
                this.forwardButton.Enabled = false;
            }
            if (this.nav.CanGoForward)
            {
                this.forwardButton.Enabled = true;
            }
        }

        /// <summary>
        /// Iterates through the parents of a given child node and returns them.
        /// </summary>
        private IEnumerable<TItem> GetAncestors<TItem>(TItem item, Func<TItem, TItem> getParentFunc)
        {
            if (ReferenceEquals(item, null))
            {
                yield break;
            }

            for (TItem curItem = getParentFunc(item); !ReferenceEquals(curItem, null); curItem = getParentFunc(curItem))
            {
                yield return curItem;
            }
        }

        private void CreateRequest(string directory)
        {
            if (directory.Equals(String.Empty))
            {
                this.request = (FtpWebRequest)WebRequest.Create(String.Format("ftp://{0}:{1}", this.Host, this.Port));
            }
            else
            {
                this.request = (FtpWebRequest)WebRequest.Create(String.Format("ftp://{0}:{1}/{2}", this.Host, this.Port, directory));
            }

            if (!Properties.Settings.Default.SaveCredentials)
            {
                Invoke(new Action(() =>
                {
                    var credentialsForm = new CredentialsDialog();
                    if (credentialsForm.ShowDialog() == DialogResult.OK)
                    {
                        this.Username = credentialsForm.Username;
                        this.Password = credentialsForm.Password;
                    }
                    else
                    {
                        this.mustCancel = true;
                    }
                }));
            }
            else
            {
                if (!this.mustCancel)
                {
                    this.request.Credentials = new NetworkCredential(this.Username, this.Password);
                }
            }

            this.request.Method = WebRequestMethods.Ftp.ListDirectory;
        }

        public string[] ListDirectories(string directory)
        {
            var list = new List<string>();
            this.CreateRequest(directory);

            if (!this.mustCancel)
            {
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, true))
                        {
                            while (!reader.EndOfStream)
                            {
                                list.Add(reader.ReadLine());
                            }
                        }
                    }
                }
            }

            request.Abort();
            return list.ToArray();
        }

        private FtpWebResponse response = null;
        private void LoadListAsync(TreeNode currentNode)
        {
            Invoke(new Action(() =>
            {
                this.serverDataTreeView.Enabled = false;
                this.loadPictureBox.Visible = true;
                this.cancelButton.Enabled = false;
                this.continueButton.Enabled = false;
            }));

            string[] dirs;

            try
            {
                if (currentNode.Index.Equals(0) && currentNode.Text.Equals("Server"))
                {
                    dirs = ListDirectories(String.Empty);
                }

                else
                {
                    dirs = ListDirectories(currentNode.Text);
                }

                if (this.mustCancel)
                {
                    this.allowCancel = true;
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }

                foreach (string dir in dirs)
                {
                    Invoke(new Action(() =>
                    {
                        if (Path.GetExtension(dir).Equals(String.Empty) && !dir.EndsWith("."))
                        {
                            //if (currentNode.Text == "Server" && currentNode.Index == 0)
                            currentNode.Nodes.Add(new TreeNode(dir, 1, 1));
                            //else
                            //    currentNode.Nodes.Add(new TreeNode(String.Format("{0}/{1}", currentNode.Text, dir), 1, 1));
                        }
                    }));
                }

                Invoke(new Action(() =>
                {
                    this.serverDataTreeView.Enabled = true;
                    this.loadPictureBox.Visible = false;
                    this.cancelButton.Enabled = true;
                    this.continueButton.Enabled = true;
                }));

                this.isGettingRootData = false;
                this.allowCancel = true;

                this.listedNodes.Add(currentNode);
            }

            catch (Exception ex)
            {
                this.allowCancel = true;
                if (ex.GetType() == typeof(WebException))
                {
                    this.response = (FtpWebResponse)(ex as WebException).Response;
                    if (this.response.StatusCode == FtpStatusCode.NotLoggedIn)
                    {
                        if (this.isGettingRootData)
                        {
                            Invoke(new Action(() =>
                            {
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while listing server-data.", "The login credentials are wrong.", PopupButtons.OK);
                                this.Close();
                            }));
                        }

                        else
                        {
                            Invoke(new Action(() =>
                            {
                                this.serverDataTreeView.Enabled = true;
                                this.loadPictureBox.Visible = false;
                                this.cancelButton.Enabled = true;
                                this.continueButton.Enabled = true;
                            }));
                        }
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            if (this.isGettingRootData)
                            {
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while listing server-data.", ex, PopupButtons.OK);
                                this.Close();
                            }
                            else
                            {
                                Invoke(new Action(() =>
                                {
                                    this.serverDataTreeView.Enabled = true;
                                    this.loadPictureBox.Visible = false;
                                    this.cancelButton.Enabled = true;
                                    this.continueButton.Enabled = true;
                                }));
                            }
                        }));
                    }
                }
                else
                {
                    Invoke(new Action(() =>
                    {
                        if (this.isGettingRootData)
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while listing server-data.", ex, PopupButtons.OK);
                            this.Close();
                        }
                        else
                        {
                            Invoke(new Action(() =>
                            {
                                this.serverDataTreeView.Enabled = true;
                                this.loadPictureBox.Visible = false;
                                this.cancelButton.Enabled = true;
                                this.continueButton.Enabled = true;
                            }));
                        }
                    }));
                }
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            this.SelectedDirectory = this.directoryTextBox.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            this.isSetByUser = false;

            if (!this.nav.CanGoBack)
            {
                this.backButton.Enabled = false;
            }
            if (this.nav.CanGoBack)
            {
                this.backButton.Enabled = true;
            }
            if (!this.nav.CanGoForward)
            {
                this.forwardButton.Enabled = false;
            }
            if (this.nav.CanGoForward)
            {
                this.forwardButton.Enabled = true;
            }

            this.nav.Back();
            if (this.nav.Current != null)
            {
                this.serverDataTreeView.SelectedNode = this.nav.Current;
            }
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            this.isSetByUser = false;

            if (!this.nav.CanGoBack)
            {
                this.backButton.Enabled = false;
            }
            if (this.nav.CanGoBack)
            {
                this.backButton.Enabled = true;
            }
            if (!this.nav.CanGoForward)
            {
                this.forwardButton.Enabled = false;
            }
            if (this.nav.CanGoForward)
            {
                this.forwardButton.Enabled = true;
            }

            this.nav.Forward();
            if (this.nav.Current != null)
            {
                this.serverDataTreeView.SelectedNode = this.nav.Current;
            }
        }
    }
}
