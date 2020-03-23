// ITransferProvider.cs, 27.07.2019
// Copyright (C) Dominic Beger 18.09.2019

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using nUpdate.Administration.PluginBase.Models;

namespace nUpdate.Administration.PluginBase.BusinessLogic
{
    public interface ITransferProvider
    {
        ITransferData TransferData { get; set; }

        /// <summary>
        ///     Deletes a directory on the server which is located at the path relative to the working directory.
        /// </summary>
        /// <param name="relativeDirectoryPath">The path of the directory to delete relative to the working directory.</param>
        Task DeleteDirectory(string relativeDirectoryPath);

        /// <summary>
        ///     Deletes a file on the server which is located at the path relative to the working directory.
        /// </summary>
        /// <param name="relativeFilePath">
        ///     The path of the file to delete (including the extension) relative to the working
        ///     directory.
        /// </param>
        Task DeleteFile(string relativeFilePath);

        /// <summary>
        ///     Determines whether a directory exists at the path relative to the working directory.
        /// </summary>
        /// <param name="relativeDirectoryPath">The path of the directory relative to the working directory.</param>
        /// <returns><c>true</c> if the directory exists, otherwise <c>false</c>.</returns>
        Task<bool> DirectoryExists(string relativeDirectoryPath);

        /// <summary>
        ///     Determines whether a file exists at the path relative to the working directory.
        /// </summary>
        /// <param name="filePath">The path of the file to check, relative to the working directory.</param>
        /// <returns><c>true</c> if the file exists, otherwise <c>false</c>.</returns>
        Task<bool> FileExists(string filePath);

        /// <summary>
        ///     Lists the content of the directory which is located at the path relative to the working directory.
        /// </summary>
        Task<IEnumerable<IServerItem>> List(string relativeDirectoryPath, bool recursive);

        /// <summary>
        ///     Creates a new directory on the server which is located at the path relative to the working directory.
        /// </summary>
        /// <param name="relativePath">The path of the directory relative to the working directory.</param>
        Task MakeDirectory(string relativePath);

        /// <summary>
        ///     Renames a file or directory which is located at the path relative to the working directory.
        /// </summary>
        /// <param name="relativePath">The path to the file or directory relative to the working directory.</param>
        /// <param name="oldName">The old name of the file or directory.</param>
        /// <param name="newName">The new name of the file or directory.</param>
        Task Rename(string relativePath, string oldName, string newName);

        /// <summary>
        ///     Determines whether a connection to the server can be established, or not.
        /// </summary>
        Task<(bool, Exception)> TestConnection();

        /// <summary>
        ///     Uploads a file into the specified directory, relative to the working directory.
        /// </summary>
        /// <param name="localFilePath">The local path of the file to upload.</param>
        /// <param name="remoteRelativePath">The remote path of the file to upload, relative to the working directory.</param>
        /// <param name="progress">
        ///     The <see cref="IProgress{T}" /> instance that should be used for reporting the upload
        ///     progress.
        /// </param>
        Task UploadFile(string localFilePath, string remoteRelativePath, IProgress<ITransferProgressData> progress);
    }
}