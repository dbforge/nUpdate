// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Runtime.Serialization;

namespace nUpdate.Administration.Core.Ftp.Exceptions
{
    /// <summary>
    ///     This exception is thrown when a general FTP exception occurs.
    /// </summary>
    [Serializable]
    public class FtpException : Exception
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public FtpException()
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        public FtpException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="innerException">The inner exception object.</param>
        public FtpException(string message, Exception innerException)
            :
                base(message, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="response">Response object.</param>
        /// <param name="innerException">The inner exception object.</param>
        public FtpException(string message, FtpResponse response, Exception innerException)
            :
                base(message, innerException)
        {
            LastResponse = response;
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="response">Ftp response object.</param>
        public FtpException(string message, FtpResponse response)
            : base(message)
        {
            LastResponse = response;
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Stream context information.</param>
        protected FtpException(SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        ///     Gets the last FTP response if one is available.
        /// </summary>
        public FtpResponse LastResponse { get; } = new FtpResponse();

        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context
            )
        {
            base.GetObjectData(info, context);
        }
    }
}