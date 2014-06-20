using System.Drawing;
using System.Drawing.Drawing2D;

namespace ExplorerNavigationButton
{
    public partial class ExplorerNavigationButton
    {
        private class AeroTemplate : Template
        {
            private readonly RectangleF backgroundRect;
            private readonly GraphicsPath upperBackgroundPath;
            private readonly GraphicsPath lowerBackgroundPath;
            private readonly GraphicsPath arrowPath;

            private readonly SolidBrush normalBackgrounBrush;
            private readonly PathGradientBrush normalUpperBackgroundBrush;
            private readonly PathGradientBrush normalLowerBackgroundBrush;
            private readonly Pen normalArrowPen;

            private readonly SolidBrush hoverBackgrounBrush;
            private readonly PathGradientBrush hoverUpperBackgroundBrush;
            private readonly PathGradientBrush hoverLowerBackgroundBrush;

            private readonly SolidBrush pressedBackgrounBrush;
            private readonly PathGradientBrush pressedUpperBackgroundBrush;
            private readonly PathGradientBrush pressedLowerBackgroundBrush;

            private readonly SolidBrush disabledUpperBackgroundBrush;
            private readonly SolidBrush disabledLowerBackgroundBrush;
            private readonly Pen disabledArrowPen;
            private readonly PathGradientBrush disabledUpperBrush;
            private readonly PathGradientBrush disabledLowerBrush;

