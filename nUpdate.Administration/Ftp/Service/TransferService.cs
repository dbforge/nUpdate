using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading;
using nUpdate.Administration.Ftp.EventArgs;
using nUpdate.Administration.Proxy;
using nUpdate.Administration.TransferInterface;
using nUpdate.Updating;

namespace nUpdate.Administration.Ftp.Service
{
    public class TransferService : ITransferProvider
    {
        private readonly ManualResetEvent _uploadPackageResetEvent = new ManualResetEvent(false);
        private bool _disposed;
        private bool _hasAlreadyFixedStrings;
        private FtpClient _packageFtpClient;

        /// <summary>
        ///     Gets or sets the protocol.
        /// </summary>
        public FtpSecurityProtocol Protocol { get; set; }

        /// <summary>
        ///     Sets if passive mode should be used.
        /// </summary>
        public bool UsePassiveMode { get; set; }

        /// <summary>
        ///     The FTP-server.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     The port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     The directory.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        ///     The username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     The password.
        /// </summary>
        public SecureString Password { get; set; }

        /// <summary>
        ///     The proxy to use, if wished.
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        ///     Gets or sets the exception appearing during the package upload.
        /// </summary>
        public Exception PackageUploadException { get; set; }

        public void CancelPackageUpload()
        {
            _packageFtpClient.CancelAsync();
        }

        public event EventHandler<System.EventArgs> CancellationFinished;

