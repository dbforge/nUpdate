// ITransferProvider.cs, 27.07.2019
// Copyright (C) Dominic Beger 10.09.2019

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate.Administration.Common
{
    public interface ITransferProvider
    {
        ITransferData TransferData { get; set; }

        /// <summary>
        ///     Determines whether a connection to the server can be established, or not.
        /// </summary>
        Task<(bool, Exception)> TestConnection();

        /// <summary>
        ///     Deletes a file on the server which is located at the specified path.
        /// </summary>
        /// <param name="filePath">The path of the file to delete (including the extension).</param>
        Task DeleteFileWithPath(string filePath);

        /// <summary>
        ///     Deletes a file on the server which is located in the current directory.
        /// </summary>
        /// <param name="fileName">The name of the file to delete (including the extension).</param>
        Task DeleteFile(string fileName);

        /// <summary>
        ///     Deletes a directory on the server which is located at the specified path.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to delete.</param>
        Task DeleteDirectoryWithPath(string directoryPath);

        /// <summary>
        ///     Deletes a directory on the server which is located in the current directory.
        /// </summary>
        /// <param name="directoryName">The name of the directory to delete.</param>
        Task DeleteDirectory(string directoryName);

        /// <summary>
        ///     Lists the directories and files of the directory at the specified path as an
        ///     <see cref="System.Collections.Generic.IEnumerable{T}" />.
        /// </summary>
        Task<IEnumerable<IServerItem>> List(string path, bool recursive);

        /// <summary>
        ///     Renames a file or directory inside the current directory.
        /// </summary>
        /// <param name="oldName">The old name of the file or directory.</param>
        /// <param name="newName">The new name of the file or directory.</param>
        Task Rename(string oldName, string newName);

        /// <summary>
        ///     Renames a file or directory on the server.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        /// <param name="oldName">The old name of the file or directory.</param>
        /// <param name="newName">The new name of the file or directory.</param>
        Task RenameAtPath(string path, string oldName, string newName);

        /// <summary>
        ///     Creates a new directory on the server which is located at the specified path.
        /// </summary>
        /// <param name="directoryPath">The path of the directory that should be created.</param>
        Task MakeDirectoryWithPath(string directoryPath);

        /// <summary>
        ///     Creates a new directory on the server which is located in the current directory.
        /// </summary>
        /// <param name="name">The name of the directory.</param>
        Task MakeDirectory(string name);

        /// <summary>
        ///     Determines whether a file exists on the server.
        /// </summary>
        /// <param name="filePath">The path of the file to check.</param>
        /// <returns><c>true</c> if the file exists, otherwise <c>false</c>.</returns>
        Task<bool> FileExistsAtPath(string filePath);

        /// <summary>
        ///     Determines whether a file exists inside the current directory.
        /// </summary>
        /// <param name="fileName">The name of the file to check.</param>
        /// <returns><c>true</c> if the file exists, otherwise <c>false</c>.</returns>
        Task<bool> FileExists(string fileName);

        /// <summary>
        ///     Determines whether a directory exists on the server.
        /// </summary>
        /// <param name="directoryPath">The path of the directory.</param>
        /// <returns><c>true</c> if the directory exists, otherwise <c>false</c>.</returns>
        Task<bool> DirectoryExistsAtPath(string directoryPath);

        /// <summary>
        ///     Determines whether a directory exists inside the current directory.
        /// </summary>
        /// <param name="destinationName">The name of the directory to check.</param>
        /// <returns><c>true</c> if the directory exists, otherwise <c>false</c>.</returns>
        Task<bool> DirectoryExists(string destinationName);

        /// <summary>
        ///     Uploads a file onto the server.
        /// </summary>
        /// <param name="filePath">The local path of the file to upload.</param>
        /// <param name="progress">
        ///     The <see cref="IProgress{T}" /> instance that should be used for reporting the upload
        ///     progress.
        /// </param>
        Task UploadFile(string filePath, IProgress<ITransferProgressData> progress);
    }
}