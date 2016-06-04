﻿// Author: Dominic Beger (Trade/ProgTrade)

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
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request != null)
            {
                //request.ServicePoint.ConnectionLimit = 5;
                request.Timeout = Timeout;
            }

            return request;
        }
    }
}