// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading;
using System.Windows;
using nUpdate.Administration.Core.Ftp;
using nUpdate.Administration.Core.Ftp.EventArgs;
using nUpdate.Administration.Core.Proxy;

namespace nUpdate.Administration.Core
{
    public class FtpManager : IDisposable
    {
        private bool _disposed;
        private bool _hasAlreadyFixedStrings;
        private FtpClient _packageFtpClient;
        private readonly ManualResetEvent _uploadPackageResetEvent = new ManualResetEvent(false);

        /// <summary>
        ///     Gets or sets the FTP-items that were listed by the <see cref="ListDirectoriesAndFiles" />-method.
        /// </summary>
        public IEnumerable<FtpItem> ListedFtpItems;

        /// <summary>
        ///     Returns the adress that was created by the <see cref="FtpManager" />-class during the call of the last function.
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        ///     Sets the protocol to use.
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
        ///     Gets or sets the exception appearing during a file-upload.
        /// </summary>
        public Exception FileUploadException { get; set; }

        /// <summary>
        ///     Gets or sets the exception appearing during the package upload.
        /// </summary>
        public Exception PackageUploadException { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Fired when the download progress changes.
        /// </summary>
        public event EventHandler<TransferProgressEventArgs> ProgressChanged;

        /// <summary>
        ///     Fired when the cancellation is finished.
        /// </summary>
        public event EventHandler<EventArgs> CancellationFinished;

        protected internal void OnProgressChanged(TransferProgressEventArgs e)
        {
            var handler = ProgressChanged;
            if (handler != null)
                handler(this, e);
        }

        protected internal void OnCancellationFinished(object sender, EventArgs e)
        {
            var handler = CancellationFinished;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        ///     Edits the properties if they are not automatically suitable for the server address.
        /// </summary>
        private void FixProperties()
        {
            if (Host.EndsWith("/"))
                Host = Host.Remove(Host.Length - 1);

            if (Directory.EndsWith("/") && Directory.Length > 1)
                Directory = Directory.Remove(Directory.Length - 1);

            _hasAlreadyFixedStrings = true;
        }

        /// <summary>
        ///     Tests the connection to the server and also if all certificates are valid.
        /// </summary>
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
                ftp.Open(Username, Password.ConvertToUnsecureString());
            }
            finally
            {
                ftp.Close();
            }
        }

        /// <summary>
        ///     Deletes a file on the server.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
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
                ftp.Open(Username, Password.ConvertToUnsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                ftp.DeleteFile(fileName);
            }
            finally
            {
                ftp.Close();
            }
        }

