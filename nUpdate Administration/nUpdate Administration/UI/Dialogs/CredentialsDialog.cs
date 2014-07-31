using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.UI.Dialogs;
using System;
using System.Drawing;
using System.Security;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class CredentialsDialog : BaseDialog
    {
        private bool allowCancel = false;

        public CredentialsDialog()
        {
            InitializeComponent();
        }

        public string Username { get; set; }
        public SecureString Password { get; set; }

        private void CredentialsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowCancel)
            {
                e.Cancel = true;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.allowCancel = true;
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidateDialog(this))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                return;
            }

            this.Username = this.userNameTextBox.Text;

            SecureString ftpPassword = new SecureString();
            foreach (Char c in this.passwordTextBox.Text)
                ftpPassword.AppendChar(c);
            this.Password = ftpPassword;

            allowCancel = true;
            this.DialogResult = DialogResult.OK;
        }
    }
}
