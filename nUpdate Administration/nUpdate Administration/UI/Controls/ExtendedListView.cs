// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using nUpdate.Administration.Core.Win32;

namespace nUpdate.Administration.UI.Controls
{
    public class ExtendedListView : ListView
    {
        private const int LVM_FIRST = 0x1000;
        private const int LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
        private const int LVS_EX_DOUBLEBUFFER = 0x00010000;
        private bool _elv;

        public ExtendedListView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
            DoubleBuffered = true;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            FullRowSelect = true;
        }

        protected override sealed bool DoubleBuffered
        {
            get { return base.DoubleBuffered; }
            set { base.DoubleBuffered = value; }
        }

        public void MakeCollapsable()
        {
            const int lvmFirst = 0x1000;
            const int LVM_SETGROUPINFO = (lvmFirst + 147);

            for (var i = 0; i <= Groups.Count - 1; i++)
            {
                var grp = new LvGroup();
                grp.CbSize = Marshal.SizeOf(grp);
                grp.State = ListViewGroupState.Collapsible;
                grp.Mask = ListViewGroupMask.State;
                grp.IGroupId = GetGroupId(Groups[i]);

                if (grp.IGroupId >= 0)
                    NativeMethods.SendMessage(Handle, LVM_SETGROUPINFO, new IntPtr(grp.IGroupId), ref grp);
            }
        }

        private static int GetGroupId(ListViewGroup lstvwgrp)
        {
            var rtnval = -1;
            var grpTp = lstvwgrp.GetType();
            {
                var pi = grpTp.GetProperty("ID", BindingFlags.NonPublic | BindingFlags.Instance);
                if (pi != null)
                {
                    var tmprtnval = pi.GetValue(lstvwgrp, null);
                    if (tmprtnval != null)
                        rtnval = (int) tmprtnval;
                }
            }
            return rtnval;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_LBUTTONUP = 0x202;

            if (m.Msg == WM_LBUTTONUP && Environment.OSVersion.Version.Major >= 6)
                DefWndProc(ref m);

            switch (m.Msg)
            {
                case 15:
                    if (!_elv)
                    {
                        NativeMethods.SetWindowTheme(Handle, "explorer", null);

                        NativeMethods.SendMessage(Handle, LVM_SETEXTENDEDLISTVIEWSTYLE, new IntPtr(LVS_EX_DOUBLEBUFFER),
                            new IntPtr(LVS_EX_DOUBLEBUFFER));
                        _elv = true;
                    }
                    break;
            }

            base.WndProc(ref m);
        }
    }
}