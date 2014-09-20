// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.Core.Application;

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
        internal UpdateProject Project { get; set; }

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