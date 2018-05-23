// Copyright © Dominic Beger 2018

using System;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.UI.Controls
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ExplorerTreeNode
    {
        public int Mask;
        public IntPtr HItem;
        public int State;
        public int StateMask;
        public readonly IntPtr PszText;
        public readonly IntPtr CchTextMax;
        public readonly int Image;
        public readonly int SelectedImage;
        public readonly int CChildren;
        public readonly int LParam;
    }
}