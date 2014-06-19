using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace nUpdate.Administration.UI.Controls
{
    class Line : Control
    {
        public enum Alignment
        {
            Horizontal,
            Vertical,
        }

        public Alignment LineAlignment { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (LineAlignment == Alignment.Horizontal)
                e.Graphics.DrawLine(new System.Drawing.Pen(new SolidBrush(Color.LightGray)), new Point(5, 5), new Point(500, 5));
            else
                e.Graphics.DrawLine(new System.Drawing.Pen(new SolidBrush(Color.LightGray)), new Point(5, 5), new Point(5, 500));
        }
    }
}
