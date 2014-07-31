using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using nUpdate.Administration.Properties;

namespace ExplorerNavigationButton
{
    /// <summary>
    ///     A button with the appearance of the explorer's navigation buttons.
    /// </summary>
    /// <remarks>The control switches its appearance based on the current operating system to fit the explorer's appearance.</remarks>
    [Description("A button with the appearance of the explorer navigation buttons.")]
    public partial class ExplorerNavigationButton : Control
    {
        private readonly Bitmap[] arrows;
        private ArrowDirection arrowDirection;
        private ButtonState state;
        private Template template;
        private ButtonTheme theme;

        /// <summary>
        ///     Creates a new ExplorerNavigationButton.
        /// </summary>
        public ExplorerNavigationButton()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();

            Size = new Size(24, 24);

            arrows = new Bitmap[4];
            arrows[0] = Resources.Left_Normal;
            arrows[1] = Resources.Right_Normal;
            arrows[2] = Resources.Left_Disabled;
            arrows[3] = Resources.Right_Disabled;

            template = AutoSelectTemplate();
        }

        /// <summary>
        ///     Indicates the direction of the arrow.
        /// </summary>
        [Category("Appearance")]
        [Description("Indicates the direction of the arrow.")]
        [DefaultValue(ArrowDirection.Left)]
        public ArrowDirection ArrowDirection
        {
            get { return arrowDirection; }
            set
            {
                if (value != arrowDirection)
                {
                    arrowDirection = value;
                    Invalidate();

                    OnArrowDirectionChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Indicates the theme of this button.
        /// </summary>
        [Category("Appearance")]
        [Description("Indicates the theme of this button.")]
        [DefaultValue(ButtonTheme.Auto)]
        public ButtonTheme Theme
        {
            get { return theme; }
            set
            {
                if (value != theme)
                {
                    theme = value;

                    if (template != null)
                        template.Dispose();

                    template = SelectTemplate(theme);
                    Invalidate();
                    OnTemplateChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Is risen if the arrow direction has changed.
        /// </summary>
        [Category("Appearance")]
        [Description("Is risen if the arrow direction has changed.")]
        public event EventHandler ArrowDirectionChanged;

        /// <summary>
        ///     Is risen if the theme has changed.
        /// </summary>
        [Category("Appearance")]
        [Description("Is risen if the theme has changed.")]
        public event EventHandler ThemeChanged;

        protected virtual void OnArrowDirectionChanged(EventArgs e)
        {
            if (ArrowDirectionChanged != null)
                ArrowDirectionChanged(this, e);
        }

        protected virtual void OnTemplateChanged(EventArgs e)
        {
            if (ThemeChanged != null)
                ThemeChanged(this, e);
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
                    return AutoSelectTemplate();
            }
        }

        private Template AutoSelectTemplate()
        {
            if (Application.RenderWithVisualStyles)
            {
                var metroVersion = new Version(6, 2);
                var aeroVersion = new Version(6, 0);

                Version version = Environment.OSVersion.Version;
                if (version >= metroVersion)
                    return new MetroTemplate();
                if (version >= aeroVersion)
                    return new AeroTemplate();
            }

            return null;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            state = ButtonState.Hover;
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            state = ButtonState.Normal;
            Invalidate();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            state = ButtonState.Pressed;
            Invalidate();

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            state = ButtonState.Hover;
            Invalidate();

            base.OnMouseUp(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();

            base.OnEnabledChanged(e);
        }

        private void DrawDefault(Graphics g, ArrowDirection direction, ButtonState state)
        {
            var arrowIndex = (int) direction;
            if (state == ButtonState.Disabled)
                arrowIndex += 2;

            int arrowSize = Math.Min(16, Math.Min(Width, Height));
            var arrowRect = new Rectangle((Width - arrowSize) / 2, (Height - arrowSize) / 2, arrowSize, arrowSize);

            ButtonRenderer.DrawButton(g, ClientRectangle, arrows[arrowIndex], arrowRect, false,
                (PushButtonState) (state + 1));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (template != null)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                int buttonSize = Math.Min(Width, Height);
                float sizeFactor = buttonSize / 24.0f;
                g.Transform = new Matrix(sizeFactor, 0, 0, sizeFactor, (Width - buttonSize) / 2.0f,
                    (Height - buttonSize) / 2.0f);

                template.Draw(g, arrowDirection, Enabled ? state : ButtonState.Disabled);
            }
            else
                DrawDefault(e.Graphics, arrowDirection, Enabled ? state : ButtonState.Disabled);

            base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            foreach (Bitmap bmp in arrows)
            {
                bmp.Dispose();
            }

            if (template != null)
                template.Dispose();

            base.Dispose(disposing);
        }
    }
}