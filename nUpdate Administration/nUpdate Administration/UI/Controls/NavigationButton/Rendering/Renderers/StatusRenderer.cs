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
    /// Renders the Windows Status
    /// </summary>
    public static class StatusRenderer
    {
        /// <summary>
        /// Renders a Windows Status Pane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StatusRenderer.CreatePaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Status Pane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePaneElement()
        {
            return VisualStyleElement.CreateElement("Status", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Status GripperPane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderGripperPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StatusRenderer.CreateGripperPaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Status GripperPane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateGripperPaneElement()
        {
            return VisualStyleElement.CreateElement("Status", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Status Gripper
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderGripper(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StatusRenderer.CreateGripperElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Status Gripper Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateGripperElement()
        {
            return VisualStyleElement.CreateElement("Status", 3, 0);
        }
    }
}