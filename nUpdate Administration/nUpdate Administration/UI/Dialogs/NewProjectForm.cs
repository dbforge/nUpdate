using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Localization;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Dialogs;
using nUpdate.Administration.UI.Popups;
using nUpdate.Administration.Core.Application;
using System.Reflection;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class NewProjectForm : BaseForm
    {
        bool allowCancel = true;
        string alreadyExistingWarn;

        /// <summary>
        /// Returns the private key.
        /// </summary>
        public string PrivateKey 
        {
            get; 
            set;
        }

        /// <summary>
        /// Returns the public key.
        /// </summary>
        public string PublicKey 
        {
            get; 
            set; 
        }

        /// <summary>
        /// Sets the current panel.
        /// </summary>
        public Panel Sender 
        { 
            get;
            set; 
        }

        /// <summary>
        /// Sets the language
        /// </summary>
        public void SetLanguage()
        {
            LocalizationProperties ls = new LocalizationProperties();
            if (File.Exists(Program.LanguageSerializerFilePath))
                ls = Serializer.Deserialize<LocalizationProperties>(File.ReadAllText(Program.LanguageSerializerFilePath));
            else
            {

                string resourceName = "nUpdate.Administration.Core.Localization.en.xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    ls = Serializer.Deserialize<LocalizationProperties>(stream);
                }
            }

            this.Text = ls.NewProjectFormTitle;
            alreadyExistingWarn = ls.AlreadyExistingWarn;

            cancelButton.Text = ls.CancelButtonText;
            continueButton.Text = ls.ContinueButtonText;

            keyPairHeaderLabel.Text = ls.PanelSignatureHeader;
            keyPairInfoLabel.Text = ls.PanelSignatureInfoText;
            keyPairGenerationLabel.Text = ls.PanelSignatureWaitText;

            generalHeaderLabel.Text = ls.PanelGeneralHeader;
            nameLabel.Text = ls.PanelGeneralNameText;
            nameTextBox.Cue = ls.PanelGeneralNameWatermarkText;
            languageLabel.Text = ls.PanelGeneralLanguageText;

            ftpHeaderLabel.Text = ls.PanelFtpHeader;
            adressLabel.Text = ls.PanelFtpServerText;
            adressTextBox.Cue = ls.PanelFtpServerWatermarkText;
            userLabel.Text = ls.PanelFtpUserText;
            userTextBox.Cue = ls.PanelFtpUserWatermarkText;
            passwordLabel.Text = ls.PanelFtpPasswordText;
            portLabel.Text = ls.PanelFtpPortText;
            portTextBox.Cue = ls.PanelFtpPortWatermarkText;
        }

        public NewProjectForm()
        {
            InitializeComponent();
        }

        private void NewProjectForm_Load(object sender, EventArgs e)
        {
            portTextBox.ShortcutsEnabled = false;

            userLabel.Enabled = Properties.Settings.Default.SaveCredentials;
            userTextBox.Enabled = Properties.Settings.Default.SaveCredentials;
            passwordTextBox.Enabled = Properties.Settings.Default.SaveCredentials;
            passwordLabel.Enabled = Properties.Settings.Default.SaveCredentials;

            languageComboBox.SelectedIndex = 0;
            modeComboBox.SelectedIndex = 0;
            protocolComboBox.SelectedIndex = 0;

            SetLanguage();

            generalPanel.Hide();
            ftpPanel.Hide();

            controlPanel1.Visible = false;
            allowCancel = false;

            new Thread(GenerateKeyPair).Start();
        }

        private void NewProjectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowCancel)
                e.Cancel = true;
        }

        /// <summary>
        /// Generates the key pair in an own thread.
        /// </summary>
        private void GenerateKeyPair()
        {
            // Create a new instance of the RsaSignature-class
            RsaSignature rsa = new RsaSignature();

            // Initialize the properties with the keys
            PrivateKey = rsa.PrivateKey;
            PublicKey = rsa.PublicKey;

            Invoke(new Action(() =>
            {
                controlPanel1.Visible = true;
                generalPanel.Show();
                Sender = generalPanel;
            }));

            allowCancel = true;
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (Sender == generalPanel)
            {
                if (!ValidationManager.ValidatePanel(generalPanel))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                    return;
                }

                if (!Uri.IsWellFormedUriString(updateUrlTextBox.Text, UriKind.Absolute))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Invalid adress.", "The given Update-URL is invalid.", PopupButtons.OK);
                    return;
                }

                Sender = ftpPanel;
                backButton.Enabled = true;
                generalPanel.Hide();
                ftpPanel.Show();
            }
            else if (Sender == ftpPanel)
            {
                //if (!ValidationManager.ValidateDialog(this, directoryTextBox))
                //{
                //    Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                //    return;
                //}

                try
                {
                    FileStream fs = File.Create(localPathTextBox.Text);
                    fs.Flush();
                    fs.Dispose();
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this ,SystemIcons.Error, "Failed to create project file.", ex, PopupButtons.OK);
                    Close();
                }

                bool usePassive;

                if (modeComboBox.SelectedIndex == 0)
                    usePassive = true;
                else
                    usePassive = false;

                // Create a new package
                var project = new UpdateProject()
                {
                    Path = localPathTextBox.Text,
                    Name = nameTextBox.Text,
                    Id = Guid.NewGuid().ToString(),
                    UpdateUrl = updateUrlTextBox.Text,
                    ProgrammingLanguage = languageComboBox.SelectedText,
                    NewestPackage = null,
                    Packages = null,
                    ReleasedPackages = "0",
                    FtpHost = adressTextBox.Text,
                    FtpPort = portTextBox.Text,
                    FtpUsername = userTextBox.Text,
                    FtpPassword = passwordTextBox.Text,
                    FtpDirectory = directoryTextBox.Text,
                    FtpProtocol = protocolComboBox.GetItemText(protocolComboBox.SelectedItem),
                    FtpUsePassiveMode = usePassive.ToString(),
                    PrivateKey = PrivateKey,
                    PublicKey = PublicKey,
                    Log = null,
                };

                string projectFileContents = Serializer.Serialize(project);
                try
                {
                    File.WriteAllText(localPathTextBox.Text, projectFileContents);
                }
                catch (IOException ex)
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Failed to write to project file.", ex, PopupButtons.OK);
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

            FtpProtocol _protocol = FtpProtocol.NormalFtp;
            if (int.Equals(modeComboBox.SelectedIndex, 0))
                _protocol = FtpProtocol.NormalFtp;
            else
                _protocol = FtpProtocol.SecureFtp;

            SecureString securePwd = new SecureString();
            if (Properties.Settings.Default.SaveCredentials)
            {
                foreach (char sign in passwordTextBox.Text)
                {
                    securePwd.AppendChar(sign);
                }
            }

            var searchForm = new DirectorySearchForm()
            {
                ProjectName = nameTextBox.Text,
                Host = adressTextBox.Text,
                Port = int.Parse(portTextBox.Text),
                UsePassiveMode = modeComboBox.SelectedIndex.Equals(0),
                Username = userTextBox.Text,
                Password = securePwd,
                Protocol = _protocol,
            };
            if (searchForm.ShowDialog() == DialogResult.OK)
                directoryTextBox.Text = searchForm.SelectedDirectory;

            searchForm.Close();
        }

        private void searchPathButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "nUpdate Project Files (*.nupdproj)|*.nupdproj";
            if (fileDialog.ShowDialog() == DialogResult.OK)
                localPathTextBox.Text = fileDialog.FileName;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            ftpPanel.Hide();
            generalPanel.Show();
            backButton.Enabled = false;
            Sender = generalPanel;
        }

    }
}
