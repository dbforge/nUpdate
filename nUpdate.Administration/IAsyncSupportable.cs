// Author: Dominic Beger (Trade/ProgTrade)

namespace nUpdate.Administration
{
    /// <summary>
    ///     Offers methods that are necessary for asynchronous operations.
    /// </summary>
    public interface IAsyncSupportable
    {
        void SetUIState(bool enabled);
    }
}