// Copyright © Dominic Beger 2017

using System.Drawing;
using System.Windows.Forms;
using nUpdate.Updating;

namespace nUpdate.UI.Dialogs
{
    public class BaseDialog : Form
    {
        public UpdateManager Updater { get; set; }

        public void InitializeComponent()
        {
            SuspendLayout();
            // 
            // BaseForm
            // 
            ClientSize = new Size(284, 262);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);
            ResumeLayout(false);
        }
    }
}