// WebClientWrapper.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Net;

namespace nUpdate
{
    public class WebClientWrapper : WebClient
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WebClientWrapper" />-class.
        /// </summary>
        public WebClientWrapper() : this(5000)
        {
            SetUserAgent();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebClientWrapper" />-class.
        /// </summary>
        /// <param name="timeout">The timeout to use.</param>
        public WebClientWrapper(int timeout)
        {
            Timeout = timeout;
            SetUserAgent();
        }

        private void SetUserAgent()
        {
            Headers.Add("User-Agent", HttpHeader.GetUserAgent());
        }

        /// <summary>
        ///     The timeout of the request.
        /// </summary>
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
                request.Timeout = Timeout;
            return request;
        }
    }
}