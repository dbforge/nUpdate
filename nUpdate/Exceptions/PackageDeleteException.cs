using System;

namespace nUpdate.Exceptions
{
    /// <summary>
    ///     The exception that is thrown if the package deletion fails.
    /// </summary>
    [Serializable]
    public class PackageDeleteException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PackageDeleteException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PackageDeleteException(string message)
            : base(message)
        {
        }
    }
}