// Author: Dominic Beger (Trade/ProgTrade)

using System.ComponentModel;

namespace nUpdate.Core.Operations
{
    /// <summary>
    ///     Represents the different areas in which operations can take place.
    /// </summary>
    public enum OperationArea
    {
        [Description("NewUpdateDialogDemandsFilesAccessText")] Files,
        [Description("NewUpdateDialogDemandsRegistryAccessText")] Registry,
        [Description("NewUpdateDialogDemandsProcessesAccessText")] Processes,
        [Description("NewUpdateDialogDemandsServicesAccessText")] Services
    }
}