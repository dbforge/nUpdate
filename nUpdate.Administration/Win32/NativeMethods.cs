using System;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.Win32
{
    internal class NativeMethods
    {
        internal const int GWL_STYLE = -16;
        internal const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
