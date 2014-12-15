using System;

namespace nUpdate.Internal.UpdateEventArgs
{
    public class FailedEventArgs : EventArgs
    {
        public FailedEventArgs(Exception exception)
        {
        }
    }
}