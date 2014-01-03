using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nUpdate.DialogWindows
{
    public partial class NoUpdateFoundDialog : Form
    {
        public NoUpdateFoundDialog()
        {
            InitializeComponent();
        }

        public Icon AppIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        private void NoUpdateFoundDialog_Load(object sender, EventArgs e)
        {
            Icon = AppIcon;
            this.Text = Application.ProductName;
            btn_ok.Focus();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
