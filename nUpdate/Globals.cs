using System;
using System.IO;

namespace nUpdate
{
    public static class Globals
    {
        public static string ApplicationUpdateDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nUpdate",
            "Updates", ApplicationParameters.ProductName);

        public static string PackageExtension => ".nupdpkg";
    }
}
