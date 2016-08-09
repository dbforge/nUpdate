using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Ftp
{
    // ReSharper disable once InconsistentNaming
    internal class FtpTransferService : ITransferProvider
    {
        private bool _disposed;
        private FtpData _ftpData;
        private SecureString _password;

        private FtpClient GetNewFtpClient()
        {
            return new FtpClient(_ftpData.Host, _ftpData.Port, _ftpData.FtpSpecificProtocol)
            {
                DataTransferMode = _ftpData.UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary
                // TODO: Proxy
                //Proxy = _ftpData.Proxy != null ? new HttpProxyClient(_ftpData.Proxy.Address.ToString()) : null
            };
        }

        internal FtpData Data
        {
            get { return _ftpData; }
            set
            {
                _ftpData = value;
                if (_ftpData.Host.EndsWith("/"))
                    _ftpData.Host = _ftpData.Host.Remove(_ftpData.Host.Length - 1);

                if (_ftpData.Directory != null && _ftpData.Directory.EndsWith("/") && _ftpData.Directory.Length > 1)
                    _ftpData.Directory = _ftpData.Directory.Remove(_ftpData.Directory.Length - 1);

                _password = AesManager.Decrypt(Convert.FromBase64String(_ftpData.Password),
                    Program.AesKeyPassword, Program.AesIvPassword);
            }
        }

        public async Task DeleteDirectory(string directoryPath)
        {
            using (var ftpClient = GetNewFtpClient())
            {
                IEnumerable<FtpItem> directoryItems = await List(directoryPath, true);
                await Login(ftpClient);

                foreach (var item in directoryItems)
                {
                    switch (item.ItemType)
                    {
                        case FtpItemType.Directory:
                            await DeleteDirectory($"/{directoryPath}/{item.Name}/");
                            break;
                        case FtpItemType.File:
                            await DeleteFile(directoryPath, item.Name);
                            break;
                    }
                }

                ftpClient.DeleteDirectory(directoryPath);
            }
        }

        public async Task DeleteFile(string directoryPath, string fileName)
        {
            using (var ftpClient = GetNewFtpClient())
            {
                await Login(ftpClient);
                ftpClient.ChangeDirectoryMultiPath(directoryPath);
                ftpClient.DeleteFile(fileName);
            }
        }

        public async Task DeleteFile(string fileName)
        {
            using (var ftpClient = GetNewFtpClient())
            {
                await Login(ftpClient);
                ftpClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                ftpClient.DeleteFile(fileName);
            }
        }

        public async Task<bool> Exists(string directoryPath, string destinationName)
        {
            using (var ftpClient = GetNewFtpClient())
            {
                await Login(ftpClient);
                ftpClient.ChangeDirectoryMultiPath(directoryPath);
                return ftpClient.Exists(destinationName);
            }
        }

        public Task<bool> Exists(string destinationName)
        {
            return Exists(_ftpData.Directory, destinationName);
        }

        public async Task<IEnumerable<FtpItem>> List(string path, bool recursive)
        {
            using (var ftpClient = GetNewFtpClient())
            {
                await Login(ftpClient);
                FtpItemCollection items = null;
                await TaskEx.Run(() =>
                {
                    items = recursive ? ftpClient.GetDirListDeep(path) : ftpClient.GetDirList(path);
                });
                return items;
            }
        }

        public async Task MakeDirectory(string name)
        {
            using (var ftpClient = GetNewFtpClient())
            {
                await Login(ftpClient);
                ftpClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                ftpClient.MakeDirectory(name);
            }
        }

        public async Task MoveContent(string aimPath)
        {
            await InternalMoveContent(_ftpData.Directory, aimPath);
        }

        public async Task RenameDirectory(string oldName, string newName)
        {
            using (var ftpClient = GetNewFtpClient())
            {
                await Login(ftpClient);
                ftpClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                ftpClient.Rename(oldName, newName);
            }
        }

        public async Task<bool> TestConnection()
        {
            using (var ftpClient = GetNewFtpClient())
            {
                try
                {
                    await Login(ftpClient);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task UploadFile(string filePath, IProgress<TransferProgressEventArgs> progress)
        {
            using (var ftpClient = GetNewFtpClient())
            {
                if (progress != null)
                    ftpClient.TransferProgress += (o, e) =>
                    {
                        progress.Report(e);
                    };

                await Login(ftpClient);
                ftpClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                ftpClient.PutFile(filePath, FileAction.Create);
            }
        }

        public async Task UploadFile(Stream fileStream, string remotePath, IProgress<TransferProgressEventArgs> progress)
        {
            using (var ftpClient = GetNewFtpClient())
            {
                if (progress != null)
                    ftpClient.TransferProgress += (o, e) =>
                    {
                        progress.Report(e);
                    };

                await Login(ftpClient);
                ftpClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                ftpClient.PutFile(fileStream, remotePath, FileAction.Create);
            }
        }

        public async Task UploadPackage(string packagePath, Guid guid,
            CancellationToken cancellationToken, IProgress<TransferProgressEventArgs> progress)
        {
            using (var ftpClient = GetNewFtpClient())
            {
                if (progress != null)
                    ftpClient.TransferProgress += (o, e) =>
                    {
                        progress.Report(e);
                    };

                await Login(ftpClient);
                ftpClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                if (!await Exists(guid.ToString()))
                    ftpClient.MakeDirectory(guid.ToString());
                ftpClient.ChangeDirectory(guid.ToString());
                ftpClient.PutFile(packagePath, FileAction.Create);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private Task Login(FtpClient ftpClient)
        {
            return Task.Run(() =>
            {
                if (!ftpClient.IsConnected)
                    ftpClient.Open(_ftpData.Username, _password.ConvertToInsecureString());
            });
        }

        public async Task InternalMoveContent(string directory, string aimPath)
        {
            foreach (var item in (await List(directory, false))
                .Where(
                    item => item.FullPath != aimPath && item.FullPath != aimPath.Substring(aimPath.Length - 1)))
            {
                using (var ftpClient = GetNewFtpClient())
                {
                    Guid guid; // Just for the out-reference of TryParse
                    if (item.ItemType == FtpItemType.Directory && UpdateVersion.IsValid(item.Name))
                    {
                        ftpClient.ChangeDirectoryMultiPath(aimPath);
                        if (!await Exists(aimPath, item.Name))
                            ftpClient.MakeDirectory(item.Name);
                        ftpClient.ChangeDirectoryMultiPath(item.Name);

                        await InternalMoveContent(item.FullPath, $"{aimPath}/{item.Name}");
                        await DeleteDirectory(item.FullPath);
                    }
                    else if (item.ItemType == FtpItemType.File &&
                             (item.Name == "updates.json" || Guid.TryParse(item.Name.Split('.')[0], out guid)))
                        // Second condition determines whether the item is a package-file or not
                    {
                        if (!await Exists(aimPath, item.Name))
                        {
                            // "MoveFile"-method damages the files, so we do it manually with a work-around
                            ftpClient.MoveFile(item.FullPath, $"{aimPath}/{item.Name}");

                            //string localFilePath = Path.Combine(Path.GetTempPath(), item.Name);
                            //ftpClient.GetFile(item.FullPath, localFilePath, FileAction.Create);
                            //ftpClient.PutFile(localFilePath, $"{aimPath}/{item.Name}",
                            //    FileAction.Create);
                            //File.Delete(localFilePath);
                        }
                        await DeleteFile(item.ParentPath, item.Name);
                    }
                }
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;
            
            _password.Dispose();
            _disposed = true;
        }
    }
}