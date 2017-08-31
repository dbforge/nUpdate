// Copyright © Dominic Beger 2017

using System.Collections.Generic;
using System.Linq;

namespace nUpdate
{
    /// <summary>
    ///     Represents the result of an update package validation.
    /// </summary>
    public class ValidationResult
    {
        internal ValidationResult(int packageAmount)
        {
            InvalidPackages = new List<UpdatePackage>(packageAmount);
        }

        /// <summary>
        ///     Gets a value indicating whether all update packages, that have been checked, are valid, or not.
        /// </summary>
        public bool AreValid => !InvalidPackages.Any();

        /// <summary>
        ///     Gets the invalid packages that have been found.
        /// </summary>
        public List<UpdatePackage> InvalidPackages { get; }
    }
}