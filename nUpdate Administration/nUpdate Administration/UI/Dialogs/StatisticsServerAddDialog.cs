using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class StatisticsServerAddDialog : BaseDialog
    {
        public StatisticsServerAddDialog()
        {
            InitializeComponent();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll")]
        public extern static int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margin);

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (DwmIsCompositionEnabled())
            {
                e.Graphics.Clear(Color.Black);

                Rectangle clientArea = new Rectangle(
                        this.margins.Left,
                        this.margins.Top,
                        this.ClientRectangle.Width - this.margins.Left - this.margins.Right,
                        this.ClientRectangle.Height - this.margins.Top - this.margins.Bottom
                    );
                Brush b = new SolidBrush(this.BackColor);
                e.Graphics.FillRectangle(b, clientArea);
            }
        }

        private MARGINS margins;
        private void StatisticsServerAddDialog_Load(object sender, EventArgs e)
        {
            if (DwmIsCompositionEnabled())
            {
                this.margins = new MARGINS();
                this.margins.Top = 30;
                DwmExtendFrameIntoClientArea(this.Handle, ref this.margins);
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {

        }
    }
}
