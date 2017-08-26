// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Starksoft.Aspen.Ftps;

namespace nUpdate.Administration.Ftp
{
    // ReSharper disable once InconsistentNaming
    internal class FtpTransferService : ITransferProvider
    {
        internal FtpData Data { get; set; }

        // TODO: Check implementation
        public async Task DeleteDirectory(string directoryName)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                var directoryItems = await List($"/{Data.Directory}/{directoryName}", true);
                await Login(ftpsClient);

                foreach (var item in directoryItems)
                {
                    string path = $"/{Data.Directory}/{directoryName}/{item.Name}/";
                    switch (item.ItemType)
                    {
                        case ServerItemType.Directory:
                            await DeleteDirectory(path);
                            break;
                        case ServerItemType.File:
                            await DeleteFile(path);
                            break;
                    }
                }

                ftpsClient.DeleteDirectory(directoryName);
            }
        }

        public async Task DeleteDirectoryWithPath(string directoryPath)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                var directoryItems = await List(directoryPath, true);
                await Login(ftpsClient);

                foreach (var item in directoryItems)
                {
                    string path = $"/{directoryPath}/{item.Name}/";
                    switch (item.ItemType)
                    {
                        case ServerItemType.Directory:
                            await DeleteDirectory(path);
                            break;
                        case ServerItemType.File:
                            await DeleteFile(path);
                            break;
                    }
                }

                ftpsClient.DeleteDirectory(directoryPath);
            }
        }

        public async Task DeleteFile(string fileName)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(Data.Directory);
                ftpsClient.DeleteFile(fileName);
            }
        }

        public async Task DeleteFileWithPath(string filePath)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                await Login(ftpsClient);
                ftpsClient.DeleteFile(filePath);
            }
        }

        public async Task<bool> Exists(string directoryPath, string destinationName)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                await Login(ftpsClient);
                bool exists = false;
                await Task.Run(() =>
                {
                    ftpsClient.ChangeDirectoryMultiPath(directoryPath);
                    exists = ftpsClient.Exists(destinationName);
                });
                return exists;
            }
        }

        public Task<bool> Exists(string destinationName)
        {
            return Exists(Data.Directory, destinationName);
        }

        public async Task<IEnumerable<IServerItem>> List(string path, bool recursive)
        {
            var ftpsClient = GetNewFtpsClient();
            await Login(ftpsClient);
            ftpsClient.ChangeDirectoryMultiPath(path);

            FtpsItemCollection items = null;
            await
                Task.Run(() => { items = recursive ? ftpsClient.GetDirListDeep(path) : ftpsClient.GetDirList(path); });
            ftpsClient.Close();

            return items.Select(x => new FtpsItemEx(x));
        }

        public async Task MakeDirectory(string directoryName)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(Data.Directory);
                ftpsClient.MakeDirectory(directoryName);
            }
        }

        public async Task MakeDirectoryWithPath(string directoryPath)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                await Login(ftpsClient);
                ftpsClient.MakeDirectory(directoryPath);
            }
        }

        public async Task MoveContent(string destinationPath, IEnumerable<string> availableChannelNames)
        {
            await MoveContent(Data.Directory, destinationPath, availableChannelNames);
        }

        public async Task RenameDirectory(string oldName, string newName)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(Data.Directory);
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
                    ftpsClient.TransferProgress += (o, e) => { progress.Report((FtpTransferProgressData) e); };

                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(Data.Directory);
                ftpsClient.PutFile(filePath, FileAction.Create);
            }
        }

        public async Task UploadFile(Stream fileStream, string remotePath, IProgress<ITransferProgressData> progress)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                if (progress != null)
                    ftpsClient.TransferProgress += (o, e) => { progress.Report((FtpTransferProgressData) e); };

                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(Data.Directory);
                ftpsClient.PutFile(fileStream, remotePath, FileAction.Create);
            }
        }

        public async Task UploadPackage(string packagePath, Guid guid,
            CancellationToken cancellationToken, IProgress<ITransferProgressData> progress)
        {
            using (var ftpsClient = GetNewFtpsClient())
            {
                if (progress != null)
                    ftpsClient.TransferProgress += (o, e) => { progress.Report((FtpTransferProgressData) e); };

                await Login(ftpsClient);
                ftpsClient.ChangeDirectoryMultiPath(Data.Directory);
                if (!await Exists(guid.ToString()))
                    ftpsClient.MakeDirectory(guid.ToString());
                ftpsClient.ChangeDirectory(guid.ToString());
                ftpsClient.PutFile(packagePath, FileAction.Create);
            }
        }

        private FtpsClient GetNewFtpsClient()
        {
            return new FtpsClient(Data.Host, Data.Port, Data.FtpSpecificProtocol)
            {
                DataTransferMode = Data.UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary
                // TODO: Proxy
                //Proxy = Data.Proxy != null ? new HttpProxyClient(Data.Proxy.Address.ToString()) : null
            };
        }

        private Task Login(FtpsClient ftpsClient)
        {
            return Task.Run(() =>
            {
                if (!ftpsClient.IsConnected)
                    ftpsClient.Open(Data.Username, (string)Data.Secret);
            });
        }

        public async Task MoveContent(string directory, string destinationPath,
            IEnumerable<string> availableChannelNames)
        {
            string[] availableChannelNamesArray = availableChannelNames.Select(x => x.ToLowerInvariant()).ToArray();
            bool IsAdministrationRelatedDirectory(string s) => s == "channels" || s == "packages";
            bool IsAdministrationRelatedFile(string s)
            {
                string fileName = Path.GetFileNameWithoutExtension(s);
                if (fileName == null) // wat
                    return false;

                Guid guid; // Just for the out-reference of TryParse
                if (Guid.TryParse(fileName, out guid))
                    return true;

                return s == "masterchannel.json" || s == "statistics.php" || availableChannelNamesArray.Contains(fileName.ToLowerInvariant());
            }

            // TODO: Test the method implementation
            foreach (var item in (await List(directory, false))
                .Where(
                    item =>
                        $"{directory}/{item.Name}" != destinationPath &&
                        $"{directory}/{item.Name}" != destinationPath.Substring(destinationPath.Length - 1)))
            {
                using (var ftpsClient = GetNewFtpsClient())
                {
                    if (item.ItemType == ServerItemType.Directory && IsAdministrationRelatedDirectory(item.Name))
                    {
                        ftpsClient.ChangeDirectoryMultiPath(destinationPath);
                        if (!await Exists(destinationPath, item.Name))
                            ftpsClient.MakeDirectory(item.Name);
                        ftpsClient.ChangeDirectoryMultiPath(item.Name);

                        await
                            MoveContent($"{directory}/{item.Name}", $"{destinationPath}/{item.Name}",
                                availableChannelNamesArray);
                        await DeleteDirectory($"{directory}/{item.Name}");
                    }
                    else if (item.ItemType == ServerItemType.File && IsAdministrationRelatedFile(item.Name))
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
    }
}