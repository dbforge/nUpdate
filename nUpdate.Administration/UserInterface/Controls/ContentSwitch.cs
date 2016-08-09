using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using nUpdate.Administration.Win32;

namespace nUpdate.Administration.UserInterface.Controls
{
    public class ContentSwitch : RadioButton
    {
        private bool _isMouseOver;

        public ContentSwitch()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.Clear(BackColor);
            pevent.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            pevent.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            const int sideOffset = 4;

            uint color;
            bool opaque;
            NativeMethods.DwmGetColorizationColor(out color, out opaque);
            var windowsColor = Color.FromArgb((int)color);

            if (Checked || _isMouseOver)
            {
                var backgroundRectangle = new Rectangle(new Point(sideOffset, 0), new Size(Width - sideOffset, Height));
                using (var sidePath = GraphicUtils.RoundedRectangle(new Rectangle(new Point(0, 0), new Size(sideOffset, Height)), 0))
                {
                    using (var pathBrush = new SolidBrush(windowsColor))
                        pevent.Graphics.FillPath(pathBrush, sidePath);

                    if (Checked)
                    {
                        using (var brushBackground = new LinearGradientBrush(backgroundRectangle, SystemColors.Control, SystemColors.Window, LinearGradientMode.Horizontal))
                            pevent.Graphics.FillRectangle(brushBackground, backgroundRectangle);
                    }
                    else
                    {
                        pevent.Graphics.FillRectangle(SystemBrushes.Control, backgroundRectangle);
                        pevent.Graphics.DrawLine(SystemPens.ActiveCaptionText, new Point(Width, 0), new Point(Width, Height));
                    }

                    pevent.Graphics.DrawLine(SystemPens.ActiveCaptionText, new Point(sideOffset, 0), new Point(Width, 0));
                    pevent.Graphics.DrawLine(SystemPens.ActiveCaptionText, new Point(sideOffset, Height), new Point(Width, Height));
                }
            }
            else
            {
                var rectangleTop = new Rectangle(new Point(0, 0), new Size(Width, 1));
                using (var brushTop = new LinearGradientBrush(rectangleTop, SystemColors.ControlLight, BackColor, LinearGradientMode.Horizontal))
                    pevent.Graphics.FillRectangle(brushTop, rectangleTop);

                pevent.Graphics.DrawLine(SystemPens.ActiveCaptionText, new Point(Width, 0), new Point(Width, Height));
            }

            TextRenderer.DrawText(pevent.Graphics, Text, Font, new Rectangle(sideOffset + 5, 0, Width - (sideOffset + 5), Height), Enabled ? ForeColor : SystemColors.GrayText, TextFormatFlags.VerticalCenter);
        }

        protected override void OnMouseEnter(EventArgs eventargs)
        {
            _isMouseOver = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs eventargs)
        {
            _isMouseOver = false;
            Invalidate();
        }
    }
}