// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace nUpdate.Core.Win32
{
    internal class NativeMethods
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetGetConnectedState(out int connDescription, int reservedValue);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon", CharSet = CharSet.Auto)]
        public static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);
    }
}