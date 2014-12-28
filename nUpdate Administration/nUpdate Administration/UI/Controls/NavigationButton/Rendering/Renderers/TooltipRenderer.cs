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
    /// Renders the Windows Tooltip
    /// </summary>
    public static class TooltipRenderer
    {
        /// <summary>
        /// Renders a Windows Tooltip Standard
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderStandard(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TooltipRenderer.CreateStandardElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tooltip Standard Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateStandardElement()
        {
            return VisualStyleElement.CreateElement("Tooltip", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Tooltip StandardTitle
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderStandardTitle(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TooltipRenderer.CreateStandardTitleElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tooltip StandardTitle Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateStandardTitleElement()
        {
            return VisualStyleElement.CreateElement("Tooltip", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Tooltip Balloon
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBalloon(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TooltipRenderer.CreateBalloonElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tooltip Balloon Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBalloonElement()
        {
            return VisualStyleElement.CreateElement("Tooltip", 3, 0);
        }

        /// <summary>
        /// Renders a Windows Tooltip BalloonTitle
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBalloonTitle(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TooltipRenderer.CreateBalloonTitleElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tooltip BalloonTitle Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBalloonTitleElement()
        {
            return VisualStyleElement.CreateElement("Tooltip", 4, 0);
        }

        /// <summary>
        /// Renders a Windows Tooltip Close
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderClose(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(TooltipRenderer.CreateCloseElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tooltip Close Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateCloseElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("Tooltip", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tooltip BalloonStem
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderBalloonStem(IDeviceContext dc, Rectangle bounds, BalloonStemState state)
        {
            new VisualStyleRenderer(TooltipRenderer.CreateBalloonStemElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tooltip BalloonStem Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateBalloonStemElement(BalloonStemState state)
        {
            return VisualStyleElement.CreateElement("Tooltip", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tooltip Wrench
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderWrench(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(TooltipRenderer.CreateWrenchElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tooltip Wrench Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateWrenchElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("Tooltip", 7, (int)state);
        }
    }
}