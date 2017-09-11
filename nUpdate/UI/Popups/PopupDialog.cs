// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace nUpdate.Internal.UI.Popups
{
    public partial class PopupDialog : Form
    {
        public PopupDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the buttons to show for the user-interaction.
        /// </summary>
        public PopupButtons Buttons { get; set; }

        /// <summary>
        ///     Gets or sets the exception containing the message that should be shown in the text of the popup.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        ///     Gets or sets the text of the popup.
        /// </summary>
        public string InfoMessage { get; set; }

        /// <summary>
        ///     Gets or sets the icon to show.
        /// </summary>
        public Icon PopupIcon { get; set; }

        /// <summary>
        ///     Gets or sets the title of the popup.
        /// </summary>
        public string Title { get; set; }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void copyEntireMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Exception.ToString());
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        private void PopupDialog_Shown(object sender, EventArgs e)
        {
            iconPictureBox.Image = PopupIcon.ToBitmap();
            headerLabel.Text = Title;
            messageLabel.Text = InfoMessage;

            if (headerLabel.Height > 20)
                headerLabel.Location = new Point(headerLabel.Location.X, headerLabel.Location.Y - 7);

            if (messageLabel.Height > 41)
            {
                var difference = messageLabel.Height - 41;
                messageLabel.Height += difference;
                Height += difference;
                controlPanel1.Location = new Point(controlPanel1.Location.X, controlPanel1.Location.Y + difference);
            }

            if (Buttons == PopupButtons.Ok)
            {
                closeButton.Visible = true;
                AcceptButton = closeButton;
            }
            else
            {
                noButton.Visible = true;
                yesButton.Visible = true;
                AcceptButton = noButton;
            }

            if (Exception == null)
                contextMenu.Enabled = false;

            if (ReferenceEquals(PopupIcon, SystemIcons.Error))
                SystemSounds.Hand.Play();
            else if (ReferenceEquals(PopupIcon, SystemIcons.Warning))
                SystemSounds.Exclamation.Play();
            else if (ReferenceEquals(PopupIcon, SystemIcons.Question))
                SystemSounds.Question.Play();
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }
    }
}