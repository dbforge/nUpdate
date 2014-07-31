using System;
using System.Drawing;
using System.Security;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.Properties;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class FtpEditDialog : BaseDialog
    {
        public FtpEditDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Sets if the cancellation is allowed.
        /// </summary>
        public bool CanCancel { get; set; }

        private void FtpProjectEditDialog_Load(object sender, EventArgs e)
        {
            Text += String.Format("{0} - nUpdate Administration 1.1.0.0", Project.Name);

            if (!CanCancel)
                cancelButton.Enabled = false;

            try
            {
                adressTextBox.Text = Project.FtpHost;
                portTextBox.Text = Project.FtpPort.ToString();
                directoryTextBox.Text = Project.FtpDirectory;
                protocolComboBox.SelectedText = Project.FtpProtocol;

                // Passive mode
                if (Project.FtpUsePassiveMode)
                    modeComboBox.SelectedIndex = 0;
                else
                    modeComboBox.SelectedIndex = 1;

                // Credentials
                if (!String.IsNullOrEmpty(Project.FtpUsername))
                    userTextBox.Text = Project.FtpUsername;

                if (!String.IsNullOrEmpty(Project.FtpPassword))
                    passwordTextBox.Text = Project.FtpPassword;

                // Protcol
                switch (Project.FtpProtocol)
                {
                    case "FTP":
                        protocolComboBox.SelectedIndex = 0;
                        break;
                    case "FTP/SSL":
                        protocolComboBox.SelectedIndex = 1;
                        break;
                    default:
                        protocolComboBox.SelectedIndex = 0;
                        break;
                }
            }
            catch
            {
                modeComboBox.SelectedIndex = 0;
                protocolComboBox.SelectedIndex = 0;
            }
        }

        private void FtpEditDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanCancel)
                e.Cancel = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidateDialog(this, directoryTextBox))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                    "All fields need to have a value.", PopupButtons.OK);
                return;
            }

            Project.FtpHost = adressTextBox.Text;
            Project.FtpPort = int.Parse(portTextBox.Text);
            if (Settings.Default.SaveCredentials)
            {
                Project.FtpUsername = userTextBox.Text;
                Project.FtpPassword = passwordTextBox.Text;
            }

            bool usePassive = modeComboBox.SelectedIndex.Equals(0);
            Project.FtpUsePassiveMode = usePassive;
            Project.FtpProtocol = protocolComboBox.GetItemText(protocolComboBox.SelectedItem);
            Project.FtpDirectory = directoryTextBox.Text;

            CanCancel = true;
            DialogResult = DialogResult.OK;
        }

        private void searchOnServerButton_Click(object sender, EventArgs e)
        {
            foreach (Control control in Controls)
            {
                if (control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox))
                {
                    if (control.Name != directoryTextBox.Name)
                    {
                        if (String.IsNullOrEmpty(control.Text))
                        {
                            Popup.ShowPopup(this, SystemIcons.Error, "Missing information.",
                                "All input fields need to have a value in order to send a request to the server.",
                                PopupButtons.OK);
                            return;
                        }
                    }
                }
            }

            var protocol = FTPProtocol.NormalFtp;
            if (Equals(modeComboBox.SelectedIndex, 0))
                protocol = FTPProtocol.NormalFtp;
            else
                protocol = FTPProtocol.SecureFtp;

            var securePwd = new SecureString();

            foreach (char sign in passwordTextBox.Text)
            {
                securePwd.AppendChar(sign);
            }

            var searchForm = new DirectorySearchDialog
            {
                ProjectName = Project.Name,
                Host = adressTextBox.Text,
                Port = int.Parse(portTextBox.Text),
                UsePassiveMode = modeComboBox.SelectedIndex.Equals(0),
                Username = userTextBox.Text,
                Password = securePwd,
                Protocol = protocol,
            };
            if (searchForm.ShowDialog() == DialogResult.OK)
                directoryTextBox.Text = searchForm.SelectedDirectory;

            searchForm.Close();
        }

        private void portTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("1234567890\b".IndexOf(e.KeyChar.ToString()) < 0)
                e.Handled = true;
        }
    }
}