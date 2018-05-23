// Copyright © Dominic Beger 2018

using System;
using System.ComponentModel;
using System.Windows.Forms;
using nUpdate.Administration.Core.Win32;

namespace nUpdate.Administration.UI.Controls
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
            get => _cue;
            set
            {
                _cue = value;
                UpdateCue();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateCue();
        }

        private void UpdateCue()
        {
            if (IsHandleCreated && _cue != null)
                NativeMethods.SendMessage(Handle, 0x1501, new IntPtr(1), _cue);
        }
    }
}