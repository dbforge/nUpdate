// FtpTransferProvider.cs, 27.07.2019
// Copyright (C) Dominic Beger 18.09.2019

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentFTP;

namespace nUpdate.Administration.Common.Ftp
{
    // ReSharper disable once InconsistentNaming
    public class FtpTransferProvider : ITransferProvider
    {
        internal FtpData Data => TransferData as FtpData;

        public async Task DeleteDirectory(string relativeDirectoryPath)
        {
            using (var ftpClient = await GetFtpClient())
            {
                await ftpClient.DeleteDirectoryAsync(Path.Combine(Data.Directory, relativeDirectoryPath));
                await ftpClient.DisconnectAsync();
            }
        }

        public async Task DeleteFile(string relativeFilePath)
        {
            using (var ftpClient = await GetFtpClient())
            {
                await ftpClient.DeleteFileAsync(Path.Combine(Data.Directory, relativeFilePath));
                await ftpClient.DisconnectAsync();
            }
        }

        public async Task<bool> DirectoryExists(string relativeDirectoryPath)
        {
            using (var ftpClient = await GetFtpClient())
            {
                var result = await ftpClient.DirectoryExistsAsync(Path.Combine(Data.Directory, relativeDirectoryPath));
                await ftpClient.DisconnectAsync();
                return result;
            }
        }

        public async Task<bool> FileExists(string relativeFilePath)
        {
            using (var ftpClient = await GetFtpClient())
            {
                var result = await ftpClient.FileExistsAsync(Path.Combine(Data.Directory, relativeFilePath));
                await ftpClient.DisconnectAsync();
                return result;
            }
        }

        public async Task<IEnumerable<IServerItem>> List(string relativePath, bool recursive)
        {
            using (var ftpClient = await GetFtpClient())
            {
                var items = await ftpClient.GetListingAsync(Path.Combine(Data.Directory, relativePath),
                    recursive ? FtpListOption.Recursive : FtpListOption.Auto);
                await ftpClient.DisconnectAsync();
                return items.Select(x => new FtpServerItem(x));
            }
        }

        public async Task MakeDirectory(string relativeDirectoryPath)
        {
            using (var ftpClient = await GetFtpClient())
            {
                await ftpClient.CreateDirectoryAsync(Path.Combine(Data.Directory, relativeDirectoryPath));
                await ftpClient.DisconnectAsync();
            }
        }

        public async Task Rename(string relativePath, string oldName, string newName)
        {
            using (var ftpClient = await GetFtpClient())
            {
                await ftpClient.RenameAsync(Path.Combine(Data.Directory, relativePath, oldName),
                    Path.Combine(Data.Directory, relativePath, newName));
                await ftpClient.DisconnectAsync();
            }
        }

        public async Task<(bool, Exception)> TestConnection()
        {
            try
            {
                using (var ftpClient = await GetFtpClient())
                {
                    await ftpClient.DisconnectAsync();
                    return (true, null);
                }
            }
            catch (Exception e)
            {
                return (false, e);
            }
        }

        public ITransferData TransferData { get; set; }

        public async Task UploadFile(string localFilePath, string remoteRelativePath,
            IProgress<ITransferProgressData> progress)
        {
            using (var ftpClient = await GetFtpClient())
            {
                var internalProgress = new Progress<FtpProgress>();
                internalProgress.ProgressChanged +=
                    (sender, ftpProgress) => progress.Report(GetFromFtpProgress(ftpProgress));

                await ftpClient.UploadFileAsync(localFilePath,
                    Path.Combine(Data.Directory, remoteRelativePath),
                    FtpExists.Overwrite, true, FtpVerify.Retry, internalProgress);
                await ftpClient.DisconnectAsync();
            }
        }

        private async Task Connect(IFtpClient ftpClient)
        {
            if (!ftpClient.IsConnected)
                await ftpClient.ConnectAsync();
        }

        private FtpTransferProgressData GetFromFtpProgress(FtpProgress ftpProgress)
        {
            return new FtpTransferProgressData(ftpProgress.TransferSpeed, ftpProgress.ETA, ftpProgress.Progress);
        }

        private async Task<FtpClient> GetFtpClient()
        {
            var c = new FtpClient(Data.Host, Data.Port, Data.Username, (string) Data.Secret);
            await Connect(c);
            return c;
        }
    }
}