using System;
using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Popups
{
    public partial class PopupDialog : Form
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

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PopupDialog_Shown(object sender, EventArgs e)
        {
            this.iconPictureBox.Image = this.PopupIcon.ToBitmap();
            this.headerLabel.Text = this.Title;
            this.messageLabel.Text = this.InfoMessage;

            if (Buttons == PopupButtons.OK)
            {
                this.closeButton.Visible = true;
                this.AcceptButton = this.closeButton;
            }
            else
            {
                this.noButton.Visible = true;
                this.yesButton.Visible = true;
                this.AcceptButton = this.noButton;
            }

            if (this.Exception == null)
            {
                this.contextMenu.Enabled = false;
            }

            if (this.Title.Length > 40)
            {
                this.headerLabel.Location = new Point(61, 13);
            }
        }

        private void copyEntireMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.Exception.ToString());
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
