/////////////////////////////////////////////////////////////
////
//// Copyright (c) 2007 Starksoft, LLC
//// All Rights Reserved.
////
/////////////////////////////////////////////////////////////

using System;

namespace nUpdate.Administration.Core.Ftp.EventArgs
{
    public class PutFileUniqueAsyncCompletedEventArgs : System.EventArgs
    {
        private readonly Exception _error;
        private readonly bool _cancelled;

        public PutFileUniqueAsyncCompletedEventArgs(Exception error, bool cancelled)
        {
            _error = error;
            _cancelled = cancelled;
        }

        public Exception Error
        {
            get { return _error; }
        }

        public bool Cancelled
        {
            get { return _cancelled; }
        }

    }

} 

