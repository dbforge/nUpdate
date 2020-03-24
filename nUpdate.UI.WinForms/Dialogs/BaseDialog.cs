// BaseDialog.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal class BaseDialog : Form
    {
        public UpdateCheckResult UpdateCheckResult { get; set; }
        public UpdateProvider UpdateProvider { get; set; }

        public void InitializeComponent()
        {
            SuspendLayout();
            // 
            // BaseForm
            // 
            ClientSize = new Size(284, 262);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);
            ResumeLayout(true);
        }
    }
}