using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace nUpdate.Administration.Core.Update
{
    internal class ProxySettings
    {
        /// <summary>
        /// Sets the host for the proxy.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Sets the port for the proxy.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Sets the username for the proxy.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Sets the password for the proxy.
        /// </summary>
        public SecureString Password { get; set; }
    }
}
