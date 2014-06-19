using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            pict_icon.Image = SystemIcons.Error.ToBitmap();

            if (!String.IsNullOrEmpty(InfoMessage) && !String.IsNullOrEmpty(ErrorMessage))
            {
                infoLabel.Text = InfoMessage;
                errorMessageTextBox.Text = ErrorMessage;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ErrorDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void errorMessageTextBox_Enter(object sender, EventArgs e)
        {
            closeButton.Focus();
        }
    }
}
