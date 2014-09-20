// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System;
using System.Security;
using System.Threading;
using Starksoft.Net.Ftp;

namespace nUpdate.Administration.Core.Update
{
    internal class FtpManager
    {
        private bool _hasAlreadyFixedStrings;
        private FtpClient _packageFtpClient;
        private ManualResetEvent _uploadResetEvent = new ManualResetEvent(false);
        private ManualResetEvent _listingResetEvent = new ManualResetEvent(false);
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
        public string UserName { get; set; }

        /// <summary>
        ///     The password.
        /// </summary>
        public SecureString Password { get; set; }

        /// <summary>
        /// Gets or sets the exception appearing during the package upload.
        /// </summary>
        public Exception PackageUploadException { get; set; }

        /// <summary>
        /// Occurs when the download progress is changed.
        /// </summary>
        public event EventHandler<TransferProgressEventArgs> ProgressChanged;

        protected internal void OnProgressChanged(TransferProgressEventArgs e)
        {
            EventHandler<TransferProgressEventArgs> handler = ProgressChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        ///     Edits the properties if they are not automatically suitable for the server adress.
        /// </summary>
        private void FixProperties()
        {
            if (Host.EndsWith("/"))
                Host = Host.Remove(Host.Length - 1);

            if (Directory.StartsWith("/"))
                Directory = Directory.Substring(1);

            if (Directory.EndsWith("/"))
                Directory = Directory.Remove(Directory.Length - 1);

            _hasAlreadyFixedStrings = true;
        }

        /// <summary>
        ///     Deletes a file on the server.
        /// </summary>
        public void DeleteFile(string fileName)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, FtpSecurityProtocol.None)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary
            };
            ftp.Open(UserName, Password.ConvertToUnsecureString());
            ftp.ChangeDirectoryMultiPath(Directory);
            ftp.DeleteFile(fileName);
            ftp.Close();
        }

        /// <summary>
        ///     Deletes a directory on the server.
        /// </summary>
        public void DeleteDirectory(string directoryName)
        {
            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, FtpSecurityProtocol.None)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary
            };
            ftp.Open(UserName, Password.ConvertToUnsecureString());
            ftp.ChangeDirectoryMultiPath(String.Format("/{0}/{1}", Directory, directoryName));
            ftp.GetDirListAsyncCompleted += DirectoryListingFinished;
            ftp.GetDirListAsync();
            _listingResetEvent.WaitOne();

            foreach (FtpItem directoryItem in _directoryCollection)
            {
                ftp.DeleteFile(directoryItem.Name);
            }
                
            ftp.ChangeDirectoryUp();
            ftp.DeleteDirectory(directoryName);
            ftp.Close();
        }

        private void DirectoryListingFinished(object sender, GetDirListAsyncCompletedEventArgs e)
        {
            _directoryCollection = e.DirectoryListingResult;
            _listingResetEvent.Set();
        }

        /// <summary>
        ///     Uploads a file to the server.
        /// </summary>
        public void UploadFile(string filePath)
        {
            if (_uploadResetEvent != null)
            {
                _uploadResetEvent.Dispose();
                _uploadResetEvent = new ManualResetEvent(false);
            }

            if (!_hasAlreadyFixedStrings)
                FixProperties();

            var ftp = new FtpClient(Host, Port, FtpSecurityProtocol.None)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary
            };
            ftp.Open(UserName, Password.ConvertToUnsecureString());
            ftp.ChangeDirectoryMultiPath(Directory);
            ftp.PutFileAsync(filePath, FileAction.Create);
            ftp.Close();
        }

        /// <summary>
        ///     Connects to the server and uploads a package file.
        /// </summary>
        public void UploadPackage(string packagePath, string packageVersion)
        {
            if (_uploadResetEvent != null)
            {
                _uploadResetEvent.Dispose();
                _uploadResetEvent = new ManualResetEvent(false);
            }

            if (!_hasAlreadyFixedStrings)
                FixProperties();

            _packageFtpClient = new FtpClient(Host, Port, FtpSecurityProtocol.None)
            {
                DataTransferMode = UsePassiveMode ? TransferMode.Passive : TransferMode.Active,
                FileTransferType = TransferType.Binary,
            };
            _packageFtpClient.TransferProgress += UploadProgressChangedEventHandler;
            _packageFtpClient.PutFileAsyncCompleted += UploadFinishedEventHandler;
            _packageFtpClient.Open(UserName, Password.ConvertToUnsecureString());
            _packageFtpClient.ChangeDirectoryMultiPath(Directory);
            _packageFtpClient.MakeDirectory(packageVersion);
            _packageFtpClient.ChangeDirectory(packageVersion);
            _packageFtpClient.PutFileAsync(packagePath, FileAction.Create);
            _packageFtpClient.Close();
        }

        /// <summary>
        ///     Terminates the package upload process.
        /// </summary>
        public void CancelPackageUpload()
        {
            _packageFtpClient.CancelAsync();
            _uploadResetEvent.Set();
        }

        private void UploadProgressChangedEventHandler(object sender, TransferProgressEventArgs e)
        {
            OnProgressChanged(e);
        }

        private void UploadFinishedEventHandler(object sender, PutFileAsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error.InnerException != null)
                    PackageUploadException = e.Error.InnerException;
            }
            _uploadResetEvent.Set();
        }
    }
}