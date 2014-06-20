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
    public partial class FtpEditForm : BaseForm
    {
        public FtpEditForm()
        {
            InitializeComponent();
        }

        private void FtpEditForm_Load(object sender, EventArgs e)
        {
            this.Text += String.Format("{0} - nUpdate Administration 1.1.0.0", Project.Name);

            this.modeComboBox.SelectedIndex = 0;
            this.protocolComboBox.SelectedIndex = 0;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidateDialog(this, this.directoryTextBox))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                return;
            }

            this.Project.FtpHost = this.adressTextBox.Text;
            this.Project.FtpPort = this.portTextBox.Text;
            if (Properties.Settings.Default.SaveCredentials)
            {
                this.Project.FtpUsername = this.userTextBox.Text;
                this.Project.FtpPassword = this.passwordTextBox.Text;
            }

            bool usePassive = this.modeComboBox.SelectedIndex.Equals(0);
            this.Project.FtpUsePassiveMode = usePassive.ToString();
            this.Project.FtpProtocol = this.protocolComboBox.GetItemText(this.protocolComboBox.SelectedItem);
            this.Project.FtpDirectory = this.directoryTextBox.Text;

            string ftpInfoFile = Path.Combine(Program.Path, "Projects", this.Project.Name, "ftp.txt");

            try
            {
                //File.WriteAllLines(ftpInfoFile, content);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Failed to save FTP-data.", ex, PopupButtons.OK);
                this.DialogResult = DialogResult.Cancel;
            }

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

            foreach (char sign in this.passwordTextBox.Text)
            {
                securePwd.AppendChar(sign);
            }

            var searchForm = new DirectorySearchForm()
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
