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
    /// Renders the Windows Tab Control
    /// </summary>
    public static class TabControlRenderer
    {
        /// <summary>
        /// Renders a Windows Tab Control TabItem
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTabItem(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(TabControlRenderer.CreateTabItemElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tab Control TabItem Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTabItemElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("Tab", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tab Control TabItemLeftEdge
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTabItemLeftEdge(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(TabControlRenderer.CreateTabItemLeftEdgeElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tab Control TabItemLeftEdge Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTabItemLeftEdgeElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("Tab", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tab Control TabItemRightEdge
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTabItemRightEdge(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(TabControlRenderer.CreateTabItemRightEdgeElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tab Control TabItemRightEdge Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTabItemRightEdgeElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("Tab", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tab Control TabItemBothEdge
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTabItemBothEdge(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(TabControlRenderer.CreateTabItemBothEdgeElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tab Control TabItemBothEdge Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTabItemBothEdgeElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("Tab", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tab Control TopTabItem
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTopTabItem(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(TabControlRenderer.CreateTopTabItemElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tab Control TopTabItem Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTopTabItemElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("Tab", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tab Control TopTabItemLeftEdge
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTopTabItemLeftEdge(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(TabControlRenderer.CreateTopTabItemLeftEdgeElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tab Control TopTabItemLeftEdge Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTopTabItemLeftEdgeElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("Tab", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tab Control TopTabItemRightEdge
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTopTabItemRightEdge(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(TabControlRenderer.CreateTopTabItemRightEdgeElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tab Control TopTabItemRightEdge Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTopTabItemRightEdgeElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("Tab", 7, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tab Control TopTabItemBothEdge
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTopTabItemBothEdge(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(TabControlRenderer.CreateTopTabItemBothEdgeElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tab Control TopTabItemBothEdge Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTopTabItemBothEdgeElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("Tab", 8, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tab Control Pane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TabControlRenderer.CreatePaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tab Control Pane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePaneElement()
        {
            return VisualStyleElement.CreateElement("Tab", 9, 0);
        }

        /// <summary>
        /// Renders a Windows Tab Control Body
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBody(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TabControlRenderer.CreateBodyElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tab Control Body Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBodyElement()
        {
            return VisualStyleElement.CreateElement("Tab", 10, 0);
        }

        /// <summary>
        /// Renders a Windows Tab Control AeroWizardBody
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderAeroWizardBody(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TabControlRenderer.CreateAeroWizardBodyElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tab Control AeroWizardBody Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateAeroWizardBodyElement()
        {
            return VisualStyleElement.CreateElement("Tab", 11, 0);
        }
    }
}