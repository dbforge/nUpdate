// VerificationResult.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Collections.Generic;
using System.Linq;

namespace nUpdate
{
    /// <summary>
    ///     Represents the result of an update package verification.
    /// </summary>
    public class VerificationResult
    {
        internal VerificationResult(int packageAmount)
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