// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System.Runtime.InteropServices;

namespace nUpdate.Administration.Core
{
    public class ConnectionChecker
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