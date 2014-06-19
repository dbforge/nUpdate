using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace nUpdate.UI.Controls
{
    public sealed class ControlPanel : Panel
    {
        public ControlPanel()
        {
            Paint += BottomPanel_Paint;
            BackColor = SystemColors.Control;
        }

        void BottomPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Brushes.LightGray), 0, 1, Width, 1);
        }
    }
}
