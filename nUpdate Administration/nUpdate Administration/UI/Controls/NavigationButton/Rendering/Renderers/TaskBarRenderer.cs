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
    /// Renders the Windows Task Bar
    /// </summary>
    public static class TaskBarRenderer
    {
        /// <summary>
        /// Renders a Windows Task Bar BackgroundBottom
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBackgroundBottom(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskBarRenderer.CreateBackgroundBottomElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Bar BackgroundBottom Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBackgroundBottomElement()
        {
            return VisualStyleElement.CreateElement("TaskBar", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Task Bar BackgroundRight
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBackgroundRight(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskBarRenderer.CreateBackgroundRightElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Bar BackgroundRight Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBackgroundRightElement()
        {
            return VisualStyleElement.CreateElement("TaskBar", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Task Bar BackgroundTop
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBackgroundTop(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskBarRenderer.CreateBackgroundTopElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Bar BackgroundTop Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBackgroundTopElement()
        {
            return VisualStyleElement.CreateElement("TaskBar", 3, 0);
        }

        /// <summary>
        /// Renders a Windows Task Bar BackgroundLeft
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBackgroundLeft(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskBarRenderer.CreateBackgroundLeftElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Bar BackgroundLeft Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBackgroundLeftElement()
        {
            return VisualStyleElement.CreateElement("TaskBar", 4, 0);
        }

        /// <summary>
        /// Renders a Windows Task Bar SizingBarBottom
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSizingBarBottom(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskBarRenderer.CreateSizingBarBottomElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Bar SizingBarBottom Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSizingBarBottomElement()
        {
            return VisualStyleElement.CreateElement("TaskBar", 5, 0);
        }

        /// <summary>
        /// Renders a Windows Task Bar SizingBarRight
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSizingBarRight(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskBarRenderer.CreateSizingBarRightElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Bar SizingBarRight Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSizingBarRightElement()
        {
            return VisualStyleElement.CreateElement("TaskBar", 6, 0);
        }

        /// <summary>
        /// Renders a Windows Task Bar SizingBarTop
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSizingBarTop(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskBarRenderer.CreateSizingBarTopElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Bar SizingBarTop Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSizingBarTopElement()
        {
            return VisualStyleElement.CreateElement("TaskBar", 7, 0);
        }

        /// <summary>
        /// Renders a Windows Task Bar SizingBarLeft
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSizingBarLeft(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskBarRenderer.CreateSizingBarLeftElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Bar SizingBarLeft Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSizingBarLeftElement()
        {
            return VisualStyleElement.CreateElement("TaskBar", 8, 0);
        }
    }
}