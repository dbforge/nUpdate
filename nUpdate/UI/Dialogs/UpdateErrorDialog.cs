using System;
using System.Drawing;
using System.Windows.Forms;

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
            this.infoLabel.Text = this.InfoMessage;

            if (this.ErrorCode == 0)
            {
                this.errorCodeLabel.Text = "Errorcode: -";
            }
            else
            {
                this.errorCodeLabel.Text = String.Format("Errorcode: {0}", this.ErrorCode);
            }

            this.errorMessageTextBox.Text = ErrorMessage;

            this.closeButton.Focus();

            this.iconPictureBox.Image = SystemIcons.Error.ToBitmap();
            this.iconPictureBox.BackgroundImageLayout = ImageLayout.Center;

            this.Icon = AppIcon;
            this.Text = Application.ProductName;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
