/////////////////////////////////////////////////////////////
////
//// Copyright (c) 2007 Starksoft, LLC
//// All Rights Reserved.
////
/////////////////////////////////////////////////////////////

namespace nUpdate.Administration.Ftp.EventArgs
{
    /// <summary>
    ///     Event arguments to facilitate the FtpClient transfer progress and complete events.
    /// </summary>
    public class FtpTransferEventArgs : System.EventArgs
    {
        private readonly long _bytesTransferred;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="bytesTransferred">The number of bytes transferred.</param>
        public FtpTransferEventArgs(long bytesTransferred)
        {
            _bytesTransferred = bytesTransferred;
        }

        /// <summary>
        ///     The number of bytes transferred.
        /// </summary>
        public long BytesTransferred => _bytesTransferred;
    }
}