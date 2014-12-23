// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using nUpdate.Administration.Core.Win32;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Controls
{
    public class WatermarkTextBox : TextBox
    {
        private string _mCue;

        public WatermarkTextBox()
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