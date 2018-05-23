// Copyright © Dominic Beger 2018

using System.Drawing;
using System.Drawing.Drawing2D;

namespace ExplorerNavigationButton
{
    public partial class ExplorerNavigationButton
    {
        private class MetroTemplate : Template
        {
            private readonly GraphicsPath _arrowPath;
            private readonly RectangleF _circleRect;
            private readonly Pen _disabledArrowPen;
            private readonly Pen _disabledPen;
            private readonly Pen _hoverArrowPen;
            private readonly SolidBrush _hoverBrush;
            private readonly Pen _normalArrowPen;
            private readonly Pen _normalPen;
            private readonly SolidBrush _pressedBrush;

            public MetroTemplate()
            {
                _circleRect = new RectangleF(2.5f, 2.5f, 18f, 18f);

                _normalPen = new Pen(Color.FromArgb(100, 100, 100), 1.5f);
                _disabledPen = new Pen(Color.FromArgb(200, 200, 200), 1.5f);

                _normalArrowPen = new Pen(Color.FromArgb(100, 100, 100), 2);
                _hoverArrowPen = new Pen(Color.White, 2);
                _disabledArrowPen = new Pen(Color.FromArgb(200, 200, 200), 2);

                _arrowPath = new GraphicsPath(FillMode.Alternate);
                var arrowTop = new PointF(7.5f, 11.5f);
                _arrowPath.AddLine(new PointF(11.5f, 15.5f), arrowTop);
                _arrowPath.AddLine(arrowTop, new PointF(11.5f, 7.5f));
                _arrowPath.StartFigure();
                _arrowPath.AddLine(arrowTop, new PointF(16.5f, 11.5f));

                _hoverBrush = new SolidBrush(Color.FromArgb(50, 152, 254));
                _pressedBrush = new SolidBrush(Color.FromArgb(54, 116, 178));
            }

            protected override void Dispose(bool disposing)
            {
                _normalPen.Dispose();
                _normalArrowPen.Dispose();
                _arrowPath.Dispose();
                _hoverBrush.Dispose();
                _hoverArrowPen.Dispose();
                _pressedBrush.Dispose();
                _disabledArrowPen.Dispose();
                _disabledPen.Dispose();

                base.Dispose(disposing);
            }

            protected override void DrawDisabled(Graphics g, ArrowDirection direction)
            {
                g.DrawEllipse(_disabledPen, _circleRect);
                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 23, 0));
                g.DrawPath(_disabledArrowPen, _arrowPath);
            }

            protected override void DrawHover(Graphics g, ArrowDirection direction)
            {
                g.FillEllipse(_hoverBrush,
                    new RectangleF(_circleRect.X - 0.5f, _circleRect.Y - 0.5f, _circleRect.Width + 1,
                        _circleRect.Height + 1));
                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 23, 0));
                g.DrawPath(_hoverArrowPen, _arrowPath);
            }

            protected override void DrawNormal(Graphics g, ArrowDirection direction)
            {
                g.DrawEllipse(_normalPen, _circleRect);
                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 23, 0));
                g.DrawPath(_normalArrowPen, _arrowPath);
            }

            protected override void DrawPressed(Graphics g, ArrowDirection direction)
            {
                g.FillEllipse(_pressedBrush,
                    new RectangleF(_circleRect.X - 0.5f, _circleRect.Y - 0.5f, _circleRect.Width + 1,
                        _circleRect.Height + 1));
                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 23, 0));
                g.DrawPath(_hoverArrowPen, _arrowPath);
            }
        }
    }
}