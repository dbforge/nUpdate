// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using nUpdate.Administration.UI.Dialogs;

namespace nUpdate.Administration.UI.Popups
{
    public partial class PopupDialog : BaseDialog
    {
        public PopupDialog()
        {
            InitializeComponent();
        }

        public Exception Exception { get; set; }
        public Icon PopupIcon { get; set; }
        public string Title { get; set; }
        public string InfoMessage { get; set; }
        public PopupButtons Buttons { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            var measuredSize = e.Graphics.MeasureString(InfoMessage, new Font("Segoe UI", 8));
            if (measuredSize.Height > messageLabel.Height)
            {
                var difference = (int)(measuredSize.Height - messageLabel.Height);
                messageLabel.Height += difference;
                Height += difference;
            }

            base.OnPaint(e);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PopupDialog_Shown(object sender, EventArgs e)
        {
            iconPictureBox.Image = PopupIcon.ToBitmap();
            headerLabel.Text = Title;
            messageLabel.Text = InfoMessage;

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

            if (Title.Length > 40)
                headerLabel.Location = new Point(61, 13);

            if (ReferenceEquals(PopupIcon, SystemIcons.Error))
                SystemSounds.Hand.Play();
            else if (ReferenceEquals(PopupIcon, SystemIcons.Warning))
                SystemSounds.Exclamation.Play();
            else if (ReferenceEquals(PopupIcon, SystemIcons.Question))
                SystemSounds.Question.Play();
        }

        private void copyEntireMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Exception.ToString());
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
    }
}