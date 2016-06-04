// Author: Dominic Beger (Trade/ProgTrade)

using System.Net;

namespace nUpdate.Administration
{
    internal struct ProxyData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProxyData"/> struct.
        /// </summary>
        /// <param name="proxy">The <see cref="WebProxy"/>.</param>
        /// <param name="proxyUsername">The <see cref="WebProxy"/> username, if necessary.</param>
        /// <param name="proxyPassword">The <see cref="WebProxy"/> password, if necessary.</param>
        internal ProxyData(WebProxy proxy, string proxyUsername, string proxyPassword)
        {
            Proxy = proxy;
            ProxyUsername = proxyUsername;
            ProxyPassword = proxyPassword;
        }

        /// <summary>
        ///     Gets or sets the <see cref="WebProxy"/> to use for data transfers.
        /// </summary>
        /// <remarks>If this value is <c>null</c>, no <see cref="WebProxy"/> will be used.</remarks>
        internal WebProxy Proxy { get; set; }

        /// <summary>
        ///     Gets or sets the username to use, if necessary.
        /// </summary>
        internal string ProxyUsername { get; set; }

        /// <summary>
        ///     Gets or sets the password to use, if necessary. (Base64-encoded)
        /// </summary>
        internal string ProxyPassword { get; set; }
    }
}