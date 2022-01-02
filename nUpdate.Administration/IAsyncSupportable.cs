// IAsyncSupportable.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

namespace nUpdate.Administration
{
    /// <summary>
    ///     Offers methods that are necessary for asynchronous operations.
    /// </summary>
    public interface IAsyncSupportable
    {
        void SetUiState(bool enabled);
    }
}