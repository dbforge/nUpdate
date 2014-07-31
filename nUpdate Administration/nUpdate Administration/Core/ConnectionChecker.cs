using System.Runtime.InteropServices;

namespace nUpdate.Administration.Core
{
    internal class ConnectionChecker
    {
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int connDescription, int ReservedValue);

        /// <summary>
        ///     Checks if an internet connection is available.
        /// </summary>
        /// <returns>This function returns a boolean.</returns>
        public static bool IsConnectionAvailable()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }
    }
}