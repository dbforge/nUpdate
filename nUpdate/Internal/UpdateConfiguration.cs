using nUpdate.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace nUpdate.Internal
{
    internal class UpdateConfiguration
    {
        /// <summary>
        /// Loads the update configuration from the server.
        /// </summary>
        /// <param name="infoFileUrl">The url of the info file.</param>
        /// <returns>Returns a deserialized list of type <see cref="UpdatePackage"/>.</returns>
        public List<UpdateConfiguration> LoadUpdateConfiguration(Uri infoFileUrl)
        {
            var wc = new WebClient();
            wc.Encoding = Encoding.UTF8;

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return (true); };

            var input = wc.DownloadString(infoFileUrl);
            if (String.IsNullOrEmpty(input))
            {
                return null;
            }
            else
            {
                return Serializer.Deserialize<List<UpdateConfiguration>>(input);
            }
        }

        /// <summary>
        /// The version of the package as string.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Sets if the package should be used within the statistics.
        /// </summary>
        public bool UseStatistics { get; set; }

        /// <summary>
        /// The url of the PHP-file which redirects to the package download and does the statistic entries.
        /// </summary>
        public Uri UpdatePhpFileUrl { get; set; }

        /// <summary>
        /// The version ID of this package to use in the statistics.
        /// </summary>
        public int VersionID { get; set; }

        /// <summary>
        /// The url of the package as relative uri. This property is not necessary if a statistics server is used.
        /// </summary>
        public string RelativePackageUri { get; set; }

        /// <summary>
        /// The changelog of the update package.
        /// </summary>
        public string Changelog { get; set; }

        /// <summary>
        /// The signature of the update package.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// The unsupported versions of the update package.
        /// </summary>
        public string[] UnsupportedVersions { get; set; }

        /// <summary>
        /// The architecture settings of the update package.
        /// </summary>
        public string Architecture { get; set; }

        /// <summary>
        /// The operations of the update package.
        /// </summary>
        //public Operation[] Operations { get; set; } // TODO: Operation property when operations are added

        /// <summary>
        /// Sets if this update must be installed.
        /// </summary>
        public bool MustUpdate { get; set; }
    }
}
