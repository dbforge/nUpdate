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
    /// Renders the Windows Calendar
    /// </summary>
    public static class CalendarRenderer
    {
        /// <summary>
        /// Renders a Windows Calendar Background
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBackground(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(CalendarRenderer.CreateBackgroundElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Calendar Background Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBackgroundElement()
        {
            return VisualStyleElement.CreateElement("MonthCal", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Calendar Borders
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBorders(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(CalendarRenderer.CreateBordersElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Calendar Borders Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBordersElement()
        {
            return VisualStyleElement.CreateElement("MonthCal", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Calendar GridBackground
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderGridBackground(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(CalendarRenderer.CreateGridBackgroundElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Calendar GridBackground Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateGridBackgroundElement()
        {
            return VisualStyleElement.CreateElement("MonthCal", 3, 0);
        }

        /// <summary>
        /// Renders a Windows Calendar ColheaderSplitter
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderColheaderSplitter(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(CalendarRenderer.CreateColheaderSplitterElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Calendar ColheaderSplitter Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateColheaderSplitterElement()
        {
            return VisualStyleElement.CreateElement("MonthCal", 4, 0);
        }

        /// <summary>
        /// Renders a Windows Calendar GridCellBackgound
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderGridCellBackgound(IDeviceContext dc, Rectangle bounds, GridCellBackgroundState state)
        {
            new VisualStyleRenderer(CalendarRenderer.CreateGridCellBackgoundElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Calendar GridCellBackgound Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateGridCellBackgoundElement(GridCellBackgroundState state)
        {
            return VisualStyleElement.CreateElement("MonthCal", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Calendar GridCell
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderGridCell(IDeviceContext dc, Rectangle bounds, GridCellState state)
        {
            new VisualStyleRenderer(CalendarRenderer.CreateGridCellElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Calendar GridCell Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateGridCellElement(GridCellState state)
        {
            return VisualStyleElement.CreateElement("MonthCal", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Calendar GridCellUpper
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderGridCellUpper(IDeviceContext dc, Rectangle bounds, GridCellUpperState state)
        {
            new VisualStyleRenderer(CalendarRenderer.CreateGridCellUpperElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Calendar GridCellUpper Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateGridCellUpperElement(GridCellUpperState state)
        {
            return VisualStyleElement.CreateElement("MonthCal", 7, (int)state);
        }

        /// <summary>
        /// Renders a Windows Calendar TrailingGridCell
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTrailingGridCell(IDeviceContext dc, Rectangle bounds, GridCellState state)
        {
            new VisualStyleRenderer(CalendarRenderer.CreateTrailingGridCellElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Calendar TrailingGridCell Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTrailingGridCellElement(GridCellState state)
        {
            return VisualStyleElement.CreateElement("MonthCal", 8, (int)state);
        }

        /// <summary>
        /// Renders a Windows Calendar TrailingGridCellUpper
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTrailingGridCellUpper(IDeviceContext dc, Rectangle bounds, GridCellUpperState state)
        {
            new VisualStyleRenderer(CalendarRenderer.CreateTrailingGridCellUpperElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Calendar TrailingGridCellUpper Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTrailingGridCellUpperElement(GridCellUpperState state)
        {
            return VisualStyleElement.CreateElement("MonthCal", 9, (int)state);
        }

        /// <summary>
        /// Renders a Windows Calendar NavNext
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderNavNext(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(CalendarRenderer.CreateNavNextElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Calendar NavNext Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateNavNextElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("MonthCal", 10, (int)state);
        }

        /// <summary>
        /// Renders a Windows Calendar NavPrevious
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderNavPrevious(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(CalendarRenderer.CreateNavPreviousElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Calendar NavPrevious Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateNavPreviousElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("MonthCal", 11, (int)state);
        }
    }
}