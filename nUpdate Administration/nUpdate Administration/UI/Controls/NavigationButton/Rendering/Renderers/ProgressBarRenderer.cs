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
    /// Renders the Windows Progress Bar
    /// </summary>
    public static class ProgressBarRenderer
    {
        /// <summary>
        /// Renders a Windows Progress Bar Bar
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBar(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreateBarElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar Bar Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBarElement()
        {
            return VisualStyleElement.CreateElement("Progress", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Progress Bar BarVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBarVertical(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreateBarVerticalElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar BarVertical Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBarVerticalElement()
        {
            return VisualStyleElement.CreateElement("Progress", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Progress Bar Chunk
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderChunk(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreateChunkElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar Chunk Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateChunkElement()
        {
            return VisualStyleElement.CreateElement("Progress", 3, 0);
        }

        /// <summary>
        /// Renders a Windows Progress Bar ChunkVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderChunkVertical(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreateChunkVerticalElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar ChunkVertical Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateChunkVerticalElement()
        {
            return VisualStyleElement.CreateElement("Progress", 4, 0);
        }

        /// <summary>
        /// Renders a Windows Progress Bar Fill
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderFill(IDeviceContext dc, Rectangle bounds, ProgressBarState state)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreateFillElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar Fill Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateFillElement(ProgressBarState state)
        {
            return VisualStyleElement.CreateElement("Progress", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Progress Bar FillVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderFillVertical(IDeviceContext dc, Rectangle bounds, ProgressBarState state)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreateFillVerticalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar FillVertical Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateFillVerticalElement(ProgressBarState state)
        {
            return VisualStyleElement.CreateElement("Progress", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Progress Bar PulseOverlay
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPulseOverlay(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreatePulseOverlayElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar PulseOverlay Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePulseOverlayElement()
        {
            return VisualStyleElement.CreateElement("Progress", 7, 0);
        }

        /// <summary>
        /// Renders a Windows Progress Bar MoveOverlay
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderMoveOverlay(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreateMoveOverlayElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar MoveOverlay Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateMoveOverlayElement()
        {
            return VisualStyleElement.CreateElement("Progress", 8, 0);
        }

        /// <summary>
        /// Renders a Windows Progress Bar PulseOverlayVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPulseOverlayVertical(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreatePulseOverlayVerticalElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar PulseOverlayVertical Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePulseOverlayVerticalElement()
        {
            return VisualStyleElement.CreateElement("Progress", 9, 0);
        }

        /// <summary>
        /// Renders a Windows Progress Bar MoveOverlayVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderMoveOverlayVertical(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreateMoveOverlayVerticalElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar MoveOverlayVertical Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateMoveOverlayVerticalElement()
        {
            return VisualStyleElement.CreateElement("Progress", 10, 0);
        }

        /// <summary>
        /// Renders a Windows Progress Bar TransparentBar
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTransparentBar(IDeviceContext dc, Rectangle bounds, NormalPartialState state)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreateTransparentBarElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar TransparentBar Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTransparentBarElement(NormalPartialState state)
        {
            return VisualStyleElement.CreateElement("Progress", 11, (int)state);
        }

        /// <summary>
        /// Renders a Windows Progress Bar TransparentBarVertical
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTransparentBarVertical(IDeviceContext dc, Rectangle bounds, NormalPartialState state)
        {
            new VisualStyleRenderer(ProgressBarRenderer.CreateTransparentBarVerticalElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Progress Bar TransparentBarVertical Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTransparentBarVerticalElement(NormalPartialState state)
        {
            return VisualStyleElement.CreateElement("Progress", 12, (int)state);
        }
    }
}