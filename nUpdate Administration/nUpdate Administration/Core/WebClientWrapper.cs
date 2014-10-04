// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;
using System.Net;

namespace nUpdate.Administration.Core
{
    internal class WebClientWrapper : WebClient
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WebClientWrapper" />-class.
        /// </summary>
        public WebClientWrapper() : this(10000)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebClientWrapper" />-class.
        /// </summary>
        /// <param name="timeout">The timeout to use.</param>
        public WebClientWrapper(int timeout)
        {
            Timeout = timeout;
        }

        /// <summary>
        ///     The timeout of the request.
        /// </summary>
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request != null)
                request.Timeout = Timeout;
            return request;
        }
    }
}