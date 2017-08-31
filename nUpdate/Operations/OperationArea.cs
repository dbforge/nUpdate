// Copyright © Dominic Beger 2017

using System.ComponentModel;

namespace nUpdate.Operations
{
    /// <summary>
    ///     Represents the different areas in which operations can take place.
    /// </summary>
    public enum OperationArea
    {
        [Description("NewUpdateDialogFilesAccessText")] Files,
        [Description("NewUpdateDialogRegistryAccessText")] Registry,
        [Description("NewUpdateDialogProcessesAccessText")] Processes,
        [Description("NewUpdateDialogServicesAccessText")] Services,
        [Description("NewUpdateDialogCodeExecutionAccessText")] Scripts
    }
}