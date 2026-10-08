using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace nUpdate.UI.WindowsForms.Win32;

internal static class NativeMethods
{
    private const uint BcmSetShield = 0x160C;

    /// <summary>Shows the UAC shield on a button whose action needs elevation.</summary>
    public static void AddShieldToButton(Button button)
    {
        button.FlatStyle = FlatStyle.System;
        _ = SendMessage(button.Handle, BcmSetShield, IntPtr.Zero, new IntPtr(1));
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
