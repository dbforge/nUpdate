// Copyright © Dominic Beger 2018

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading;
using nUpdate.Administration.TransferInterface;
using nUpdate.Updating;
using Starksoft.Aspen.Ftps;
using Starksoft.Aspen.Proxy;
using TransferProgressEventArgs = nUpdate.Administration.TransferInterface.TransferProgressEventArgs;

namespace nUpdate.Administration.Core.Ftp.Service
{
    public class FtpTransferService : ITransferProvider
    {
        private readonly ManualResetEvent _uploadPackageResetEvent = new ManualResetEvent(false);
        private string _directory;
        private bool _disposed;
        private string _host;
        private FtpsClient _packageFtpsClient;

        /// <summary>
        ///     Gets or sets the network version.
        /// </summary>
        public NetworkVersion NetworkVersion { get; set; }

        /// <summary>
        ///     Gets or sets the protocol.
        /// </summary>
        public FtpsSecurityProtocol Protocol { get; set; }

        public event EventHandler<EventArgs> CancellationFinished;

        public void CancelPackageUpload()
        {
            _packageFtpsClient?.CancelAsync();
        }

        public void DeleteDirectory(string directoryPath)
        {
            using (var ftp = GetNewFtpsClient())
            {
                IEnumerable<ServerItem> items = ListDirectoriesAndFiles(directoryPath, true);
                ftp.Open(Username, Password.ConvertToInsecureString());
                foreach (var item in items)
                    switch (item.ItemType)
                    {
                        case ServerItemType.Directory:
                            DeleteDirectory($"/{directoryPath}/{item.Name}/");
                            break;
                        case ServerItemType.File:
                            DeleteFile(directoryPath, item.Name);
                            break;
                    }
                ftp.DeleteDirectory(directoryPath);
            }
        }

        public void DeleteFile(string directoryPath, string fileName)
        {
            using (var ftp = GetNewFtpsClient())
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.ChangeDirectoryMultiPath(directoryPath);
                ftp.DeleteFile(fileName);
            }
        }

        public void DeleteFile(string fileName)
        {
            using (var ftp = GetNewFtpsClient())
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                ftp.DeleteFile(fileName);
            }
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

        public bool IsExisting(string directoryPath, string destinationName)
        {
            using (var ftp = GetNewFtpsClient())
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.ChangeDirectoryMultiPath(directoryPath);
                return ftp.Exists(directoryPath, destinationName);
            }
        }

        public bool IsExisting(string destinationName)
        {
            using (var ftp = GetNewFtpsClient())
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

        public IEnumerable<ServerItem> ListDirectoriesAndFiles(string path, bool recursive)
        {
            using (var ftp = GetNewFtpsClient())
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                var items = recursive ? ftp.GetDirListDeep(path) : ftp.GetDirList(path);
                return items.Select(f => f.ToServerItem());
            }
        }

        public void MakeDirectory(string name)
        {
            using (var ftp = GetNewFtpsClient())
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.MakeDirectory(name);
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

        public void RenameDirectory(string oldName, string newName)
        {
            using (var ftp = GetNewFtpsClient())
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                ftp.Rename(oldName, newName);
            }
        }

        public void TestConnection()
        {
            using (var ftp = GetNewFtpsClient())
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
            }
        }

        public void UploadFile(string filePath)
        {
            using (var ftp = GetNewFtpsClient())
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                ftp.PutFile(filePath, FileAction.Create);
            }
        }

        public void UploadPackage(string packagePath, string packageVersion)
        {
            _packageFtpsClient = GetNewFtpsClient();
            _packageFtpsClient.TransferProgress += TransferProgressChangedEventHandler;
            _packageFtpsClient.PutFileAsyncCompleted += UploadPackageFinished;
            _packageFtpsClient.Open(Username, Password.ConvertToInsecureString());
            _packageFtpsClient.ChangeDirectoryMultiPath(Directory);
            _packageFtpsClient.MakeDirectory(packageVersion);
            _packageFtpsClient.ChangeDirectory(packageVersion);
            _packageFtpsClient.PutFileAsync(packagePath, FileAction.Create);
            _uploadPackageResetEvent.WaitOne();
            _packageFtpsClient.Close();
            _uploadPackageResetEvent.Reset();
        }

        /// <summary>
        ///     Sets if passive mode should be used.
        /// </summary>
        public bool UsePassiveMode { get; set; }

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

        private FtpsClient GetNewFtpsClient()
        {
            return new FtpsClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                NetworkProtocol = NetworkVersion,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };
        }

        public void MoveContent(string directory, string aimPath)
        {
            foreach (var item in ListDirectoriesAndFiles(directory, false)
                .Where(
                    item => item.FullPath != aimPath && item.FullPath != aimPath.Substring(aimPath.Length - 1)))
                using (var ftp = GetNewFtpsClient())
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