using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nUpdate.Administration.Core
{
    internal class UriConnecter
    {
        /// <summary>
        /// Connects to uri parts to one uri.
        /// </summary>
        /// <param name="start">The first absolute part.</param>
        /// <param name="end">The second relative part.</param>
        /// <returns>Returns the connected uri.</returns>
        public static Uri ConnectUri(string start, string end)
        {
            Uri baseUri = new Uri(start);
            Uri endUri = new Uri(baseUri, end);
            return endUri;
        }
    }
}
