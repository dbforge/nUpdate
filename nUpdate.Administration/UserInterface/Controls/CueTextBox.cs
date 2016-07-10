// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.ComponentModel;
using System.Windows.Forms;
using nUpdate.Administration.Win32;

namespace nUpdate.Administration.UserInterface.Controls
{
    public class CueTextBox : TextBox
    {
        private string _cue;

        public CueTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        [Localizable(true)]
        public string Cue
        {
            get { return _cue; }
            set
            {
                _cue = value;
                UpdateCue();
            }
        }

        private void UpdateCue()
        {
            if (IsHandleCreated && _cue != null)
                NativeMethods.SendMessage(Handle, 0x1501, new IntPtr(1), _cue);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateCue();
        }
    }
}