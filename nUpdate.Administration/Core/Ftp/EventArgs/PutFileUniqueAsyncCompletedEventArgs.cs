// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;

namespace nUpdate.Administration.Core.Ftp.EventArgs
{
    public class PutFileUniqueAsyncCompletedEventArgs : System.EventArgs
    {
        public PutFileUniqueAsyncCompletedEventArgs(Exception error, bool cancelled)
        {
            Error = error;
            Cancelled = cancelled;
        }

        public Exception Error { get; }
        public bool Cancelled { get; }
    }
}