        public void DeleteDirectory(string directoryPath)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, Protocol);
            try
            {
                IEnumerable<FtpItem> items = ListDirectoriesAndFiles(directoryPath, true);
                ftp.DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active;
                ftp.FileTransferType = TransferType.Binary;
                ftp.Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null;
                ftp.Open(Username, Password.ConvertToInsecureString());
                foreach (FtpItem item in items)
                {
                    switch (item.ItemType)
                    {
                        case FtpItemType.Directory:
                            DeleteDirectory($"/{directoryPath}/{item.Name}/");
                            break;
                        case FtpItemType.File:
                            DeleteFile(directoryPath, item.Name);
                            break;
                    }
                }
                ftp.DeleteDirectory(directoryPath);
            }
            finally
            {
                ftp.Close();
            }
        }

        public void DeleteFile(string directoryPath, string fileName)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };

            try
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.ChangeDirectoryMultiPath(directoryPath);
                ftp.DeleteFile(fileName);
            }
            finally
            {
                ftp.Close();
            }
        }

        public void DeleteFile(string fileName)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };

            try
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                ftp.DeleteFile(fileName);
            }
            finally
            {
                ftp.Close();
            }
        }

        public bool IsExisting(string directoryPath, string destinationName)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };

            try
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.ChangeDirectoryMultiPath(directoryPath);
                string nameList;
                try
                {
                    nameList = ftp.GetNameList(destinationName);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("No such file or directory"))
                        return false;
                    throw;
                }
                return !string.IsNullOrEmpty(nameList);
            }
            finally
            {
                ftp.Close();
            }
        }

        public bool IsExisting(string destinationName)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };

            try
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
            finally
            {
                ftp.Close();
            }
        }

        public IEnumerable<FtpItem> ListDirectoriesAndFiles(string path, bool recursive)
        {
            var ftp = new FtpClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };

            try
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                FtpItemCollection items = recursive ? ftp.GetDirListDeep(path) : ftp.GetDirList(path);
                return items;
            }
            finally
            {
                ftp.Close();
            }
        }

        public void MakeDirectory(string name)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };

            try
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.MakeDirectory(name);
            }
            finally
            {
                ftp.Close();
            }
        }

        public void MoveContent(string aimPath)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            InternalMoveContent(Directory, aimPath);
        }

        public event EventHandler<TransferProgressEventArgs> ProgressChanged;

        public void RenameDirectory(string oldName, string newName)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };

            try
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                ftp.Rename(oldName, newName);
            }
            finally
            {
                ftp.Close();
            }
        }

        public void TestConnection()
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };

            try
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
            }
            finally
            {
                ftp.Close();
            }
        }

        public void UploadFile(string filePath)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };

            try
            {
                ftp.Open(Username, Password.ConvertToInsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                ftp.PutFile(filePath, FileAction.Create);
            }
            finally
            {
                ftp.Close();
            }
        }

        public void UploadPackage(string packagePath, string packageVersion)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            _packageFtpClient = new FtpClient();
            _packageFtpClient = new FtpClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };

            try
            {
                _packageFtpClient.TransferProgress += TransferProgressChangedEventHandler;
                _packageFtpClient.PutFileAsyncCompleted += UploadPackageFinished;
                _packageFtpClient.Open(Username, Password.ConvertToInsecureString());
                _packageFtpClient.ChangeDirectoryMultiPath(Directory);
                _packageFtpClient.MakeDirectory(packageVersion);
                _packageFtpClient.ChangeDirectory(packageVersion);
                _packageFtpClient.PutFileAsync(packagePath, FileAction.Create);
                _uploadPackageResetEvent.WaitOne();
            }
            finally
            {
                _packageFtpClient.Close();
                _uploadPackageResetEvent.Reset();
            }
        }

        public void FixProperties()
        {
            if (Host.EndsWith("/"))
                Host = Host.Remove(Host.Length - 1);

            if (Directory != null && Directory.EndsWith("/") && Directory.Length > 1)
                Directory = Directory.Remove(Directory.Length - 1);

            _hasAlreadyFixedStrings = true;
        }

        public void OnProgressChanged(TransferProgressEventArgs e)
        {
            EventHandler<TransferProgressEventArgs> handler = ProgressChanged;
            if (handler != null)
                handler(this, e);
        }

        public void OnCancellationFinished(object sender, System.EventArgs e)
        {
            EventHandler<System.EventArgs> handler = CancellationFinished;
            if (handler != null)
                handler(this, e);
        }

        public void InternalMoveContent(string directory, string aimPath)
        {
            foreach (FtpItem item in ListDirectoriesAndFiles(directory, false)
                .Where(
                    item => item.FullPath != aimPath && item.FullPath != aimPath.Substring(aimPath.Length - 1)))
            {
                FtpClient ftp = null;
                try
                {
                    ftp = new FtpClient(Host, Port, Protocol)
                    {
                        DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                        FileTransferType = TransferType.Binary,
                        Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
                    };
                    ftp.Open(Username, Password.ConvertToInsecureString());

                    Guid guid; // Just for the out-reference of TryParse
                    if (item.ItemType == FtpItemType.Directory && UpdateVersion.IsValid(item.Name))
                    {
                        ftp.ChangeDirectoryMultiPath(aimPath);
                        if (!IsExisting(aimPath, item.Name))
                            ftp.MakeDirectory(item.Name);
                        ftp.ChangeDirectoryMultiPath(item.Name);

                        InternalMoveContent(item.FullPath, $"{aimPath}/{item.Name}");
                        DeleteDirectory(item.FullPath);
                    }
                    else if (item.ItemType == FtpItemType.File &&
                             (item.Name == "updates.json" || Guid.TryParse(item.Name.Split('.')[0], out guid)))
                        // Second condition determines whether the item is a package-file or not
                    {
                        if (!IsExisting(aimPath, item.Name))
                        {
                            // "MoveFile"-method damages the files, so we do it manually with a work-around
                            //ftp.MoveFile(item.FullPath, String.Format("{0}/{1}", aimPath, item.Name));

                            string localFilePath = Path.Combine(Path.GetTempPath(), item.Name);
                            ftp.GetFile(item.FullPath, localFilePath, FileAction.Create);
                            ftp.PutFile(localFilePath, $"{aimPath}/{item.Name}",
                                FileAction.Create);
                            File.Delete(localFilePath);
                        }
                        DeleteFile(item.ParentPath, item.Name);
                    }
                }
                finally
                {
                    if (ftp != null)
                        ftp.Close();
                }
            }
        }

        public void UploadPackageFinished(object sender, PutFileAsyncCompletedEventArgs e)
        {
            _uploadPackageResetEvent.Set();

            if (e.Cancelled)
            {
                OnCancellationFinished(this, System.EventArgs.Empty);
                return;
            }

            if (e.Error != null)
                PackageUploadException = e.Error;
        }

        public void TransferProgressChangedEventHandler(object sender, TransferProgressEventArgs e)
        {
            OnProgressChanged(e);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _packageFtpClient.Dispose();
            _uploadPackageResetEvent.Dispose();
            Password.Dispose();
            _disposed = true;
        }
    }
}