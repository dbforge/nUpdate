// Copyright © Dominic Beger 2017

using System;
using System.IO;

namespace nUpdate.Core
{
    public class SizeHelper
    {
        private const float GB = 1073741824;
        private const int MB = 1048576;
        private const int KB = 1024;

        public static Tuple<double, string> ConvertSize(double packageSize)
        {
            if (packageSize >= GB / 10)
                return new Tuple<double, string>(Math.Round(packageSize / GB, 1), "GB");
            if (packageSize >= MB / 10)
                return new Tuple<double, string>(Math.Round(packageSize / MB, 1), "MB");
            if (packageSize >= KB / 10)
                return new Tuple<double, string>(Math.Round(packageSize / KB, 1), "KB");
            if (packageSize >= 1)
                return new Tuple<double, string>(packageSize, "B");

            return new Tuple<double, string>(double.NaN, "NaN");
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