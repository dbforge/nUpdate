// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System.Drawing;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Controls
{
    public class Line : Control
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
                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.LightGray)), new Point(5, 5), new Point(500, 5));
            else
                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.LightGray)), new Point(5, 5), new Point(5, 500));
        }
    }
}