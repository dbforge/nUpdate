using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;

namespace nUpdate.Core
{
    public class SystemInformation
    {
        /// <summary>
        ///     Gets the Mac-Address of the current computer for the statistics.
        /// </summary>
        /// <returns>Returns the found mac address.</returns>
        public static string GetMacAddress()
        {
            return NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();
        }

        /// <summary>
        ///     Gets the name of the current computer's operating system.
        /// </summary>
        /// <returns>The found operating system name.</returns>
        public static string GetOperatingSystemName()
        {
            var osVersion = Environment.OSVersion.Version;
            switch (osVersion.Major)
            {
                case 6:
                    switch (osVersion.Minor)
                    {
                        case 0:
                            return "Windows Vista";
                        case 1:
                            return "Windows 7";
                        case 2:
                            return "Windows 8";
                        case 3:
                            return "Windows 8.1";
                    }
                    break;

                case 10:
                    switch (osVersion.Minor)
                    {
                        case 0:
                            return "Windows 10";
                    }
                    break;
            }

            return "Unknown";
        }
    }
}
