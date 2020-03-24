// FtpTransferProgressData.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

using System;
using nUpdate.Administration.PluginBase.Models;

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

        public double BytesPerSecond { get; }


        public TimeSpan FinishTime { get; }

        public double GigabytesPerSecond => MegabytesPerSecond / 1000;

        public double KilobytesPerSecond => BytesPerSecond / 1000;

        public double MegabytesPerSecond => KilobytesPerSecond / 1000;

        public double Progress { get; }
    }
}