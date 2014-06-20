using System.Security;

namespace nUpdate.Internal
{
    public class ProxySettings
    {
        /// <summary>
        /// Host for the proxy.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port for the proxy.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Username for the proxy.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for the proxy.
        /// </summary>
        public SecureString Password { get; set; }
    }
}
