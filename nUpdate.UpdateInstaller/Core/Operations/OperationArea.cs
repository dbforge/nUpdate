// Copyright © Dominic Beger 2018

namespace nUpdate.UpdateInstaller.Core.Operations
{
    /// <summary>
    ///     Represents the different areas in which operations can take place.
    /// </summary>
    public enum OperationArea
    {
        Files,
        Registry,
        Processes,
        Services,
        Scripts
    }
}