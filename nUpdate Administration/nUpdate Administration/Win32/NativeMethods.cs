using System;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.Win32
{
    internal sealed class NativeMethods
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, SHGetFileInfoFlags flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto,SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [Flags]
        public enum SHGetFileInfoFlags
        {
            AddOverlays = 0x000000020,
            AttributesSpecified = 0x000020000,
            Attributes = 0x000000800,
            DisplayName = 0x000000200,
            ExeType = 0x000002000,
            Icon = 0x000000100,
            IconLocation = 0x000001000,
            LargeIcon = 0x000000000,
            LinkOverlay = 0x000008000,
            OpenIcon = 0x000000002,
            OverlayIndex = 0x000000040,
            PathIsItemList = 0x000000008,
            Selected = 0x000010000,
            ShellIconSize = 0x000000004,
            SmallIcon = 0x000000001,
            SysIconIndex = 0x000004000,
            TypeName = 0x000000400,
            UseFileAttributes = 0x000000010
        }
    }
}
