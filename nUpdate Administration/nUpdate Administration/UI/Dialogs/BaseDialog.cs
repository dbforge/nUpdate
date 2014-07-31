using nUpdate.Administration.Core.Update;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class BaseDialog : Form
    {
        public BaseDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The update project in the current state.
        /// </summary>
        internal UpdateProject Project { get; set; }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseDialog
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "BaseDialog";
            this.BackColor = SystemColors.Window;
            this.Font = new Font("SeogeUI", 8);
            this.ResumeLayout(false);
        }
    }
}
