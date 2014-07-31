using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace nUpdate.Administration
{
    internal class WatermarkTextBox : TextBox
    {
        public WatermarkTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        [Localizable(true)]
        public string Cue
        {
            get { return this.mCue; }
            set { this.mCue = value; this.updateCue(); }
        }

        private void updateCue()
        {
            if (this.IsHandleCreated && mCue != null)
            {
                SendMessage(this.Handle, 0x1501, (IntPtr)1, mCue);
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.updateCue();
        }
        private string mCue;

        // PInvoke
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, string lp);
    }
}
