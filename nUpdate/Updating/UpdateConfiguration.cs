// UpdateConfiguration.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Operations;

namespace nUpdate.Updating
{
    [Serializable]
    public class UpdateConfiguration
    {
        /// <summary>
        ///     The architecture settings of the update package.
        /// </summary>
        public Architecture Architecture { get; set; }

        /// <summary>
        ///     The whole changelog of the update package.
        /// </summary>
        public Dictionary<CultureInfo, string> Changelog { get; set; }

        /// <summary>
        ///     The literal version of the package.
        /// </summary>
        public string LiteralVersion { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the update package should be favored over other packages, even if they have
        ///     a higher <see cref="UpdateVersion" />.
        /// </summary>
        public bool NecessaryUpdate { get; set; }

        /// <summary>
        ///     Gets or sets the rollout condition mode used for selecting the updates.
        /// </summary>
        public RolloutConditionMode RolloutConditionMode { get; set; }

        /// <summary>
        ///     Gets or sets the rollout conditions which contain additional specifications that the client must meet in order to
        ///     receive an update.
        /// </summary>
        public List<RolloutCondition> RolloutConditions { get; set; }

        public List<Operation> Operations { get; set; }

        /// <summary>
        ///     The signature of the update package (Base64 encoded).
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        ///     The unsupported versions of the update package.
        /// </summary>
        public string[] UnsupportedVersions { get; set; }

        /// <summary>
        ///     The URI of the update package.
        /// </summary>
        public Uri UpdatePackageUri { get; set; }

        /// <summary>
        ///     The URI of the PHP-file which does the statistic entries.
        /// </summary>
        public Uri UpdatePhpFileUri { get; set; }

        /// <summary>
        ///     Sets if the package should be used within the statistics.
        /// </summary>
        public bool UseStatistics { get; set; }

        /// <summary>
        ///     The version ID of this package to use in the statistics, if used.
        /// </summary>
        public int VersionId { get; set; }

        /// <summary>
        ///     Downloads the update configurations from the server.
        /// </summary>
        /// <param name="configFileUri">The url of the configuration file.</param>
        /// <param name="proxy">The optional proxy to use.</param>
        /// <param name="timeout">The timeout for the download request. In milliseconds. Default 10000.</param>
        /// <returns>Returns an <see cref="IEnumerable{UpdateConfiguration}" /> containing the package configurations.</returns>
        public static IEnumerable<UpdateConfiguration> Download(Uri configFileUri, WebProxy proxy, int timeout = 10000)
        {
            return Download(configFileUri, null, proxy, timeout);
        }

        /// <summary>
        ///     Downloads the update configurations from the server.
        /// </summary>
        /// <param name="configFileUri">The url of the configuration file.</param>
        /// <param name="credentials">The HTTP authentication credentials.</param>
        /// <param name="proxy">The optional proxy to use.</param>
        /// <param name="timeout">The timeout for the download request. In milliseconds. Default 10000.</param>
        /// <returns>Returns an <see cref="IEnumerable{UpdateConfiguration}" /> containing the package configurations.</returns>
        public static IEnumerable<UpdateConfiguration> Download(Uri configFileUri, NetworkCredential credentials,
            WebProxy proxy, int timeout = 10000)
        {
            using (var wc = new WebClientWrapper(timeout))
            {
                wc.Encoding = Encoding.UTF8;
                if (credentials != null)
                    wc.Credentials = credentials;

                if (proxy != null)
                    wc.Proxy = proxy;

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                var source = wc.DownloadString(configFileUri);
                if (!string.IsNullOrEmpty(source))
                    return Serializer.Deserialize<IEnumerable<UpdateConfiguration>>(source);
            }

            return Enumerable.Empty<UpdateConfiguration>();
        }

        /// <summary>
        ///     Downloads the update configurations from the server.
        /// </summary>
        /// <param name="configFileUri">The url of the configuration file.</param>
        /// <param name="proxy">The optional proxy to use.</param>
        /// <param name="cancellationTokenSource">
        ///     The optional <see cref="CancellationTokenSource" /> to use for canceling the
        ///     operation.
        /// </param>
        /// <param name="timeout">The timeout for the download request. In milliseconds. Default 10000.</param>
        /// <returns>Returns an <see cref="IEnumerable{UpdateConfiguration}" /> containing the package configurations.</returns>
        public static Task<IEnumerable<UpdateConfiguration>> DownloadAsync(Uri configFileUri, WebProxy proxy,
            CancellationTokenSource cancellationTokenSource = null, int timeout = 10000)
        {
            return DownloadAsync(configFileUri, null, proxy, cancellationTokenSource, timeout);
        }

        /// <summary>
        ///     Downloads the update configurations from the server.
        /// </summary>
        /// <param name="configFileUri">The url of the configuration file.</param>
        /// <param name="credentials">The HTTP authentication credentials.</param>
        /// <param name="proxy">The optional proxy to use.</param>
        /// <param name="cancellationTokenSource">
        ///     The optional <see cref="CancellationTokenSource" /> to use for canceling the
        ///     operation.
        /// </param>
        /// <param name="timeout">The timeout for the download request. In milliseconds. Default 10000.</param>
        /// <returns>Returns an <see cref="IEnumerable{UpdateConfiguration}" /> containing the package configurations.</returns>
        public static async Task<IEnumerable<UpdateConfiguration>> DownloadAsync(Uri configFileUri,
            NetworkCredential credentials,
            WebProxy proxy, CancellationTokenSource cancellationTokenSource = null, int timeout = 10000)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            var request = (HttpWebRequest)WebRequestWrapper.Create(configFileUri);
            request.Timeout = timeout;

            if (credentials != null)
                request.Credentials = credentials;
            if (proxy != null)
                request.Proxy = proxy;

            string source;
            var response = await request.GetResponseAsync();

            using (var sr = new StreamReader(response.GetResponseStream() ??
                                             throw new InvalidOperationException(
                                                 "The response stream of the configuration file web request is invalid."))
            )
            {
                source = await sr.ReadToEndAsync();
            }

            return !string.IsNullOrEmpty(source)
                ? Serializer.Deserialize<IEnumerable<UpdateConfiguration>>(source)
                : Enumerable.Empty<UpdateConfiguration>();
        }

        /// <summary>
        ///     Loads an update configuration from a local file.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        public static IEnumerable<UpdateConfiguration> FromFile(string filePath)
        {
            return Serializer.Deserialize<IEnumerable<UpdateConfiguration>>(File.ReadAllText(filePath)) ??
                   Enumerable.Empty<UpdateConfiguration>();
        }
    }
}