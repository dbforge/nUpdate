// Copyright © Dominic Beger 2018

using System;
using System.Runtime.InteropServices;
using System.Text;
using nUpdate.Administration.UI.Controls;

namespace nUpdate.Administration.Core.Win32
{
    internal sealed class NativeMethods
    {
        [Flags]
        public enum ShGetFileInfoFlags
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

        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hWnd, ref PaintStruct paintStruct);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins margins);

        // Not portable (bool)

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, ref PaintStruct paintStruct);

        [DllImport("wininet.dll")]
        public static extern bool InternetGetConnectedState(out int connDescription, int reservedValue);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint msg, ref int wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint msg, IntPtr wParam, bool lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, [MarshalAs(UnmanagedType.U4)] int msg, IntPtr wParam,
            ref ExplorerTreeNode item);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref ExplorerListViewGroup lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, ref Shfileinfo psfi,
            int cbFileInfo, ShGetFileInfoFlags flags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct Shfileinfo
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
    }
}