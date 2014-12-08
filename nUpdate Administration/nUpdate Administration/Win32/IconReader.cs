using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.Win32
{
    /// <summary>
    /// IconReader
    /// </summary>
    public static class IconReader
    {
        /// <summary>
        /// Returns the associated icon of a <paramref name="fileExtension"/>.
        /// </summary>
        /// <param name="fileExtension">The file extension to get the associated icon for.</param>
        /// <returns>The associated icon of the <paramref name="fileExtension"/>.</returns>
        public static Icon GetFileIcon(string fileExtension)
        {
            if(String.IsNullOrWhiteSpace(fileExtension))
                throw new ArgumentNullException("fileExtension");

            var shfileinfo = new NativeMethods.SHFILEINFO();
            const NativeMethods.SHGetFileInfoFlags flags = 
                                NativeMethods.SHGetFileInfoFlags.TypeName |
                                NativeMethods.SHGetFileInfoFlags.DisplayName |
                                NativeMethods.SHGetFileInfoFlags.Icon |
                                NativeMethods.SHGetFileInfoFlags.SysIconIndex |
                                NativeMethods.SHGetFileInfoFlags.UseFileAttributes |
                                NativeMethods.SHGetFileInfoFlags.SmallIcon;

            IntPtr hIcon = IntPtr.Zero;

            try
            {
                hIcon = NativeMethods.SHGetFileInfo(fileExtension, 0, ref shfileinfo, Marshal.SizeOf(shfileinfo), flags);
                if (hIcon == IntPtr.Zero)
                    return null;

                return (Icon) Icon.FromHandle(shfileinfo.hIcon).Clone();
            }
            finally
            {
                NativeMethods.DestroyIcon(hIcon);
            }
        }
    }
}
