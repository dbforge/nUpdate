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
    /// Renders the Windows Text Style
    /// </summary>
    public static class TextStyleRenderer
    {
        /// <summary>
        /// Renders a Windows Text Style MainInstruction
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderMainInstruction(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TextStyleRenderer.CreateMainInstructionElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Style MainInstruction Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateMainInstructionElement()
        {
            return VisualStyleElement.CreateElement("TextStyle", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Text Style Instruction
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderInstruction(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TextStyleRenderer.CreateInstructionElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Style Instruction Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateInstructionElement()
        {
            return VisualStyleElement.CreateElement("TextStyle", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Text Style BodyTitle
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBodyTitle(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TextStyleRenderer.CreateBodyTitleElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Style BodyTitle Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBodyTitleElement()
        {
            return VisualStyleElement.CreateElement("TextStyle", 3, 0);
        }

        /// <summary>
        /// Renders a Windows Text Style BodyText
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBodyText(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TextStyleRenderer.CreateBodyTextElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Style BodyText Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBodyTextElement()
        {
            return VisualStyleElement.CreateElement("TextStyle", 4, 0);
        }

        /// <summary>
        /// Renders a Windows Text Style SecondaryText
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSecondaryText(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TextStyleRenderer.CreateSecondaryTextElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Style SecondaryText Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSecondaryTextElement()
        {
            return VisualStyleElement.CreateElement("TextStyle", 5, 0);
        }

        /// <summary>
        /// Renders a Windows Text Style HyperLinkText
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHyperLinkText(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(TextStyleRenderer.CreateHyperLinkTextElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Style HyperLinkText Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHyperLinkTextElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("TextStyle", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Text Style Expanded
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderExpanded(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TextStyleRenderer.CreateExpandedElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Style Expanded Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateExpandedElement()
        {
            return VisualStyleElement.CreateElement("TextStyle", 7, 0);
        }

        /// <summary>
        /// Renders a Windows Text Style Label
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderLabel(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TextStyleRenderer.CreateLabelElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Style Label Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateLabelElement()
        {
            return VisualStyleElement.CreateElement("TextStyle", 8, 0);
        }

        /// <summary>
        /// Renders a Windows Text Style ControlLabel
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderControlLabel(IDeviceContext dc, Rectangle bounds, NormalDisabledState state)
        {
            new VisualStyleRenderer(TextStyleRenderer.CreateControlLabelElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Text Style ControlLabel Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateControlLabelElement(NormalDisabledState state)
        {
            return VisualStyleElement.CreateElement("TextStyle", 9, (int)state);
        }
    }
}