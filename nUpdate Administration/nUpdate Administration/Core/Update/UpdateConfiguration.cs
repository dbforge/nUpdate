// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using nUpdate.Administration.Core.Application;
using nUpdate.Administration.Core.Update.Operations;

namespace nUpdate.Administration.Core.Update
{
    [Serializable]
    internal class UpdateConfiguration
    {
        /// <summary>
        ///     The version of the package as string.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     Sets if the package should be used within the statistics.
        /// </summary>
        public bool UseStatistics { get; set; }

        /// <summary>
        ///     The url of the PHP-file which redirects to the package download and does the statistic entries.
        /// </summary>
        public Uri UpdatePhpFileUrl { get; set; }

        /// <summary>
        ///     The version ID of this package to use in the statistics, if used.
        /// </summary>
        public int VersionId { get; set; }

        /// <summary>
        ///     The url of the update package.
        /// </summary>
        public Uri UpdatePackageUrl { get; set; }

        /// <summary>
        ///     The whole changelog of the update package.
        /// </summary>
        public Dictionary<CultureInfo, string> Changelog { get; set; }

        /// <summary>
        ///     The signature of the update package.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        ///     The unsupported versions of the update package.
        /// </summary>
        public string[] UnsupportedVersions { get; set; }

        /// <summary>
        ///     The architecture settings of the update package.
        /// </summary>
        public string Architecture { get; set; }

        /// <summary>
        ///     The operations of the update package.
        /// </summary>
        public List<Operation> Operations { get; set; }

        /// <summary>
        ///     Sets if this update must be installed.
        /// </summary>
        public bool MustUpdate { get; set; }

        /// <summary>
        ///     Loads the update configuration from the server.
        /// </summary>
        /// <param name="configFileUrl">The url of the info file.</param>
        /// <param name="proxy">The optional proxy to use.</param>
        /// <returns>Returns a deserialized list of type <see cref="UpdatePackage" />.</returns>
        public static List<UpdateConfiguration> DownloadUpdateConfiguration(Uri configFileUrl, WebProxy proxy)
        {
            var wc = new WebClientWrapper();
            if (proxy != null)
                wc.Proxy = proxy;

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return (true); };
            return Serializer.Deserialize<List<UpdateConfiguration>>(wc.DownloadString(configFileUrl));
        }

        /// <summary>
        ///     Loads an update configuration from a local file.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        public static List<UpdateConfiguration> LoadFromFile(string filePath)
        {
            return Serializer.Deserialize<List<UpdateConfiguration>>(File.ReadAllText(filePath));
        }
    }
}