using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Update;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;
using nUpdate.Administration.UI.Popups;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class DirectorySearchForm : BaseForm
    {
        bool allowCancel;
        bool isGettingRootData = true;
        bool isSetByUser = false;

        private List<TreeNode> listedNodes = new List<TreeNode>();
        private Navigator<TreeNode> nav = new Navigator<TreeNode>();
        FtpWebRequest request = null;

        public DirectorySearchForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The directory selected.
        /// </summary>
        public string SelectedDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string ProjectName
        {
            get;
            set;
        }

        /// <summary>
        /// The host of the FTP-server.
        /// </summary>
        public string Host
        {
            get;
            set;
        }

        /// <summary>
        /// The port of the FTP-server.
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// The username for the FTP's credentials.
        /// </summary>
        public string Username
        {
            get;
            set;
        }

        /// <summary>
        /// The password for the FTP's credentials.
        /// </summary>
        public SecureString Password
        {
            get;
            set;
        }

        /// <summary>
        /// Sets if passive mode should be used.
        /// </summary>
        public bool UsePassiveMode
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the protocol for the request.
        /// </summary>
        public FtpProtocol Protocol
        {
            get;
            set;
        }

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
        public extern static int DwmExtendFrameIntoClientArea(IntPtr hwnd,
                                 ref MARGINS margin);

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (DwmIsCompositionEnabled())
            {

                e.Graphics.Clear(Color.Black);

                Rectangle clientArea = new Rectangle(
                        margins.Left,
                        margins.Top,
                        this.ClientRectangle.Width - margins.Left - margins.Right,
                        this.ClientRectangle.Height - margins.Top - margins.Bottom
                    );
                Brush b = new SolidBrush(this.BackColor);
                e.Graphics.FillRectangle(b, clientArea);
            }
        }

        private MARGINS margins;
        private void DirectorySearchForm_Load(object sender, EventArgs e)
        {
            Text = String.Format("Set directory - {0} - nUpdate Administration 1.1.0.0", ProjectName);

            if (DwmIsCompositionEnabled())
            {
                margins = new MARGINS();
                margins.Top = 40;
                DwmExtendFrameIntoClientArea(this.Handle, ref margins);
            }
        }

        private void DirectorySearchForm_Shown(object sender, EventArgs e)
        {
            bool mustCancel = false;

            if (!Properties.Settings.Default.SaveCredentials)
            {
                Invoke(new Action(() =>
                {
                    var credentialsForm = new CredentialsForm();
                    if (credentialsForm.ShowDialog() == DialogResult.OK)
                    {
                        Username = credentialsForm.Username;
                        Password = credentialsForm.Password;
                    }
                    else
                    {
                        mustCancel = true;
                    }
                }));
            }
            else
            {
                request.Credentials = new NetworkCredential(Username, Password);
            }

            if (mustCancel) {

                allowCancel = true;
                DialogResult = DialogResult.Cancel;
                Close();

                return; // Exit this void
            }

            var node = serverDataTreeView.Nodes[0];
            Thread thread = new Thread(() => LoadListAsync(node));
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
                var node = serverDataTreeView.SelectedNode;
                if (!listedNodes.Contains(node))
                {
                    Thread thread = new Thread(() => LoadListAsync(node));
                    thread.Start();
                }

                directoryTextBox.Text = String.Empty;

                IList<TreeNode> ancestorList = GetAncestors(node, x => x.Parent).ToList();
                Stack<TreeNode> parents = new Stack<TreeNode>();
                foreach (TreeNode nodeParent in ancestorList)
                {
                    parents.Push(nodeParent);
                }

                foreach (TreeNode parent in parents)
                {
                    if (parent.Index.Equals(0) && parent.Text.Equals("Server"))
                        directoryTextBox.Text = String.Format("/{0}/", node.Text);
                    else
                        directoryTextBox.Text = String.Format("/{0}/{1}/", parent.Text, node.Text.Split(new char[] { '/' }).Last());
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
        /// Iterates through the parents of a given child node and returns them.
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

        private FtpWebRequest CreateRequest(string directory)
        {
            if (directory.Equals(String.Empty))
                request = (FtpWebRequest)WebRequest.Create(String.Format("ftp://{0}:{1}", Host, Port));
            else
                request = (FtpWebRequest)WebRequest.Create(String.Format("ftp://{0}:{1}/{2}", Host, Port, directory));

            request.Credentials = new NetworkCredential(Username, Password);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            return request;
        }

        public string[] ListDirectories(string directory)
        {
            var list = new List<string>();
            var request = CreateRequest(directory);

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

            request.Abort();
            return list.ToArray();
        }

        FtpWebResponse response = null;

        private void LoadListAsync(TreeNode currentNode)
        {
            allowCancel = false;

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
                if (ex.GetType() == typeof(WebException))
                {
                    response = (FtpWebResponse)(ex as WebException).Response;
                    if (response.StatusCode == FtpStatusCode.NotLoggedIn)
                    {
                        if (isGettingRootData)
                        {
                            Invoke(new Action(() =>
                                {
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while listing server-data.", "The login credentials are wrong.", PopupButtons.OK);
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
                                    Popup.ShowPopup(this, SystemIcons.Error, "Error while listing server-data.", ex, PopupButtons.OK);
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
                                Popup.ShowPopup(this, SystemIcons.Error, "Error while listing server-data.", ex, PopupButtons.OK);
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
    }
}
