// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Runtime.Serialization;

namespace nUpdate.Administration.Core.Ftp.Exceptions
{
    /// <summary>
    ///     This exception is thrown when a ftp connection is broken and fails.
    /// </summary>
    [Serializable]
    public class FtpConnectionBrokenException : FtpException
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public FtpConnectionBrokenException()
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        public FtpConnectionBrokenException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="innerException">The inner exception object.</param>
        public FtpConnectionBrokenException(string message, Exception innerException)
            :
                base(message, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Stream context information.</param>
        protected FtpConnectionBrokenException(SerializationInfo info,
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
        public FtpConnectionBrokenException(string message, FtpResponse response, Exception innerException)
            :
                base(message, response, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="response">Ftp response object.</param>
        public FtpConnectionBrokenException(string message, FtpResponse response)
            : base(message, response)
        {
        }
    }
}