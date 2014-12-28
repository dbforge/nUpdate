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
    /// Renders the Windows Flyout
    /// </summary>
    public static class FlyoutRenderer
    {
        /// <summary>
        /// Renders a Windows Flyout Header
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderHeader(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(FlyoutRenderer.CreateHeaderElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Flyout Header Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateHeaderElement()
        {
            return VisualStyleElement.CreateElement("Flyout", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Flyout Body
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderBody(IDeviceContext dc, Rectangle bounds, NormalEmphasizedState state)
        {
            new VisualStyleRenderer(FlyoutRenderer.CreateBodyElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Flyout Body Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateBodyElement(NormalEmphasizedState state)
        {
            return VisualStyleElement.CreateElement("Flyout", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Flyout Label
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderLabel(IDeviceContext dc, Rectangle bounds, NormalSelectedEmphasizedDisabledState state)
        {
            new VisualStyleRenderer(FlyoutRenderer.CreateLabelElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Flyout Label Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateLabelElement(NormalSelectedEmphasizedDisabledState state)
        {
            return VisualStyleElement.CreateElement("Flyout", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Flyout Link
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderLink(IDeviceContext dc, Rectangle bounds, NormalHoverState state)
        {
            new VisualStyleRenderer(FlyoutRenderer.CreateLinkElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Flyout Link Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateLinkElement(NormalHoverState state)
        {
            return VisualStyleElement.CreateElement("Flyout", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Flyout Divider
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderDivider(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(FlyoutRenderer.CreateDividerElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Flyout Divider Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateDividerElement()
        {
            return VisualStyleElement.CreateElement("Flyout", 5, 0);
        }

        /// <summary>
        /// Renders a Windows Flyout Window
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderWindow(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(FlyoutRenderer.CreateWindowElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Flyout Window Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateWindowElement()
        {
            return VisualStyleElement.CreateElement("Flyout", 6, 0);
        }

        /// <summary>
        /// Renders a Windows Flyout LinkArea
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderLinkArea(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(FlyoutRenderer.CreateLinkAreaElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Flyout LinkArea Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateLinkAreaElement()
        {
            return VisualStyleElement.CreateElement("Flyout", 7, 0);
        }

        /// <summary>
        /// Renders a Windows Flyout LinkHeader
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderLinkHeader(IDeviceContext dc, Rectangle bounds, NormalHoverState state)
        {
            new VisualStyleRenderer(FlyoutRenderer.CreateLinkHeaderElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Flyout LinkHeader Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateLinkHeaderElement(NormalHoverState state)
        {
            return VisualStyleElement.CreateElement("Flyout", 8, (int)state);
        }
    }
}