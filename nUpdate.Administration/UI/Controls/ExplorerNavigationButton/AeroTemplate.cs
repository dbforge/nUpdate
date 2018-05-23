// Copyright © Dominic Beger 2018

using System.Drawing;
using System.Drawing.Drawing2D;

namespace ExplorerNavigationButton
{
    public partial class ExplorerNavigationButton
    {
        private class AeroTemplate : Template
        {
            private readonly GraphicsPath _arrowPath;
            private readonly RectangleF _backgroundRect;
            private readonly Pen _disabledArrowPen;
            private readonly SolidBrush _disabledLowerBackgroundBrush;
            private readonly PathGradientBrush _disabledLowerBrush;
            private readonly SolidBrush _disabledUpperBackgroundBrush;
            private readonly PathGradientBrush _disabledUpperBrush;
            private readonly SolidBrush _hoverBackgrounBrush;
            private readonly PathGradientBrush _hoverLowerBackgroundBrush;
            private readonly PathGradientBrush _hoverUpperBackgroundBrush;
            private readonly GraphicsPath _lowerBackgroundPath;
            private readonly Pen _normalArrowPen;
            private readonly SolidBrush _normalBackgrounBrush;
            private readonly PathGradientBrush _normalLowerBackgroundBrush;
            private readonly PathGradientBrush _normalUpperBackgroundBrush;
            private readonly SolidBrush _pressedBackgrounBrush;
            private readonly PathGradientBrush _pressedLowerBackgroundBrush;
            private readonly PathGradientBrush _pressedUpperBackgroundBrush;
            private readonly GraphicsPath _upperBackgroundPath;

            public AeroTemplate()
            {
                _backgroundRect = new RectangleF(0.5f, 0.5f, 23, 23);

                _normalBackgrounBrush = new SolidBrush(Color.FromArgb(0, 35, 130));
                _hoverBackgrounBrush = new SolidBrush(Color.FromArgb(0, 55, 150));
                _pressedBackgrounBrush = new SolidBrush(Color.FromArgb(0, 10, 50));

                _upperBackgroundPath = new GraphicsPath();
                _upperBackgroundPath.AddArc(_backgroundRect, 180, 180);
                _upperBackgroundPath.CloseFigure();

                _lowerBackgroundPath = new GraphicsPath();
                _lowerBackgroundPath.AddArc(_backgroundRect, 0, 180);
                _lowerBackgroundPath.CloseFigure();

                var path1 = new GraphicsPath();
                path1.AddEllipse(new RectangleF(_backgroundRect.X - 2, _backgroundRect.Y, _backgroundRect.Width + 4,
                    _backgroundRect.Height + 10));
                _normalUpperBackgroundBrush = new PathGradientBrush(path1);
                var blend1 = new ColorBlend();
                blend1.Colors = new[]
                    {Color.FromArgb(240, Color.White), Color.FromArgb(50, 110, 180), Color.FromArgb(50, 110, 180)};
                blend1.Positions = new[] {0f, 0.3f, 1f};
                _normalUpperBackgroundBrush.InterpolationColors = blend1;
                _hoverUpperBackgroundBrush = new PathGradientBrush(path1);
                var blend9 = new ColorBlend();
                blend9.Colors = new[] {Color.White, Color.FromArgb(65, 150, 240), Color.FromArgb(65, 150, 240)};
                blend9.Positions = new[] {0f, 0.3f, 1f};
                _pressedUpperBackgroundBrush = new PathGradientBrush(path1);
                _hoverUpperBackgroundBrush.InterpolationColors = blend9;
                var blend8 = new ColorBlend();
                blend8.Colors = new[]
                    {Color.FromArgb(200, Color.White), Color.FromArgb(35, 70, 120), Color.FromArgb(35, 70, 120)};
                blend8.Positions = new[] {0f, 0.4f, 1f};
                _pressedUpperBackgroundBrush.InterpolationColors = blend8;
                path1.Dispose();

                var path2 = new GraphicsPath();
                path2.AddEllipse(new RectangleF(-16, 13, 56, 20));
                _normalLowerBackgroundBrush = new PathGradientBrush(path2);
                var blend2 = new ColorBlend();
                blend2.Colors = new[]
                    {Color.FromArgb(0, 35, 130), Color.FromArgb(60, 195, 235), Color.FromArgb(200, 250, 255)};
                blend2.Positions = new[] {0f, 0.7f, 1f};
                _normalLowerBackgroundBrush.InterpolationColors = blend2;
                _hoverLowerBackgroundBrush = new PathGradientBrush(path2);
                var blend10 = new ColorBlend();
                blend10.Colors = new[]
                    {Color.FromArgb(0, 55, 150), Color.FromArgb(60, 195, 235), Color.FromArgb(200, 250, 255)};
                blend10.Positions = new[] {0f, 0.7f, 1f};
                _hoverLowerBackgroundBrush.InterpolationColors = blend10;
                _pressedLowerBackgroundBrush = new PathGradientBrush(path2);
                var blend7 = new ColorBlend();
                blend7.Colors = new[]
                    {Color.FromArgb(0, 10, 50), Color.FromArgb(60, 195, 235), Color.FromArgb(200, 250, 255)};
                blend7.Positions = new[] {0f, 0.7f, 1f};
                _pressedLowerBackgroundBrush.InterpolationColors = blend7;
                path2.Dispose();

                _arrowPath = new GraphicsPath();
                var arrowTop = new PointF(7f, 12f);
                _arrowPath.AddLine(new PointF(11.5f, 17f), arrowTop);
                _arrowPath.AddLine(arrowTop, new PointF(11.5f, 7f));
                _arrowPath.StartFigure();
                _arrowPath.AddLine(arrowTop, new PointF(17, 12f));

                var arrowBrush = new LinearGradientBrush(_backgroundRect, Color.Transparent, Color.Transparent,
                    LinearGradientMode.Vertical);
                var blend3 = new ColorBlend();
                blend3.Colors = new[] {Color.White, Color.White, Color.FromArgb(130, 130, 130)};
                blend3.Positions = new[] {0, 0.5f, 1};
                arrowBrush.InterpolationColors = blend3;

                _normalArrowPen = new Pen(arrowBrush, 3.5f);
                _normalArrowPen.StartCap = LineCap.Round;
                _normalArrowPen.EndCap = LineCap.Round;
                arrowBrush.Dispose();

                _disabledUpperBackgroundBrush = new SolidBrush(Color.FromArgb(193, 215, 245));
                _disabledLowerBackgroundBrush = new SolidBrush(Color.FromArgb(160, 195, 225));

                var disabledArrowBrush = new LinearGradientBrush(_backgroundRect, Color.Transparent, Color.Transparent,
                    LinearGradientMode.Vertical);
                var blend4 = new ColorBlend();
                blend4.Colors = new[]
                    {Color.FromArgb(215, 230, 250), Color.FromArgb(215, 230, 250), Color.FromArgb(165, 190, 230)};
                blend4.Positions = new[] {0, 0.5f, 1};
                disabledArrowBrush.InterpolationColors = blend4;

                _disabledArrowPen = new Pen(disabledArrowBrush, 3.5f);
                _disabledArrowPen.StartCap = LineCap.Round;
                _disabledArrowPen.EndCap = LineCap.Round;
                disabledArrowBrush.Dispose();

                var path3 = new GraphicsPath();
                path3.AddEllipse(new RectangleF(_backgroundRect.X - 6, _backgroundRect.Y, _backgroundRect.Width + 12,
                    _backgroundRect.Height + 12));
                _disabledUpperBrush = new PathGradientBrush(path3);
                var blend5 = new ColorBlend();
                blend5.Colors = new[] {Color.FromArgb(200, Color.White), Color.Transparent, Color.Transparent};
                blend5.Positions = new[] {0f, 0.3f, 1f};
                _disabledUpperBrush.InterpolationColors = blend5;
                path3.Dispose();

                var path4 = new GraphicsPath();
                path4.AddEllipse(new RectangleF(_backgroundRect.X - 2, _backgroundRect.Y, _backgroundRect.Width + 4,
                    _backgroundRect.Height));
                _disabledLowerBrush = new PathGradientBrush(path4);
                var blend6 = new ColorBlend();
                blend6.Colors = new[] {Color.FromArgb(150, Color.White), Color.Transparent, Color.Transparent};
                blend6.Positions = new[] {0f, 0.3f, 1f};
                _disabledLowerBrush.InterpolationColors = blend6;
                path4.Dispose();
            }

