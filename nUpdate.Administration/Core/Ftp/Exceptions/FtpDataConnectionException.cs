// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Runtime.Serialization;

namespace nUpdate.Administration.Core.Ftp.Exceptions
{
    /// <summary>
    ///     This exception is thrown when the FTP client is unable to establish a data connection with the FTP server.
    ///     Data connection are temporary, secondary connnections used to transfer files and other types of data between the
    ///     FTP client and the FTP server.  The method in which data connections are established is determined by the type
    ///     of data transfer mode specified when connection to an FTP server (e.g. Passive, Active)
    /// </summary>
    [Serializable]
    public class FtpDataConnectionException : FtpConnectionClosedException
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public FtpDataConnectionException()
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        public FtpDataConnectionException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="innerException">The inner exception object.</param>
        public FtpDataConnectionException(string message, Exception innerException)
            :
                base(message, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Stream context information.</param>
        protected FtpDataConnectionException(SerializationInfo info,
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
        public FtpDataConnectionException(string message, FtpResponse response, Exception innerException)
            : base(message, response, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="response">Ftp response object.</param>
        public FtpDataConnectionException(string message, FtpResponse response)
            : base(message, response)
        {
        }
    }
}