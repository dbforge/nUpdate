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
    /// Renders the Windows Tool Bar
    /// </summary>
    public static class ToolBarRenderer
    {
        /// <summary>
        /// Renders a Windows Tool Bar Button
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderButton(IDeviceContext dc, Rectangle bounds, ToolBarElementState state)
        {
            new VisualStyleRenderer(ToolBarRenderer.CreateButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tool Bar Button Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateButtonElement(ToolBarElementState state)
        {
            return VisualStyleElement.CreateElement("Toolbar", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tool Bar DropDownButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderDropDownButton(IDeviceContext dc, Rectangle bounds, ToolBarElementState state)
        {
            new VisualStyleRenderer(ToolBarRenderer.CreateDropDownButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tool Bar DropDownButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateDropDownButtonElement(ToolBarElementState state)
        {
            return VisualStyleElement.CreateElement("Toolbar", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tool Bar SplitButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSplitButton(IDeviceContext dc, Rectangle bounds, ToolBarElementState state)
        {
            new VisualStyleRenderer(ToolBarRenderer.CreateSplitButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tool Bar SplitButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSplitButtonElement(ToolBarElementState state)
        {
            return VisualStyleElement.CreateElement("Toolbar", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tool Bar SplitButtonDropDown
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSplitButtonDropDown(IDeviceContext dc, Rectangle bounds, ToolBarElementState state)
        {
            new VisualStyleRenderer(ToolBarRenderer.CreateSplitButtonDropDownElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tool Bar SplitButtonDropDown Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSplitButtonDropDownElement(ToolBarElementState state)
        {
            return VisualStyleElement.CreateElement("Toolbar", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tool Bar Seperator
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSeperator(IDeviceContext dc, Rectangle bounds, ToolBarElementState state)
        {
            new VisualStyleRenderer(ToolBarRenderer.CreateSeperatorElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tool Bar Seperator Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSeperatorElement(ToolBarElementState state)
        {
            return VisualStyleElement.CreateElement("Toolbar", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tool Bar SeperatorVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSeperatorVertical(IDeviceContext dc, Rectangle bounds, ToolBarElementState state)
        {
            new VisualStyleRenderer(ToolBarRenderer.CreateSeperatorVerticalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tool Bar SeperatorVertical Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSeperatorVerticalElement(ToolBarElementState state)
        {
            return VisualStyleElement.CreateElement("Toolbar", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Tool Bar DropDownButtonGlyph
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderDropDownButtonGlyph(IDeviceContext dc, Rectangle bounds, ToolBarElementState state)
        {
            new VisualStyleRenderer(ToolBarRenderer.CreateDropDownButtonGlyphElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Tool Bar DropDownButtonGlyph Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateDropDownButtonGlyphElement(ToolBarElementState state)
        {
            return VisualStyleElement.CreateElement("Toolbar", 7, (int)state);
        }
    }
}