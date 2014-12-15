// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace nUpdate.Administration.UI.Controls
{
    public class CommandLink : Button
    {
        private const int BS_COMMANDLINK = 0x0000000E;
        private const uint BCM_SETNOTE = 0x00001609;
        private const uint BCM_GETNOTE = 0x0000160A;
        private const uint BCM_GETNOTELENGTH = 0x0000160B;
        private const uint BCM_SETSHIELD = 0x0000160C;
        private bool shield;

        public CommandLink()
        {
            FlatStyle = FlatStyle.System;
        }

        protected override Size DefaultSize
        {
            get { return new Size(180, 60); }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cParams = base.CreateParams;
                cParams.Style |= BS_COMMANDLINK;
                return cParams;
            }
        }

        [Category("Command Link"),
         Description("Gets or sets the shield icon visibility of the command link."),
         DefaultValue(false)]
        public bool Shield
        {
            get { return shield; }
            set
            {
                shield = value;
                SendMessage(new HandleRef(this, Handle), BCM_SETSHIELD, IntPtr.Zero, shield);
            }
        }

        [Category("Command Link"),
         Description("Gets or sets the note text of the command link."),
         DefaultValue("")]
        public string Note
        {
            get { return GetNoteText(); }
            set { SetNoteText(value); }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SendMessage(HandleRef hWnd, UInt32 Msg, ref int wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SendMessage(HandleRef hWnd, UInt32 Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SendMessage(HandleRef hWnd, UInt32 Msg, IntPtr wParam, bool lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SendMessage(HandleRef hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private void SetNoteText(string value)
        {
            SendMessage(new HandleRef(this, Handle), BCM_SETNOTE, IntPtr.Zero, value);
        }

        private string GetNoteText()
        {
            int length = SendMessage(new HandleRef(this, Handle), BCM_GETNOTELENGTH, IntPtr.Zero, IntPtr.Zero) + 1;

            var sb = new StringBuilder(length);

            SendMessage(new HandleRef(this, Handle), BCM_GETNOTE, ref length, sb);

            return sb.ToString();
        }
    }
}