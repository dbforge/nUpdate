// NativeMethods.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace nUpdate.UI.WinForms.Win32
{
    internal class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern int StrFormatByteSize(long fileSize,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);
    }
}