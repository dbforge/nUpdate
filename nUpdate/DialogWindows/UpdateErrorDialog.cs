using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace nUpdate.DialogWindows
{
    public partial class UpdateErrorDialog : Form
    {
        public UpdateErrorDialog()
        {
            InitializeComponent();
        }


        #region "Properties"

        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string InfoMessage { get; set; }

        #endregion

        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        private void UpdateErrorDialog_Load(object sender, EventArgs e)
        {
            lbl_info.Text = InfoMessage;
            lbl_errorcode.Text = "Fehlercode: " + ErrorCode;
            txt_errormessage.Text = ErrorMessage;
            btn_close.Focus();
            pictureBox1.Image = SystemIcons.Error.ToBitmap();
            pictureBox1.BackgroundImageLayout = ImageLayout.Center;

            this.Icon = AppIcon;
            this.Text = Application.ProductName;
        }

        private void btn_close_Click(object sender, EventArgs e)
        {

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txt_errormessage_MouseEnter(object sender, EventArgs e)
        {
            btn_close.Focus();
        }

        private void txt_errormessage_Enter(object sender, EventArgs e)
        {
            btn_close.Focus();
        }
    }
}
