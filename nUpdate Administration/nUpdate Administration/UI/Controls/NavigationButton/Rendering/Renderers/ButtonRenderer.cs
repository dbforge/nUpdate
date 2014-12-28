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
    /// Renders the Windows Button
    /// </summary>
    public static class ButtonRenderer
    {
        /// <summary>
        /// Renders a Windows Button PushButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderPushButton(IDeviceContext dc, Rectangle bounds, PushButtonState state)
        {
            new VisualStyleRenderer(ButtonRenderer.CreatePushButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Button PushButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreatePushButtonElement(PushButtonState state)
        {
            return VisualStyleElement.CreateElement("Button", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Button RadioButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderRadioButton(IDeviceContext dc, Rectangle bounds, RadioButtonState state)
        {
            new VisualStyleRenderer(ButtonRenderer.CreateRadioButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Button RadioButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateRadioButtonElement(RadioButtonState state)
        {
            return VisualStyleElement.CreateElement("Button", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Button CheckBox
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderCheckBox(IDeviceContext dc, Rectangle bounds, CheckBoxState state)
        {
            new VisualStyleRenderer(ButtonRenderer.CreateCheckBoxElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Button CheckBox Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateCheckBoxElement(CheckBoxState state)
        {
            return VisualStyleElement.CreateElement("Button", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Button GroupBox
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderGroupBox(IDeviceContext dc, Rectangle bounds, NormalDisabledState state)
        {
            new VisualStyleRenderer(ButtonRenderer.CreateGroupBoxElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Button GroupBox Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateGroupBoxElement(NormalDisabledState state)
        {
            return VisualStyleElement.CreateElement("Button", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Button UserButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderUserButton(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ButtonRenderer.CreateUserButtonElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Button UserButton Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateUserButtonElement()
        {
            return VisualStyleElement.CreateElement("Button", 5, 0);
        }

        /// <summary>
        /// Renders a Windows Button CommandLink
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderCommandLink(IDeviceContext dc, Rectangle bounds, CommandLinkState state)
        {
            new VisualStyleRenderer(ButtonRenderer.CreateCommandLinkElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Button CommandLink Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateCommandLinkElement(CommandLinkState state)
        {
            return VisualStyleElement.CreateElement("Button", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Button CommandLinkGlyph
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderCommandLinkGlyph(IDeviceContext dc, Rectangle bounds, CommandLinkState state)
        {
            new VisualStyleRenderer(ButtonRenderer.CreateCommandLinkGlyphElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Button CommandLinkGlyph Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateCommandLinkGlyphElement(CommandLinkState state)
        {
            return VisualStyleElement.CreateElement("Button", 7, (int)state);
        }
    }
}