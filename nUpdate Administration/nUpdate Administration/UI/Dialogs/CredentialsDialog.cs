// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.Core;
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

        public string Username { get; set; }
        public string Password { get; set; }

        private void CredentialsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowCancel)
                e.Cancel = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _allowCancel = true;
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidateDialog(this))
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information found.",
                    "All fields need to have a value.", PopupButtons.Ok);
                return;
            }

            Username = userNameTextBox.Text;
            Password = passwordTextBox.Text;

            _allowCancel = true;
            DialogResult = DialogResult.OK;
        }
    }
}