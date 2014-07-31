using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Dialogs;
using nUpdate.Administration.UI.Popups;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using System.Text;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class NewProjectDialog : BaseDialog
    {
        private LocalizationProperties lp = new LocalizationProperties();
        private bool allowCancel = true;

        /// <summary>
        /// Returns the private key.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Returns the public key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Sets the current tabpage.
        /// </summary>
        public TabPage Sender { get; set; }

        /// <summary>
        /// Sets the language
        /// </summary>
        public void SetLanguage()
        {
            string languageFilePath = Path.Combine(Program.LanguagesDirectory, String.Format("{0}.json", Properties.Settings.Default.Language.Name));
            if (File.Exists(languageFilePath))
            {
                lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
            }
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            this.Text = lp.NewProjectDialogTitle;
            this.Text = String.Format(this.Text, lp.ProductTitle);

            this.cancelButton.Text = lp.CancelButtonText;
            this.continueButton.Text = lp.ContinueButtonText;

            this.keyPairHeaderLabel.Text = lp.PanelSignatureHeader;
            this.keyPairInfoLabel.Text = lp.PanelSignatureInfoText;
            this.keyPairGenerationLabel.Text = lp.PanelSignatureWaitText;

            this.generalHeaderLabel.Text = lp.PanelGeneralHeader;
            this.nameLabel.Text = lp.PanelGeneralNameText;
            this.nameTextBox.Cue = lp.PanelGeneralNameWatermarkText;
            this.languageLabel.Text = lp.PanelGeneralLanguageText;

            this.ftpHeaderLabel.Text = lp.PanelFtpHeader;
            this.ftpHostLabel.Text = lp.PanelFtpServerText;
            this.ftpUserLabel.Text = lp.PanelFtpUserText;
            this.ftpUserTextBox.Cue = lp.PanelFtpUserWatermarkText;
            this.ftpPasswordLabel.Text = lp.PanelFtpPasswordText;
            this.ftpPortLabel.Text = lp.PanelFtpPortText;
            this.ftpPortTextBox.Cue = lp.PanelFtpPortWatermarkText;
        }

        public NewProjectDialog()
        {
            InitializeComponent();
        }

        private void NewProjectDialog_Load(object sender, EventArgs e)
        {
            this.ftpPortTextBox.ShortcutsEnabled = false;

            this.ftpUserLabel.Enabled = Properties.Settings.Default.SaveCredentials;
            this.ftpUserTextBox.Enabled = Properties.Settings.Default.SaveCredentials;
            this.ftpPasswordTextBox.Enabled = Properties.Settings.Default.SaveCredentials;
            this.ftpPasswordLabel.Enabled = Properties.Settings.Default.SaveCredentials;

            this.languageComboBox.SelectedIndex = 0;
            this.ftpModeComboBox.SelectedIndex = 0;
            this.ftpProtocolComboBox.SelectedIndex = 0;

            this.SetLanguage();

            this.controlPanel1.Visible = false;
            this.allowCancel = false;

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state)
            { this.GenerateKeyPair(); }), null);
        }

        private void NewProjectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowCancel)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Generates the key pair in an own thread.
        /// </summary>
        private void GenerateKeyPair()
        {
            // Create a new instance of the RsaSignature-class
            RsaSignature rsa = new RsaSignature();

            // Initialize the properties with the keys
            this.PrivateKey = rsa.PrivateKey;
            this.PublicKey = rsa.PublicKey;

            Invoke(new Action(() =>
            {
                this.controlPanel1.Visible = true;
                this.tablessTabControl1.SelectedTab = this.generalTabPage;
                this.Sender = this.generalTabPage;
            }));

            this.allowCancel = true;
        }

        /// <summary>
        /// Converts a string into a byte array.
        /// </summary>
        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (this.Sender == this.generalTabPage)
            {
                if (!ValidationManager.ValidatePanel(this.generalPanel))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                    return;
                }

                List<string> entriesToDelete = new List<string>();
                foreach (KeyValuePair<string, string> existingProject in Program.ExisitingProjects)
                {
                    if (existingProject.Key == this.nameTextBox.Text && File.Exists(existingProject.Value))
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "The project is already existing.", String.Format("The project \"{0}\" is already existing. Please choose another name for it.", this.nameTextBox.Text), PopupButtons.OK);
                        return;
                    }
                    else if (!File.Exists(existingProject.Value))
                    {
                        entriesToDelete.Add(existingProject.Key);
                    }
                }

                foreach (string entryToDelete in entriesToDelete)
                {
                    Program.ExisitingProjects.Remove(entryToDelete);
                }

                if (!Uri.IsWellFormedUriString(updateUrlTextBox.Text, UriKind.Absolute))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid adress.", "The given Update-URL is invalid.", PopupButtons.OK);
                    return;
                }

                if (!Path.IsPathRooted(this.localPathTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.", "The given local path for the project is invalid.", PopupButtons.OK);
                    return;
                }

                try
                {
                    Path.GetFullPath(this.localPathTextBox.Text);
                }
                catch
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.", "The given local path for the project is invalid.", PopupButtons.OK);
                    return;
                }

                this.Sender = this.ftpTabPage;
                this.backButton.Enabled = true;
                this.tablessTabControl1.SelectedTab = this.ftpTabPage;
            }
            else if (this.Sender == this.ftpTabPage)
            {
                if (!ValidationManager.ValidatePanel(ftpPanel) || String.IsNullOrEmpty(this.ftpPasswordTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                    return;
                }

                this.Sender = this.statisticsServerTabPage;
                this.tablessTabControl1.SelectedTab = this.statisticsServerTabPage;
            }
            else if (this.Sender == this.statisticsServerTabPage)
            {
                if (!ValidationManager.ValidatePanel(this.statisticsServerTabPage))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                    return;
                }

                try
                {
                    using (FileStream fs = File.Create(this.localPathTextBox.Text)) { }
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to create project file.", ex, PopupButtons.OK);
                    this.Close();
                }

                bool usePassive;
                if (this.ftpModeComboBox.SelectedIndex == 0)
                {
                    usePassive = true;
                }
                else
                {
                    usePassive = false;
                }

                // Create a new package...
                var project = new UpdateProject()
                {
                    Path = this.localPathTextBox.Text,
                    Name = this.nameTextBox.Text,
                    Id = Guid.NewGuid().ToString(),
                    UpdateUrl = this.updateUrlTextBox.Text,
                    ProgrammingLanguage = this.languageComboBox.SelectedText,
                    NewestPackage = null,
                    Packages = null,
                    ReleasedPackages = 0,
                    FtpHost = this.ftpHostTextBox.Text,
                    FtpPort = int.Parse(this.ftpPortTextBox.Text),
                    FtpUsername = this.ftpUserTextBox.Text,
                    FtpPassword = Convert.ToBase64String(AESManager.Encrypt(this.ftpPasswordTextBox.Text, this.ftpUserTextBox.Text, this.ftpPasswordTextBox.Text)),
                    FtpDirectory = this.ftpDirectoryTextBox.Text,
                    FtpProtocol = this.ftpProtocolComboBox.GetItemText(this.ftpProtocolComboBox.SelectedItem),
                    FtpUsePassiveMode = usePassive,
                    PrivateKey = this.PrivateKey,
                    PublicKey = this.PublicKey,
                    Log = null,
                };

                try
                {
                    ApplicationInstance.SaveProject(this.localPathTextBox.Text, project); // ... and save it
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to save project file.", ex, PopupButtons.OK);
                }

                Program.ExisitingProjects.Add(project.Name, project.Path);
                try
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (KeyValuePair<string, string> dictionaryEntry in Program.ExisitingProjects)
                    {
                        builder.AppendLine(String.Format("{0}-{1}", dictionaryEntry.Key, dictionaryEntry.Value));
                    }
                    File.WriteAllText(Program.ProjectsConfigFilePath, builder.ToString());
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to create the project-entry.", ex, PopupButtons.OK);
                }
                this.Close();
            }
        }

        private void portTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("1234567890\b".IndexOf(e.KeyChar.ToString()) < 0)
            {
                e.Handled = true;
            }
        }

        private void searchOnServerButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidatePanelWithIgnoring(ftpPanel, ftpDirectoryTextBox))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information.", "All input fields need to have a value in order to send a request to the server.", PopupButtons.OK);
                return;
            }

            FTPProtocol protocol = FTPProtocol.NormalFtp;
            if (int.Equals(this.ftpModeComboBox.SelectedIndex, 0))
            {
                protocol = FTPProtocol.NormalFtp;
            }
            else
            {
                protocol = FTPProtocol.SecureFtp;
            }

            var securePwd = new SecureString();
            foreach (char sign in this.ftpPasswordTextBox.Text)
            {
                securePwd.AppendChar(sign);
            }

            var searchForm = new DirectorySearchDialog()
            {
                ProjectName = this.nameTextBox.Text,
                Host = this.ftpHostTextBox.Text,
                Port = int.Parse(this.ftpPortTextBox.Text),
                UsePassiveMode = this.ftpModeComboBox.SelectedIndex.Equals(0),
                Username = this.ftpUserTextBox.Text,
                Password = securePwd,
                Protocol = protocol,
            };
            if (searchForm.ShowDialog() == DialogResult.OK)
            {
                this.ftpDirectoryTextBox.Text = searchForm.SelectedDirectory;
            }

            searchForm.Close();
        }

        private void searchPathButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                this.localPathTextBox.Text = fileDialog.FileName;
            }
        }

        private void securityInfoButton_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Management of sensible data.", "All your passwords will be encrypted with AES 256 and then saved on your PC. The key and initializing vector is your FTP-username and password, consecutively you have to enter them each time you open a project.", PopupButtons.OK);
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (this.Sender == this.ftpPanel)
            {
                this.tablessTabControl1.SelectedTab = this.generalTabPage;
                this.backButton.Enabled = false;
                this.Sender = this.generalTabPage;
            }
            else if (this.Sender == this.statisticsServerTabPage)
            {
                this.tablessTabControl1.SelectedTab = this.ftpTabPage;
                this.Sender = this.ftpTabPage;
            }
        }

        private void useStatisticsServerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.panel1.Enabled = this.useStatisticsServerRadioButton.Checked;
        }

        private void ftpImportButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
                fileDialog.Multiselect = false;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var importProject = ApplicationInstance.LoadProject(fileDialog.FileName);
                        this.ftpHostTextBox.Text = importProject.FtpHost;
                        this.ftpPortTextBox.Text = importProject.FtpPort.ToString();
                        this.ftpUserTextBox.Text = importProject.FtpUsername;
                        this.ftpProtocolComboBox.SelectedIndex = (importProject.FtpProtocol == "FTP") ? 0 : 1;
                        this.ftpModeComboBox.SelectedIndex = importProject.FtpUsePassiveMode ? 0 : 1;
                        this.ftpDirectoryTextBox.Text = importProject.FtpDirectory;
                        this.ftpPasswordTextBox.Focus();
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Error while importing project data.", ex, PopupButtons.OK);
                    }
                }
            }
        }
    }
}
