// Author: Dominic Beger (Trade/ProgTrade)

using System;

namespace nUpdate.Administration.Core
{
    public class UriConnector
    {
        /// <summary>
        ///     Connects to uri parts to one uri.
        /// </summary>
        /// <param name="start">The first absolute part.</param>
        /// <param name="end">The second relative part.</param>
        /// <returns>Returns the connected uri.</returns>
        public static Uri ConnectUri(string start, string end)
        {
            if (!start.EndsWith("/"))
                start += "/";
            var baseUri = new Uri(start);
            return new Uri(baseUri, end);
        }
    }
}