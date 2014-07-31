using System;
using System.Runtime.InteropServices;
using System.Security;

namespace nUpdate.Administration.Core
{
    internal class EncryptionConverter
    {
        /// <summary>
        ///     Converts a string into a byte-array.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>Returns the byte-array for the assuming string.</returns>
        public static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof (char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static unsafe byte[] GetBytes(SecureString str)
        {
            IntPtr unmanagedBytes = Marshal.SecureStringToGlobalAllocUnicode(str);
            byte[] bValue = null;
            try
            {
                var byteArray = (byte*) unmanagedBytes.ToPointer();

                // Find the end of the string
                byte* pEnd = byteArray;
                char c = '\0';
                do
                {
                    byte b1 = *pEnd++;
                    byte b2 = *pEnd++;
                    c = '\0';
                    c = (char) (b1 << 8);
                    c += (char) b2;
                } while (c != '\0');

                // Length is effectively the difference here (note we're 2 past end) 
                var length = (int) ((pEnd - byteArray) - 2);
                bValue = new byte[length];
                for (int i = 0; i < length; ++i)
                {
                    // Work with data in byte array as necessary, via pointers, here
                    bValue[i] = *(byteArray + i);
                }

                return bValue;
            }
            finally
            {
                // This will completely remove the data from memory
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedBytes);
            }
        }

        /// <summary>
        ///     Converts a byte-array into a string.
        /// </summary>
        /// <param name="bytes">The byte-array to convert.</param>
        /// <returns>Returns the string for the assuming byte-array.</returns>
        public static string GetString(byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof (char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}