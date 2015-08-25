/////////////////////////////////////////////////////////////
////
//// Copyright (c) 2007 Starksoft, LLC
//// All Rights Reserved.
////
/////////////////////////////////////////////////////////////

using System;

namespace nUpdate.Administration.Ftp.EventArgs
{
    public class PutFileUniqueAsyncCompletedEventArgs : System.EventArgs
    {
        private readonly bool _cancelled;
        private readonly Exception _error;

        public PutFileUniqueAsyncCompletedEventArgs(Exception error, bool cancelled)
        {
            _error = error;
            _cancelled = cancelled;
        }

        public Exception Error => _error;

        public bool Cancelled => _cancelled;
    }
}