// Author: Dominic Beger (Trade/ProgTrade)

namespace nUpdate.UpdateInstaller.Core.Operations
{
    /// <summary>
    ///     Represents the different methods of the operations performed in different areas.
    /// </summary>
    public enum OperationMethod
    {
        Create,
        Delete,
        Rename,
        SetValue,
        DeleteValue,
        Start,
        Stop
    }
}