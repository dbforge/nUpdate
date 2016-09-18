// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.UI.Dialogs
{
    public class BaseDialog : Form
    {
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