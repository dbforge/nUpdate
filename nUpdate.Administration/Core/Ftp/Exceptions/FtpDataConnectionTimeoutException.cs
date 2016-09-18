// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Runtime.Serialization;

namespace nUpdate.Administration.Core.Ftp.Exceptions
{
    /// <summary>
    ///     This exception is thrown when the server fails to respond to an FTP data connection in a timely manner.
    ///     The waiting time can be adjusted by specifing a different value for the TransferTimeout property.
    /// </summary>
    [Serializable]
    public class FtpDataConnectionTimeoutException : FtpDataConnectionException
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public FtpDataConnectionTimeoutException()
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        public FtpDataConnectionTimeoutException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="innerException">The inner exception object.</param>
        public FtpDataConnectionTimeoutException(string message, Exception innerException)
            :
                base(message, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Stream context information.</param>
        protected FtpDataConnectionTimeoutException(SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="response">Ftp response object.</param>
        /// <param name="innerException">The inner exception object.</param>
        public FtpDataConnectionTimeoutException(string message, FtpResponse response, Exception innerException)
            : base(message, response, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="response">Ftp response object.</param>
        public FtpDataConnectionTimeoutException(string message, FtpResponse response)
            : base(message, response)
        {
        }
    }
}