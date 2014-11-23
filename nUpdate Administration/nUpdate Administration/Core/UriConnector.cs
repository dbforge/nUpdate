// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
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
            var baseUri = new Uri(start);
            var endUri = new Uri(baseUri, end);
            return endUri;
        }
    }
}