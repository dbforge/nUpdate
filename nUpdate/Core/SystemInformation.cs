// Copyright © Dominic Beger 2017

using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace nUpdate.Internal.Core
{
    internal class SystemInformation
    {
        /// <summary>
        ///     Gets the MAC-Address of the current computer for the statistics.
        /// </summary>
        public static string MacAddress
        {
            get
            {
                return
                    NetworkInterface.GetAllNetworkInterfaces()
                        .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                        .Select(nic => nic.GetPhysicalAddress().ToString())
                        .FirstOrDefault();
            }
        }

        /// <summary>
        ///     Gets the name of the current computer's operating system.
        /// </summary>
        public static string OperatingSystemName
        {
            get
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
}