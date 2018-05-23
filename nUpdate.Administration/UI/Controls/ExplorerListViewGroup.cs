// Copyright © Dominic Beger 2018

using System;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.UI.Controls
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct ExplorerListViewGroup
    {
        public int CbSize;
        public ExplorerListViewGroupMask Mask;
        [MarshalAs(UnmanagedType.LPWStr)] public string PszHeader;
        public int CchHeader;
        [MarshalAs(UnmanagedType.LPWStr)] public string PszFooter;
        public int CchFooter;
        public int GroupId;
        public int StateMask;
        public ExplorerListViewGroupState State;
        public uint UAlign;
        private readonly IntPtr PszSubtitle;
        public uint CchSubtitle;
        [MarshalAs(UnmanagedType.LPWStr)] public string PszTask;
        public uint CchTask;
        [MarshalAs(UnmanagedType.LPWStr)] public string PszDescriptionTop;
        public uint CchDescriptionTop;
        [MarshalAs(UnmanagedType.LPWStr)] public string PszDescriptionBottom;
        public uint CchDescriptionBottom;
        public int TitleImage;
        public int ExtendedImage;
        public int FirstItem;
        private readonly IntPtr CItems;
        private readonly IntPtr PszSubsetTitle;
        private readonly IntPtr CchSubsetTitle;
    }

    public enum ExplorerListViewGroupMask
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

    public enum ExplorerListViewGroupState
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
}