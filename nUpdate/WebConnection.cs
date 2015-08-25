// Author: Dominic Beger (Trade/ProgTrade)

using nUpdate.Win32;

namespace nUpdate
{
    public class WebConnection
    {
        /// <summary>
        ///     Checks whether an internet connection is available, or not.
        /// </summary>
        /// <returns>Returns <c>true</c> if a network connection is available, otherwise <c>false</c>.</returns>
        public static bool IsAvailable()
        {
            int desc;
            return NativeMethods.InternetGetConnectedState(out desc, 0);
        }
    }
}