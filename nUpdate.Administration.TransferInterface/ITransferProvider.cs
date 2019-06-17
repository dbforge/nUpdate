// ITransferProvider.cs, 01.08.2018
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Collections.Generic;
using System.Net;
using System.Security;

namespace nUpdate.Administration.TransferInterface
{
    public interface ITransferProvider
    {
        /// <summary>
        ///     Sets if passive mode should be used.
        /// </summary>
        bool UsePassiveMode { get; set; }

        /// <summary>
        ///     The FTP-server.
        /// </summary>
        string Host { get; set; }

        /// <summary>
        ///     The port.
        /// </summary>
        int Port { get; set; }

        /// <summary>
        ///     The directory.
        /// </summary>
        string Directory { get; set; }

        /// <summary>
        ///     The username.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        ///     The password.
        /// </summary>
        SecureString Password { get; set; }

        /// <summary>
        ///     The proxy to use, if wished.
        /// </summary>
        WebProxy Proxy { get; set; }

        /// <summary>
        ///     Gets or sets the exception appearing during the package upload.
        /// </summary>
        Exception PackageUploadException { get; set; }

        /// <summary>
        ///     Fired when the download progress changes.
        /// </summary>
        event EventHandler<TransferProgressEventArgs> ProgressChanged;

        /// <summary>
        ///     Fired when the cancellation is finished.
        /// </summary>
        event EventHandler<EventArgs> CancellationFinished;

        /// <summary>
        ///     Tests the connection to the server and also if all certificates are valid.
        /// </summary>
        void TestConnection();

        /// <summary>
        ///     Deletes a file on the server.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        void DeleteFile(string fileName);

        /// <summary>
        ///     Deletes a file on the server which is located at the specified path.
        /// </summary>
        /// <param name="directoryPath">The path of the directory where the file is located.</param>
        /// <param name="fileName">The name of the file to delete.</param>
        void DeleteFile(string directoryPath, string fileName);

        /// <summary>
        ///     Deletes a directory on the server.
        /// </summary>
        /// <param name="directoryPath">The name of the directory to delete.</param>
        void DeleteDirectory(string directoryPath);

        /// <summary>
        ///     Lists the directories and files of the current directory.
        /// </summary>
        IEnumerable<ServerItem> ListDirectoriesAndFiles(string path, bool recursive);

        /// <summary>
        ///     Renames a directory on the server.
        /// </summary>
        /// <param name="oldName">The old name of the directory.</param>
        /// <param name="newName">The new name of the directory.</param>
        void RenameDirectory(string oldName, string newName);

        /// <summary>
        ///     Creates a new directory on the server.
        /// </summary>
        /// <param name="name">The name of the directory.</param>
        void MakeDirectory(string name);

        /// <summary>
        ///     Moves all files and subdirectories from the current FTP-directory to the given aim directory.
        /// </summary>
        /// <param name="aimPath">The aim directory to move the files and subdirectories to.</param>
        void MoveContent(string aimPath);

        /// <summary>
        ///     Returns if a file or directory is existing on the server.
        /// </summary>
        /// <param name="destinationName">The name of the file or folder to check.</param>
        /// <returns>Returns "true" if the file or folder exists, otherwise "false".</returns>
        bool IsExisting(string destinationName);

        /// <summary>
        ///     Returns if a file or directory is existing on the server.
        /// </summary>
        /// <param name="directoryPath">The directory in which the file should be existing.</param>
        /// <param name="destinationName">The name of the file or folder to check.</param>
        /// <returns>Returns "true" if the file or folder exists, otherwise "false".</returns>
        bool IsExisting(string directoryPath, string destinationName);

        /// <summary>
        ///     Uploads a file to the server.
        /// </summary>
        /// <param name="filePath">The local path of the file to upload.</param>
        void UploadFile(string filePath);

        /// <summary>
        ///     Uploads an update package to the server.
        /// </summary>
        /// <param name="packagePath">The local path of the package..</param>
        /// <param name="packageVersion">The package version for the directory name.</param>
        void UploadPackage(string packagePath, string packageVersion);

        /// <summary>
        ///     Terminates the package upload process.
        /// </summary>
        void CancelPackageUpload();
    }
}