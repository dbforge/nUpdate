using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using nUpdate.UI.Controls;

namespace nUpdate.Dialogs
{
    public partial class UpdateErrorDialog : BaseForm
    {
        public UpdateErrorDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the code that is shown in the dialog.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Sets the exact message that is shown in the dialog.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Sets the short message that is shown on top of the dialog.
        /// </summary>
        public string InfoMessage { get; set; }

        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        private void UpdateErrorDialog_Load(object sender, EventArgs e)
        {
            infoLabel.Text = InfoMessage;

            if (ErrorCode == 0)
                errorCodeLabel.Text = "Errorcode: -";
            else
                errorCodeLabel.Text = String.Format("Errorcode: {0}", ErrorCode);

            errorMessageTextBox.Text = ErrorMessage;

            closeButton.Focus();

            iconPictureBox.Image = SystemIcons.Error.ToBitmap();
            iconPictureBox.BackgroundImageLayout = ImageLayout.Center;

            this.Icon = AppIcon;
            this.Text = Application.ProductName;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
