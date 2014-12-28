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
    /// Renders the Windows Spinner
    /// </summary>
    public static class SpinnerRenderer
    {
        /// <summary>
        /// Renders a Windows Spinner Up
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderUp(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(SpinnerRenderer.CreateUpElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Spinner Up Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateUpElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Spin", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Spinner Down
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderDown(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(SpinnerRenderer.CreateDownElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Spinner Down Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateDownElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Spin", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Spinner UpHorizontal
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderUpHorizontal(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(SpinnerRenderer.CreateUpHorizontalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Spinner UpHorizontal Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateUpHorizontalElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Spin", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Spinner DownHorizontal
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderDownHorizontal(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(SpinnerRenderer.CreateDownHorizontalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Spinner DownHorizontal Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateDownHorizontalElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Spin", 4, (int)state);
        }
    }
}