            protected override void Dispose(bool disposing)
            {
                _upperBackgroundPath.Dispose();
                _lowerBackgroundPath.Dispose();
                _arrowPath.Dispose();
                _normalBackgrounBrush.Dispose();
                _normalUpperBackgroundBrush.Dispose();
                _normalLowerBackgroundBrush.Dispose();
                _normalArrowPen.Dispose();

                _hoverBackgrounBrush.Dispose();
                _hoverUpperBackgroundBrush.Dispose();
                _hoverLowerBackgroundBrush.Dispose();

                _pressedBackgrounBrush.Dispose();
                _pressedUpperBackgroundBrush.Dispose();
                _pressedLowerBackgroundBrush.Dispose();

                _disabledUpperBackgroundBrush.Dispose();
                _disabledLowerBackgroundBrush.Dispose();
                _disabledArrowPen.Dispose();
                _disabledUpperBrush.Dispose();
                _disabledLowerBrush.Dispose();
            }

            protected override void DrawDisabled(Graphics g, ArrowDirection direction)
            {
                g.FillPath(_disabledUpperBackgroundBrush, _upperBackgroundPath);
                g.FillPath(_disabledLowerBackgroundBrush, _lowerBackgroundPath);
                g.FillPath(_disabledUpperBrush, _upperBackgroundPath);
                g.FillPath(_disabledLowerBrush, _lowerBackgroundPath);

                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                g.DrawPath(_disabledArrowPen, _arrowPath);
            }

            protected override void DrawHover(Graphics g, ArrowDirection direction)
            {
                g.FillPath(_hoverBackgrounBrush, _lowerBackgroundPath);
                g.FillPath(_hoverUpperBackgroundBrush, _upperBackgroundPath);
                g.FillEllipse(_hoverLowerBackgroundBrush, _backgroundRect);

                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                g.DrawPath(_normalArrowPen, _arrowPath);
            }

            protected override void DrawNormal(Graphics g, ArrowDirection direction)
            {
                g.FillPath(_normalBackgrounBrush, _lowerBackgroundPath);
                g.FillPath(_normalUpperBackgroundBrush, _upperBackgroundPath);
                g.FillEllipse(_normalLowerBackgroundBrush, _backgroundRect);

                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                g.DrawPath(_normalArrowPen, _arrowPath);
            }

            protected override void DrawPressed(Graphics g, ArrowDirection direction)
            {
                g.FillPath(_pressedBackgrounBrush, _lowerBackgroundPath);
                g.FillPath(_pressedUpperBackgroundBrush, _upperBackgroundPath);
                g.FillEllipse(_pressedLowerBackgroundBrush, _backgroundRect);

                if (direction == ArrowDirection.Right)
                    g.MultiplyTransform(new Matrix(-1, 0, 0, 1, 24f, 0));
                g.DrawPath(_normalArrowPen, _arrowPath);
            }
        }
    }
}