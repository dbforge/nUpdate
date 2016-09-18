// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace nUpdate.Administration.Core.Ftp.EventArgs
{
    /// <summary>
    ///     Event arguments to facilitate the FtpClient transfer progress and complete events.
    /// </summary>
    public class ValidateServerCertificateEventArgs : System.EventArgs
    {
        /// <summary>
        ///     ValidateServerCertificateEventArgs constructor.
        /// </summary>
        /// <param name="certificate">X.509 certificate object.</param>
        /// <param name="chain">X.509 certificate chain.</param>
        /// <param name="policyErrors">SSL policy errors.</param>
        public ValidateServerCertificateEventArgs(X509Certificate2 certificate, X509Chain chain,
            SslPolicyErrors policyErrors)
        {
            Certificate = certificate;
            Chain = chain;
            PolicyErrors = policyErrors;
        }

        /// <summary>
        ///     The X.509 version 3 server certificate.
        /// </summary>
        public X509Certificate2 Certificate { get; }

        /// <summary>
        ///     Server chain building engine for server certificate.
        /// </summary>
        public X509Chain Chain { get; }

        /// <summary>
        ///     Enumeration representing SSL (Secure Socket Layer) errors.
        /// </summary>
        public SslPolicyErrors PolicyErrors { get; }

        /// <summary>
        ///     Boolean value indicating if the server certificate is valid and can
        ///     be accepted by the FtpClient object.
        /// </summary>
        /// <remarks>
        ///     Default value is false which results in certificate being rejected and the SSL
        ///     connection abandoned.  Set this value to true to accept the server certificate
        ///     otherwise the SSL connection will be closed.
        /// </remarks>
        public bool IsCertificateValid { get; set; }
    }
}