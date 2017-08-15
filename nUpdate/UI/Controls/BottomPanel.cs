// Copyright © Dominic Beger 2017

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace nUpdate.UI.Controls
{
    /// <summary>
    ///     A "Bottom Panel" drawn by Windows via Visual Styles if available.
    ///     This Panel can be used for providing additional information or Buttons on the bottom of a Form/Dialog.
    /// </summary>
    /// <remarks>
    ///     The panel is drawn with Visual Styles (TaskDialog > SecondaryPanel). If running on XP or another OS, the panel is
    ///     drawn manually
    /// </remarks>
    [DesignerCategory("Code")]
    [DisplayName("Bottom Panel")]
    [Description(
        "A \"Bottom Panel\" that can be used for providing additional information or Buttons on the bottom of a Form/Dialog."
    )]
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Panel))]
    public class BottomPanel
        : Panel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BottomPanel" /> class.
        /// </summary>
        public BottomPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        /// <summary>
        ///     Raises the <see cref="E:Paint" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PaintEventArgs" /> instance containing the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Application.RenderWithVisualStyles &&
                VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement("TaskDialog", 8, 0)))
                PaintWithVisualStyles(e.Graphics);
            else
                PaintManually(e.Graphics);

            base.OnPaint(e);
        }

        /// <summary>
        ///     Paints the button manually.
        /// </summary>
        /// <param name="g">The targeted graphics.</param>
        protected virtual void PaintManually(Graphics g)
        {
            g.Clear(SystemColors.Control);
            g.DrawLine(SystemPens.ControlDark, new Point(0, 0), new Point(Width, 0));
        }

        /// <summary>
        ///     Paints the panel with visual styles.
        /// </summary>
        /// <param name="g">The targeted graphics.</param>
        protected virtual void PaintWithVisualStyles(Graphics g)
        {
            //Draw panel
            new VisualStyleRenderer("TaskDialog", 8, 0).DrawBackground(g, DisplayRectangle);
        }
    }
}