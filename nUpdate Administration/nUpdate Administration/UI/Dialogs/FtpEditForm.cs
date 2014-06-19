using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Popups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
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
            Text += String.Format("{0} - nUpdate Administration 1.1.0.0", Project.Name);

            modeComboBox.SelectedIndex = 0;
            protocolComboBox.SelectedIndex = 0;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidateDialog(this, directoryTextBox))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                return;
            }

            Project.FtpHost = adressTextBox.Text;
            Project.FtpPort = portTextBox.Text;
            if (Properties.Settings.Default.SaveCredentials)
            {
                Project.FtpUsername = userTextBox.Text;
                Project.FtpPassword = passwordTextBox.Text;
            }

            bool usePassive = modeComboBox.SelectedIndex.Equals(0);
            Project.FtpUsePassiveMode = usePassive.ToString();
            Project.FtpProtocol = protocolComboBox.GetItemText(protocolComboBox.SelectedItem);
            Project.FtpDirectory = directoryTextBox.Text;

            string ftpInfoFile = Path.Combine(Program.Path, "Projects", Project.Name, "ftp.txt");

            try
            {
                //File.WriteAllLines(ftpInfoFile, content);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Failed to save FTP-data.", ex, PopupButtons.OK);
                DialogResult = DialogResult.Cancel;
            }

            DialogResult = DialogResult.OK;
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

            FtpProtocol _protocol = FtpProtocol.NormalFtp;
            if (int.Equals(modeComboBox.SelectedIndex, 0))
                _protocol = FtpProtocol.NormalFtp;
            else
                _protocol = FtpProtocol.SecureFtp;

            SecureString securePwd = new SecureString();

            foreach (char sign in passwordTextBox.Text)
            {
                securePwd.AppendChar(sign);
            }

            var searchForm = new DirectorySearchForm()
            {
                ProjectName = this.Project.Name,
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

        private void portTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("1234567890\b".IndexOf(e.KeyChar.ToString()) < 0)
                e.Handled = true;
        }
    }
}
