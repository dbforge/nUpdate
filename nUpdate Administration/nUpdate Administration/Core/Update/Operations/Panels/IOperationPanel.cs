// Author: Dominic Beger (Trade/ProgTrade)

namespace nUpdate.Administration.Core.Update.Operations.Panels
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