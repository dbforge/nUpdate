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
    /// Renders the Windows Date Picker
    /// </summary>
    public static class DatePickerRenderer
    {
        /// <summary>
        /// Renders a Windows Date Picker DateText
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderDateText(IDeviceContext dc, Rectangle bounds, NormalDisabledSelectedState state)
        {
            new VisualStyleRenderer(DatePickerRenderer.CreateDateTextElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Date Picker DateText Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateDateTextElement(NormalDisabledSelectedState state)
        {
            return VisualStyleElement.CreateElement("DatePicker", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Date Picker DateBorder
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderDateBorder(IDeviceContext dc, Rectangle bounds, BorderState state)
        {
            new VisualStyleRenderer(DatePickerRenderer.CreateDateBorderElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Date Picker DateBorder Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateDateBorderElement(BorderState state)
        {
            return VisualStyleElement.CreateElement("DatePicker", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Date Picker ShowCalendarButtonRight
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderShowCalendarButtonRight(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(DatePickerRenderer.CreateShowCalendarButtonRightElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Date Picker ShowCalendarButtonRight Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateShowCalendarButtonRightElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("DatePicker", 3, (int)state);
        }
    }
}