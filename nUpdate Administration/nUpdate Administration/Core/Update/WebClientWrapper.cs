using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace nUpdate.Administration.Core.Update
{
    internal class WebClientWrapper : WebClient
    {
        /// <summary>
        /// The timeout of the request.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebClientWrapper"/>-class.
        /// </summary>
        public WebClientWrapper() : this(5000) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebClientWrapper"/>-class.
        /// </summary>
        /// <param name="timeout">The timeout to use.</param>
        public WebClientWrapper(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }
            return request;
        }
    }
}
