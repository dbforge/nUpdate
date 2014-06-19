using System.Drawing;
using System.Drawing.Drawing2D;

namespace ExplorerNavigationButton
{
    public partial class ExplorerNavigationButton
    {
        private class AeroTemplate : Template
        {
            readonly RectangleF backgroundRect;
            readonly GraphicsPath upperBackgroundPath;
            readonly GraphicsPath lowerBackgroundPath;
            readonly GraphicsPath arrowPath;

            readonly SolidBrush normalBackgrounBrush;
            readonly PathGradientBrush normalUpperBackgroundBrush;
            readonly PathGradientBrush normalLowerBackgroundBrush;
            readonly Pen normalArrowPen;

            readonly SolidBrush hoverBackgrounBrush;
            readonly PathGradientBrush hoverUpperBackgroundBrush;
            readonly PathGradientBrush hoverLowerBackgroundBrush;

            readonly SolidBrush pressedBackgrounBrush;
            readonly PathGradientBrush pressedUpperBackgroundBrush;
            readonly PathGradientBrush pressedLowerBackgroundBrush;

            readonly SolidBrush disabledUpperBackgroundBrush;
            readonly SolidBrush disabledLowerBackgroundBrush;
            readonly Pen disabledArrowPen;
            readonly PathGradientBrush disabledUpperBrush;
            readonly PathGradientBrush disabledLowerBrush;

