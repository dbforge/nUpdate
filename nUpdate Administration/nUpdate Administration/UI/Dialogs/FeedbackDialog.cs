using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;
using HttpPostRequestLib.Net;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class FeedbackDialog : BaseDialog
    {
        private readonly List<string> unwishedWords = new List<string>();
        private HTTPPostRequest httpPost;

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
            unwishedWords.AddRange(unwishedContent);
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
                var m = new MailAddress(address);
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

            if (unwishedWords.Any(item => emailTextBox.Text.ToLower().Contains(item)) ||
                unwishedWords.Any(item => nameTextBox.Text.ToLower().Contains(item)) ||
                unwishedWords.Any(item => contentTextBox.Text.ToLower().Contains(item)))
            {
                MessageBox.Show("Your text contains insulting words. Think about it again!", "Insulting content",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string returnedString = null;

            try
            {
                httpPost = new HTTPPostRequest("http://www.trade-programming.de/nupdate/mail.php");
                httpPost.Post.Add("name", nameTextBox.Text);
                httpPost.Post.Add("sender", emailTextBox.Text);
                httpPost.Post.Add("content", contentTextBox.Text);
                returnedString = httpPost.Submit();
                if (!String.IsNullOrEmpty(returnedString))
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while sending feedback.",
                        String.Format("Please report this message: {0}", returnedString), PopupButtons.OK);
                    return;
                }

                Popup.ShowPopup(this, SystemIcons.Information, "Delivering successful.",
                    "The feedback was sent. Thank you!", PopupButtons.OK);
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
            Popup.ShowPopup(this, SystemIcons.Information, "Privacy policy.",
                "Your e-mail-adress and/or name will not be published or shared.", PopupButtons.OK);
        }
    }
}