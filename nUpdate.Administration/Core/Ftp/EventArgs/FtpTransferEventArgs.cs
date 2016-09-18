// Author: Dominic Beger (Trade/ProgTrade) 2016

namespace nUpdate.Administration.Core.Ftp.EventArgs
{
    /// <summary>
    ///     Event arguments to facilitate the FtpClient transfer progress and complete events.
    /// </summary>
    public class FtpTransferEventArgs : System.EventArgs
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="bytesTransferred">The number of bytes transferred.</param>
        public FtpTransferEventArgs(long bytesTransferred)
        {
            BytesTransferred = bytesTransferred;
        }

        /// <summary>
        ///     The number of bytes transferred.
        /// </summary>
        public long BytesTransferred { get; }
    }
}