// BaseDialog.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public class BaseDialog : Form
    {
        public BaseDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The update project in the current state.
        /// </summary>
        public UpdateProject Project { get; set; }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // BaseDialog
            // 
            ClientSize = new Size(284, 262);
            Name = "BaseDialog";
            BackColor = SystemColors.Window;
            Font = new Font("SegoeUI", 8);
            ResumeLayout(false);
        }
    }
}