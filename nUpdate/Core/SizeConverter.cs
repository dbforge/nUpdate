
namespace nUpdate.Core
{
    internal class SizeConverter
    {
        /// <summary>
        /// Converts Bytes to MegaBytes.
        /// </summary>
        /// <param name="bytes">The long for the size.</param>
        /// <returns>Returns a double containing the converted size in MB.</returns>
        public static int ConvertBytesToMegabytes(int bytes)
        {
            return bytes / 1024 / 1024;
        }
    }
}
