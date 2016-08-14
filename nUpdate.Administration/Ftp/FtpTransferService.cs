using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Administration.TransferInterface;
using Starksoft.Aspen.Ftps;

namespace nUpdate.Administration.Ftp
{
    // ReSharper disable once InconsistentNaming
    internal class FtpTransferService : ITransferProvider
    {
        private bool _disposed;
        private FtpData _ftpData;
        private SecureString _password;

        private FtpsClient GetNewFtpsClient()
        {
            return new FtpsClient(_ftpData.Host, _ftpData.Port, _ftpData.FtpSpecificProtocol)
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
            using (var ftpsClient = GetNewFtpsClient())
            {
                var directoryItems = await List(directoryPath, true);
                await Login(ftpsClient);

                foreach (var item in directoryItems)
                {
                    switch (item.ItemType)
                    {
                        case ServerItemType.Directory:
                            await DeleteDirectory($"/{directoryPath}/{item.Name}/");
                            break;
                        case ServerItemType.File:
                            await DeleteFile(directoryPath, item.Name);
                            break;
                    }
                }

                ftpsClient.DeleteDirectory(directoryPath);
            }
        }

        public async Task DeleteFile(string directoryPath, string fileName)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(directoryPath);
                ftpsClient.DeleteFile(fileName);
            }
        }

        public async Task DeleteFile(string fileName)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                ftpsClient.DeleteFile(fileName);
            }
        }

        public async Task<bool> Exists(string directoryPath, string destinationName)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(directoryPath);
                return ftpsClient.Exists(destinationName);
            }
        }

        public Task<bool> Exists(string destinationName)
        {
            return Exists(_ftpData.Directory, destinationName);
        }

        public async Task<IEnumerable<IServerItem>> List(string path, bool recursive)
        {
            var ftpsClient = GetNewFtpsClient();
            await Login(ftpsClient);
            ftpsClient.ChangeDirectoryMultiPath(_ftpData.Directory);

            FtpsItemCollection items = null;
            await TaskEx.Run(() =>
            {
                items = recursive ? ftpsClient.GetDirListDeep(path) : ftpsClient.GetDirList(path);
            });
            ftpsClient.Close();

            return items.Select(x => new FtpsItemEx(x));
        }

        public async Task MakeDirectory(string name)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                ftpsClient.MakeDirectory(name);
            }
        }

        public async Task MoveContent(string destinationPath, IEnumerable<string> availableChannelNames)
        {
            await InternalMoveContent(_ftpData.Directory, destinationPath, availableChannelNames);
        }

        public async Task RenameDirectory(string oldName, string newName)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                ftpsClient.Rename(oldName, newName);
            }
        }

        public async Task<bool> TestConnection()
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                try
                {
                    await Login(ftpsClient);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task UploadFile(string filePath, IProgress<ITransferProgressData> progress)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                if (progress != null)
                    ftpsClient.TransferProgress += (o, e) =>
                    {
                        progress.Report((FtpTransferProgressData)e);
                    };

                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                ftpsClient.PutFile(filePath, FileAction.Create);
            }
        }

        public async Task UploadFile(Stream fileStream, string remotePath, IProgress<ITransferProgressData> progress)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                if (progress != null)
                    ftpsClient.TransferProgress += (o, e) =>
                    {
                        progress.Report((FtpTransferProgressData)e);
                    };

                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                ftpsClient.PutFile(fileStream, remotePath, FileAction.Create);
            }
        }

        public async Task UploadPackage(string packagePath, Guid guid,
            CancellationToken cancellationToken, IProgress<ITransferProgressData> progress)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                if (progress != null)
                    ftpsClient.TransferProgress += (o, e) =>
                    {
                        progress.Report((FtpTransferProgressData)e);
                    };

                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(_ftpData.Directory);
                if (!await Exists(guid.ToString()))
                    ftpsClient.MakeDirectory(guid.ToString());
                ftpsClient.ChangeDirectory(guid.ToString());
                ftpsClient.PutFile(packagePath, FileAction.Create);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private Task Login(FtpsClient ftpsClient)
        {
            return Task.Run(() =>
            {
                if (!ftpsClient.IsConnected)
                    ftpsClient.Open(_ftpData.Username, _password.ConvertToInsecureString());
            });
        }

        public async Task InternalMoveContent(string directory, string destinationPath, IEnumerable<string> availableChannelNames)
        {
            string[] availableChannelNamesArray = availableChannelNames.Select(x => x.ToLowerInvariant()).ToArray();
            Func<string, bool> isAdministrationRelatedDirectory = s => s == "channels" || s == "packages";
            Func<string, bool> isAdministrationRelatedFile = s =>
            {
                string fileName = Path.GetFileNameWithoutExtension(s);
                if (fileName == null) // wat
                    return false;

                Guid guid; // Just for the out-reference of TryParse
                if (Guid.TryParse(fileName, out guid))
                    return true;

                return s == "masterchannel.json" || availableChannelNamesArray.Contains(fileName.ToLowerInvariant());
            };

            // TODO: Test the method implementation
            foreach (var item in (await List(directory, false))
                .Where(
                    item => $"{directory}/{item.Name}" != destinationPath && $"{directory}/{item.Name}" != destinationPath.Substring(destinationPath.Length - 1)))
            {
                using (var ftpsClient = GetNewFtpsClient())
                {
                    if (item.ItemType == ServerItemType.Directory && isAdministrationRelatedDirectory(item.Name))
                    {
                        ftpsClient.ChangeDirectoryMultiPath(destinationPath);
                        if (!await Exists(destinationPath, item.Name))
                            ftpsClient.MakeDirectory(item.Name);
                        ftpsClient.ChangeDirectoryMultiPath(item.Name);

                        await InternalMoveContent($"{directory}/{item.Name}", $"{destinationPath}/{item.Name}", availableChannelNamesArray);
                        await DeleteDirectory($"{directory}/{item.Name}");
                    }
                    else if (item.ItemType == ServerItemType.File && isAdministrationRelatedFile(item.Name))
                    {
                        if (!await Exists(destinationPath, item.Name))
                        {
                            ftpsClient.MoveFile($"{directory}/{item.Name}", $"{destinationPath}/{item.Name}");

                            //string localFilePath = Path.Combine(Path.GetTempPath(), item.Name);
                            //ftpsClient.GetFile(item.FullPath, localFilePath, FileAction.Create);
                            //ftpsClient.PutFile(localFilePath, $"{aimPath}/{item.Name}",
                            //    FileAction.Create);
                            //File.Delete(localFilePath);
                        }
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