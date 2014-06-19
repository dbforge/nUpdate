/**
 *  Updatesystem for managing and downloading updates on .NET-applications
 *  ConnectionChecker-Class for checking internet connections and remote files availability
 *  
 *  Author: Trade
 *  License: Creative Commons Attribution NoDerivs (CC-ND) 
 *  Created: 09th December 2013
 *  Copyright (C) Trade
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.InteropServices;

namespace nUpdate.Core
{
    public class ConnectionChecker
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
