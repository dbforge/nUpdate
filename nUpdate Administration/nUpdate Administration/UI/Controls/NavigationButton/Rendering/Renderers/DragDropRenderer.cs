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
    /// Renders the Windows Drag & Drop
    /// </summary>
    public static class DragDropRenderer
    {
        /// <summary>
        /// Renders a Windows Drag & Drop Copy
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderCopy(IDeviceContext dc, Rectangle bounds, DragDropState state)
        {
            new VisualStyleRenderer(DragDropRenderer.CreateCopyElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Drag & Drop Copy Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateCopyElement(DragDropState state)
        {
            return VisualStyleElement.CreateElement("DragDrop", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Drag & Drop Move
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMove(IDeviceContext dc, Rectangle bounds, DragDropState state)
        {
            new VisualStyleRenderer(DragDropRenderer.CreateMoveElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Drag & Drop Move Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMoveElement(DragDropState state)
        {
            return VisualStyleElement.CreateElement("DragDrop", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Drag & Drop UpdateMetaData
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderUpdateMetaData(IDeviceContext dc, Rectangle bounds, DragDropState state)
        {
            new VisualStyleRenderer(DragDropRenderer.CreateUpdateMetaDataElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Drag & Drop UpdateMetaData Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateUpdateMetaDataElement(DragDropState state)
        {
            return VisualStyleElement.CreateElement("DragDrop", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Drag & Drop CreateLink
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderCreateLink(IDeviceContext dc, Rectangle bounds, DragDropState state)
        {
            new VisualStyleRenderer(DragDropRenderer.CreateCreateLinkElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Drag & Drop CreateLink Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateCreateLinkElement(DragDropState state)
        {
            return VisualStyleElement.CreateElement("DragDrop", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Drag & Drop Warning
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderWarning(IDeviceContext dc, Rectangle bounds, DragDropState state)
        {
            new VisualStyleRenderer(DragDropRenderer.CreateWarningElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Drag & Drop Warning Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateWarningElement(DragDropState state)
        {
            return VisualStyleElement.CreateElement("DragDrop", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Drag & Drop None
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderNone(IDeviceContext dc, Rectangle bounds, DragDropState state)
        {
            new VisualStyleRenderer(DragDropRenderer.CreateNoneElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Drag & Drop None Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateNoneElement(DragDropState state)
        {
            return VisualStyleElement.CreateElement("DragDrop", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Drag & Drop ImageBg
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderImageBg(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(DragDropRenderer.CreateImageBgElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Drag & Drop ImageBg Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateImageBgElement()
        {
            return VisualStyleElement.CreateElement("DragDrop", 7, 0);
        }

        /// <summary>
        /// Renders a Windows Drag & Drop TextBg
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderTextBg(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(DragDropRenderer.CreateTextBgElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Drag & Drop TextBg Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateTextBgElement()
        {
            return VisualStyleElement.CreateElement("DragDrop", 8, 0);
        }
    }
}