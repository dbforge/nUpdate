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
    /// Renders the Windows Aero Wizard
    /// </summary>
    public static class AeroWizardRenderer
    {
        /// <summary>
        /// Renders a Windows Aero Wizard TitleBar
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTitleBar(IDeviceContext dc, Rectangle bounds, ActiveInactiveState state)
        {
            new VisualStyleRenderer(AeroWizardRenderer.CreateTitleBarElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Aero Wizard TitleBar Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTitleBarElement(ActiveInactiveState state)
        {
            return VisualStyleElement.CreateElement("AeroWizard", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Aero Wizard HeaderArea
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderHeaderArea(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(AeroWizardRenderer.CreateHeaderAreaElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Aero Wizard HeaderArea Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateHeaderAreaElement()
        {
            return VisualStyleElement.CreateElement("AeroWizard", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Aero Wizard ContentArea
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderContentArea(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(AeroWizardRenderer.CreateContentAreaElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Aero Wizard ContentArea Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateContentAreaElement()
        {
            return VisualStyleElement.CreateElement("AeroWizard", 3, 0);
        }

        /// <summary>
        /// Renders a Windows Aero Wizard CommandArea
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderCommandArea(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(AeroWizardRenderer.CreateCommandAreaElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Aero Wizard CommandArea Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateCommandAreaElement()
        {
            return VisualStyleElement.CreateElement("AeroWizard", 4, 0);
        }

        /// <summary>
        /// Renders a Windows Aero Wizard Button
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderButton(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(AeroWizardRenderer.CreateButtonElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Aero Wizard Button Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateButtonElement()
        {
            return VisualStyleElement.CreateElement("AeroWizard", 5, 0);
        }
    }
}