            public AeroTemplate()
            {
                backgroundRect = new RectangleF(0.5f, 0.5f, 23, 23);

                normalBackgrounBrush = new SolidBrush(Color.FromArgb(0, 35, 130));
                hoverBackgrounBrush = new SolidBrush(Color.FromArgb(0, 55, 150));
                pressedBackgrounBrush = new SolidBrush(Color.FromArgb(0, 10, 50));

                upperBackgroundPath = new GraphicsPath();
                upperBackgroundPath.AddArc(backgroundRect, 180, 180);
                upperBackgroundPath.CloseFigure();

                lowerBackgroundPath = new GraphicsPath();
                lowerBackgroundPath.AddArc(backgroundRect, 0, 180);
                lowerBackgroundPath.CloseFigure();

                var path1 = new GraphicsPath();
                path1.AddEllipse(new RectangleF(backgroundRect.X - 2, backgroundRect.Y, backgroundRect.Width + 4, backgroundRect.Height + 10));
                normalUpperBackgroundBrush = new PathGradientBrush(path1);
                var blend1 = new ColorBlend();
                blend1.Colors = new[] { Color.FromArgb(240, Color.White), Color.FromArgb(50, 110, 180), Color.FromArgb(50, 110, 180) };
                blend1.Positions = new[] { 0f, 0.3f, 1f };
                normalUpperBackgroundBrush.InterpolationColors = blend1;
                hoverUpperBackgroundBrush = new PathGradientBrush(path1);
                var blend9 = new ColorBlend();
                blend9.Colors = new[] { Color.White, Color.FromArgb(65, 150, 240), Color.FromArgb(65, 150, 240) };
                blend9.Positions = new[] { 0f, 0.3f, 1f };
                pressedUpperBackgroundBrush = new PathGradientBrush(path1);
                hoverUpperBackgroundBrush.InterpolationColors = blend9;
                var blend8 = new ColorBlend();
                blend8.Colors = new[] { Color.FromArgb(200, Color.White), Color.FromArgb(35, 70, 120), Color.FromArgb(35, 70, 120) };
                blend8.Positions = new[] { 0f, 0.4f, 1f };
                pressedUpperBackgroundBrush.InterpolationColors = blend8;
                path1.Dispose();

                var path2 = new GraphicsPath();
                path2.AddEllipse(new RectangleF(-16, 13, 56, 20));
                normalLowerBackgroundBrush = new PathGradientBrush(path2);
                var blend2 = new ColorBlend();
                blend2.Colors = new[] { Color.FromArgb(0, 35, 130), Color.FromArgb(60, 195, 235), Color.FromArgb(200, 250, 255) };
                blend2.Positions = new[] { 0f, 0.7f, 1f };
                normalLowerBackgroundBrush.InterpolationColors = blend2;
                hoverLowerBackgroundBrush = new PathGradientBrush(path2);
                var blend10 = new ColorBlend();
                blend10.Colors = new[] { Color.FromArgb(0, 55, 150), Color.FromArgb(60, 195, 235), Color.FromArgb(200, 250, 255) };
                blend10.Positions = new[] { 0f, 0.7f, 1f };
                hoverLowerBackgroundBrush.InterpolationColors = blend10;
                pressedLowerBackgroundBrush = new PathGradientBrush(path2);
                var blend7 = new ColorBlend();
                blend7.Colors = new[] { Color.FromArgb(0, 10, 50), Color.FromArgb(60, 195, 235), Color.FromArgb(200, 250, 255) };
                blend7.Positions = new[] { 0f, 0.7f, 1f };
                pressedLowerBackgroundBrush.InterpolationColors = blend7;
                path2.Dispose();

                arrowPath = new GraphicsPath();
                var arrowTop = new PointF(7f, 12f);
                arrowPath.AddLine(new PointF(11.5f, 17f), arrowTop);
                arrowPath.AddLine(arrowTop, new PointF(11.5f, 7f));
                arrowPath.StartFigure();
                arrowPath.AddLine(arrowTop, new PointF(17, 12f));

                var arrowBrush = new LinearGradientBrush(backgroundRect, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
                var blend3 = new ColorBlend();
                blend3.Colors = new[] { Color.White, Color.White, Color.FromArgb(130, 130, 130) };
                blend3.Positions = new[] { 0, 0.5f, 1 };
                arrowBrush.InterpolationColors = blend3;

                normalArrowPen = new Pen(arrowBrush, 3.5f);
                normalArrowPen.StartCap = LineCap.Round;
                normalArrowPen.EndCap = LineCap.Round;
                arrowBrush.Dispose();

                disabledUpperBackgroundBrush = new SolidBrush(Color.FromArgb(193, 215, 245));
                disabledLowerBackgroundBrush = new SolidBrush(Color.FromArgb(160, 195, 225));

                var disabledArrowBrush = new LinearGradientBrush(backgroundRect, Color.Transparent, Color.Transparent, LinearGradientMode.Vertical);
                var blend4 = new ColorBlend();
                blend4.Colors = new[] { Color.FromArgb(215, 230, 250), Color.FromArgb(215, 230, 250), Color.FromArgb(165, 190, 230) };
                blend4.Positions = new[] { 0, 0.5f, 1 };
                disabledArrowBrush.InterpolationColors = blend4;

                disabledArrowPen = new Pen(disabledArrowBrush, 3.5f);
                disabledArrowPen.StartCap = LineCap.Round;
                disabledArrowPen.EndCap = LineCap.Round;
                disabledArrowBrush.Dispose();

                var path3 = new GraphicsPath();
                path3.AddEllipse(new RectangleF(backgroundRect.X - 6, backgroundRect.Y, backgroundRect.Width + 12, backgroundRect.Height + 12));
                disabledUpperBrush = new PathGradientBrush(path3);
                var blend5 = new ColorBlend();
                blend5.Colors = new[] { Color.FromArgb(200, Color.White), Color.Transparent, Color.Transparent };
                blend5.Positions = new[] { 0f, 0.3f, 1f };
                disabledUpperBrush.InterpolationColors = blend5;
                path3.Dispose();

                var path4 = new GraphicsPath();
                path4.AddEllipse(new RectangleF(backgroundRect.X - 2, backgroundRect.Y, backgroundRect.Width + 4, backgroundRect.Height));
                disabledLowerBrush = new PathGradientBrush(path4);
                var blend6 = new ColorBlend();
                blend6.Colors = new[] { Color.FromArgb(150, Color.White), Color.Transparent, Color.Transparent };
                blend6.Positions = new[] { 0f, 0.3f, 1f };
                disabledLowerBrush.InterpolationColors = blend6;
                path4.Dispose();
            }

            protected override void DrawNormal(Graphics g, ArrowDirection direction)
            {
                g.FillPath(normalBackgrounBrush, lowerBackgroundPath);
                g.FillPath(normalUpperBackgroundBrush, upperBackgroundPath);
                g.FillEllipse(normalLowerBackgroundBrush, backgroundRect);

                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                g.DrawPath(normalArrowPen, arrowPath);
            }

            protected override void DrawHover(Graphics g, ArrowDirection direction)
            {
                g.FillPath(hoverBackgrounBrush, lowerBackgroundPath);
                g.FillPath(hoverUpperBackgroundBrush, upperBackgroundPath);
                g.FillEllipse(hoverLowerBackgroundBrush, backgroundRect);

                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                g.DrawPath(normalArrowPen, arrowPath);
            }

            protected override void DrawPressed(Graphics g, ArrowDirection direction)
            {
                g.FillPath(pressedBackgrounBrush, lowerBackgroundPath);
                g.FillPath(pressedUpperBackgroundBrush, upperBackgroundPath);
                g.FillEllipse(pressedLowerBackgroundBrush, backgroundRect);

                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                g.DrawPath(normalArrowPen, arrowPath);
            }

            protected override void DrawDisabled(Graphics g, ArrowDirection direction)
            {
                g.FillPath(disabledUpperBackgroundBrush, upperBackgroundPath);
                g.FillPath(disabledLowerBackgroundBrush, lowerBackgroundPath);
                g.FillPath(disabledUpperBrush, upperBackgroundPath);
                g.FillPath(disabledLowerBrush, lowerBackgroundPath);

                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                g.DrawPath(disabledArrowPen, arrowPath);
            }

            protected override void Dispose(bool disposing)
            {
                upperBackgroundPath.Dispose();
                lowerBackgroundPath.Dispose();
                arrowPath.Dispose();
                normalBackgrounBrush.Dispose();
                normalUpperBackgroundBrush.Dispose();
                normalLowerBackgroundBrush.Dispose();
                normalArrowPen.Dispose();

                hoverBackgrounBrush.Dispose();
                hoverUpperBackgroundBrush.Dispose();
                hoverLowerBackgroundBrush.Dispose();

                pressedBackgrounBrush.Dispose();
                pressedUpperBackgroundBrush.Dispose();
                pressedLowerBackgroundBrush.Dispose();

                disabledUpperBackgroundBrush.Dispose();
                disabledLowerBackgroundBrush.Dispose();
                disabledArrowPen.Dispose();
                disabledUpperBrush.Dispose();
                disabledLowerBrush.Dispose();
            }
        }
    }
}
