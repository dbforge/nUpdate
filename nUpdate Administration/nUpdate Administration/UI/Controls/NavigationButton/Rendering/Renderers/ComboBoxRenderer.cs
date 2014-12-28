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
    /// Renders the Windows Combo Box
    /// </summary>
    public static class ComboBoxRenderer
    {
        /// <summary>
        /// Renders a Windows Combo Box DropDownButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderDropDownButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(ComboBoxRenderer.CreateDropDownButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Combo Box DropDownButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateDropDownButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Combobox", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Combo Box Background
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBackground(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ComboBoxRenderer.CreateBackgroundElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Combo Box Background Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBackgroundElement()
        {
            return VisualStyleElement.CreateElement("Combobox", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Combo Box TransparentBackground
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderTransparentBackground(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ComboBoxRenderer.CreateTransparentBackgroundElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Combo Box TransparentBackground Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateTransparentBackgroundElement()
        {
            return VisualStyleElement.CreateElement("Combobox", 3, 0);
        }

        /// <summary>
        /// Renders a Windows Combo Box Border
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderBorder(IDeviceContext dc, Rectangle bounds, BorderState state)
        {
            new VisualStyleRenderer(ComboBoxRenderer.CreateBorderElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Combo Box Border Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateBorderElement(BorderState state)
        {
            return VisualStyleElement.CreateElement("Combobox", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Combo Box ReadOnly
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderReadOnly(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(ComboBoxRenderer.CreateReadOnlyElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Combo Box ReadOnly Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateReadOnlyElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Combobox", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Combo Box DropDownButtonRight
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderDropDownButtonRight(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(ComboBoxRenderer.CreateDropDownButtonRightElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Combo Box DropDownButtonRight Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateDropDownButtonRightElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Combobox", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Combo Box DropDownButtonLeft
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderDropDownButtonLeft(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(ComboBoxRenderer.CreateDropDownButtonLeftElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Combo Box DropDownButtonLeft Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateDropDownButtonLeftElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Combobox", 7, (int)state);
        }

        /// <summary>
        /// Renders a Windows Combo Box CueBanner
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderCueBanner(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(ComboBoxRenderer.CreateCueBannerElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Combo Box CueBanner Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateCueBannerElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Combobox", 8, (int)state);
        }
    }
}