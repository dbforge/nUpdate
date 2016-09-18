// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Runtime.Serialization;

namespace nUpdate.Administration.Core.Ftp.Exceptions
{
    /// <summary>
    ///     This exception is thrown when a file integrity check fails.
    ///     For detailed information about the error, the FTP server response
    ///     can be inspected via the Reponse property on this exception.
    /// </summary>
    [Serializable]
    public class FtpResponseException : FtpException
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public FtpResponseException()
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="response">Ftp response object.</param>
        public FtpResponseException(string message, FtpResponse response)
            : base(message, response)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="response">Ftp response object.</param>
        /// <param name="message">Exception message text.</param>
        /// <param name="innerException">The inner exception object.</param>
        public FtpResponseException(string message, FtpResponse response, Exception innerException)
            :
                base(message, response, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Stream context information.</param>
        protected FtpResponseException(SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}