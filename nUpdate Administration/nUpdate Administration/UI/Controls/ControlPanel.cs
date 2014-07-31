using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Controls
{
    public sealed class ControlPanel : Panel
    {
        public ControlPanel()
        {
            this.Paint += BottomPanel_Paint;
            this.BackColor = SystemColors.Control;
        }

        void BottomPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Brushes.LightGray), 0, 1, Width, 1);
        }
    }
}
