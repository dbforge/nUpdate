using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nUpdate.Administration.Core.Update.Operations
{
    /// <summary>
    /// Represents the different methods of the operations performed in different areas.
    /// </summary>
    internal enum OperationMethods
    {
        Create,
        Delete,
        Rename,
        SetValue,
        Start,
        Stop,
    }
}
