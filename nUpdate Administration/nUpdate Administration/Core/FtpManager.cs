// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using Starksoft.Net.Ftp;

namespace nUpdate.Administration.Core
{
    public class FtpManager
    {
        private bool _hasAlreadyFixedStrings;
        private string _directoryName;
        private FtpClient _packageFtpClient;
        private FtpClient _directoryEmptyingClient;
        private readonly ManualResetEvent _uploadPackageResetEvent = new ManualResetEvent(false);
        private FtpItemCollection _directoryCollection;
        
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
        /// Gets or sets the exception appearing during a file-upload.
        /// </summary>
        public Exception FileUploadException { get; set; }

        /// <summary>
        /// Gets or sets the exception appearing during the package upload.
        /// </summary>
        public Exception PackageUploadException { get; set; }

        /// <summary>
        /// Gets or sets the FTP-items that were listed by the <see cref="ListDirectoriesAndFilesRecursively"/>-method.
        /// </summary>
        public IEnumerable<FtpItem> ListedFtpItems; 

        /// <summary>
        /// Occurs when the download progress changes.
        /// </summary>
        public event EventHandler<TransferProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Occurs when the cancellation is finished.
        /// </summary>
        public event EventHandler<EventArgs> CancellationFinished;

        protected internal void OnProgressChanged(TransferProgressEventArgs e)
        {
            EventHandler<TransferProgressEventArgs> handler = ProgressChanged;
            if (handler != null)
                handler(this, e);
        }

        protected internal void OnCancellationFinished(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = CancellationFinished;
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

            if (Directory.EndsWith("/"))
                Directory = Directory.Remove(Directory.Length - 1);

            _hasAlreadyFixedStrings = true;
        }

        /// <summary>
        ///     Deletes a file on the server.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        public void DeleteFile(string fileName)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, FtpSecurityProtocol.None)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary
            };
            ftp.Open(Username, Password.ConvertToUnsecureString());
            ftp.ChangeDirectoryMultiPath(Directory);
            ftp.DeleteFile(fileName);
            ftp.Close();
        }

        /// <summary>
        ///     Deletes a directory on the server.
        /// </summary>
        /// <param name="directoryName">The name of the directory to delete.</param>
        public void DeleteDirectory(string directoryName)
        {
            _directoryName = directoryName;

            if (!_hasAlreadyFixedStrings)
                FixProperties();

            _directoryEmptyingClient = new FtpClient(Host, Port, FtpSecurityProtocol.None)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary
            };
            _directoryEmptyingClient.Open(Username, Password.ConvertToUnsecureString());
            _directoryEmptyingClient.ChangeDirectoryMultiPath(String.Format("/{0}/{1}", Directory, directoryName));
            _directoryEmptyingClient.GetDirListAsyncCompleted += DirectoryListingFinished;
            _directoryEmptyingClient.GetDirListAsync();
        }

        private void DirectoryListingFinished(object sender, GetDirListAsyncCompletedEventArgs e)
        {
            _directoryCollection = e.DirectoryListingResult;
            if (_directoryCollection != null)
            {
                foreach (FtpItem directoryItem in _directoryCollection)
                {
                    _directoryEmptyingClient.DeleteFile(directoryItem.Name);
                }
            }
            _directoryEmptyingClient.Close();

            var directoryDeletingClient = new FtpClient(Host, Port, FtpSecurityProtocol.None)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary
            };
            directoryDeletingClient.Open(Username, Password.ConvertToUnsecureString());
            directoryDeletingClient.ChangeDirectoryMultiPath(Directory);
            directoryDeletingClient.DeleteDirectory(_directoryName);
            directoryDeletingClient.Close();
        }

        /// <summary>
        ///      Lists the directories and files of the current FTP-directory recursively.
        /// </summary>
        public IEnumerable<FtpItem> ListDirectoriesAndFilesRecursively()
        {
            using (var ftp = new FtpClient(Host, Port, FtpSecurityProtocol.None))
            {
                ftp.DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active;
                ftp.FileTransferType = TransferType.Binary;
                ftp.Open(Username, Password.ConvertToUnsecureString());
                ftp.ChangeDirectoryMultiPath(Directory ?? "/downloads/");
                var items = ftp.GetDirList();
                ftp.Close();
                return items;
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

            using (var ftp = new FtpClient(Host, Port, FtpSecurityProtocol.None))
            {
                ftp.DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active;
                ftp.FileTransferType = TransferType.Binary;

                ftp.Open(Username, Password.ConvertToUnsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                
                foreach (var file in ListDirectoriesAndFilesRecursively())
                {
                    ftp.MoveFile(file.FullPath, aimPath);
                }

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

            using (var ftp = new FtpClient(Host, Port, FtpSecurityProtocol.None))
            {
                ftp.DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active;
                ftp.FileTransferType = TransferType.Binary;

                ftp.Open(Username, Password.ConvertToUnsecureString());
                ftp.ChangeDirectoryMultiPath(Directory);
                ftp.PutFile(filePath, FileAction.Create);
                ftp.Close();
            }
        }

        /// <summary>
        ///     Uploads an update package  to the server.
        /// </summary>
        /// <param name="packagePath">The local path of the package..</param>
        /// <param name="packageVersion">The package version for the directory name.</param>
        public void UploadPackage(string packagePath, string packageVersion)
        {
            _directoryName = packageVersion;

            if (!_hasAlreadyFixedStrings)
                FixProperties();

            _packageFtpClient = new FtpClient(Host, Port, FtpSecurityProtocol.None)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
            };
            _packageFtpClient.TransferProgress += TransferProgressChangedEventHandler;
            _packageFtpClient.PutFileAsyncCompleted += UploadPackageFinished;
            _packageFtpClient.Open(Username, Password.ConvertToUnsecureString());
            _packageFtpClient.ChangeDirectoryMultiPath(Directory);
            _packageFtpClient.MakeDirectory(packageVersion);
            _packageFtpClient.ChangeDirectory(packageVersion);
            _packageFtpClient.PutFileAsync(packagePath, FileAction.Create);
            _uploadPackageResetEvent.WaitOne();
            _packageFtpClient.Close();
            _uploadPackageResetEvent.Reset();
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
    }
}