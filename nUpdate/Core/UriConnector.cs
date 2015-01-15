// Author: Dominic Beger (Trade/ProgTrade)

using System;

namespace nUpdate.Core
{
    public class UriConnector
    {
        public static Uri ConnectUri(string start, string end)
        {
            if (!Uri.IsWellFormedUriString(start, UriKind.RelativeOrAbsolute))
                return null;

            var baseUri = new Uri(start);
            var endUri = new Uri(baseUri, end);
            return endUri;
        }
    }
}