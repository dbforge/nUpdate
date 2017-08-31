// Copyright © Dominic Beger 2017

using System;

namespace nUpdate
{
    /// <summary>
    ///     Represents an attribute that will be used to determine the current version of the application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    // ReSharper disable once InconsistentNaming
    public sealed class nUpdateVersionAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="nUpdateVersionAttribute" /> class.
        /// </summary>
        /// <param name="nUpdateVersionString">
        ///     The version string to use. All string representations of
        ///     <see cref="UpdateVersion" /> can be used.
        /// </param>
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