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
    /// Renders the Windows List Box
    /// </summary>
    public static class ListBoxRenderer
    {
        /// <summary>
        /// Renders a Windows List Box BorderHScroll
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderBorderHScroll(IDeviceContext dc, Rectangle bounds, NormalFocusedHotDisabledState state)
        {
            new VisualStyleRenderer(ListBoxRenderer.CreateBorderHScrollElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows List Box BorderHScroll Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateBorderHScrollElement(NormalFocusedHotDisabledState state)
        {
            return VisualStyleElement.CreateElement("ListBox", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows List Box BorderHVScroll
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderBorderHVScroll(IDeviceContext dc, Rectangle bounds, NormalFocusedHotDisabledState state)
        {
            new VisualStyleRenderer(ListBoxRenderer.CreateBorderHVScrollElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows List Box BorderHVScroll Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateBorderHVScrollElement(NormalFocusedHotDisabledState state)
        {
            return VisualStyleElement.CreateElement("ListBox", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows List Box BorderNoScroll
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderBorderNoScroll(IDeviceContext dc, Rectangle bounds, NormalFocusedHotDisabledState state)
        {
            new VisualStyleRenderer(ListBoxRenderer.CreateBorderNoScrollElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows List Box BorderNoScroll Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateBorderNoScrollElement(NormalFocusedHotDisabledState state)
        {
            return VisualStyleElement.CreateElement("ListBox", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows List Box BorderVScroll
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderBorderVScroll(IDeviceContext dc, Rectangle bounds, NormalFocusedHotDisabledState state)
        {
            new VisualStyleRenderer(ListBoxRenderer.CreateBorderVScrollElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows List Box BorderVScroll Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateBorderVScrollElement(NormalFocusedHotDisabledState state)
        {
            return VisualStyleElement.CreateElement("ListBox", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows List Box Item
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderItem(IDeviceContext dc, Rectangle bounds, ItemState state)
        {
            new VisualStyleRenderer(ListBoxRenderer.CreateItemElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows List Box Item Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateItemElement(ItemState state)
        {
            return VisualStyleElement.CreateElement("ListBox", 5, (int)state);
        }
    }
}