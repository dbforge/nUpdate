// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Runtime.InteropServices;

namespace nUpdate.Win32
{
    internal class NativeMethods
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetGetConnectedState(out int connDescription, int reservedValue);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}