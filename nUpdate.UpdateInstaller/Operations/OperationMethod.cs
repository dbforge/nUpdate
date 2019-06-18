// OperationMethod.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

namespace nUpdate.UpdateInstaller.Operations
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