using System.Drawing;
using System.Drawing.Drawing2D;

namespace ExplorerNavigationButton
{
    public partial class ExplorerNavigationButton
    {
        private class MetroTemplate : Template
        {
            private readonly RectangleF circleRect;
            private readonly Pen normalPen;
            private readonly Pen normalArrowPen;
            private readonly GraphicsPath arrowPath;
            private readonly SolidBrush hoverBrush;
            private readonly Pen hoverArrowPen;
            private readonly SolidBrush pressedBrush;
            private readonly Pen disabledArrowPen;
            private readonly Pen disabledPen;

            public MetroTemplate()
            {
                this.circleRect = new RectangleF(2.5f, 2.5f, 18f, 18f);

                this.normalPen = new Pen(Color.FromArgb(100, 100, 100), 1.5f);
                this.disabledPen = new Pen(Color.FromArgb(200, 200, 200), 1.5f);

                this.normalArrowPen = new Pen(Color.FromArgb(100, 100, 100), 2);
                this.hoverArrowPen = new Pen(Color.White, 2);
                this.disabledArrowPen = new Pen(Color.FromArgb(200, 200, 200), 2);

                this.arrowPath = new GraphicsPath(FillMode.Alternate);
                var arrowTop = new PointF(7.5f, 11.5f);
                this.arrowPath.AddLine(new PointF(11.5f, 15.5f), arrowTop);
                this.arrowPath.AddLine(arrowTop, new PointF(11.5f, 7.5f));
                this.arrowPath.StartFigure();
                this.arrowPath.AddLine(arrowTop, new PointF(16.5f, 11.5f));

                this.hoverBrush = new SolidBrush(Color.FromArgb(50, 152, 254));
                this.pressedBrush = new SolidBrush(Color.FromArgb(54, 116, 178));
            }

            protected override void DrawNormal(Graphics g, ArrowDirection direction)
            {
                g.DrawEllipse(this.normalPen, this.circleRect);
                if (direction == ArrowDirection.Right)
                {
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 23, 0));
                }
                g.DrawPath(this.normalArrowPen, this.arrowPath);
            }

            protected override void DrawHover(Graphics g, ArrowDirection direction)
            {
                g.FillEllipse(this.hoverBrush, new RectangleF(this.circleRect.X - 0.5f, this.circleRect.Y - 0.5f, this.circleRect.Width + 1, this.circleRect.Height + 1));
                if (direction == ArrowDirection.Right)
                {
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 23, 0));
                }
                g.DrawPath(this.hoverArrowPen, this.arrowPath);
            }

            protected override void DrawPressed(Graphics g, ArrowDirection direction)
            {
                g.FillEllipse(this.pressedBrush, new RectangleF(this.circleRect.X - 0.5f, this.circleRect.Y - 0.5f, this.circleRect.Width + 1, this.circleRect.Height + 1));
                if (direction == ArrowDirection.Right)
                {
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 23, 0));
                }
                g.DrawPath(this.hoverArrowPen, this.arrowPath);
            }

            protected override void DrawDisabled(Graphics g, ArrowDirection direction)
            {
                g.DrawEllipse(this.disabledPen, this.circleRect);
                if (direction == ArrowDirection.Right)
                {
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 23, 0));
                }
                g.DrawPath(this.disabledArrowPen, this.arrowPath);
            }

            protected override void Dispose(bool disposing)
            {
                this.normalPen.Dispose();
                this.normalArrowPen.Dispose();
                this.arrowPath.Dispose();
                this.hoverBrush.Dispose();
                this.hoverArrowPen.Dispose();
                this.pressedBrush.Dispose();
                this.disabledArrowPen.Dispose();
                this.disabledPen.Dispose();

                base.Dispose(disposing);
            }
        }
    }
}
