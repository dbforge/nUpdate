// Copyright © Dominic Beger 2017

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using nUpdate.Core.Win32;

namespace nUpdate.Core
{
    internal class IconHelper
    {
        /// <summary>
        ///     Returns an icon representation of an image contained in the specified file.
        /// </summary>
        /// <param name="filePath">The path to the file that contains an image.</param>
        /// <returns>The System.Drawing.Icon representation of the image contained in the specified file.</returns>
        /// <exception cref="System.ArgumentException">filePath does not indicate a valid file.</exception>
        /// <remarks>
        ///     This function is identical to <see cref="IconHelper.ExtractAssociatedIcon" />, except this version accepts the
        ///     UNC format.
        /// </remarks>
        internal static Icon ExtractAssociatedIcon(string filePath)
        {
            var index = 0;

            Uri uri;
            if (filePath == null)
                throw new ArgumentException($"'{"null"}' is not valid for '{"filePath"}'", nameof(filePath));

            try
            {
                uri = new Uri(filePath);
            }
            catch (UriFormatException)
            {
                filePath = Path.GetFullPath(filePath);
                uri = new Uri(filePath);
            }

            // This makes no sense, Microsoft.
            //if (uri.IsUnc)
            //  throw new ArgumentException(String.Format("'{0}' is not valid for '{1}'", filePath, "filePath"), "filePath");

            if (!uri.IsFile)
                return null;

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            var iconPath = new StringBuilder(260);
            iconPath.Append(filePath);

            var handle = NativeMethods.ExtractAssociatedIcon(new HandleRef(null, IntPtr.Zero), iconPath, ref index);
            return handle != IntPtr.Zero ? Icon.FromHandle(handle) : null;
        }
    }
}