using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nUpdate.Core
{
    public class SizeHelper
    {
        private const float GB = 1073741824;
        private const int MB = 1048576;
        private const int KB = 1024;

        public static bool HasEnoughSpace(double packageSize, out double necessaryBytesToFree)
        {
            DriveInfo drive = new DriveInfo(new FileInfo(Path.GetTempPath()).Directory.Root.FullName);
            if (drive.AvailableFreeSpace > (packageSize * 2))
            {
                necessaryBytesToFree = 0;
                return true;
            }
            else
            {
                necessaryBytesToFree = Math.Abs((packageSize * 2) - drive.AvailableFreeSpace); // Multiply this value with 2 because the files are copied during the installation, so we have download + installation (copying)
                return false;
            }
        }

        public static Tuple<double , string> ConvertSize(double packageSize)
        {
            if (packageSize >= (GB / 10))
            {
                return new Tuple<double, string>(Math.Round((packageSize / GB), 1), "GB");
            }
            else if (packageSize >= (MB / 10))
            {
                return new Tuple<double, string>(Math.Round((packageSize / MB), 1), "MB");
            }
            else if (packageSize >= (KB / 10))
            {
                return new Tuple<double, string>(Math.Round((packageSize / KB), 1), "KB");
            }
            else if (packageSize >= 1)
            {
                return new Tuple<double, string>(packageSize, "B");
            }

            return new Tuple<double, string>(Double.NaN, "NaN");
        }
    }
}
