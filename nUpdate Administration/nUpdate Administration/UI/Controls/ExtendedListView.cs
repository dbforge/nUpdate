// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref LVGROUP lParam);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        public void MakeCollapsable()
        {
            const int lvmFirst = 0x1000;
            const int LVM_SETGROUPINFO = (lvmFirst + 147);

            for (int i = 0; i <= Groups.Count - 1; i++)
            {
                var grp = new LVGROUP();
                grp.CbSize = Marshal.SizeOf(grp);
                grp.State = ListViewGroupState.Collapsible;
                grp.Mask = ListViewGroupMask.State;
                grp.IGroupId = GetGroupId(Groups[i]);

                if (grp.IGroupId >= 0)
                    SendMessage(Handle, LVM_SETGROUPINFO, grp.IGroupId, ref grp);
            }
        }

        private static int GetGroupId(ListViewGroup lstvwgrp)
        {
            int rtnval = -1;
            Type grpTp = lstvwgrp.GetType();
            {
                PropertyInfo pi = grpTp.GetProperty("ID", BindingFlags.NonPublic | BindingFlags.Instance);
                if (pi != null)
                {
                    object tmprtnval = pi.GetValue(lstvwgrp, null);
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
                        SetWindowTheme(Handle, "explorer", null);

                        SendMessage(Handle, LVM_SETEXTENDEDLISTVIEWSTYLE, LVS_EX_DOUBLEBUFFER, LVS_EX_DOUBLEBUFFER);
                        _elv = true;
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        #region Nested type: LVGROUP

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LVGROUP
        {
            public int CbSize;
            public ListViewGroupMask Mask;
            [MarshalAs(UnmanagedType.LPWStr)] public string PszHeader;
            public int CchHeader;
            [MarshalAs(UnmanagedType.LPWStr)] public string PszFooter;
            public int CchFooter;
            public int IGroupId;
            public int StateMask;
            public ListViewGroupState State;
            public uint UAlign;
            public IntPtr PszSubtitle;
            public uint CchSubtitle;
            [MarshalAs(UnmanagedType.LPWStr)] public string PszTask;
            public uint CchTask;
            [MarshalAs(UnmanagedType.LPWStr)] public string PszDescriptionTop;
            public uint CchDescriptionTop;
            [MarshalAs(UnmanagedType.LPWStr)] public string PszDescriptionBottom;
            public uint CchDescriptionBottom;
            public int ITitleImage;
            public int IExtendedImage;
            public int IFirstItem;
            public IntPtr CItems;
            public IntPtr PszSubsetTitle;
            public IntPtr CchSubsetTitle;
        }

        #endregion

        #region Nested type: ListViewGroupMask

        public enum ListViewGroupMask
        {
            None = 0x0,
            Header = 0x1,
            Footer = 0x2,
            State = 0x4,
            Align = 0x8,
            GroupId = 0x10,
            SubTitle = 0x100,
            Task = 0x200,
            DescriptionTop = 0x400,
            DescriptionBottom = 0x800,
            TitleImage = 0x1000,
            ExtendedImage = 0x2000,
            Items = 0x4000,
            Subset = 0x8000,
            SubsetItems = 0x10000
        }

        #endregion

        #region Nested type: ListViewGroupState

        public enum ListViewGroupState
        {
            Normal = 0,
            Collapsed = 1,
            Hidden = 2,
            NoHeader = 4,
            Collapsible = 8,
            Focused = 16,
            Selected = 32,
            SubSeted = 64,
            SubSetLinkFocused = 128
        }

        #endregion
    }
}