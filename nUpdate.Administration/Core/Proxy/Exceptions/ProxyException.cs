// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Runtime.Serialization;

namespace nUpdate.Administration.Core.Proxy.Exceptions
{
    /// <summary>
    ///     This exception is thrown when a general, unexpected proxy error.
    /// </summary>
    [Serializable]
    public class ProxyException : Exception
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public ProxyException()
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        public ProxyException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="innerException">The inner exception object.</param>
        public ProxyException(string message, Exception innerException)
            :
                base(message, innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Stream context information.</param>
        protected ProxyException(SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}