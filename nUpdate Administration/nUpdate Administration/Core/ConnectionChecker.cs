using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace nUpdate.Administration.Core
{
    internal class ConnectionChecker
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connDescription, int ReservedValue);

        /// <summary>
        /// Checks if an internet connection is available.
        /// </summary>
        /// <returns>This function returns a boolean.</returns>
        public static bool IsConnectionAvailable()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }
    }
}
