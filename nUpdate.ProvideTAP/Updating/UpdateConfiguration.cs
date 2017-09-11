// Copyright © Dominic Beger 2017

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Internal.Core;

namespace nUpdate.Updating
{
    // PROVIDE TAP
    public partial class UpdateConfiguration
    {
        /// <summary>
        ///     Downloads the update configurations from the server.
        /// </summary>
        /// <param name="configFileUri">The url of the configuration file.</param>
        /// <param name="proxy">The optional proxy to use.</param>
        /// <param name="cancellationTokenSource">
        ///     The optional <see cref="CancellationTokenSource" /> to use for canceling the
        ///     operation.
        /// </param>
        /// <returns>Returns an <see cref="IEnumerable{UpdateConfiguration}" /> containing the package configurations.</returns>
        public static Task<IEnumerable<UpdateConfiguration>> DownloadAsync(Uri configFileUri, WebProxy proxy,
            CancellationTokenSource cancellationTokenSource = null)
        {
            return DownloadAsync(configFileUri, null, proxy, cancellationTokenSource);
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
        /// <returns>Returns an <see cref="IEnumerable{UpdateConfiguration}" /> containing the package configurations.</returns>
        public static async Task<IEnumerable<UpdateConfiguration>> DownloadAsync(Uri configFileUri,
            NetworkCredential credentials,
            WebProxy proxy, CancellationTokenSource cancellationTokenSource = null)
        {
            // Check for SSL and ignore it
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            
            var request = (HttpWebRequest) WebRequest.Create(configFileUri);
            request.Timeout = 10000;

            if (credentials != null)
                request.Credentials = credentials;
            if (proxy != null)
                request.Proxy = proxy;

            string source;
            var response = cancellationTokenSource != null ?
                await request.GetResponseAsync(cancellationTokenSource.Token) 
                : (HttpWebResponse)await request.GetResponseAsync();

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
    }
}