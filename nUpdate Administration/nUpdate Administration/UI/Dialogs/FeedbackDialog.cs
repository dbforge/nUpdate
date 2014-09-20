// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class FeedbackDialog : BaseDialog
    {
        private readonly List<string> _unwishedWords = new List<string>();

        public FeedbackDialog()
        {
            InitializeComponent();
        }

        private void InitializeUnwishedWords()
        {
            string[] unwishedContent =
            {
                "dumm", "arsch", "hure", "hurensohn", "wichser", "wixxer", "wixer", "schlampe",
                "hurä", "bullshit", "scheiß", "scheis", "fotze", "muschi", "bastard", "fick", "idiot", "depp", "dreck",
                "müll", "bitch", "asshole", "fuck", "fool", "heil", "hitler", "nazi", "penis", "vagina", "screw", "shit",
                "baisse", "merde", "nique"
            };
            _unwishedWords.AddRange(unwishedContent);
        }

        /// <summary>
        ///     Checks if the adress is a valid e-mail address.
        /// </summary>
        /// <param name="address">The adress to check.</param>
        /// <returns>Returns true if the mail address is valid.</returns>
        public bool IsValidMailAdress(string address)
        {
            try
            {
                MailAddress m = new MailAddress(address);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (!ValidationManager.ValidateDialog(this))
            {
                MessageBox.Show("Please fill out all fields.", "Empty fields", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (!IsValidMailAdress(emailTextBox.Text))
            {
                MessageBox.Show("Please enter a valid E-mail address.", "Invalid address", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (_unwishedWords.Any(item => emailTextBox.Text.ToLower().Contains(item)) ||
                _unwishedWords.Any(item => nameTextBox.Text.ToLower().Contains(item)) ||
                _unwishedWords.Any(item => contentTextBox.Text.ToLower().Contains(item)))
            {
                MessageBox.Show("Your text contains insulting words. Think about it again!", "Insulting content",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string responseString = new WebClient().DownloadString(String.Format("http://www.trade-programming.de/nupdate/mail.php?name={0}&sender={1}&content={2}",
                    nameTextBox.Text, emailTextBox.Text, contentTextBox.Text));
                if (!String.IsNullOrEmpty(responseString))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while sending feedback.",
                        String.Format("Please report this message: {0}", responseString), PopupButtons.Ok);
                    return;
                }

                Popup.ShowPopup(this, SystemIcons.Information, "Delivering successful.",
                    "The feedback was sent. Thank you!", PopupButtons.Ok);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while sending feedback.", ex, PopupButtons.Ok);
            }
        }

        private void FeedbackForm_Load(object sender, EventArgs e)
        {
            InitializeUnwishedWords();
        }

        private void privacyTermsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Privacy policy.",
                "Your e-mail-adress and/or name will not be published or shared.", PopupButtons.Ok);
        }
    }
}