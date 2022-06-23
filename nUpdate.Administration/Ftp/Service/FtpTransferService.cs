// FtpTransferService.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using nUpdate.Administration.TransferInterface;
using nUpdate.Updating;
using TransferProgressEventArgs = nUpdate.Administration.TransferInterface.TransferProgressEventArgs;

namespace nUpdate.Administration.Ftp.Service
{
    public class FtpTransferService : ITransferProvider
    {
        private readonly ManualResetEvent _uploadPackageResetEvent = new ManualResetEvent(false);
        private string _directory;
        private bool _disposed;
        private string _host;
        private FtpsClient _packageFtpsClient;

        public event EventHandler<EventArgs> CancellationFinished;

        public void CancelPackageUpload()
        {
            _packageFtpsClient?.CancelAsync();
        }

        public async Task DeleteDirectory(string directoryPath)
        {
            using (var ftpClient = await GetFtpClient())
            {
                await ftpClient.DeleteDirectoryAsync(directoryPath);
            }
        }

        public async Task DeleteFile(string directoryPath, string fileName)
        {
            string fullPath = Path.Combine(directoryPath, fileName);

            using (var ftpClient = await GetFtpClient())
            {
                await ftpClient.DeleteFileAsync(fullPath);
            }
        }

        public Task DeleteFile(string fileName)
        {
            return DeleteFile(Directory, fileName);
        }

        /// <summary>
        ///     The directory.
        /// </summary>
        public string Directory
        {
            get => _directory;
            set
            {
                _directory = value;
                if (Directory != null && Directory.EndsWith("/") && Directory.Length > 1)
                    Directory = Directory.Remove(Directory.Length - 1);
            }
        }

