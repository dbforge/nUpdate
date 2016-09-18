// Author: Dominic Beger (Trade/ProgTrade) 2016

namespace nUpdate.Administration.Core.Ftp.EventArgs
{
    /// <summary>
    ///     Event arguments to facilitate the FtpClient request event.
    /// </summary>
    public class FtpRequestEventArgs : System.EventArgs
    {
        /// <summary>
        ///     Constructor for FtpRequestEventArgs.
        /// </summary>
        /// <param name="request">An FtpRequest object.</param>
        public FtpRequestEventArgs(FtpRequest request)
        {
            Request = request;
        }

        /// <summary>
        ///     Client request command text sent from the client to the server.
        /// </summary>
        public FtpRequest Request { get; }
    }
}