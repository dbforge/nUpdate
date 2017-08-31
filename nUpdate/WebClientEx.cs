// Copyright © Dominic Beger 2017

using System;
using System.Net;

namespace nUpdate
{
    public class WebClientEx : WebClient
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WebClientEx" />-class.
        /// </summary>
        public WebClientEx() : this(5000)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebClientEx" />-class.
        /// </summary>
        /// <param name="timeout">The timeout to use.</param>
        public WebClientEx(int timeout)
        {
            Timeout = timeout;
        }

        /// <summary>
        ///     The timeout of the request.
        /// </summary>
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request != null)
                request.Timeout = Timeout;

            return request;
        }
    }
}