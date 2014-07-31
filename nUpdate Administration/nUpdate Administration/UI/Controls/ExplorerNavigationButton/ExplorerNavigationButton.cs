using nUpdate.Administration.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ExplorerNavigationButton
{
    /// <summary>
    /// A button with the appearance of the explorer's navigation buttons.
    /// </summary>
    /// <remarks>The control switches its appearance based on the current operating system to fit the explorer's appearance.</remarks>
    [Description("A button with the appearance of the explorer navigation buttons.")]
    public partial class ExplorerNavigationButton : Control
    {
        private Template template;
        private readonly Bitmap[] arrows;
        private ArrowDirection arrowDirection;
        private ButtonTheme theme;
        private ButtonState state;

        /// <summary>
        /// Is risen if the arrow direction has changed.
        /// </summary>
        [Category("Appearance")]
        [Description("Is risen if the arrow direction has changed.")]
        public event EventHandler ArrowDirectionChanged;

        /// <summary>
        /// Is risen if the theme has changed.
        /// </summary>
        [Category("Appearance")]
        [Description("Is risen if the theme has changed.")]
        public event EventHandler ThemeChanged;

        /// <summary>
        /// Indicates the direction of the arrow.
        /// </summary>
        [Category("Appearance")]
        [Description("Indicates the direction of the arrow.")]
        [DefaultValue(ArrowDirection.Left)]
        public ArrowDirection ArrowDirection
        {
            get { return this.arrowDirection; }
            set
            {
                if (value != this.arrowDirection)
                {
                    this.arrowDirection = value;
                    this.Invalidate();

                    this.OnArrowDirectionChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Indicates the theme of this button.
        /// </summary>
        [Category("Appearance")]
        [Description("Indicates the theme of this button.")]
        [DefaultValue(ButtonTheme.Auto)]
        public ButtonTheme Theme
        {
            get { return this.theme; }
            set
            {
                if (value != this.theme)
                {
                    this.theme = value;

                    if (this.template != null)
                    {
                        this.template.Dispose();
                    }

                    this.template = this.SelectTemplate(this.theme);
                    this.Invalidate();
                    this.OnTemplateChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnArrowDirectionChanged(EventArgs e)
        {
            if (this.ArrowDirectionChanged != null)
            {
                this.ArrowDirectionChanged(this, e);
            }
        }

        protected virtual void OnTemplateChanged(EventArgs e)
        {
            if (this.ThemeChanged != null)
            {
                this.ThemeChanged(this, e);
            }
        }

        private Template SelectTemplate(ButtonTheme theme)
        {
            switch (theme)
            {
                case ButtonTheme.Default:
                    return null;
                case ButtonTheme.Aero:
                    return new AeroTemplate();
                case ButtonTheme.Metro:
                    return new MetroTemplate();
                default:
                    return this.AutoSelectTemplate();
            }
        }

        private Template AutoSelectTemplate()
        {
            if (Application.RenderWithVisualStyles)
            {
                Version metroVersion = new Version(6, 2);
                Version aeroVersion = new Version(6, 0);

                Version version = Environment.OSVersion.Version;
                if (version >= metroVersion)
                {
                    return new MetroTemplate();
                }
                else if (version >= aeroVersion)
                {
                    return new AeroTemplate();
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a new ExplorerNavigationButton.
        /// </summary>
        public ExplorerNavigationButton()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            this.UpdateStyles();

            this.Size = new Size(24, 24);

            this.arrows = new Bitmap[4];
            this.arrows[0] = Resources.Left_Normal;
            this.arrows[1] = Resources.Right_Normal;
            this.arrows[2] = Resources.Left_Disabled;
            this.arrows[3] = Resources.Right_Disabled;

            this.template = this.AutoSelectTemplate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.state = ButtonState.Hover;
            this.Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.state = ButtonState.Normal;
            this.Invalidate();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.state = ButtonState.Pressed;
            this.Invalidate();

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.state = ButtonState.Hover;
            this.Invalidate();

            base.OnMouseUp(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            this.Invalidate();

            base.OnEnabledChanged(e);
        }

        private void DrawDefault(Graphics g, ArrowDirection direction, ButtonState state)
        {
            int arrowIndex = (int)direction;
            if (state == ButtonState.Disabled)
            {
                arrowIndex += 2;
            }

            int arrowSize = Math.Min(16, Math.Min(this.Width, this.Height));
            var arrowRect = new Rectangle((this.Width - arrowSize) / 2, (this.Height - arrowSize) / 2, arrowSize, arrowSize);

            ButtonRenderer.DrawButton(g, this.ClientRectangle, this.arrows[arrowIndex], arrowRect, false, (PushButtonState)(state + 1));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.template != null)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                int buttonSize = Math.Min(this.Width, this.Height);
                float sizeFactor = buttonSize / 24.0f;
                g.Transform = new Matrix(sizeFactor, 0, 0, sizeFactor, (this.Width - buttonSize) / 2.0f, (this.Height - buttonSize) / 2.0f);

                template.Draw(g, this.arrowDirection, this.Enabled ? this.state : ButtonState.Disabled);
            }
            else
            {
                this.DrawDefault(e.Graphics, this.arrowDirection, this.Enabled ? this.state : ButtonState.Disabled);
            }

            base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            foreach (Bitmap bmp in this.arrows)
            {
                bmp.Dispose();
            }

            if (template != null)
            {
                this.template.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
