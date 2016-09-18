// Author: Dominic Beger (Trade/ProgTrade) 2016

namespace nUpdate.Administration.Core.Ftp.EventArgs
{
    /// <summary>
    ///     Event arguments to facilitate the response event from the FTP server.
    /// </summary>
    public class FtpResponseEventArgs : System.EventArgs
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="response">FtpResponse object.</param>
        public FtpResponseEventArgs(FtpResponse response)
        {
            Response = response;
        }

        /// <summary>
        ///     Response object containing response received from the server.
        /// </summary>
        public FtpResponse Response { get; }
    }
}