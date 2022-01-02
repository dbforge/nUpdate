using System;
using System.IO;

namespace nUpdate.UpdateInstaller
{
    internal class PathDetector
    {
        /// <summary>
        ///     Gets the full directory path for the given tag.
        /// </summary>
        /// <param name="tag">The tag to use.</param>
        /// <returns>Returns the full path of the relating directory on the system.</returns>
        public static string GetDirectory(string tag)
        {
            switch (tag)
            {
                case "%program%":
                    return Program.AimFolder;
                case "%appdata%":
                    return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                case "%temp%":
                    return Path.GetTempPath();
                case "%desktop%":
                    return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                default:
                    return tag;
            }
        }
    }
}
