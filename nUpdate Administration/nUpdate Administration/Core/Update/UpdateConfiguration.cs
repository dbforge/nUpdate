using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Remoting;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace nUpdate.Administration.Core.Update
{
    [Serializable()]
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

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate(
            Object obj, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
            {
                return (true);
            };

            var input = wc.DownloadString(infoFileUrl);
            if (String.IsNullOrEmpty(input))
                return null;
            else
                return Serializer.Deserialize<List<UpdateConfiguration>>(input);
        }

        /// <summary>
        /// Loads the update configuration from the server.
        /// </summary>
        /// <param name="infoFileUrl">The url of the info file.</param>
        /// <param name="proxySettings">The proxy settings for the server.</param>
        /// <returns>Returns a deserialized list of type <see cref="UpdatePackage"/>.</returns>
        public List<UpdateConfiguration> LoadUpdateConfiguration(Uri infoFileUrl, ProxySettings proxySettings)
        {
            var wc = new WebClient();

            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate(
            Object obj, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
            {
                return (true);
            };

            // Set the proxy
            wc.Proxy = new WebProxy(proxySettings.Host, proxySettings.Port);
            wc.Proxy.Credentials = new NetworkCredential(proxySettings.Username, proxySettings.Password);

            return Serializer.Deserialize<List<UpdateConfiguration>>(wc.DownloadString(infoFileUrl));
        }

        /// <summary>
        /// The version of the package.
        /// </summary>
        public string UpdateVersion
        {
            get;
            set;
        }

        /// <summary>
        /// The url of the update package.
        /// </summary>
        public string UpdatePackageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// The changelog of the update package.
        /// </summary>
        public string Changelog
        {
            get;
            set;
        }

        /// <summary>
        /// The signature of the update package.
        /// </summary>
        public string Signature
        {
            get;
            set;
        }

        /// <summary>
        /// The developmental stage of the update package.
        /// </summary>
        public string DevelopmentalStage
        {
            get;
            set;
        }
        
        /// <summary>
        /// The unsupported versions of the update package.
        /// </summary>
        public string[] UnsupportedVersions
        {
            get;
            set;
        }

        /// <summary>
        /// The architecture settings of the update package.
        /// </summary>
        public string Architecture
        {
            get;
            set;
        }

        /// <summary>
        /// The operations of the update package.
        /// </summary>
        public string[] Operations
        {
            get;
            set;
        }

        public bool MustUpdate
        {
            get;
            set;
        }
    }
}
