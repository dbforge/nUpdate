// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Runtime.Serialization;

namespace nUpdate.Administration.Core.Ftp.Exceptions
{
    /// <summary>
    ///     This exception is thrown when the server fails to respond to an FTP command in a timely manner.
    ///     The waiting time can be adjusted by specifing a different value for the CommandTimeout property.
    /// </summary>
    [Serializable]
    public class FtpCommandResponseTimeoutException : FtpException
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public FtpCommandResponseTimeoutException()
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        public FtpCommandResponseTimeoutException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="innerException">The inner exception object.</param>
        public FtpCommandResponseTimeoutException(string message, Exception innerException)
            :
                base(message, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Stream context information.</param>
        protected FtpCommandResponseTimeoutException(SerializationInfo info,
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
        public FtpCommandResponseTimeoutException(string message, FtpResponse response, Exception innerException)
            :
                base(message, response, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="response">Ftp response object.</param>
        public FtpCommandResponseTimeoutException(string message, FtpResponse response)
            : base(message, response)
        {
        }
    }
}