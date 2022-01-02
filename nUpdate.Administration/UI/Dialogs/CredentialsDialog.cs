// CredentialsDialog.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class CredentialsDialog : BaseDialog
    {
        private bool _allowCancel;

        public CredentialsDialog()
        {
            InitializeComponent();
        }

        public string Password { get; set; }

        public string Username { get; set; }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _allowCancel = true;
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                    "All fields need to have a value.", PopupButtons.Ok);
                return;
            }

            Username = usernameTextBox.Text;
            Password = passwordTextBox.Text;

            usernameTextBox.Clear();
            passwordTextBox.Clear();

            _allowCancel = true;
            DialogResult = DialogResult.OK;
        }

        private void CredentialsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }
    }
}