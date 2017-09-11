// Copyright © Dominic Beger 2017

using System;
using System.IO;
using System.Text;
using nUpdate.Internal.Core.Win32;

namespace nUpdate.Internal.Core
{
    public class SizeHelper
    {
        public static string ConvertSize(long packageSize)
        {
            var sb = new StringBuilder(20);
            NativeMethods.StrFormatByteSize(packageSize, sb, 20);
            return sb.ToString();
        }

        public static bool HasEnoughSpace(double packageSize, out double necessaryBytesToFree)
        {
            var drive = new DriveInfo(new FileInfo(Path.GetTempPath()).Directory.Root.FullName);
            if (drive.AvailableFreeSpace > packageSize * 2)
            {
                necessaryBytesToFree = 0;
                return true;
            }
            necessaryBytesToFree = Math.Abs(packageSize * 2 - drive.AvailableFreeSpace);
            // Multiply this value with 2 because the files are copied during the installation, so we have download + installation (copying)
            return false;
        }
    }
}