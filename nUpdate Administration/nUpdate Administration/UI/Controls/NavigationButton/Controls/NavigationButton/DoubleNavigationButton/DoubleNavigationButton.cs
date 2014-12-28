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
    /// A simple Back & Forward Button drawn by Windows via Visual Styles.
    /// </summary>
    [ToolboxBitmap(typeof(Button))]
    [Designer(typeof(DoubleNavigationButtonDesigner))]
    [DefaultEvent("Click")]
    [Description("A simple Back & Forward Button drawn by Windows via Visual Styles")]
    public class DoubleNavigationButton
        : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:VisualStyleControls.Controls.DoubleNavigationButton"/> class.
        /// </summary>
        public DoubleNavigationButton()
            : base()
        {
            this.SuspendLayout();

            this.Size = new Size(60, 30);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            this.Controls.Add(this.backButton = new NavigationButton() { ButtonType = NavigationButtonType.Back, Location = new Point(0, 0) });
            this.Controls.Add(this.forwardButton = new NavigationButton() { ButtonType = NavigationButtonType.Forward, Location = new Point(30, 0) });
            this.backButton.Click += (s, e) => this.OnBackButtonClicked();
            this.forwardButton.Click += (s, e) => this.OnForwardButtonClicked();

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Gets raised if the Back Button got clicked.
        /// </summary>
        [Description("Gets raised when the Back Button got clicked.")]
        public event EventHandler BackButtonClicked;

        /// <summary>
        /// Raises the <see cref="E:VisualStyleControls.Controls.DoubleNavigationButton.BackButtonClicked"/> event.
        /// </summary>
        protected virtual void OnBackButtonClicked()
        {
            if (this.BackButtonClicked != null)
            {
                this.BackButtonClicked(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets raised if the Forward Button got clicked.
        /// </summary>
        [Description("Gets raised when the Back Button got clicked.")]
        public event EventHandler ForwardButtonClicked;

        /// <summary>
        /// Raises the <see cref="E:VisualStyleControls.Controls.DoubleNavigationButton.ForwardButtonClicked"/> event.
        /// </summary>
        protected virtual void OnForwardButtonClicked()
        {
            if (this.ForwardButtonClicked != null)
            {
                this.ForwardButtonClicked(this, EventArgs.Empty);
            }
        }

        protected NavigationButton backButton;
        protected NavigationButton forwardButton;

        protected override void OnEnabledChanged(EventArgs e)
        {
            this.backButton.Enabled = this.Enabled;
            this.forwardButton.Enabled = this.Enabled;
            base.OnEnabledChanged(e);
        }
    }
}