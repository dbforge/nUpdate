// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Net.Mail;
using nUpdate.Administration.Core;
using nUpdate.Administration.UI.Popups;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class FeedbackDialog : BaseDialog
    {
        public FeedbackDialog()
        {
            InitializeComponent();
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
                new MailAddress(address);
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
                Popup.ShowPopup(this, SystemIcons.Error, "Missing information", "All fields need to have a value.",
                    PopupButtons.Ok);
                return;
            }

            if (!IsValidMailAdress(emailTextBox.Text))
            {
                Popup.ShowPopup(SystemIcons.Error, "Invalid e-mail address", "Please enter a valid e-mail address.",
                    PopupButtons.Ok);
                return;
            }

            try
            {
                string responseString;
                using (var client = new WebClientWrapper())
                {
                    responseString =
                        client.DownloadString(
                            String.Format(
                                "http://www.nupdate.net/mail.php?name={0}&sender={1}&content={2}",
                                nameTextBox.Text, emailTextBox.Text, contentTextBox.Text));
                }

                if (!String.IsNullOrEmpty(responseString) && responseString != "\n")
                {
                    Popup.ShowPopup(this, SystemIcons.Error, "Error while sending feedback.",
                        String.Format("Please report this message: {0}", responseString), PopupButtons.Ok);
                    return;
                }

                Popup.ShowPopup(this, SystemIcons.Information, "Delivering successful.",
                    "The feedback was sent. Thank you!", PopupButtons.Ok);
                Close();
            }
            catch (Exception ex)
            {
                Popup.ShowPopup(this, SystemIcons.Error, "Error while sending feedback.", ex, PopupButtons.Ok);
            }
        }

        private void FeedbackDialog_Load(object sender, EventArgs e)
        {
            Text = String.Format(Text, Program.VersionString);
        }
    }
}