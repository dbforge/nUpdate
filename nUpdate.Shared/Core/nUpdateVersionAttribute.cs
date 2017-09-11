// Copyright © Dominic Beger 2017

using System;

namespace nUpdate.Core
{
    /// <summary>
    ///     Represents an attribute that will be used to determine the current version in the constructor of the
    ///     <see cref="UpdateManager" /> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    // ReSharper disable once InconsistentNaming
    public sealed class nUpdateVersionAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="nUpdateVersionAttribute" /> class.
        /// </summary>
        /// <param name="nUpdateVersionString">The version string to use.</param>
        /// <remarks>The version string will be represented in an instance of the <see cref="UpdateVersion" /> class.</remarks>
        public nUpdateVersionAttribute(string nUpdateVersionString)
        {
            VersionString = nUpdateVersionString;
        }

        /// <summary>
        ///     Gets the version string.
        /// </summary>
        public string VersionString { get; }
    }
}