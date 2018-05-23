using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using nUpdate.Internal.Core;

namespace nUpdate.Updating
{
    // WITHOUT TAP
    public partial class UpdateConfiguration
    {
        /// <summary>
        ///     Downloads the update configurations from the server.
        /// </summary>
        /// <param name="configFileUri">The url of the configuration file.</param>
        /// <param name="proxy">The optional proxy to use.</param>
        /// <param name="finishedCallback">The <see cref="Action"/> to invoke when the operation has finished.</param>
        /// <param name="cancellationTokenSource">The optional <see cref="CancellationTokenSource"/> to use for canceling the operation.</param>
        /// <param name="timeout">The timeout for the download request. In milliseconds. Default 10000.</param>
        /// <returns>Returns an <see cref="IEnumerable{UpdateConfiguration}" /> containing the package configurations.</returns>
        public static void DownloadAsync(Uri configFileUri, WebProxy proxy, Action<IEnumerable<UpdateConfiguration>, Exception> finishedCallback, CancellationTokenSource cancellationTokenSource = null, int timeout = 10000)
        {
            DownloadAsync(configFileUri, null, proxy, finishedCallback, cancellationTokenSource, timeout);
        }

        /// <summary>
        ///     Downloads the update configurations from the server.
        /// </summary>
        /// <param name="configFileUri">The url of the configuration file.</param>
        /// <param name="credentials">The HTTP authentication credentials.</param>
        /// <param name="proxy">The optional proxy to use.</param>
        /// <param name="finishedCallback">The <see cref="Action"/> to invoke when the operation has finished.</param>
        /// <param name="cancellationTokenSource">The optional <see cref="CancellationTokenSource"/> to use for canceling the operation.</param>
        /// <param name="timeout">The timeout for the download request. In milliseconds. Default 10000.</param>
        /// <returns>Returns an <see cref="IEnumerable{UpdateConfiguration}" /> containing the package configurations.</returns>
        public static void DownloadAsync(Uri configFileUri, NetworkCredential credentials,
            WebProxy proxy, Action<IEnumerable<UpdateConfiguration>, Exception> finishedCallback, CancellationTokenSource cancellationTokenSource = null, int timeout = 10000)
        {
            using (var wc = new WebClientWrapper(timeout))
            {
                var resetEvent = new ManualResetEvent(false);
                string source = null;
                Exception exception = null;

                wc.Encoding = Encoding.UTF8;
                if (credentials != null)
                    wc.Credentials = credentials;

                if (proxy != null)
                    wc.Proxy = proxy;

                wc.DownloadStringCompleted += (sender, args) =>
                {
                    if (!args.Cancelled)
                    {
                        if (args.Error != null)
                            exception = args.Error;
                        else
                            source = args.Result;
                    }

                    resetEvent.Set();
                    resetEvent.Dispose();
                };

                // Register the cancel async method of the webclient to be called
                cancellationTokenSource?.Token.Register(wc.CancelAsync);

                // Check for SSL and ignore it
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

                wc.DownloadStringAsync(configFileUri);
                resetEvent.WaitOne();
                finishedCallback?.Invoke(!string.IsNullOrEmpty(source)
                    ? Serializer.Deserialize<IEnumerable<UpdateConfiguration>>(source)
                    : Enumerable.Empty<UpdateConfiguration>(), exception);
            }
        }
    }
}
