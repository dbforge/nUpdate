// Copyright © Dominic Beger 2017

using System;

namespace nUpdate.Internal.Core
{
    public class UriConnector
    {
        public static Uri ConnectUri(string start, string end)
        {
            if (!Uri.IsWellFormedUriString(start, UriKind.RelativeOrAbsolute))
                return null;

            if (!start.EndsWith("/"))
                start += "/";
            var baseUri = new Uri(start);
            return new Uri(baseUri, end);
        }
    }
}