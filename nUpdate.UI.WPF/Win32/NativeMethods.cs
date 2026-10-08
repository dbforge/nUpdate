using System.Runtime.InteropServices;

namespace nUpdate.UI.WPF.Win32;

internal static class NativeMethods
{
    public const int MaxPath = 260;

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, [In, Out] char[] iconPath, ref ushort index);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyIcon(IntPtr handle);
}
