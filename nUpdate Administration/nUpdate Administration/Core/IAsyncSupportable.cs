// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 30-10-2014 21:37
namespace nUpdate.Administration.Core
{
    /// <summary>
    ///     Offers methods that are necessary for asynchronous operations.
    /// </summary>
    public interface IAsyncSupportable
    {
        void SetUiState(bool enabled);
    }
}