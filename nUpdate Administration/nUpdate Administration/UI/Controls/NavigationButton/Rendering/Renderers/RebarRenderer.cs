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
    /// Renders the Windows Rebar
    /// </summary>
    public static class RebarRenderer
    {
        /// <summary>
        /// Renders a Windows Rebar Gripper
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderGripper(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(RebarRenderer.CreateGripperElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Rebar Gripper Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateGripperElement()
        {
            return VisualStyleElement.CreateElement("Rebar", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Rebar GripperVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderGripperVertical(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(RebarRenderer.CreateGripperVerticalElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Rebar GripperVertical Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateGripperVerticalElement()
        {
            return VisualStyleElement.CreateElement("Rebar", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Rebar Band
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBand(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(RebarRenderer.CreateBandElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Rebar Band Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBandElement()
        {
            return VisualStyleElement.CreateElement("Rebar", 3, 0);
        }

        /// <summary>
        /// Renders a Windows Rebar Chevron
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderChevron(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(RebarRenderer.CreateChevronElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Rebar Chevron Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateChevronElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("Rebar", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Rebar ChevronVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderChevronVertical(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(RebarRenderer.CreateChevronVerticalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Rebar ChevronVertical Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateChevronVerticalElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("Rebar", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Rebar Background
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBackground(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(RebarRenderer.CreateBackgroundElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Rebar Background Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBackgroundElement()
        {
            return VisualStyleElement.CreateElement("Rebar", 6, 0);
        }

        /// <summary>
        /// Renders a Windows Rebar Splitter
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSplitter(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(RebarRenderer.CreateSplitterElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Rebar Splitter Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSplitterElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("Rebar", 7, (int)state);
        }

        /// <summary>
        /// Renders a Windows Rebar SplitterVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSplitterVertical(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(RebarRenderer.CreateSplitterVerticalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Rebar SplitterVertical Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSplitterVerticalElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("Rebar", 8, (int)state);
        }
    }
}