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
    /// Renders the Windows Navigation Button
    /// </summary>
    public static class NavigationButtonRenderer
    {
        /// <summary>
        /// Renders a Windows Navigation Button BackButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderBackButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(NavigationButtonRenderer.CreateBackButtonElement(state)).DrawBackground(dc, bounds);
            
        }

        /// <summary>
        /// Creates a Windows Navigation Button BackButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateBackButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Navigation", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Navigation Button ForwardButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderForwardButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(NavigationButtonRenderer.CreateForwardButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Navigation Button ForwardButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateForwardButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Navigation", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Navigation Button MenuButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMenuButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(NavigationButtonRenderer.CreateMenuButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Navigation Button MenuButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMenuButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Navigation", 3, (int)state);
        }
    }
}