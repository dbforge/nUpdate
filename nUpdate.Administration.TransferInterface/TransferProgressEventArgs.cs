/*
Copyright (c) 2007-2009 Benton Stark, Starksoft LLC (http://www.starksoft.com) 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;

namespace nUpdate.Administration.TransferInterface
{
    /// <summary>
    ///     Provides the data for the <see cref="TransferProgressEventArgs"/>-event.
    /// </summary>
    public class TransferProgressEventArgs : EventArgs
    {
        private readonly int _bytesTransferred;
        private readonly long _totalBytesTransferred;
        private readonly int _bytesPerSecond;
        private TimeSpan _elapsedTime;
        private readonly long _totalBytes;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TransferProgressEventArgs"/>-class.
        /// </summary>
        /// <param name="bytesTransferred">The number of bytes transferred.</param>
        /// <param name="totalBytesTransferred">Total number of bytes transferred.</param>
        /// <param name="bytesPerSecond">The data transfer speed in bytes per second.</param>
        /// <param name="elapsedTime">The time that has elapsed since the data transfer started.</param>
        /// <param name="totalBytes">Total bytes of data.</param>
        public TransferProgressEventArgs(int bytesTransferred, long totalBytesTransferred, int bytesPerSecond, TimeSpan elapsedTime, long totalBytes)
        {
            _bytesTransferred = bytesTransferred;
            _totalBytesTransferred = totalBytesTransferred;
            _bytesPerSecond = bytesPerSecond;
            _elapsedTime = elapsedTime;
            _totalBytes = totalBytes;
        }

        /// <summary>
        ///     The number of bytes transferred.
        /// </summary>
        public int BytesTransferred
        {
            get { return _bytesTransferred; }
        }

        /// <summary>
        ///     Total number of bytes transferred.
        /// </summary>
        public long TotalBytesTransferred
        {
            get { return _totalBytesTransferred; }
        }

        /// <summary>
        ///     Total bytes of data.
        /// </summary>
        public long TotalBytes
        {
            get { return _totalBytes; }
        }

        /// <summary>
        ///     Gets the data transfer speed in bytes per second.
        /// </summary>
        public int BytesPerSecond
        {
            get { return _bytesPerSecond; }
        }

        /// <summary>
        ///     Gets the data transfer speed in kilobytes per second.
        /// </summary>
        public int KilobytesPerSecond
        {
            get { return _bytesPerSecond / 1024; }
        }

        /// <summary>
        ///     Gets the time that has elapsed since the data transfer started.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get { return _elapsedTime; }
        }

        /// <summary>
        ///     Transferred data percentage.
        /// </summary>
        public float Percentage
        {
            get { return (_totalBytesTransferred / _totalBytes) * 100; }
        }

        public TimeSpan EstimatedCompleteTime
        {
            get
            {
                double elapsed = _elapsedTime.TotalMilliseconds;
                double totalTime = (double)_totalBytes / _totalBytesTransferred * elapsed;
                return TimeSpan.FromMilliseconds(totalTime - elapsed);
            }
        }
    }
}