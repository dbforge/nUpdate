using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Renders the Windows Track Bar
    /// </summary>
    public static class TrackBarRenderer
    {
        /// <summary>
        /// Renders a Windows Track Bar Track
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderTrack(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TrackBarRenderer.CreateTrackElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Track Bar Track Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateTrackElement()
        {
            return VisualStyleElement.CreateElement("TrackBar", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Track Bar TrackVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderTrackVertical(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TrackBarRenderer.CreateTrackVerticalElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Track Bar TrackVertical Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateTrackVerticalElement()
        {
            return VisualStyleElement.CreateElement("TrackBar", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Track Bar Thumb
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderThumb(IDeviceContext dc, Rectangle bounds, NormalHotPressedFocusedDisabledState state)
        {
            new VisualStyleRenderer(TrackBarRenderer.CreateThumbElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Track Bar Thumb Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateThumbElement(NormalHotPressedFocusedDisabledState state)
        {
            return VisualStyleElement.CreateElement("TrackBar", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Track Bar ThumbBottom
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderThumbBottom(IDeviceContext dc, Rectangle bounds, NormalHotPressedFocusedDisabledState state)
        {
            new VisualStyleRenderer(TrackBarRenderer.CreateThumbBottomElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Track Bar ThumbBottom Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateThumbBottomElement(NormalHotPressedFocusedDisabledState state)
        {
            return VisualStyleElement.CreateElement("TrackBar", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Track Bar ThumbTop
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderThumbTop(IDeviceContext dc, Rectangle bounds, NormalHotPressedFocusedDisabledState state)
        {
            new VisualStyleRenderer(TrackBarRenderer.CreateThumbTopElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Track Bar ThumbTop Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateThumbTopElement(NormalHotPressedFocusedDisabledState state)
        {
            return VisualStyleElement.CreateElement("TrackBar", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Track Bar ThumbVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderThumbVertical(IDeviceContext dc, Rectangle bounds, NormalHotPressedFocusedDisabledState state)
        {
            new VisualStyleRenderer(TrackBarRenderer.CreateThumbVerticalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Track Bar ThumbVertical Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateThumbVerticalElement(NormalHotPressedFocusedDisabledState state)
        {
            return VisualStyleElement.CreateElement("TrackBar", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Track Bar ThumbLeft
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderThumbLeft(IDeviceContext dc, Rectangle bounds, NormalHotPressedFocusedDisabledState state)
        {
            new VisualStyleRenderer(TrackBarRenderer.CreateThumbLeftElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Track Bar ThumbLeft Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateThumbLeftElement(NormalHotPressedFocusedDisabledState state)
        {
            return VisualStyleElement.CreateElement("TrackBar", 7, (int)state);
        }

        /// <summary>
        /// Renders a Windows Track Bar ThumbRight
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderThumbRight(IDeviceContext dc, Rectangle bounds, NormalHotPressedFocusedDisabledState state)
        {
            new VisualStyleRenderer(TrackBarRenderer.CreateThumbRightElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Track Bar ThumbRight Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateThumbRightElement(NormalHotPressedFocusedDisabledState state)
        {
            return VisualStyleElement.CreateElement("TrackBar", 8, (int)state);
        }

        /// <summary>
        /// Renders a Windows Track Bar Tics
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderTics(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TrackBarRenderer.CreateTicsElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Track Bar Tics Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateTicsElement()
        {
            return VisualStyleElement.CreateElement("TrackBar", 9, 0);
        }

        /// <summary>
        /// Renders a Windows Track Bar TicsVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderTicsVertical(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TrackBarRenderer.CreateTicsVerticalElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Track Bar TicsVertical Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateTicsVerticalElement()
        {
            return VisualStyleElement.CreateElement("TrackBar", 10, 0);
        }
    }
}