        /// <summary>
        ///     Deletes a file on the server which is located at the specified path.
        /// </summary>
        /// <param name="directoryPath">The path of the directory where the file is located.</param>
        /// <param name="fileName">The name of the file to delete.</param>
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
                ftp.Open(Username, Password.ConvertToUnsecureString());
                ftp.ChangeDirectoryMultiPath(directoryPath);
                ftp.DeleteFile(fileName);
            }
            finally
            {
                ftp.Close();
            }
        }

        /// <summary>
        ///     Deletes a directory on the server.
        /// </summary>
        /// <param name="directoryPath">The name of the directory to delete.</param>
        public void DeleteDirectory(string directoryPath)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, Protocol);
            try
            {
                var items = ListDirectoriesAndFiles(directoryPath, true);
                ftp.DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active;
                ftp.FileTransferType = TransferType.Binary;
                ftp.Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null;
                ftp.Open(Username, Password.ConvertToUnsecureString());
                foreach (var item in items)
                {
                    switch (item.ItemType)
                    {
                        case FtpItemType.Directory:
                            DeleteDirectory(String.Format("/{0}/{1}/", directoryPath, item.Name));
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

        /// <summary>
        ///     Lists the directories and files of the current FTP-directory.
        /// </summary>
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
                ftp.Open(Username, Password.ConvertToUnsecureString());
                var items = recursive ? ftp.GetDirListDeep(path) : ftp.GetDirList(path);
                return items;
            }
            finally
            {
                ftp.Close();
            }
        }

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
                ftp.Open(Username, Password.ConvertToUnsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                ftp.Rename(oldName, newName);
            }
            finally
            {
                ftp.Close();
            }
        }

        /// <summary>
        ///     Moves all files and subdirectories from the current FTP-directory to the given aim directory.
        /// </summary>
        /// <param name="aimPath">The aim directory to move the files and subdirectories to.</param>
        public void MoveContent(string aimPath)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            InternalMoveContent(Directory, aimPath);
        }

        private void InternalMoveContent(string directory, string aimPath)
        {
            var ftp = new FtpClient(Host, Port, Protocol)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
                Proxy = Proxy != null ? new HttpProxyClient(Proxy.Address.ToString()) : null
            };

            try
            {
                ftp.Open(Username, Password.ConvertToUnsecureString());
                foreach (var item in ListDirectoriesAndFiles(directory, false)
                    .Where(
                        item => item.FullPath != aimPath && item.FullPath != aimPath.Substring(aimPath.Length - 1)))
                {
                    Guid guid; // Just for the out-reference of TryParse
                    if (item.ItemType == FtpItemType.Directory && UpdateVersion.IsValid(item.Name))
                    {
                        ftp.ChangeDirectoryMultiPath(aimPath);
                        if (!IsExisting(item.Name))
                            ftp.MakeDirectory(item.Name);
                        ftp.ChangeDirectoryMultiPath(item.Name);
                        InternalMoveContent(item.FullPath, String.Format("{0}/{1}", aimPath, item.Name));
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
         
                            var localFilePath = Path.Combine(Path.GetTempPath(), item.Name);
                            ftp.GetFile(item.FullPath, localFilePath, FileAction.Create);
                            ftp.PutFile(localFilePath, String.Format("{0}/{1}", aimPath, item.Name),
                                FileAction.Create);
                            File.Delete(localFilePath);
                        }
                        DeleteFile(item.ParentPath, item.Name);
                    }
                }
            }
            finally
            {
                ftp.Close();
            }
        }

        /// <summary>
        ///     Returns if a file or directory is existing on the server.
        /// </summary>
        /// <param name="destinationName">The name of the file or folder to check.</param>
        /// <returns>Returns "true" if the file or folder exists, otherwise "false".</returns>
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
                ftp.Open(Username, Password.ConvertToUnsecureString());
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

        /// <summary>
        ///     Returns if a file or directory is existing on the server.
        /// </summary>
        /// <param name="directoryPath">The directory in which the file should be existing.</param>
        /// <param name="destinationName">The name of the file or folder to check.</param>
        /// <returns>Returns "true" if the file or folder exists, otherwise "false".</returns>
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
                ftp.Open(Username, Password.ConvertToUnsecureString());
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

        /// <summary>
        ///     Uploads a file to the server.
        /// </summary>
        /// <param name="filePath">The local path of the file to upload.</param>
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
                ftp.Open(Username, Password.ConvertToUnsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                ftp.PutFile(filePath, FileAction.Create);
            }
            finally
            {
                ftp.Close();
            }
        }

        /// <summary>
        ///     Uploads an update package to the server.
        /// </summary>
        /// <param name="packagePath">The local path of the package..</param>
        /// <param name="packageVersion">The package version for the directory name.</param>
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
                _packageFtpClient.Open(Username, Password.ConvertToUnsecureString());
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

        private void UploadPackageFinished(object sender, PutFileAsyncCompletedEventArgs e)
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

        /// <summary>
        ///     Terminates the package upload process.
        /// </summary>
        public void CancelPackageUpload()
        {
            _packageFtpClient.CancelAsync();
        }

        private void TransferProgressChangedEventHandler(object sender, TransferProgressEventArgs e)
        {
            OnProgressChanged(e);
        }

        protected virtual void Dispose(bool disposing)
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