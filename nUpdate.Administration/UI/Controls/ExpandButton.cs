// Copyright © Dominic Beger 2018

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;

namespace nUpdate.Administration.UI.Controls
{
    /// <summary>
    ///     A simple Expander Button drawn by Windows via Visual Styles.
    /// </summary>
    [DesignerCategory("Code")]
    [Description("A simple Expander Button drawn by Windows via Visual Styles")]
    [Designer(typeof(ExpandButtonDesigner))]
    [DefaultProperty("ExpandedChanged")]
    [ToolboxBitmap(typeof(Button))]
    public class ExpandButton
        : Control
    {
        private bool _expanded;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpandButton" /> class.
        /// </summary>
        public ExpandButton()
        {
            if (Environment.OSVersion.Version.Major < 6)
                throw new NotSupportedException();
            ButtonRenderer = new VisualStyleRenderer("TaskDialog", 13, (int) ButtonState.Normal);
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        private VisualStyleRenderer ButtonRenderer { get; set; }

        /// <summary>
        ///     Gets the default size.
        /// </summary>
        /// <value>
        ///     The default size.
        /// </value>
        protected override Size DefaultSize => new Size(19, 21);

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="ExpandButton" /> is expanded.
        /// </summary>
        /// <value>
        ///     <c>true</c> if expanded; otherwise, <c>false</c>.
        /// </value>
        public bool Expanded
        {
            get => _expanded;
            set
            {
                if (value == _expanded)
                    return;
                _expanded = value;
                ButtonRenderer = new VisualStyleRenderer("TaskDialog", 13, (ButtonRenderer.State + 3) % 6);
                Invalidate();
            }
        }

        /// <summary>
        ///     Gets raised when the Expanded State changed.
        /// </summary>
        [Description("Gets raised when the Expanded State changed.")]
        public event EventHandler ExpandedChanged;

        /// <summary>
        ///     Raises the <see cref="E:Click" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            Expanded = !Expanded;
            Focus();
            Invalidate();
            base.OnClick(e);
        }

        /// <summary>
        ///     Raises the <see cref="E:EnabledChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            ButtonRenderer = new VisualStyleRenderer("TaskDialog", 13,
                (int) (Enabled ? PushButtonState.Normal : PushButtonState.Disabled) + (Expanded ? 3 : 0));
            Invalidate();
            base.OnEnabledChanged(e);
        }

        /// <summary>
        ///     Raises the <see cref="ExpandedChanged" /> event.
        /// </summary>
        protected virtual void OnExpandedChanged()
        {
            if (ExpandedChanged != null) ExpandedChanged(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Raises the <see cref="E:GotFocus" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            Invalidate();
            base.OnGotFocus(e);
        }

        /// <summary>
        ///     Raises the <see cref="E:LostFocus" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            Invalidate();
            base.OnLostFocus(e);
        }

        /// <summary>
        ///     Raises the <see cref="E:MouseDown" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            ButtonRenderer = new VisualStyleRenderer("TaskDialog", 13,
                (int) PushButtonState.Pressed + (Expanded ? 3 : 0));
            Invalidate();
            base.OnMouseDown(e);
        }

        /// <summary>
        ///     Raises the <see cref="E:MouseEnter" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            ButtonRenderer = new VisualStyleRenderer("TaskDialog", 13, (int) PushButtonState.Hot + (Expanded ? 3 : 0));
            Invalidate();
            base.OnMouseEnter(e);
        }

        /// <summary>
        ///     Raises the <see cref="E:MouseLeave" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            ButtonRenderer =
                new VisualStyleRenderer("TaskDialog", 13, (int) PushButtonState.Normal + (Expanded ? 3 : 0));
            Invalidate();
            base.OnMouseLeave(e);
        }

        /// <summary>
        ///     Raises the <see cref="E:MouseUp" /> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            ButtonRenderer = new VisualStyleRenderer("TaskDialog", 13,
                (int)
                (e.X >= 0 && e.X < Width && e.Y >= 0 && e.Y < Height
                    ? PushButtonState.Hot
                    : PushButtonState.Normal) +
                (Expanded ? 3 : 0));
            Invalidate();
            base.OnMouseUp(e);
        }

        /// <summary>
        ///     Raises the <see cref="E:Paint" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PaintEventArgs" /> instance containing the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            ButtonRenderer.DrawBackground(e.Graphics, DisplayRectangle);
            if (Focused && ShowFocusCues) ControlPaint.DrawFocusRectangle(e.Graphics, DisplayRectangle);

            base.OnPaint(e);
        }

        /// <summary>
        ///     Provides a ControlDesigner for the <see cref="ExpandButton" />.
        /// </summary>
        public class ExpandButtonDesigner
            : ControlDesigner
        {
            /// <summary>
            ///     Returns the selection rules.
            /// </summary>
            /// <value>
            ///     The selection rules.
            /// </value>
            public override SelectionRules SelectionRules => SelectionRules.Moveable;
        }
    }
}