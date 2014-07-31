using HttpPostRequestLib.Net;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class FeedbackDialog : BaseDialog
    {
        private List<string> unwishedWords = new List<string>();
        private HTTPPostRequest httpPost = null;

        public FeedbackDialog()
        {
            InitializeComponent();
        }

        private void InitializeUnwishedWords()
        {
            string[] unwishedContent = new string[] { "dumm", "arsch", "hure", "hurensohn", "wichser", "wixxer", "wixer", "schlampe", "hurä", "bullshit", "scheiß", "scheis", "fotze", "muschi", "bastard", "fick", "idiot", "depp", "dreck", "müll", "bitch", "asshole", "fuck", "fool", "heil", "hitler", "nazi", "penis", "vagina", "screw", "shit", "baisse", "merde", "nique" };
            this.unwishedWords.AddRange(unwishedContent);
        }

        /// <summary>
        /// Checks if the adress is a valid e-mail address.
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
                MessageBox.Show("Please fill out all fields.", "Empty fields", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!this.IsValidMailAdress(emailTextBox.Text))
            {
                MessageBox.Show("Please enter a valid E-mail address.", "Invalid address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.unwishedWords.Any(item => emailTextBox.Text.ToLower().Contains(item)) || this.unwishedWords.Any(item => this.nameTextBox.Text.ToLower().Contains(item)) || this.unwishedWords.Any(item => this.contentTextBox.Text.ToLower().Contains(item)))
            {
                MessageBox.Show("Your text contains insulting words. Think about it again!", "Insulting content", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string returnedString = null;

            try
            {
                this.httpPost = new HTTPPostRequest("http://www.trade-programming.de/nupdate/mail.php");
                this.httpPost.Post.Add("name", nameTextBox.Text);
                this.httpPost.Post.Add("sender", emailTextBox.Text);
                this.httpPost.Post.Add("content", contentTextBox.Text);
                returnedString = this.httpPost.Submit();
                if (!String.IsNullOrEmpty(returnedString))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while sending feedback.", String.Format("Please report this message: {0}", returnedString), PopupButtons.OK);
                    return;
                }

                Popup.ShowPopup(this, SystemIcons.Information, "Delivering successful.", "The feedback was sent. Thank you!", PopupButtons.OK);
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while sending feedback.", ex, PopupButtons.OK);
            }
        }

        private void FeedbackForm_Load(object sender, EventArgs e)
        {
            InitializeUnwishedWords();
        }

        private void privacyTermsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Popup.ShowPopup(this, SystemIcons.Information, "Privacy policy.", "Your e-mail-adress and/or name will not be published or shared.", PopupButtons.OK);
        }
    }
}
