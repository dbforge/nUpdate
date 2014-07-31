using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Popups;
using System;
using System.Drawing;
using System.IO;
using System.Security;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class FtpEditDialog : BaseDialog
    {
        /// <summary>
        /// Sets if the cancellation is allowed.
        /// </summary>
        public bool CanCancel { get; set; }

        public FtpEditDialog()
        {
            InitializeComponent();
        }

        private void FtpProjectEditDialog_Load(object sender, EventArgs e)
        {
            this.Text += String.Format("{0} - nUpdate Administration 1.1.0.0", this.Project.Name);

            if (!this.CanCancel)
            {
                this.cancelButton.Enabled = false;
            }

            try
            {
                this.adressTextBox.Text = this.Project.FtpHost;
                this.portTextBox.Text = this.Project.FtpPort.ToString();
                this.directoryTextBox.Text = this.Project.FtpDirectory;
                this.protocolComboBox.SelectedText = this.Project.FtpProtocol;

                // Passive mode
                if (this.Project.FtpUsePassiveMode)
                {
                    this.modeComboBox.SelectedIndex = 0;
                }
                else
                {
                    this.modeComboBox.SelectedIndex = 1;
                }
                
                // Credentials
                if (!String.IsNullOrEmpty(this.Project.FtpUsername))
                {
                    this.userTextBox.Text = this.Project.FtpUsername;
                }

                if (!String.IsNullOrEmpty(this.Project.FtpPassword))
                {
                    this.passwordTextBox.Text = this.Project.FtpPassword;
                }

                // Protcol
                switch (this.Project.FtpProtocol)
                {
                    case "FTP":
                        this.protocolComboBox.SelectedIndex = 0;
                        break;
                    case "FTP/SSL":
                        this.protocolComboBox.SelectedIndex = 1;
                        break;
                    default:
                        this.protocolComboBox.SelectedIndex = 0;
                        break;
                }
            }
            catch 
            {
                this.modeComboBox.SelectedIndex = 0;
                this.protocolComboBox.SelectedIndex = 0;
            }
        }

        private void FtpEditDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.CanCancel)
            {
                e.Cancel = true;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidateDialog(this, this.directoryTextBox))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                return;
            }

            this.Project.FtpHost = this.adressTextBox.Text;
            this.Project.FtpPort = int.Parse(this.portTextBox.Text);
            if (Properties.Settings.Default.SaveCredentials)
            {
                this.Project.FtpUsername = this.userTextBox.Text;
                this.Project.FtpPassword = this.passwordTextBox.Text;
            }

            bool usePassive = this.modeComboBox.SelectedIndex.Equals(0);
            this.Project.FtpUsePassiveMode = usePassive;
            this.Project.FtpProtocol = this.protocolComboBox.GetItemText(this.protocolComboBox.SelectedItem);
            this.Project.FtpDirectory = this.directoryTextBox.Text;

            this.CanCancel = true;
            this.DialogResult = DialogResult.OK;
        }

        private void searchOnServerButton_Click(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                if (control.GetType() == typeof(TextBox) || control.GetType() == typeof(WatermarkTextBox))
                {
                    if (control.Name != directoryTextBox.Name)
                    {
                        if (String.IsNullOrEmpty(control.Text))
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Missing information.", "All input fields need to have a value in order to send a request to the server.", PopupButtons.OK);
                            return;
                        }
                    }
                }
            }

            FTPProtocol protocol = FTPProtocol.NormalFtp;
            if (int.Equals(this.modeComboBox.SelectedIndex, 0))
            {
                protocol = FTPProtocol.NormalFtp;
            }
            else
            {
                protocol = FTPProtocol.SecureFtp;
            }

            SecureString securePwd = new SecureString();

            foreach (char sign in this.passwordTextBox.Text)
            {
                securePwd.AppendChar(sign);
            }

            var searchForm = new DirectorySearchDialog()
            {
                ProjectName = this.Project.Name,
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

        private void portTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("1234567890\b".IndexOf(e.KeyChar.ToString()) < 0)
            {
                e.Handled = true;
            }
        }
    }
}
