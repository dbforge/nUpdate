using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisualStyleControls.Rendering;

namespace VisualStyleControls.Controls
{
    /// <summary>
    /// A simple Back/Forward Button drawn by Windows via Visual Styles.
    /// </summary>
    [ToolboxBitmap(typeof(Button))]
    [Designer(typeof(NavigationButtonDesigner))]
    [DefaultEvent("Click")]
    [Description("A simple Back/Forward Button drawn by Windows via Visual Styles")]
    public class NavigationButton
        : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:VisualStyleControls.Controls.NavigationButton"/> class.
        /// </summary>
        public NavigationButton()
            : base()
        {
            this.SuspendLayout();

            this.Size = new Size(30, 30);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Gets raised if the arrow direction has changed.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets raised when the Button Type got changed.")]
        public event EventHandler ButtonTypeChanged;

        /// <summary>
        /// Raises the <see cref="E:VisualStyleControls.Controls.NavigationButton.ButtonTypeChanged"/> event.
        /// </summary>
        protected virtual void OnButtonTypeChanged()
        {
            if (this.ButtonTypeChanged != null)
            {
                this.ButtonTypeChanged(this, EventArgs.Empty);
            }
        }

        private NavigationButtonType type = NavigationButtonType.Back;
        private VisualStyleControls.Rendering.ButtonState state = VisualStyleControls.Rendering.ButtonState.Normal;

        /// <summary>
        /// Indicates the Type of this Button.
        /// </summary>
        /// <value>
        /// The current Type.
        /// </value>
        [Category("Appearance")]
        [Description("Indicates the Type of this Button.")]
        public NavigationButtonType ButtonType
        {
            get
            {
                return this.type;
            }
            set
            {
                if (value != this.type)
                {
                    this.type = value;
                    this.Invalidate();
                    this.OnButtonTypeChanged();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            switch (this.ButtonType)
            {
                case NavigationButtonType.Back:
                    NavigationButtonRenderer.RenderBackButton(e.Graphics, new Rectangle(0, 0, this.Width, this.Height), this.Enabled ? this.state : VisualStyleControls.Rendering.ButtonState.Disabled);
                    break;
                case NavigationButtonType.Forward:
                    NavigationButtonRenderer.RenderForwardButton(e.Graphics, new Rectangle(0, 0, this.Width, this.Height), this.Enabled ? this.state : VisualStyleControls.Rendering.ButtonState.Disabled);
                    break;
                default:
                    Debug.WriteLine("Really don't know what happened here but the ButtonType property has a value nonexistent in NavigationButtonType");
                    break;
            }

            base.OnPaint(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.state = VisualStyleControls.Rendering.ButtonState.Hot;
            this.Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.state = VisualStyleControls.Rendering.ButtonState.Normal;
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.state = VisualStyleControls.Rendering.ButtonState.Pressed;
            this.Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.state = (e.X >= 0 && e.X < this.Width && e.Y >= 0 && e.Y < this.Height) ? VisualStyleControls.Rendering.ButtonState.Hot : VisualStyleControls.Rendering.ButtonState.Normal;
            this.Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnEnabledChanged(e);
        }
    }
}