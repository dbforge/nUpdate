using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Dialogs
{
    public partial class StatisticsServerAddDialog : BaseDialog
    {
        private MARGINS margins;

        public StatisticsServerAddDialog()
        {
            InitializeComponent();
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margin);

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (DwmIsCompositionEnabled())
            {
                e.Graphics.Clear(Color.Black);

                var clientArea = new Rectangle(
                    margins.Left,
                    margins.Top,
                    ClientRectangle.Width - margins.Left - margins.Right,
                    ClientRectangle.Height - margins.Top - margins.Bottom
                    );
                Brush b = new SolidBrush(BackColor);
                e.Graphics.FillRectangle(b, clientArea);
            }
        }

        private void StatisticsServerAddDialog_Load(object sender, EventArgs e)
        {
            if (DwmIsCompositionEnabled())
            {
                margins = new MARGINS();
                margins.Top = 30;
                DwmExtendFrameIntoClientArea(Handle, ref margins);
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }
    }
}