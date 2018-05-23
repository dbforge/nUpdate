// Copyright © Dominic Beger 2018

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using nUpdate.Administration.Core.Win32;

namespace nUpdate.Administration.UI.Controls
{
    public class ExplorerListView : ListView
    {
        private const int LVM_FIRST = 0x1000;
        private const int LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
        private const int LVS_EX_DOUBLEBUFFER = 0x00010000;
        private const int WM_LBUTTONUP = 0x202;
        private const int LVM_SETGROUPINFO = LVM_FIRST + 147;
        private bool _isExplorerListView;

        public ExplorerListView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            FullRowSelect = true;
        }

        private static int GetGroupId(ListViewGroup group)
        {
            var groupType = group.GetType();
            {
                var groupIdProperty = groupType.GetProperty("ID", BindingFlags.NonPublic | BindingFlags.Instance);
                // Include inner fields and instance members
                if (groupIdProperty == null)
                    return -1;
                var value = groupIdProperty.GetValue(group, null);
                if (value != null)
                    return (int) value;
            }
            return -1;
        }

        public void MakeCollapsable() // Adds expanders to the groups, should be called in form's Show-event
        {
            if (Environment.OSVersion.Version.Major < 6)
                return;

            foreach (ListViewGroup group in Groups)
            {
                var placeHolderGroup = new ExplorerListViewGroup();
                placeHolderGroup.CbSize = Marshal.SizeOf(placeHolderGroup);
                placeHolderGroup.State = ExplorerListViewGroupState.Collapsible;
                placeHolderGroup.Mask = ExplorerListViewGroupMask.State;
                placeHolderGroup.GroupId = GetGroupId(group);

                if (placeHolderGroup.GroupId >= 0)
                    NativeMethods.SendMessage(Handle, LVM_SETGROUPINFO, new IntPtr(placeHolderGroup.GroupId),
                        ref placeHolderGroup);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONUP && Environment.OSVersion.Version.Major >= 6)
                DefWndProc(ref m);

            switch (m.Msg)
            {
                case 15:
                    if (!_isExplorerListView)
                    {
                        NativeMethods.SetWindowTheme(Handle, "explorer", null);
                        NativeMethods.SendMessage(Handle, LVM_SETEXTENDEDLISTVIEWSTYLE, new IntPtr(LVS_EX_DOUBLEBUFFER),
                            new IntPtr(LVS_EX_DOUBLEBUFFER));
                        _isExplorerListView = true;
                    }

                    break;
            }

            base.WndProc(ref m);
        }
    }
}