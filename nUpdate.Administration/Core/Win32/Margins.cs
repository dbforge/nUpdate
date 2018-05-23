// Copyright © Dominic Beger 2018

using System.Runtime.InteropServices;

namespace nUpdate.Administration.Core.Win32
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