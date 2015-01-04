using System;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.Core.Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct TvItem
    {
        public int mask;
        public IntPtr hItem;
        public int state;
        public int stateMask;
        public readonly IntPtr pszText;
        public readonly IntPtr cchTextMax;
        public readonly int iImage;
        public readonly int iSelectedImage;
        public readonly int cChildren;
        public readonly int lParam;
    }
}