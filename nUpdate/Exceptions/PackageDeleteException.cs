// PackageDeleteException.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;

namespace nUpdate.Exceptions
{
    /// <summary>
    ///     The exception that is thrown if the package deletion fails in case the signature is invalid.
    /// </summary>
    [Serializable]
    public class PackageDeleteException : Exception
    {
        public PackageDeleteException(string message)
            : base(message)
        {
        }
    }
}