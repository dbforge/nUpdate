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
    /// Renders the Windows Explorer Bar
    /// </summary>
    public static class ExplorerBarRenderer
    {
        /// <summary>
        /// Renders a Windows Explorer Bar HeaderBackground
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderHeaderBackground(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateHeaderBackgroundElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar HeaderBackground Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateHeaderBackgroundElement()
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Explorer Bar HeaderClose
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHeaderClose(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateHeaderCloseElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar HeaderClose Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHeaderCloseElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Explorer Bar HeaderPin
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHeaderPin(IDeviceContext dc, Rectangle bounds, PinState state)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateHeaderPinElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar HeaderPin Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHeaderPinElement(PinState state)
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Explorer Bar HeaderIEBarMenu
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHeaderIEBarMenu(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateHeaderIEBarMenuElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar HeaderIEBarMenu Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHeaderIEBarMenuElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Explorer Bar NormalGroupBackground
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderNormalGroupBackground(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateNormalGroupBackgroundElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar NormalGroupBackground Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateNormalGroupBackgroundElement()
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 5, 0);
        }

        /// <summary>
        /// Renders a Windows Explorer Bar NormalGroupCollapse
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderNormalGroupCollapse(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateNormalGroupCollapseElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar NormalGroupCollapse Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateNormalGroupCollapseElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Explorer Bar NormalGroupExpand
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderNormalGroupExpand(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateNormalGroupExpandElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar NormalGroupExpand Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateNormalGroupExpandElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 7, (int)state);
        }

        /// <summary>
        /// Renders a Windows Explorer Bar NormalGroupHead
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderNormalGroupHead(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateNormalGroupHeadElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar NormalGroupHead Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateNormalGroupHeadElement()
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 8, 0);
        }

        /// <summary>
        /// Renders a Windows Explorer Bar SpecialGroupBackground
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSpecialGroupBackground(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateSpecialGroupBackgroundElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar SpecialGroupBackground Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSpecialGroupBackgroundElement()
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 9, 0);
        }

        /// <summary>
        /// Renders a Windows Explorer Bar SpecialGroupCollapse
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSpecialGroupCollapse(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateSpecialGroupCollapseElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar SpecialGroupCollapse Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSpecialGroupCollapseElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 10, (int)state);
        }

        /// <summary>
        /// Renders a Windows Explorer Bar SpecialGroupExpand
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSpecialGroupExpand(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateSpecialGroupExpandElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar SpecialGroupExpand Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSpecialGroupExpandElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 11, (int)state);
        }

        /// <summary>
        /// Renders a Windows Explorer Bar SpecialGroupHead
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSpecialGroupHead(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ExplorerBarRenderer.CreateSpecialGroupHeadElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Explorer Bar SpecialGroupHead Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSpecialGroupHeadElement()
        {
            return VisualStyleElement.CreateElement("ExplorerBar", 12, 0);
        }
    }
}