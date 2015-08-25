// Author: Dominic Beger (Trade/ProgTrade)

namespace nUpdate.Administration
{
    /// <summary>
    ///     Offers methods for resetting data when asynchronous operations failed.
    /// </summary>
    public interface IResettable
    {
        void Reset();
    }
}