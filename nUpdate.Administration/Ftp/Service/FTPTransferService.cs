using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Ftp.Service
{
    // ReSharper disable once InconsistentNaming
    internal class FtpTransferService : ITransferProvider
    {
        private bool _disposed;
        private FtpClient _ftpClient;
        private FtpData _ftpData;
        private SecureString _password;

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

                _ftpClient = new FtpClient(_ftpData.Host, _ftpData.Port, _ftpData.FtpSpecificProtocol)
                {
                    DataTransferMode = _ftpData.UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                    FileTransferType = TransferType.Binary,
                    // TODO: Proxy
                    //Proxy = _ftpData.Proxy != null ? new HttpProxyClient(_ftpData.Proxy.Address.ToString()) : null
                };

                _password = AesManager.Decrypt(Convert.FromBase64String(_ftpData.Password),
                    Program.AesKeyPassword, Program.AesIvPassword);
            }
        }

        public async Task DeleteDirectory(string directoryPath)
        {
            try
            {
                IEnumerable<FtpItem> directoryItems = await List(directoryPath, true);
                await Login();

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

                _ftpClient.DeleteDirectory(directoryPath);
            }
            finally
            {
                _ftpClient.Close();
            }
        }

        public async Task DeleteFile(string directoryPath, string fileName)
        {
            try
            {
                await Login();
                _ftpClient.ChangeDirectoryMultiPath(directoryPath);
                _ftpClient.DeleteFile(fileName);
            }
            finally
            {
                _ftpClient.Close();
            }
        }

        public async Task DeleteFile(string fileName)
        {
            try
            {
                await Login();
                _ftpClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                _ftpClient.DeleteFile(fileName);
            }
            finally
            {
                _ftpClient.Close();
            }
        }

        public async Task<bool> Exists(string directoryPath, string destinationName)
        {
            try
            {
                await Login();
                _ftpClient.ChangeDirectoryMultiPath(directoryPath);
                return _ftpClient.Exists(destinationName);
            }
            finally
            {
                _ftpClient.Close();
            }
        }

        public Task<bool> Exists(string destinationName)
        {
            return Exists(_ftpData.Directory, destinationName);
        }

        public async Task<IEnumerable<FtpItem>> List(string path, bool recursive)
        {
            try
            {
                await Login();
                var items = recursive ? _ftpClient.GetDirListDeep(path) : _ftpClient.GetDirList(path);
                return items;
            }
            finally
            {
                _ftpClient.Close();
            }
        }

        public async Task MakeDirectory(string name)
        {
            try
            {
                await Login();
                _ftpClient.MakeDirectory(name);
            }
            finally
            {
                _ftpClient.Close();
            }
        }

        public async Task MoveContent(string aimPath)
        {
            await InternalMoveContent(_ftpData.Directory, aimPath);
        }

        public async Task RenameDirectory(string oldName, string newName)
        {
            try
            {
                await Login();
                _ftpClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                _ftpClient.Rename(oldName, newName);
            }
            finally
            {
                _ftpClient.Close();
            }
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                await Login();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _ftpClient.Close();
            }
        }

        public async Task UploadFile(string filePath, IProgress<TransferProgressEventArgs> progress)
        {
            try
            {
                if (progress != null)
                    _ftpClient.TransferProgress += (o, e) =>
                    {
                        progress.Report(e);
                    };

                await Login();
                _ftpClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                _ftpClient.PutFile(filePath, FileAction.Create);
            }
            finally
            {
                _ftpClient.Close();
            }
        }

        public async Task UploadFile(Stream fileStream, string remotePath, IProgress<TransferProgressEventArgs> progress)
        {
            try
            {
                if (progress != null)
                    _ftpClient.TransferProgress += (o, e) =>
                    {
                        progress.Report(e);
                    };

                await Login();
                _ftpClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                _ftpClient.PutFile(fileStream, remotePath, FileAction.Create);
            }
            finally
            {
                _ftpClient.Close();
            }
        }

        public async Task UploadPackage(string packagePath, string packageVersionString,
            CancellationToken cancellationToken, IProgress<TransferProgressEventArgs> progress)
        {
            if (progress != null)
                _ftpClient.TransferProgress += (o, e) =>
                {
                    progress.Report(e);
                };

            try
            {
                await Login();
                _ftpClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                if (!await Exists(packageVersionString))
                    _ftpClient.MakeDirectory(packageVersionString);
                _ftpClient.ChangeDirectory(packageVersionString);
                _ftpClient.PutFile(packagePath, FileAction.Create);
            }
            finally
            {
                _ftpClient.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private async Task Login()
        {
            await Task.Run(() =>
            {
                if (!_ftpClient.IsConnected)
                    _ftpClient.Open(_ftpData.Username, _password.ConvertToInsecureString());
            });
        }

        public async Task InternalMoveContent(string directory, string aimPath)
        {
            foreach (FtpItem item in (await List(directory, false))
                .Where(
                    item => item.FullPath != aimPath && item.FullPath != aimPath.Substring(aimPath.Length - 1)))
            {
                try
                {
                    Guid guid; // Just for the out-reference of TryParse
                    if (item.ItemType == FtpItemType.Directory && UpdateVersion.IsValid(item.Name))
                    {
                        _ftpClient.ChangeDirectoryMultiPath(aimPath);
                        if (!await Exists(aimPath, item.Name))
                            _ftpClient.MakeDirectory(item.Name);
                        _ftpClient.ChangeDirectoryMultiPath(item.Name);

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
                            _ftpClient.MoveFile(item.FullPath, $"{aimPath}/{item.Name}");

                            //string localFilePath = Path.Combine(Path.GetTempPath(), item.Name);
                            //_ftpClient.GetFile(item.FullPath, localFilePath, FileAction.Create);
                            //_ftpClient.PutFile(localFilePath, $"{aimPath}/{item.Name}",
                            //    FileAction.Create);
                            //File.Delete(localFilePath);
                        }
                        await DeleteFile(item.ParentPath, item.Name);
                    }
                }
                finally
                {
                    _ftpClient.Close();
                }
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _ftpClient.Dispose();
            _password.Dispose();
            _disposed = true;
        }
    }
}