using System;

namespace nUpdate.Core
{
    public class UriConnecter
    {
        public Uri ConnectUri(string start, string end)
        {
            if (!Uri.IsWellFormedUriString(start, UriKind.RelativeOrAbsolute))
            {
                return null;
            }

            Uri baseUri = new Uri(start);
            Uri endUri = new Uri(baseUri, end);
            return endUri;
        }
    }
}
