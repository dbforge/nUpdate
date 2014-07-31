using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nUpdate.Administration.Core.Update.Operations
{
    /// <summary>
    /// Represents the different areas in which operations can take place.
    /// </summary>
    internal enum OperationArea
    {
        Files,
        Registry,
        Processes,
        Services,
    }
}
