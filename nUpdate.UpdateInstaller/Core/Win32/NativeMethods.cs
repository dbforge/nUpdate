// Copyright © Dominic Beger 2018

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace nUpdate.UpdateInstaller.Core.Win32
{
    internal class NativeMethods
    {
        [DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon", CharSet = CharSet.Auto)]
        public static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);
    }
}