// Author: Dominic Beger (Trade/ProgTrade)License: Creative Commons Attribution NoDerivs (CC-ND)\nCreated: 23-12-2014 19:11

using System;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.Core.Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PaintStruct
    {
        public readonly IntPtr hdc;
        public readonly bool fErase;
        public readonly int rcPaint_left;
        public readonly int rcPaint_top;
        public readonly int rcPaint_right;
        public readonly int rcPaint_bottom;
        public readonly bool fRestore;
        public readonly bool fIncUpdate;
        public readonly int reserved1;
        public readonly int reserved2;
        public readonly int reserved3;
        public readonly int reserved4;
        public readonly int reserved5;
        public readonly int reserved6;
        public readonly int reserved7;
        public readonly int reserved8;
    }
}