        /// <summary>
        ///     The FTP-server.
        /// </summary>
        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                if (_host.EndsWith("/"))
                    _host = _host.Remove(Host.Length - 1);
            }
        }

        public async Task<bool> DirectoryExists(string directoryPath, string destinationName)
        {
            string fullPath = Path.Combine(directoryPath, destinationName);

            using (var ftp = await GetFtpClient())
            {
                await ftp.DirectoryExistsAsync(fullPath);
            }
        }

        public bool IsExisting(string destinationName)
        {
            using (var ftp = GetFtpClient())
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                string nameList = null;
                try
                {
                    nameList = ftp.GetNameList(destinationName);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("No such file or directory") || ex.Message.Contains("Directory not found"))
                        return false;
                }

                return !string.IsNullOrEmpty(nameList);
            }
        }

        public async Task<IEnumerable<ServerItem>> ListDirectoriesAndFiles(string path, bool recursive)
        {
            string fullPath = Path.Combine(Directory, path);

            using (var ftp = await GetFtpClient())
            {
                var items = await ftp.GetListingAsync(fullPath, recursive ? FtpListOption.Recursive : FtpListOption.Auto);
                return items.Select(f => f.ToServerItem());
            }
        }

        public async Task MakeDirectory(string name)
        {
            string fullPath = Path.Combine(Directory, name);

            using (var ftp = await GetFtpClient())
            {
                await ftp.CreateDirectoryAsync(fullPath, true);
            }
        }

        public void MoveContent(string aimPath)
        {
            MoveContent(Directory, aimPath);
        }

        /// <summary>
        ///     Gets or sets the exception appearing during the package upload.
        /// </summary>
        public Exception PackageUploadException { get; set; }

        /// <summary>
        ///     The password.
        /// </summary>
        public SecureString Password { get; set; }

        /// <summary>
        ///     The port.
        /// </summary>
        public int Port { get; set; }

        public event EventHandler<TransferProgressEventArgs> ProgressChanged;

        /// <summary>
        ///     The proxy to use, if wished.
        /// </summary>
        public WebProxy Proxy { get; set; }

        public async Task RenameDirectory(string oldName, string newName)
        {
            string oldPath = Path.Combine(Directory, oldName);
            string newPath = Path.Combine(Directory, newName);

            using (var ftp = await GetFtpClient())
            {
                await ftp.RenameAsync(oldPath, newPath);
            }
        }

        public void TestConnection()
        {
            using (var ftp = GetFtpClient())
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
            }
        }

        public async Task UploadFile(string filePath)
        {
            string remotePath = Path.Combine(Directory, Path.GetFileName(filePath));

            using (var ftp = await GetFtpClient())
            {
                await ftp.UploadFileAsync(filePath, remotePath, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);
            }
        }

        public void UploadPackage(string packagePath, string packageVersion)
        {
            _packageFtpsClient = GetFtpClient();
            _packageFtpsClient.TransferProgress += TransferProgressChangedEventHandler;
            _packageFtpsClient.PutFileAsyncCompleted += UploadPackageFinished;
            _packageFtpsClient.Open(Username, Password.ConvertToInsecureString());
            _packageFtpsClient.ChangeDirectoryMultiPath(Directory);
            if (!_packageFtpsClient.Exists(packageVersion))
                _packageFtpsClient.MakeDirectory(packageVersion);
            _packageFtpsClient.ChangeDirectory(packageVersion);
            _packageFtpsClient.PutFileAsync(packagePath, FileAction.Create);
            _uploadPackageResetEvent.WaitOne();
            _packageFtpsClient.Close();
            _uploadPackageResetEvent.Reset();
        }

        /// <summary>
        ///     The username.
        /// </summary>
        public string Username { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _packageFtpsClient.Dispose();
            _uploadPackageResetEvent.Dispose();
            Password.Dispose();
            _disposed = true;
        }

        private async Task<FtpClient> GetFtpClient()
        {
            var connection = new FtpClient(Host, Username, Password.ConvertToInsecureString());
            await connection.AutoConnectAsync();
            return connection;
        }

        public void MoveContent(string directory, string aimPath)
        {
            foreach (var item in ListDirectoriesAndFiles(directory, false)
                .Where(
                    item => item.FullPath != aimPath && item.FullPath != aimPath.Substring(aimPath.Length - 1)))
                using (var ftp = GetFtpClient())
                {
                    ftp.Open(Username, Password.ConvertToInsecureString());

                    Guid guid; // Just for the out-reference of TryParse
                    if (item.ItemType == ServerItemType.Directory && UpdateVersion.IsValid(item.Name))
                    {
                        ftp.ChangeDirectoryMultiPath(aimPath);
                        if (!IsExisting(aimPath, item.Name))
                            ftp.MakeDirectory(item.Name);
                        ftp.ChangeDirectoryMultiPath(item.Name);

                        MoveContent(item.FullPath, $"{aimPath}/{item.Name}");
                        DeleteDirectory(item.FullPath);
                    }
                    else if (item.ItemType == ServerItemType.File &&
                             (item.Name == "updates.json" || item.Name == "statistics.php" ||
                              Guid.TryParse(item.Name.Split('.')[0], out guid)))
                        // Second condition determines whether the item is a package-file or not
                    {
                        if (!IsExisting(aimPath, item.Name)) ftp.MoveFile(item.FullPath, $"{aimPath}/{item.Name}");
                    }
                }
        }

        public void OnCancellationFinished(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = CancellationFinished;
            handler?.Invoke(this, e);
        }

        public void OnProgressChanged(TransferProgressEventArgs e)
        {
            EventHandler<TransferProgressEventArgs> handler = ProgressChanged;
            handler?.Invoke(this, e);
        }

        public void TransferProgressChangedEventHandler(object sender, Starksoft.Aspen.Ftps.TransferProgressEventArgs e)
        {
            OnProgressChanged(e.ToTransferInterfaceProgressEventArgs());
        }

        public void UploadPackageFinished(object sender, PutFileAsyncCompletedEventArgs e)
        {
            _uploadPackageResetEvent.Set();

            if (e.Cancelled)
            {
                OnCancellationFinished(this, EventArgs.Empty);
                return;
            }

            if (e.Error != null)
                PackageUploadException = e.Error;
        }
    }
}