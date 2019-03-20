// Author: Dominic Beger (Trade/ProgTrade)

using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.UI.WinForms.Dialogs
{
    internal class BaseDialog : Form
    {
        public IUpdateProvider UpdateProvider { get; set; }

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