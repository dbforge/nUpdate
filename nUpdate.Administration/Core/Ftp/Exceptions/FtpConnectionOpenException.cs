// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Runtime.Serialization;

namespace nUpdate.Administration.Core.Ftp.Exceptions
{
    /// <summary>
    ///     This exception is thrown when an error occurs while attempt to open a command connection
    ///     to the FTP server.
    /// </summary>
    [Serializable]
    public class FtpConnectionOpenException : FtpException
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public FtpConnectionOpenException()
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        public FtpConnectionOpenException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="innerException">The inner exception object.</param>
        public FtpConnectionOpenException(string message, Exception innerException)
            :
                base(message, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Stream context information.</param>
        protected FtpConnectionOpenException(SerializationInfo info,
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
        public FtpConnectionOpenException(string message, FtpResponse response, Exception innerException)
            : base(message, response, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="response">Ftp response object.</param>
        public FtpConnectionOpenException(string message, FtpResponse response)
            : base(message, response)
        {
        }
    }
}