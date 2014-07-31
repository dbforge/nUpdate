using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace nUpdate.Administration
{
    internal class WatermarkTextBox : TextBox
    {
        private string mCue;

        public WatermarkTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        [Localizable(true)]
        public string Cue
        {
            get { return mCue; }
            set
            {
                mCue = value;
                updateCue();
            }
        }

        private void updateCue()
        {
            if (IsHandleCreated && mCue != null)
                SendMessage(Handle, 0x1501, (IntPtr) 1, mCue);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            updateCue();
        }

        // PInvoke
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, string lp);
    }
}