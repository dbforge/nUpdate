// PaintStruct.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PaintStruct
    {
        public readonly IntPtr Hdc;
        public readonly bool FErase;
        public readonly int RcPaint_left;
        public readonly int RcPaint_top;
        public readonly int RcPaint_right;
        public readonly int RcPaint_bottom;
        public readonly bool FRestore;
        public readonly bool FIncUpdate;
        public readonly int Reserved1;
        public readonly int Reserved2;
        public readonly int Reserved3;
        public readonly int Reserved4;
        public readonly int Reserved5;
        public readonly int Reserved6;
        public readonly int Reserved7;
        public readonly int Reserved8;
    }
}