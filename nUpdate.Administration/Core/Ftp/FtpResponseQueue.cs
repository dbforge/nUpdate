// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Collections.Generic;

namespace nUpdate.Administration.Core.Ftp
{
    /// <summary>
    ///     Thread safe FtpResponse queue object.
    /// </summary>
    internal class FtpResponseQueue
    {
        private readonly Queue<FtpResponse> _queue = new Queue<FtpResponse>(10);

        /// <summary>
        ///     Gets the number of elements contained in the FtpResponseQueue.
        /// </summary>
        public int Count
        {
            get
            {
                lock (this)
                {
                    return _queue.Count;
                }
            }
        }

        /// <summary>
        ///     Adds an Response object to the end of the FtpResponseQueue.
        /// </summary>
        /// <param name="item">An FtpResponse item.</param>
        public void Enqueue(FtpResponse item)
        {
            lock (this)
            {
                _queue.Enqueue(item);
            }
        }

        /// <summary>
        ///     Removes and returns the FtpResponse object at the beginning of the FtpResponseQueue.
        /// </summary>
        /// <returns>FtpResponse object at the beginning of the FtpResponseQueue</returns>
        public FtpResponse Dequeue()
        {
            lock (this)
            {
                return _queue.Dequeue();
            }
        }
    }
}