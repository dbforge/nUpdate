// NativeMethods.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace nUpdate.UpdateInstaller.Win32
{
    internal class NativeMethods
    {
        [DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon", CharSet = CharSet.Auto)]
        public static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);
    }
}