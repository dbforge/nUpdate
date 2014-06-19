using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nUpdate.Internal.UpdateEventArgs
{
    public class FailedEventArgs : System.EventArgs
    {
        public FailedEventArgs(Exception exception)
        { }
    }
}
