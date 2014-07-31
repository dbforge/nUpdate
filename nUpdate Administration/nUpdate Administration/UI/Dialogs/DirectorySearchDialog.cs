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
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class DirectorySearchDialog : BaseDialog
    {
        private readonly List<TreeNode> listedNodes = new List<TreeNode>();
        private readonly Navigator<TreeNode> nav = new Navigator<TreeNode>();
        private bool allowCancel;
        private bool isGettingRootData = true;
        private bool isSetByUser;
        private MARGINS margins;
        private bool mustCancel;

        private FtpWebRequest request;
        private FtpWebResponse response;

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

        /// <summary>
        ///     Sets the protocol for the request.
        /// </summary>
        public FTPProtocol Protocol { get; set; }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margin);

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (DwmIsCompositionEnabled())
            {
                e.Graphics.Clear(Color.Black);

                var clientArea = new Rectangle(
                    margins.Left,
                    margins.Top,
                    ClientRectangle.Width - margins.Left - margins.Right,
                    ClientRectangle.Height - margins.Top - margins.Bottom
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
                margins = new MARGINS();
                margins.Top = 40;
                DwmExtendFrameIntoClientArea(Handle, ref margins);
            }
        }

        private void DirectorySearchForm_Shown(object sender, EventArgs e)
        {
            TreeNode node = serverDataTreeView.Nodes[0];
            var thread = new Thread(() => LoadListAsync(node));
            thread.Start();
        }

        private void DirectorySearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowCancel)
                e.Cancel = true;
        }

        private void serverDataTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (serverDataTreeView.SelectedNode.Text.Equals("Server") && serverDataTreeView.SelectedNode.Index.Equals(0))
                directoryTextBox.Text = "/";
            else
            {
                TreeNode node = serverDataTreeView.SelectedNode;
                if (!listedNodes.Contains(node))
                {
                    var thread = new Thread(() => LoadListAsync(node));
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

            if (isSetByUser)
                nav.Set(e.Node);
            else
                isSetByUser = true;

            if (!nav.CanGoBack)
                backButton.Enabled = false;
            if (nav.CanGoBack)
                backButton.Enabled = true;
            if (!nav.CanGoForward)
                forwardButton.Enabled = false;
            if (nav.CanGoForward)
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

        private void CreateRequest(string directory)
        {
            if (directory.Equals(String.Empty))
                request = (FtpWebRequest) WebRequest.Create(String.Format("ftp://{0}:{1}", Host, Port));
            else
                request = (FtpWebRequest) WebRequest.Create(String.Format("ftp://{0}:{1}/{2}", Host, Port, directory));

            if (!Settings.Default.SaveCredentials)
            {
                Invoke(new Action(() =>
                {
                    var credentialsForm = new CredentialsDialog();
                    if (credentialsForm.ShowDialog() == DialogResult.OK)
                    {
                        Username = credentialsForm.Username;
                        Password = credentialsForm.Password;
                    }
                    else
                        mustCancel = true;
                }));
            }
            else
            {
                if (!mustCancel)
                    request.Credentials = new NetworkCredential(Username, Password);
            }

            request.Method = WebRequestMethods.Ftp.ListDirectory;
        }

        public string[] ListDirectories(string directory)
        {
            var list = new List<string>();
            CreateRequest(directory);

            if (!mustCancel)
            {
                using (var response = (FtpWebResponse) request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
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

        private void LoadListAsync(TreeNode currentNode)
        {
            Invoke(new Action(() =>
            {
                serverDataTreeView.Enabled = false;
                loadPictureBox.Visible = true;
                cancelButton.Enabled = false;
                continueButton.Enabled = false;
            }));

            string[] dirs;

            try
            {
                if (currentNode.Index.Equals(0) && currentNode.Text.Equals("Server"))
                    dirs = ListDirectories(String.Empty);

                else
                    dirs = ListDirectories(currentNode.Text);

                if (mustCancel)
                {
                    allowCancel = true;
                    DialogResult = DialogResult.Cancel;
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
                    serverDataTreeView.Enabled = true;
                    loadPictureBox.Visible = false;
                    cancelButton.Enabled = true;
                    continueButton.Enabled = true;
                }));

                isGettingRootData = false;
                allowCancel = true;

                listedNodes.Add(currentNode);
            }

            catch (Exception ex)
            {
                allowCancel = true;
                if (ex.GetType() == typeof (WebException))
                {
                    response = (FtpWebResponse) (ex as WebException).Response;
                    if (response.StatusCode == FtpStatusCode.NotLoggedIn)
                    {
                        if (isGettingRootData)
                        {
                            Invoke(new Action(() =>
                            {
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while listing server-data.",
                                    "The login credentials are wrong.", PopupButtons.OK);
                                Close();
                            }));
                        }

                        else
                        {
                            Invoke(new Action(() =>
                            {
                                serverDataTreeView.Enabled = true;
                                loadPictureBox.Visible = false;
                                cancelButton.Enabled = true;
                                continueButton.Enabled = true;
                            }));
                        }
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            if (isGettingRootData)
                            {
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while listing server-data.", ex,
                                    PopupButtons.OK);
                                Close();
                            }
                            else
                            {
                                Invoke(new Action(() =>
                                {
                                    serverDataTreeView.Enabled = true;
                                    loadPictureBox.Visible = false;
                                    cancelButton.Enabled = true;
                                    continueButton.Enabled = true;
                                }));
                            }
                        }));
                    }
                }
                else
                {
                    Invoke(new Action(() =>
                    {
                        if (isGettingRootData)
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while listing server-data.", ex,
                                PopupButtons.OK);
                            Close();
                        }
                        else
                        {
                            Invoke(new Action(() =>
                            {
                                serverDataTreeView.Enabled = true;
                                loadPictureBox.Visible = false;
                                cancelButton.Enabled = true;
                                continueButton.Enabled = true;
                            }));
                        }
                    }));
                }
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            SelectedDirectory = directoryTextBox.Text;
            DialogResult = DialogResult.OK;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            isSetByUser = false;

            if (!nav.CanGoBack)
                backButton.Enabled = false;
            if (nav.CanGoBack)
                backButton.Enabled = true;
            if (!nav.CanGoForward)
                forwardButton.Enabled = false;
            if (nav.CanGoForward)
                forwardButton.Enabled = true;

            nav.Back();
            if (nav.Current != null)
                serverDataTreeView.SelectedNode = nav.Current;
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            isSetByUser = false;

            if (!nav.CanGoBack)
                backButton.Enabled = false;
            if (nav.CanGoBack)
                backButton.Enabled = true;
            if (!nav.CanGoForward)
                forwardButton.Enabled = false;
            if (nav.CanGoForward)
                forwardButton.Enabled = true;

            nav.Forward();
            if (nav.Current != null)
                serverDataTreeView.SelectedNode = nav.Current;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }
    }
}