using System;
using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.UpdateInstaller
{
    public partial class ErrorDialog : Form
    {
        public string InfoMessage { get; set; }
        public string ErrorMessage { get; set; }

        public ErrorDialog()
        {
            InitializeComponent();
        }

        private void ErrorDialog_Load(object sender, EventArgs e)
        {
            this.pict_icon.Image = SystemIcons.Error.ToBitmap();

            if (!String.IsNullOrEmpty(this.InfoMessage) && !String.IsNullOrEmpty(this.ErrorMessage))
            {
                this.infoLabel.Text = this.InfoMessage;
                this.errorMessageTextBox.Text = this.ErrorMessage;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void ErrorDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void errorMessageTextBox_Enter(object sender, EventArgs e)
        {
            this.closeButton.Focus();
        }
    }
}
