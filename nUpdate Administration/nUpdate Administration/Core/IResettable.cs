// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 30-10-2014 21:35
namespace nUpdate.Administration.Core
{
    /// <summary>
    ///     Offers methods for resetting data when asynchronous operations failed.
    /// </summary>
    public interface IResettable
    {
        void Reset();
    }
}