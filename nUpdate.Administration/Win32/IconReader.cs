// IconReader.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace nUpdate.Administration.Win32
{
    public static class IconReader
    {
        /// <summary>
        ///     Returns the associated icon of a <paramref name="fileExtension" />.
        /// </summary>
        /// <param name="fileExtension">The file extension to get the associated icon for.</param>
        /// <returns>The associated icon of the <paramref name="fileExtension" />.</returns>
        public static Icon GetFileIcon(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
                throw new ArgumentNullException(nameof(fileExtension));

            var shfileinfo = new NativeMethods.Shfileinfo();
            const NativeMethods.ShGetFileInfoFlags flags =
                NativeMethods.ShGetFileInfoFlags.TypeName |
                NativeMethods.ShGetFileInfoFlags.DisplayName |
                NativeMethods.ShGetFileInfoFlags.Icon |
                NativeMethods.ShGetFileInfoFlags.SysIconIndex |
                NativeMethods.ShGetFileInfoFlags.UseFileAttributes |
                NativeMethods.ShGetFileInfoFlags.SmallIcon;

            var hIcon = IntPtr.Zero;

            try
            {
                hIcon = NativeMethods.SHGetFileInfo(fileExtension, 0, ref shfileinfo, Marshal.SizeOf(shfileinfo),
                    flags);
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