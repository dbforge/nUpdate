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
    /// Renders the Windows Scroll Bar
    /// </summary>
    public static class ScrollBarRenderer
    {
        /// <summary>
        /// Renders a Windows Scroll Bar ArrowButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderArrowButton(IDeviceContext dc, Rectangle bounds, ArrowButtonState state)
        {
            new VisualStyleRenderer(ScrollBarRenderer.CreateArrowButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Scroll Bar ArrowButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateArrowButtonElement(ArrowButtonState state)
        {
            return VisualStyleElement.CreateElement("Scrollbar", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Scroll Bar ThumbButtonHorizontal
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderThumbButtonHorizontal(IDeviceContext dc, Rectangle bounds, NormalHotPressedDisabledHoverState state)
        {
            new VisualStyleRenderer(ScrollBarRenderer.CreateThumbButtonHorizontalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Scroll Bar ThumbButtonHorizontal Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateThumbButtonHorizontalElement(NormalHotPressedDisabledHoverState state)
        {
            return VisualStyleElement.CreateElement("Scrollbar", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Scroll Bar ThumbButtonVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderThumbButtonVertical(IDeviceContext dc, Rectangle bounds, NormalHotPressedDisabledHoverState state)
        {
            new VisualStyleRenderer(ScrollBarRenderer.CreateThumbButtonVerticalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Scroll Bar ThumbButtonVertical Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateThumbButtonVerticalElement(NormalHotPressedDisabledHoverState state)
        {
            return VisualStyleElement.CreateElement("Scrollbar", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Scroll Bar LowerTrackHorizontal
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderLowerTrackHorizontal(IDeviceContext dc, Rectangle bounds, NormalHotPressedDisabledHoverState state)
        {
            new VisualStyleRenderer(ScrollBarRenderer.CreateLowerTrackHorizontalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Scroll Bar LowerTrackHorizontal Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateLowerTrackHorizontalElement(NormalHotPressedDisabledHoverState state)
        {
            return VisualStyleElement.CreateElement("Scrollbar", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Scroll Bar UpperTrackHorizontal
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderUpperTrackHorizontal(IDeviceContext dc, Rectangle bounds, NormalHotPressedDisabledHoverState state)
        {
            new VisualStyleRenderer(ScrollBarRenderer.CreateUpperTrackHorizontalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Scroll Bar UpperTrackHorizontal Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateUpperTrackHorizontalElement(NormalHotPressedDisabledHoverState state)
        {
            return VisualStyleElement.CreateElement("Scrollbar", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Scroll Bar LowerTrackVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderLowerTrackVertical(IDeviceContext dc, Rectangle bounds, NormalHotPressedDisabledHoverState state)
        {
            new VisualStyleRenderer(ScrollBarRenderer.CreateLowerTrackVerticalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Scroll Bar LowerTrackVertical Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateLowerTrackVerticalElement(NormalHotPressedDisabledHoverState state)
        {
            return VisualStyleElement.CreateElement("Scrollbar", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Scroll Bar UpperTrackVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderUpperTrackVertical(IDeviceContext dc, Rectangle bounds, NormalHotPressedDisabledHoverState state)
        {
            new VisualStyleRenderer(ScrollBarRenderer.CreateUpperTrackVerticalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Scroll Bar UpperTrackVertical Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateUpperTrackVerticalElement(NormalHotPressedDisabledHoverState state)
        {
            return VisualStyleElement.CreateElement("Scrollbar", 7, (int)state);
        }

        /// <summary>
        /// Renders a Windows Scroll Bar GripperHorizontal
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderGripperHorizontal(IDeviceContext dc, Rectangle bounds, NormalHotPressedDisabledHoverState state)
        {
            new VisualStyleRenderer(ScrollBarRenderer.CreateGripperHorizontalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Scroll Bar GripperHorizontal Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateGripperHorizontalElement(NormalHotPressedDisabledHoverState state)
        {
            return VisualStyleElement.CreateElement("Scrollbar", 8, (int)state);
        }

        /// <summary>
        /// Renders a Windows Scroll Bar GripperVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderGripperVertical(IDeviceContext dc, Rectangle bounds, NormalHotPressedDisabledHoverState state)
        {
            new VisualStyleRenderer(ScrollBarRenderer.CreateGripperVerticalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Scroll Bar GripperVertical Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateGripperVerticalElement(NormalHotPressedDisabledHoverState state)
        {
            return VisualStyleElement.CreateElement("Scrollbar", 9, (int)state);
        }

        /// <summary>
        /// Renders a Windows Scroll Bar SizeBox
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSizeBox(IDeviceContext dc, Rectangle bounds, AlignState state)
        {
            new VisualStyleRenderer(ScrollBarRenderer.CreateSizeBoxElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Scroll Bar SizeBox Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSizeBoxElement(AlignState state)
        {
            return VisualStyleElement.CreateElement("Scrollbar", 10, (int)state);
        }
    }
}