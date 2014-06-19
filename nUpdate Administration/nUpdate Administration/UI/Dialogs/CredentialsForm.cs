using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms;
using nUpdate.Administration.UI.Controls;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;

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

            Username = usernameTextBox.Text;
            SecureString pass = new SecureString();
            foreach (Char c in passwordTextBox.Text)
                pass.AppendChar(c);
            Password = pass;

            DialogResult = DialogResult.OK;
        }

        private void CredentialsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
