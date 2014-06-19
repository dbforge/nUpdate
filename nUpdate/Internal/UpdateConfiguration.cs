using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using nUpdate.Core;

namespace nUpdate.Internal
{
    internal class UpdateConfiguration
    {
        /// <summary>
        /// Loads the update configuration from the server.
        /// </summary>
        /// <param name="infoFileUrl">The url of the info file.</param>
        /// <returns>Returns a deserialized stack of type <see cref="UpdatePackage"/>.</returns>
        public Stack<UpdateConfiguration> LoadUpdateConfiguration(Uri infoFileUrl)
        {
            var wc = new WebClient();

            // Cheeck for SSL and ignore it
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
                return Serializer.Deserialize<Stack<UpdateConfiguration>>(input);
        }

        /// <summary>
        /// Loads the update configuration from the server.
        /// </summary>
        /// <param name="infoFileUrl">The url of the info file.</param>
        /// <param name="proxySettings">The proxy settings for the server.</param>
        /// <returns>Returns a deserialized stack of type <see cref="UpdatePackage"/>.</returns>
        public Stack<UpdateConfiguration> LoadUpdateConfiguration(Uri infoFileUrl, ProxySettings proxySettings)
        {
            var wc = new WebClient();

            // Cheeck for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate(
            Object obj, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
            {
                return (true);
            };

            // Set the proxy
            wc.Proxy = new WebProxy(proxySettings.Host, proxySettings.Port);
            wc.Proxy.Credentials = new NetworkCredential(proxySettings.Username, proxySettings.Password);

            return Serializer.Deserialize<Stack<UpdateConfiguration>>(wc.DownloadString(infoFileUrl));
        }

        public string UpdateVersion
        {
            get;
            set;
        }

        public string UpdatePackageUrl
        {
            get;
            set;
        }

        public string Changelog
        {
            get;
            set;
        }

        public string Signature
        {
            get;
            set;
        }

        public string DevelopmentalStage
        {
            get;
            set;
        }

        public string Environment
        {
            get;
            set;
        }

        public string[] UnsupportedVersions
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
