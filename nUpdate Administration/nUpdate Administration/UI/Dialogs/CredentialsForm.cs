using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;
using System;
using System.Drawing;
using System.Security;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class CredentialsForm : Form
    {
        public CredentialsForm()
        {
            InitializeComponent();
        }

        public string ProjectName { get; set; }

        public string Username { get; set; }
        public SecureString Password { get; set; }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidateDialog(this))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.", "All fields need to have a value.", PopupButtons.OK);
                return;
            }

            this.Username = this.usernameTextBox.Text;
            SecureString pass = new SecureString();
            foreach (Char c in this.passwordTextBox.Text)
                pass.AppendChar(c);
            this.Password = pass;

            this.DialogResult = DialogResult.OK;
        }

        private void CredentialsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
