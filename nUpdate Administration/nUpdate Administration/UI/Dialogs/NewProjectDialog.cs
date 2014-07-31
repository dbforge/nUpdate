using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class NewProjectDialog : BaseDialog
    {
        private bool allowCancel = true;
        private LocalizationProperties lp = new LocalizationProperties();

        public NewProjectDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Returns the private key.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        ///     Returns the public key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        ///     Sets the current tabpage.
        /// </summary>
        public TabPage Sender { get; set; }

        /// <summary>
        ///     Sets the language
        /// </summary>
        public void SetLanguage()
        {
            string languageFilePath = Path.Combine(Program.LanguagesDirectory,
                String.Format("{0}.json", Settings.Default.Language.Name));
            if (File.Exists(languageFilePath))
                lp = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(languageFilePath));
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    lp = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            Text = lp.NewProjectDialogTitle;
            Text = String.Format(Text, lp.ProductTitle);

            cancelButton.Text = lp.CancelButtonText;
            continueButton.Text = lp.ContinueButtonText;

            keyPairHeaderLabel.Text = lp.PanelSignatureHeader;
            keyPairInfoLabel.Text = lp.PanelSignatureInfoText;
            keyPairGenerationLabel.Text = lp.PanelSignatureWaitText;

            generalHeaderLabel.Text = lp.PanelGeneralHeader;
            nameLabel.Text = lp.PanelGeneralNameText;
            nameTextBox.Cue = lp.PanelGeneralNameWatermarkText;
            languageLabel.Text = lp.PanelGeneralLanguageText;

            ftpHeaderLabel.Text = lp.PanelFtpHeader;
            ftpHostLabel.Text = lp.PanelFtpServerText;
            ftpUserLabel.Text = lp.PanelFtpUserText;
            ftpUserTextBox.Cue = lp.PanelFtpUserWatermarkText;
            ftpPasswordLabel.Text = lp.PanelFtpPasswordText;
            ftpPortLabel.Text = lp.PanelFtpPortText;
            ftpPortTextBox.Cue = lp.PanelFtpPortWatermarkText;
        }

        private void NewProjectDialog_Load(object sender, EventArgs e)
        {
            ftpPortTextBox.ShortcutsEnabled = false;

            ftpUserLabel.Enabled = Settings.Default.SaveCredentials;
            ftpUserTextBox.Enabled = Settings.Default.SaveCredentials;
            ftpPasswordTextBox.Enabled = Settings.Default.SaveCredentials;
            ftpPasswordLabel.Enabled = Settings.Default.SaveCredentials;

            languageComboBox.SelectedIndex = 0;
            ftpModeComboBox.SelectedIndex = 0;
            ftpProtocolComboBox.SelectedIndex = 0;

            SetLanguage();

            controlPanel1.Visible = false;
            allowCancel = false;

            ThreadPool.QueueUserWorkItem(delegate { GenerateKeyPair(); }, null);
        }

        private void NewProjectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowCancel)
                e.Cancel = true;
        }

        /// <summary>
        ///     Generates the key pair in an own thread.
        /// </summary>
        private void GenerateKeyPair()
        {
            // Create a new instance of the RsaSignature-class
            var rsa = new RsaSignature();

            // Initialize the properties with the keys
            PrivateKey = rsa.PrivateKey;
            PublicKey = rsa.PublicKey;

            Invoke(new Action(() =>
            {
                controlPanel1.Visible = true;
                tablessTabControl1.SelectedTab = generalTabPage;
                Sender = generalTabPage;
            }));

            allowCancel = true;
        }

        /// <summary>
        ///     Converts a string into a byte array.
        /// </summary>
        private byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof (char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (Sender == generalTabPage)
            {
                if (!ValidationManager.ValidatePanel(generalPanel))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.OK);
                    return;
                }

                var entriesToDelete = new List<string>();
                foreach (var existingProject in Program.ExisitingProjects)
                {
                    if (existingProject.Key == nameTextBox.Text && File.Exists(existingProject.Value))
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "The project is already existing.",
                            String.Format(
                                "The project \"{0}\" is already existing. Please choose another name for it.",
                                nameTextBox.Text), PopupButtons.OK);
                        return;
                    }
                    if (!File.Exists(existingProject.Value))
                        entriesToDelete.Add(existingProject.Key);
                }

                foreach (string entryToDelete in entriesToDelete)
                {
                    Program.ExisitingProjects.Remove(entryToDelete);
                }

                if (!Uri.IsWellFormedUriString(updateUrlTextBox.Text, UriKind.Absolute))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid adress.", "The given Update-URL is invalid.",
                        PopupButtons.OK);
                    return;
                }

                if (!Path.IsPathRooted(localPathTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.",
                        "The given local path for the project is invalid.", PopupButtons.OK);
                    return;
                }

                try
                {
                    Path.GetFullPath(localPathTextBox.Text);
                }
                catch
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid path.",
                        "The given local path for the project is invalid.", PopupButtons.OK);
                    return;
                }

                Sender = ftpTabPage;
                backButton.Enabled = true;
                tablessTabControl1.SelectedTab = ftpTabPage;
            }
            else if (Sender == ftpTabPage)
            {
                if (!ValidationManager.ValidatePanel(ftpPanel) || String.IsNullOrEmpty(ftpPasswordTextBox.Text))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.OK);
                    return;
                }

                Sender = statisticsServerTabPage;
                tablessTabControl1.SelectedTab = statisticsServerTabPage;
            }
            else if (Sender == statisticsServerTabPage)
            {
                if (!ValidationManager.ValidatePanel(statisticsServerTabPage))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                        "All fields need to have a value.", PopupButtons.OK);
                    return;
                }

                try
                {
                    using (FileStream fs = File.Create(localPathTextBox.Text))
                    {
                    }
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to create project file.", ex, PopupButtons.OK);
                    Close();
                }

                bool usePassive;
                if (ftpModeComboBox.SelectedIndex == 0)
                    usePassive = true;
                else
                    usePassive = false;

                // Create a new package...
                var project = new UpdateProject
                {
                    Path = localPathTextBox.Text,
                    Name = nameTextBox.Text,
                    Id = Guid.NewGuid().ToString(),
                    UpdateUrl = updateUrlTextBox.Text,
                    ProgrammingLanguage = languageComboBox.SelectedText,
                    NewestPackage = null,
                    Packages = null,
                    ReleasedPackages = 0,
                    FtpHost = ftpHostTextBox.Text,
                    FtpPort = int.Parse(ftpPortTextBox.Text),
                    FtpUsername = ftpUserTextBox.Text,
                    FtpPassword =
                        Convert.ToBase64String(AESManager.Encrypt(ftpPasswordTextBox.Text, ftpUserTextBox.Text,
                            ftpPasswordTextBox.Text)),
                    FtpDirectory = ftpDirectoryTextBox.Text,
                    FtpProtocol = ftpProtocolComboBox.GetItemText(ftpProtocolComboBox.SelectedItem),
                    FtpUsePassiveMode = usePassive,
                    PrivateKey = PrivateKey,
                    PublicKey = PublicKey,
                    Log = null,
                };

                try
                {
                    ApplicationInstance.SaveProject(localPathTextBox.Text, project); // ... and save it
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to save project file.", ex, PopupButtons.OK);
                }

                Program.ExisitingProjects.Add(project.Name, project.Path);
                try
                {
                    var builder = new StringBuilder();
                    foreach (var dictionaryEntry in Program.ExisitingProjects)
                    {
                        builder.AppendLine(String.Format("{0}-{1}", dictionaryEntry.Key, dictionaryEntry.Value));
                    }
                    File.WriteAllText(Program.ProjectsConfigFilePath, builder.ToString());
                }
                catch (Exception ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to create the project-entry.", ex, PopupButtons.OK);
                }
                Close();
            }
        }

        private void portTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("1234567890\b".IndexOf(e.KeyChar.ToString()) < 0)
                e.Handled = true;
        }

        private void searchOnServerButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidatePanelWithIgnoring(ftpPanel, ftpDirectoryTextBox))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information.",
                    "All input fields need to have a value in order to send a request to the server.", PopupButtons.OK);
                return;
            }

            var protocol = FTPProtocol.NormalFtp;
            if (Equals(ftpModeComboBox.SelectedIndex, 0))
                protocol = FTPProtocol.NormalFtp;
            else
                protocol = FTPProtocol.SecureFtp;

            var securePwd = new SecureString();
            foreach (char sign in ftpPasswordTextBox.Text)
            {
                securePwd.AppendChar(sign);
            }

            var searchForm = new DirectorySearchDialog
            {
                ProjectName = nameTextBox.Text,
                Host = ftpHostTextBox.Text,
                Port = int.Parse(ftpPortTextBox.Text),
                UsePassiveMode = ftpModeComboBox.SelectedIndex.Equals(0),
                Username = ftpUserTextBox.Text,
                Password = securePwd,
                Protocol = protocol,
            };
            if (searchForm.ShowDialog() == DialogResult.OK)
                ftpDirectoryTextBox.Text = searchForm.SelectedDirectory;

            searchForm.Close();
        }

        private void searchPathButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new SaveFileDialog();
            fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
            if (fileDialog.ShowDialog() == DialogResult.OK)
                localPathTextBox.Text = fileDialog.FileName;
        }

        private void securityInfoButton_Click(object sender, EventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Management of sensible data.",
                "All your passwords will be encrypted with AES 256 and then saved on your PC. The key and initializing vector is your FTP-username and password, consecutively you have to enter them each time you open a project.",
                PopupButtons.OK);
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (Sender == ftpPanel)
            {
                tablessTabControl1.SelectedTab = generalTabPage;
                backButton.Enabled = false;
                Sender = generalTabPage;
            }
            else if (Sender == statisticsServerTabPage)
            {
                tablessTabControl1.SelectedTab = ftpTabPage;
                Sender = ftpTabPage;
            }
        }

        private void useStatisticsServerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Enabled = useStatisticsServerRadioButton.Checked;
        }

        private void ftpImportButton_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
                fileDialog.Multiselect = false;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        UpdateProject importProject = ApplicationInstance.LoadProject(fileDialog.FileName);
                        ftpHostTextBox.Text = importProject.FtpHost;
                        ftpPortTextBox.Text = importProject.FtpPort.ToString();
                        ftpUserTextBox.Text = importProject.FtpUsername;
                        ftpProtocolComboBox.SelectedIndex = (importProject.FtpProtocol == "FTP") ? 0 : 1;
                        ftpModeComboBox.SelectedIndex = importProject.FtpUsePassiveMode ? 0 : 1;
                        ftpDirectoryTextBox.Text = importProject.FtpDirectory;
                        ftpPasswordTextBox.Focus();
                    }
                    catch (Exception ex)
                    {
                        Popup.ShowPopup(this, SystemIcons.Error, "Error while importing project data.", ex,
                            PopupButtons.OK);
                    }
                }
            }
        }
    }
}