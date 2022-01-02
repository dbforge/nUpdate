// Line.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.UI.WindowsForms.Controls
{
    public class Line : Control
    {
        public enum Alignment
        {
            Horizontal,
            Vertical
        }

        public Alignment LineAlignment { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.LightGray)), new Point(5, 5),
                LineAlignment == Alignment.Horizontal ? new Point(500, 5) : new Point(5, 500));
        }
    }
}