using System;
using System.Net;
using System.Security;

namespace nUpdate
{
    public class WebRequestWrapper
    {
        /// <summary>
        ///     Initializes a new WebRequest instance for the specified URI scheme.
        /// </summary>
        /// <param name="requestUri">A Uri containing the URI of the requested resource.</param>
        /// <returns>A WebRequest descendant for the specified URI scheme.</returns>
        /// <exception cref="NotSupportedException">The request scheme specified in requestUri is not registered.</exception>
        /// <exception cref="ArgumentNullException">requestUri is null.</exception>
        /// <exception cref="SecurityException">The caller does not have WebPermissionAttribute permission to connect to the requested URI or a URI that the request is redirected to.</exception>
        public static WebRequest Create(Uri requestUri)
        {
            var request = (HttpWebRequest) WebRequest.Create(requestUri);
            request.UserAgent = HttpHeader.GetUserAgent();

            return request;
        }
    }
}