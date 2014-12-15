// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 20-09-2014 23:10
namespace nUpdate.Administration.Core.Update.Operations.Panels
{
    public interface IOperationPanel
    {
        /// <summary>
        /// Returns the current operation to perform.
        /// </summary>
        Operation Operation { get; }
    }
}