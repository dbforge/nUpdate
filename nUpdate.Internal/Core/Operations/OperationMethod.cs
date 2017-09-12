// Copyright © Dominic Beger 2017

namespace nUpdate.Internal.Core.Operations
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
        Stop,
        Execute
    }
}