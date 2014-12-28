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
    /// Renders the Windows Task Band
    /// </summary>
    public static class TaskBandRenderer
    {
        /// <summary>
        /// Renders a Windows Task Band GroupCount
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderGroupCount(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskBandRenderer.CreateGroupCountElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Band GroupCount Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateGroupCountElement()
        {
            return VisualStyleElement.CreateElement("TaskBand", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Task Band FlashButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderFlashButton(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskBandRenderer.CreateFlashButtonElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Band FlashButton Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateFlashButtonElement()
        {
            return VisualStyleElement.CreateElement("TaskBand", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Task Band FlashButtonGroupMenu
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderFlashButtonGroupMenu(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskBandRenderer.CreateFlashButtonGroupMenuElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Band FlashButtonGroupMenu Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateFlashButtonGroupMenuElement()
        {
            return VisualStyleElement.CreateElement("TaskBand", 3, 0);
        }
    }
}