// FtpTransferProvider.cs, 27.07.2019
// Copyright (C) Dominic Beger 10.09.2019

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

        public Task DeleteDirectoryInWorkingDirectory(string directoryName)
        {
            return DeleteDirectory(Path.Combine(Data.Directory, directoryName));
        }

        public async Task DeleteDirectory(string directoryPath)
        {
            using (var ftpClient = await GetFtpClient())
            {
                await ftpClient.DeleteDirectoryAsync(directoryPath);
                await ftpClient.DisconnectAsync();
            }
        }

        public async Task DeleteFileInWorkingDirectory(string fileName)
        {
            using (var ftpClient = await GetFtpClient())
            {
                await ftpClient.DeleteFileAsync(Path.Combine(Data.Directory, fileName));
                await ftpClient.DisconnectAsync();
            }
        }

        public async Task DeleteFile(string filePath)
        {
            using (var ftpClient = await GetFtpClient())
            {
                await ftpClient.DeleteFileAsync(filePath);
                await ftpClient.DisconnectAsync();
            }
        }

        public async Task<bool> FileExists(string filePath)
        {
            using (var ftpClient = await GetFtpClient())
            {
                bool result = await ftpClient.FileExistsAsync(filePath);
                await ftpClient.DisconnectAsync();
                return result;
            }
        }

        public Task<bool> FileExistsInWorkingDirectory(string fileName)
        {
            return FileExists(Path.Combine(Data.Directory, fileName));
        }

        public async Task<IEnumerable<IServerItem>> List(string path, bool recursive)
        {
            using (var ftpClient = await GetFtpClient())
            {
                var items = await ftpClient.GetListingAsync(path,
                    recursive ? FtpListOption.Recursive : FtpListOption.Auto);
                await ftpClient.DisconnectAsync();
                return items.Select(x => new FtpsItemEx(x));
            }
        }

        public Task MakeDirectoryInWorkingDirectory(string directoryName)
        {
            return MakeDirectory(Path.Combine(Data.Directory, directoryName));
        }

        public async Task MakeDirectory(string directoryPath)
        {
            using (var ftpClient = await GetFtpClient())
            {
                await ftpClient.CreateDirectoryAsync(directoryPath);
                await ftpClient.DisconnectAsync();
            }
        }

        public Task RenameInWorkingDirectory(string oldName, string newName)
        {
            return Rename(Data.Directory, oldName, newName);
        }

        public async Task Rename(string path, string oldName, string newName)
        {
            using (var ftpClient = await GetFtpClient())
            {
                await ftpClient.RenameAsync(Path.Combine(path, oldName), Path.Combine(path, newName));
                await ftpClient.DisconnectAsync();
            }
        }

        public ITransferData TransferData { get; set; }

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

        public async Task UploadFile(string filePath, IProgress<ITransferProgressData> progress)
        {
            using (var ftpClient = await GetFtpClient())
            {
                var internalProgress = new Progress<FtpProgress>();
                internalProgress.ProgressChanged +=
                    (sender, ftpProgress) => progress.Report(GetFromFtpProgress(ftpProgress));

                await ftpClient.UploadFileAsync(filePath,
                    Path.Combine(Data.Directory, Path.GetFileName(filePath) ?? throw new InvalidOperationException()),
                    FtpExists.Overwrite, true, FtpVerify.Retry, internalProgress);
                await ftpClient.DisconnectAsync();
            }
        }

        public async Task<bool> DirectoryExists(string directoryPath)
        {
            using (var ftpClient = await GetFtpClient())
            {
                bool result = await ftpClient.DirectoryExistsAsync(directoryPath);
                await ftpClient.DisconnectAsync();
                return result;
            }
        }

        public Task<bool> DirectoryExistsInWorkingDirectory(string destinationName)
        {
            return DirectoryExists(Path.Combine(Data.Directory, destinationName));
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

        private async Task Connect(IFtpClient ftpClient)
        {
            if (!ftpClient.IsConnected)
                await ftpClient.ConnectAsync();
        }
    }
}