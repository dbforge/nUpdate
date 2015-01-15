// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.ComponentModel;
using System.Windows.Forms;
using nUpdate.Administration.Core.Win32;

namespace nUpdate.Administration.UI.Controls
{
    public class CueTextBox : TextBox
    {
        private string _mCue;

        public CueTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        [Localizable(true)]
        public string Cue
        {
            get { return _mCue; }
            set
            {
                _mCue = value;
                UpdateCue();
            }
        }

        private void UpdateCue()
        {
            if (IsHandleCreated && _mCue != null)
                NativeMethods.SendMessage(Handle, 0x1501, (IntPtr) 1, _mCue);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateCue();
        }
    }
}