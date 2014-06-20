using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Popups;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class NewProjectForm : BaseForm
    {
        private bool allowCancel = true;
        private string alreadyExistingWarn;

        /// <summary>
        /// Returns the private key.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Returns the public key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Sets the current panel.
        /// </summary>
        public Panel Sender { get; set; }

        /// <summary>
        /// Sets the language
        /// </summary>
        public void SetLanguage()
        {
            LocalizationProperties ls = new LocalizationProperties();
            if (File.Exists(Program.LanguageSerializerFilePath))
            {
                ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(Program.LanguageSerializerFilePath));
            }
            else
            {
                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            this.Text = ls.NewProjectFormTitle;
            this.alreadyExistingWarn = ls.AlreadyExistingWarn;

            this.cancelButton.Text = ls.CancelButtonText;
            this.continueButton.Text = ls.ContinueButtonText;

            this.keyPairHeaderLabel.Text = ls.PanelSignatureHeader;
            this.keyPairInfoLabel.Text = ls.PanelSignatureInfoText;
            this.keyPairGenerationLabel.Text = ls.PanelSignatureWaitText;

            this.generalHeaderLabel.Text = ls.PanelGeneralHeader;
            this.nameLabel.Text = ls.PanelGeneralNameText;
            this.nameTextBox.Cue = ls.PanelGeneralNameWatermarkText;
            this.languageLabel.Text = ls.PanelGeneralLanguageText;

            this.ftpHeaderLabel.Text = ls.PanelFtpHeader;
            this.adressLabel.Text = ls.PanelFtpServerText;
            this.adressTextBox.Cue = ls.PanelFtpServerWatermarkText;
            this.userLabel.Text = ls.PanelFtpUserText;
            this.userTextBox.Cue = ls.PanelFtpUserWatermarkText;
            this.passwordLabel.Text = ls.PanelFtpPasswordText;
            this.portLabel.Text = ls.PanelFtpPortText;
            this.portTextBox.Cue = ls.PanelFtpPortWatermarkText;
        }

        public NewProjectForm()
        {
            InitializeComponent();
        }

        private void NewProjectForm_Load(object sender, EventArgs e)
        {
            portTextBox.ShortcutsEnabled = false;

            this.userLabel.Enabled = Properties.Settings.Default.SaveCredentials;
            this.userTextBox.Enabled = Properties.Settings.Default.SaveCredentials;
            this.passwordTextBox.Enabled = Properties.Settings.Default.SaveCredentials;
            this.passwordLabel.Enabled = Properties.Settings.Default.SaveCredentials;

            this.languageComboBox.SelectedIndex = 0;
            this.modeComboBox.SelectedIndex = 0;
            this.protocolComboBox.SelectedIndex = 0;

            this.SetLanguage();

            this.generalPanel.Hide();
            this.ftpPanel.Hide();

            this.controlPanel1.Visible = false;
            this.allowCancel = false;

            new Thread(this.GenerateKeyPair).Start();
        }

        private void NewProjectForm_FormClosing(object sender, FormClosingEventArgs e)
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
                this.generalPanel.Show();
                this.Sender = this.generalPanel;
            }));

            this.allowCancel = true;
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (this.Sender == this.generalPanel)
            {
                if (!ValidationManager.ValidatePanel(this.generalPanel))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                    return;
                }

                if (!Uri.IsWellFormedUriString(updateUrlTextBox.Text, UriKind.Absolute))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid adress.", "The given Update-URL is invalid.", PopupButtons.OK);
                    return;
                }

                this.Sender = this.ftpPanel;
                this.backButton.Enabled = true;
                this.generalPanel.Hide();
                this.ftpPanel.Show();
            }
            else if (this.Sender == this.ftpPanel)
            {
                //if (!ValidationManager.ValidateDialog(this, directoryTextBox))
                //{
                //    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                //    return;
                //}

                try
                {
                    FileStream fs = File.Create(this.localPathTextBox.Text);
                    fs.Flush();
                    fs.Dispose();
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to create project file.", ex, PopupButtons.OK);
                    this.Close();
                }

                bool usePassive;

                if (this.modeComboBox.SelectedIndex == 0)
                {
                    usePassive = true;
                }
                else
                {
                    usePassive = false;
                }

                // Create a new package
                var project = new UpdateProject()
                {
                    Path = this.localPathTextBox.Text,
                    Name = this.nameTextBox.Text,
                    Id = Guid.NewGuid().ToString(),
                    UpdateUrl = this.updateUrlTextBox.Text,
                    ProgrammingLanguage = this.languageComboBox.SelectedText,
                    NewestPackage = null,
                    Packages = null,
                    ReleasedPackages = "0",
                    FtpHost = this.adressTextBox.Text,
                    FtpPort = this.portTextBox.Text,
                    FtpUsername = this.userTextBox.Text,
                    FtpPassword = this.passwordTextBox.Text,
                    FtpDirectory = this.directoryTextBox.Text,
                    FtpProtocol = this.protocolComboBox.GetItemText(this.protocolComboBox.SelectedItem),
                    FtpUsePassiveMode = usePassive.ToString(),
                    PrivateKey = this.PrivateKey,
                    PublicKey = this.PublicKey,
                    Log = null,
                };

                string projectFileContents = Serializer.Serialize(project);
                try
                {
                    File.WriteAllText(this.localPathTextBox.Text, projectFileContents);
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to write to project file.", ex, PopupButtons.OK);
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
            //foreach (Control control in ftpPanel.Controls)
            //{
            //    if (control.GetType() == typeof(TextBox) || control.GetType() == typeof(WatermarkTextBox) && control.Enabled == true)
            //    {
            //        if (control.Name != directoryTextBox.Name)
            //        {
            //            if (String.IsNullOrEmpty(control.Text))
            //            {
            //                Popup.ShowPopup(this, SystemIcons.Error, "Missing information.", "All input fields need to have a value in order to send a request to the server.", PopupButtons.OK);
            //                return;
            //            }
            //        }
            //    }
            //}

            FtpProtocol protocol = FtpProtocol.NormalFtp;
            if (int.Equals(this.modeComboBox.SelectedIndex, 0))
            {
                protocol = FtpProtocol.NormalFtp;
            }
            else
            {
                protocol = FtpProtocol.SecureFtp;
            }

            SecureString securePwd = new SecureString();
            if (Properties.Settings.Default.SaveCredentials)
            {
                foreach (char sign in this.passwordTextBox.Text)
                {
                    securePwd.AppendChar(sign);
                }
            }

            var searchForm = new DirectorySearchForm()
            {
                ProjectName = this.nameTextBox.Text,
                Host = this.adressTextBox.Text,
                Port = int.Parse(this.portTextBox.Text),
                UsePassiveMode = this.modeComboBox.SelectedIndex.Equals(0),
                Username = this.userTextBox.Text,
                Password = securePwd,
                Protocol = protocol,
            };
            if (searchForm.ShowDialog() == DialogResult.OK)
            {
                this.directoryTextBox.Text = searchForm.SelectedDirectory;
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

        private void backButton_Click(object sender, EventArgs e)
        {
            this.ftpPanel.Hide();
            this.generalPanel.Show();
            this.backButton.Enabled = false;
            this.Sender = this.generalPanel;
        }

    }
}
