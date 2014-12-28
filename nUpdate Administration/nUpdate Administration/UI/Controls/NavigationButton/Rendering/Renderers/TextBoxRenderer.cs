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
    /// Renders the Windows Text Box
    /// </summary>
    public static class TextBoxRenderer
    {
        /// <summary>
        /// Renders a Windows Text Box EditText
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderEditText(IDeviceContext dc, Rectangle bounds, TextEditState state)
        {
            new VisualStyleRenderer(TextBoxRenderer.CreateEditTextElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Box EditText Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateEditTextElement(TextEditState state)
        {
            return VisualStyleElement.CreateElement("Edit", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Text Box Caret
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderCaret(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TextBoxRenderer.CreateCaretElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Box Caret Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateCaretElement()
        {
            return VisualStyleElement.CreateElement("Edit", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Text Box Background
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderBackground(IDeviceContext dc, Rectangle bounds, TextEditState state)
        {
            new VisualStyleRenderer(TextBoxRenderer.CreateBackgroundElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Box Background Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateBackgroundElement(TextEditState state)
        {
            return VisualStyleElement.CreateElement("Edit", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Text Box Password
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPassword(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TextBoxRenderer.CreatePasswordElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Box Password Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePasswordElement()
        {
            return VisualStyleElement.CreateElement("Edit", 4, 0);
        }

        /// <summary>
        /// Renders a Windows Text Box BackgroundWithBorder
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderBackgroundWithBorder(IDeviceContext dc, Rectangle bounds, NormalHotFocusedDisabledState state)
        {
            new VisualStyleRenderer(TextBoxRenderer.CreateBackgroundWithBorderElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Box BackgroundWithBorder Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateBackgroundWithBorderElement(NormalHotFocusedDisabledState state)
        {
            return VisualStyleElement.CreateElement("Edit", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Text Box EditBorderNoScroll
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderEditBorderNoScroll(IDeviceContext dc, Rectangle bounds, NormalHotFocusedDisabledState state)
        {
            new VisualStyleRenderer(TextBoxRenderer.CreateEditBorderNoScrollElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Box EditBorderNoScroll Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateEditBorderNoScrollElement(NormalHotFocusedDisabledState state)
        {
            return VisualStyleElement.CreateElement("Edit", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Text Box EditBorderHScroll
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderEditBorderHScroll(IDeviceContext dc, Rectangle bounds, NormalHotFocusedDisabledState state)
        {
            new VisualStyleRenderer(TextBoxRenderer.CreateEditBorderHScrollElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Box EditBorderHScroll Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateEditBorderHScrollElement(NormalHotFocusedDisabledState state)
        {
            return VisualStyleElement.CreateElement("Edit", 7, (int)state);
        }

        /// <summary>
        /// Renders a Windows Text Box EditBorderVScroll
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderEditBorderVScroll(IDeviceContext dc, Rectangle bounds, NormalHotFocusedDisabledState state)
        {
            new VisualStyleRenderer(TextBoxRenderer.CreateEditBorderVScrollElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Box EditBorderVScroll Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateEditBorderVScrollElement(NormalHotFocusedDisabledState state)
        {
            return VisualStyleElement.CreateElement("Edit", 8, (int)state);
        }

        /// <summary>
        /// Renders a Windows Text Box EditBorderHVScroll
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderEditBorderHVScroll(IDeviceContext dc, Rectangle bounds, NormalHotFocusedDisabledState state)
        {
            new VisualStyleRenderer(TextBoxRenderer.CreateEditBorderHVScrollElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Box EditBorderHVScroll Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateEditBorderHVScrollElement(NormalHotFocusedDisabledState state)
        {
            return VisualStyleElement.CreateElement("Edit", 9, (int)state);
        }
    }
}