// OperationArea.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.ComponentModel;

namespace nUpdate.Operations
{
    /// <summary>
    ///     Represents the different areas in which operations can take place.
    /// </summary>
    public enum OperationArea
    {
        [Description("NewUpdateDialogFilesAccessText")]
        Files,

        [Description("NewUpdateDialogRegistryAccessText")]
        Registry,

        [Description("NewUpdateDialogProcessesAccessText")]
        Processes,

        [Description("NewUpdateDialogServicesAccessText")]
        Services,

        [Description("NewUpdateDialogCodeExecutionAccessText")]
        Scripts
    }
}