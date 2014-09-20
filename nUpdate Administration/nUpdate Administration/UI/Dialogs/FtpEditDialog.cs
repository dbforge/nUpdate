// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Update;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class FtpEditDialog : BaseDialog
    {
        public FtpEditDialog()
        {
            InitializeComponent();
        }

        private void FtpProjectEditDialog_Load(object sender, EventArgs e)
        {
            Text += String.Format("{0} - nUpdate Administration 1.1.0.0", Project.Name);

            try
            {
                hostTextBox.Text = Project.FtpHost;
                portTextBox.Text = Project.FtpPort.ToString();
                directoryTextBox.Text = Project.FtpDirectory;
                modeComboBox.SelectedIndex = Project.FtpUsePassiveMode ? 0 : 1;
                userTextBox.Text = Project.FtpUsername;
                passwordTextBox.Text = Program.FtpPassword.ConvertToUnsecureString();

                protocolComboBox.SelectedIndex = Project.FtpProtocol;
            }
            catch
            {
                modeComboBox.SelectedIndex = 0;
                protocolComboBox.SelectedIndex = 0;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidateDialogWithIgnoring(this, directoryTextBox))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                    "All fields need to have a value.", PopupButtons.Ok);
                return;
            }

            Project.FtpHost = hostTextBox.Text;
            Project.FtpPort = int.Parse(portTextBox.Text);
            Project.FtpUsername = userTextBox.Text;
            Project.FtpPassword =
                Convert.ToBase64String(AesManager.Encrypt(passwordTextBox.Text, passwordTextBox.Text, userTextBox.Text));

            if (Project.SqlPassword != null)
            {
                Project.SqlPassword =
                    Convert.ToBase64String(AesManager.Encrypt(Program.SqlPassword.ConvertToUnsecureString(),
                        passwordTextBox.Text, userTextBox.Text));
            }

            if (Project.ProxyPassword != null)
            {
                Project.ProxyPassword =
                    Convert.ToBase64String(AesManager.Encrypt(Program.ProxyPassword.ConvertToUnsecureString(),
                        passwordTextBox.Text, userTextBox.Text));
            }

            bool usePassive = modeComboBox.SelectedIndex.Equals(0);
            Project.FtpUsePassiveMode = usePassive;
            Project.FtpProtocol = protocolComboBox.SelectedIndex;
            Project.FtpDirectory = directoryTextBox.Text;

            try
            {
                ApplicationInstance.SaveProject(Project.Path, Project);
            }
            catch (Exception ex)
            {
                Invoke(
                    new Action(
                        () =>
                            Popup.ShowPopup(this, SystemIcons.Error, "Error while saving new project info.", ex,
                                PopupButtons.Ok)));
            }

            Program.FtpPassword.Dispose();
            Program.FtpPassword = CreateSecureString(passwordTextBox.Text);

            DialogResult = DialogResult.OK;
        }

        private SecureString CreateSecureString(string plainString)
        {
            var secureString = new SecureString();
            foreach (Char c in plainString)
            {
                secureString.AppendChar(c);
            }

            return secureString;
        }

        private void searchOnServerButton_Click(object sender, EventArgs e)
        {
            if ((from Control control in Controls
                where control.GetType() == typeof (TextBox) || control.GetType() == typeof (WatermarkTextBox)
                where control.Name != directoryTextBox.Name
                select control).Any(control => String.IsNullOrEmpty(control.Text)))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information.",
                    "All input fields need to have a value in order to send a request to the server.",
                    PopupButtons.Ok);
                return;
            }

            FtpProtocol protocol = Equals(modeComboBox.SelectedIndex, 0) ? FtpProtocol.FTP : FtpProtocol.FTPS;

            var securePwd = new SecureString();

            foreach (char sign in passwordTextBox.Text)
            {
                securePwd.AppendChar(sign);
            }

            var searchForm = new DirectorySearchDialog
            {
                ProjectName = Project.Name,
                Host = hostTextBox.Text,
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
            if ("1234567890\b".IndexOf(e.KeyChar.ToString(), StringComparison.Ordinal) < 0)
                e.Handled = true;
        }
    }
}