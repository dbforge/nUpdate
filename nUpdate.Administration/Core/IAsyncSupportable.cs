// Copyright © Dominic Beger 2018

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