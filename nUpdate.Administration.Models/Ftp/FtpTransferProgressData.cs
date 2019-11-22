using System;

namespace nUpdate.Administration.Models.Ftp
{
    public class FtpTransferProgressData : ITransferProgressData
    {
        public FtpTransferProgressData(double bytesPerSecond, TimeSpan finishTime, double progress)
        {
            BytesPerSecond = bytesPerSecond;
            FinishTime = finishTime;
            Progress = progress;
        }


        public TimeSpan FinishTime { get; }

        public double Progress { get; }

        public double BytesPerSecond  { get; }

        public double KilobytesPerSecond => BytesPerSecond / 1000;

        public double MegabytesPerSecond => KilobytesPerSecond / 1000;

        public double GigabytesPerSecond => MegabytesPerSecond / 1000;
    }
}