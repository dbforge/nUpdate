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
    /// Renders the Windows Header
    /// </summary>
    public static class HeaderRenderer
    {
        /// <summary>
        /// Renders a Windows Header HeaderItem
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHeaderItem(IDeviceContext dc, Rectangle bounds, HeaderItemState state)
        {
            new VisualStyleRenderer(HeaderRenderer.CreateHeaderItemElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Header HeaderItem Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHeaderItemElement(HeaderItemState state)
        {
            return VisualStyleElement.CreateElement("Header", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Header HeaderItemLeft
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHeaderItemLeft(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(HeaderRenderer.CreateHeaderItemLeftElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Header HeaderItemLeft Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHeaderItemLeftElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("Header", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Header HeaderItemRight
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHeaderItemRight(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(HeaderRenderer.CreateHeaderItemRightElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Header HeaderItemRight Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHeaderItemRightElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("Header", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Header HeaderSortArrow
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHeaderSortArrow(IDeviceContext dc, Rectangle bounds, SortState state)
        {
            new VisualStyleRenderer(HeaderRenderer.CreateHeaderSortArrowElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Header HeaderSortArrow Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHeaderSortArrowElement(SortState state)
        {
            return VisualStyleElement.CreateElement("Header", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Header HeaderDropDown
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHeaderDropDown(IDeviceContext dc, Rectangle bounds, NormalSoftHotHotState state)
        {
            new VisualStyleRenderer(HeaderRenderer.CreateHeaderDropDownElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Header HeaderDropDown Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHeaderDropDownElement(NormalSoftHotHotState state)
        {
            return VisualStyleElement.CreateElement("Header", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Header HeaderDropDownFilter
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHeaderDropDownFilter(IDeviceContext dc, Rectangle bounds, NormalSoftHotHotState state)
        {
            new VisualStyleRenderer(HeaderRenderer.CreateHeaderDropDownFilterElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Header HeaderDropDownFilter Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHeaderDropDownFilterElement(NormalSoftHotHotState state)
        {
            return VisualStyleElement.CreateElement("Header", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Header HeaderOverflow
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHeaderOverflow(IDeviceContext dc, Rectangle bounds, NormalHotState state)
        {
            new VisualStyleRenderer(HeaderRenderer.CreateHeaderOverflowElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Header HeaderOverflow Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHeaderOverflowElement(NormalHotState state)
        {
            return VisualStyleElement.CreateElement("Header", 7, (int)state);
        }
    }
}