            public AeroTemplate()
            {
                this.backgroundRect = new RectangleF(0.5f, 0.5f, 23, 23);

                this.normalBackgrounBrush = new SolidBrush(Color.FromArgb(0, 35, 130));
                this.hoverBackgrounBrush = new SolidBrush(Color.FromArgb(0, 55, 150));
                this.pressedBackgrounBrush = new SolidBrush(Color.FromArgb(0, 10, 50));

                this.upperBackgroundPath = new GraphicsPath();
                this.upperBackgroundPath.AddArc(this.backgroundRect, 180, 180);
                this.upperBackgroundPath.CloseFigure();

                this.lowerBackgroundPath = new GraphicsPath();
                this.lowerBackgroundPath.AddArc(this.backgroundRect, 0, 180);
                this.lowerBackgroundPath.CloseFigure();

                var path1 = new GraphicsPath();
                path1.AddEllipse(new RectangleF(this.backgroundRect.X - 2, this.backgroundRect.Y, this.backgroundRect.Width + 4, this.backgroundRect.Height + 10));
                this.normalUpperBackgroundBrush = new PathGradientBrush(path1);
                var blend1 = new ColorBlend();
                blend1.Colors = new[] { Color.FromArgb(240, Color.White), Color.FromArgb(50, 110, 180), Color.FromArgb(50, 110, 180) };
                blend1.Positions = new[] { 0f, 0.3f, 1f };
                this.normalUpperBackgroundBrush.InterpolationColors = blend1;
                this.hoverUpperBackgroundBrush = new PathGradientBrush(path1);
                var blend9 = new ColorBlend();
                blend9.Colors = new[] { Color.White, Color.FromArgb(65, 150, 240), Color.FromArgb(65, 150, 240) };
                blend9.Positions = new[] { 0f, 0.3f, 1f };
                this.pressedUpperBackgroundBrush = new PathGradientBrush(path1);
                this.hoverUpperBackgroundBrush.InterpolationColors = blend9;
                var blend8 = new ColorBlend();
                blend8.Colors = new[] { Color.FromArgb(200, Color.White), Color.FromArgb(35, 70, 120), Color.FromArgb(35, 70, 120) };
                blend8.Positions = new[] { 0f, 0.4f, 1f };
                this.pressedUpperBackgroundBrush.InterpolationColors = blend8;
                path1.Dispose();

                var path2 = new GraphicsPath();
                path2.AddEllipse(new RectangleF(-16, 13, 56, 20));
                this.normalLowerBackgroundBrush = new PathGradientBrush(path2);
                var blend2 = new ColorBlend();
                blend2.Colors = new[] { Color.FromArgb(0, 35, 130), Color.FromArgb(60, 195, 235), Color.FromArgb(200, 250, 255) };
                blend2.Positions = new[] { 0f, 0.7f, 1f };
                this.normalLowerBackgroundBrush.InterpolationColors = blend2;
                this.hoverLowerBackgroundBrush = new PathGradientBrush(path2);
                var blend10 = new ColorBlend();
                blend10.Colors = new[] { Color.FromArgb(0, 55, 150), Color.FromArgb(60, 195, 235), Color.FromArgb(200, 250, 255) };
                blend10.Positions = new[] { 0f, 0.7f, 1f };
                this.hoverLowerBackgroundBrush.InterpolationColors = blend10;
                this.pressedLowerBackgroundBrush = new PathGradientBrush(path2);
                var blend7 = new ColorBlend();
                blend7.Colors = new[] { Color.FromArgb(0, 10, 50), Color.FromArgb(60, 195, 235), Color.FromArgb(200, 250, 255) };
                blend7.Positions = new[] { 0f, 0.7f, 1f };
                this.pressedLowerBackgroundBrush.InterpolationColors = blend7;
                path2.Dispose();

                arrowPath = new GraphicsPath();
                var arrowTop = new PointF(7f, 12f);
                this.arrowPath.AddLine(new PointF(11.5f, 17f), arrowTop);
                this.arrowPath.AddLine(arrowTop, new PointF(11.5f, 7f));
                this.arrowPath.StartFigure();
                this.arrowPath.AddLine(arrowTop, new PointF(17, 12f));

                var arrowBrush = new LinearGradientBrush(this.backgroundRect, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
                var blend3 = new ColorBlend();
                blend3.Colors = new[] { Color.White, Color.White, Color.FromArgb(130, 130, 130) };
                blend3.Positions = new[] { 0, 0.5f, 1 };
                arrowBrush.InterpolationColors = blend3;

                this.normalArrowPen = new Pen(arrowBrush, 3.5f);
                this.normalArrowPen.StartCap = LineCap.Round;
                this.normalArrowPen.EndCap = LineCap.Round;
                arrowBrush.Dispose();

                this.disabledUpperBackgroundBrush = new SolidBrush(Color.FromArgb(193, 215, 245));
                this.disabledLowerBackgroundBrush = new SolidBrush(Color.FromArgb(160, 195, 225));

                var disabledArrowBrush = new LinearGradientBrush(this.backgroundRect, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
                var blend4 = new ColorBlend();
                blend4.Colors = new[] { Color.FromArgb(215, 230, 250), Color.FromArgb(215, 230, 250), Color.FromArgb(165, 190, 230) };
                blend4.Positions = new[] { 0, 0.5f, 1 };
                disabledArrowBrush.InterpolationColors = blend4;

                this.disabledArrowPen = new Pen(disabledArrowBrush, 3.5f);
                this.disabledArrowPen.StartCap = LineCap.Round;
                this.disabledArrowPen.EndCap = LineCap.Round;
                disabledArrowBrush.Dispose();

                var path3 = new GraphicsPath();
                path3.AddEllipse(new RectangleF(this.backgroundRect.X - 6, this.backgroundRect.Y, this.backgroundRect.Width + 12, this.backgroundRect.Height + 12));
                this.disabledUpperBrush = new PathGradientBrush(path3);
                var blend5 = new ColorBlend();
                blend5.Colors = new[] { Color.FromArgb(200, Color.White), Color.Transparent, Color.Transparent };
                blend5.Positions = new[] { 0f, 0.3f, 1f };
                this.disabledUpperBrush.InterpolationColors = blend5;
                path3.Dispose();

                var path4 = new GraphicsPath();
                path4.AddEllipse(new RectangleF(this.backgroundRect.X - 2, this.backgroundRect.Y, this.backgroundRect.Width + 4, this.backgroundRect.Height));
                this.disabledLowerBrush = new PathGradientBrush(path4);
                var blend6 = new ColorBlend();
                blend6.Colors = new[] { Color.FromArgb(150, Color.White), Color.Transparent, Color.Transparent };
                blend6.Positions = new[] { 0f, 0.3f, 1f };
                this.disabledLowerBrush.InterpolationColors = blend6;
                path4.Dispose();
            }

            protected override void DrawNormal(Graphics g, ArrowDirection direction)
            {
                g.FillPath(this.normalBackgrounBrush, this.lowerBackgroundPath);
                g.FillPath(this.normalUpperBackgroundBrush, this.upperBackgroundPath);
                g.FillEllipse(this.normalLowerBackgroundBrush, this.backgroundRect);

                if (direction == ArrowDirection.Right)
                {
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                }
                g.DrawPath(this.normalArrowPen, this.arrowPath);
            }

            protected override void DrawHover(Graphics g, ArrowDirection direction)
            {
                g.FillPath(this.hoverBackgrounBrush, this.lowerBackgroundPath);
                g.FillPath(this.hoverUpperBackgroundBrush, this.upperBackgroundPath);
                g.FillEllipse(this.hoverLowerBackgroundBrush, this.backgroundRect);

                if (direction == ArrowDirection.Right)
                {
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                }
                g.DrawPath(this.normalArrowPen, this.arrowPath);
            }

            protected override void DrawPressed(Graphics g, ArrowDirection direction)
            {
                g.FillPath(this.pressedBackgrounBrush, this.lowerBackgroundPath);
                g.FillPath(this.pressedUpperBackgroundBrush, this.upperBackgroundPath);
                g.FillEllipse(this.pressedLowerBackgroundBrush, this.backgroundRect);

                if (direction == ArrowDirection.Right)
                {
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                }
                g.DrawPath(this.normalArrowPen, this.arrowPath);
            }

            protected override void DrawDisabled(Graphics g, ArrowDirection direction)
            {
                g.FillPath(this.disabledUpperBackgroundBrush, this.upperBackgroundPath);
                g.FillPath(this.disabledLowerBackgroundBrush, this.lowerBackgroundPath);
                g.FillPath(this.disabledUpperBrush, this.upperBackgroundPath);
                g.FillPath(this.disabledLowerBrush, this.lowerBackgroundPath);

                if (direction == ArrowDirection.Right)
                {
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                }
                g.DrawPath(this.disabledArrowPen, this.arrowPath);
            }

            protected override void Dispose(bool disposing)
            {
                this.upperBackgroundPath.Dispose();
                this.lowerBackgroundPath.Dispose();
                this.arrowPath.Dispose();
                this.normalBackgrounBrush.Dispose();
                this.normalUpperBackgroundBrush.Dispose();
                this.normalLowerBackgroundBrush.Dispose();
                this.normalArrowPen.Dispose();

                this.hoverBackgrounBrush.Dispose();
                this.hoverUpperBackgroundBrush.Dispose();
                this.hoverLowerBackgroundBrush.Dispose();

                this.pressedBackgrounBrush.Dispose();
                this.pressedUpperBackgroundBrush.Dispose();
                this.pressedLowerBackgroundBrush.Dispose();

                this.disabledUpperBackgroundBrush.Dispose();
                this.disabledLowerBackgroundBrush.Dispose();
                this.disabledArrowPen.Dispose();
                this.disabledUpperBrush.Dispose();
                this.disabledLowerBrush.Dispose();
            }
        }
    }
}
