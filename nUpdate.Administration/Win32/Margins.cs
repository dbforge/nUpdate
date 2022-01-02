// Margins.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.Runtime.InteropServices;

namespace nUpdate.Administration.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }
}