// Copyright © Dominic Beger 2017

using System.Net.NetworkInformation;
using nUpdate.Win32;

namespace nUpdate
{
    public static class WebConnection
    {
        static WebConnection()
        {
            NetworkChange.NetworkAvailabilityChanged += NetworkAvaialbilityChanged;
        }

        /// <summary>
        ///     Checks whether an internet connection is available, or not.
        /// </summary>
        /// <returns>Returns <c>true</c> if a network connection is available, otherwise <c>false</c>.</returns>
        public static bool IsAvailable()
        {
            int desc;
            return NativeMethods.InternetGetConnectedState(out desc, 0);
        }

        public static void NetworkAvaialbilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
        }
    }
}