// Author: Dominic Beger (Trade/ProgTrade)

using nUpdate.Operations;

namespace nUpdate.Administration.Operations.Panels
{
    public interface IOperationPanel
    {
        /// <summary>
        ///     Gets the current operation to perform.
        /// </summary>
        Operation Operation { get; }

        /// <summary>
        ///     Gets a value indicating whether the operation is valid or not.
        /// </summary>
        bool IsValid { get; }
    }
}