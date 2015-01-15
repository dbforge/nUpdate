// Author: Dominic Beger (Trade/ProgTrade)

namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    public interface IOperationPanel
    {
        /// <summary>
        ///     Returns the current operation to perform.
        /// </summary>
        Operation Operation { get; }
    }
}