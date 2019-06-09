using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate.Administration.Common
{
    public interface ITransferProvider
    {
        /// <summary>
        ///     Determines whether a connection to the server can be established, or not.
        /// </summary>
        Task<bool> TestConnection();

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
        ///     Lists the directories and files of the directory at the specified path as an <see cref="System.Collections.Generic.IEnumerable{T}"/>.
        /// </summary>
        Task<IEnumerable<IServerItem>> List(string path, bool recursive);

        /// <summary>
        ///     Renames a directory on the server.
        /// </summary>
        /// <param name="oldName">The old name of the directory.</param>
        /// <param name="newName">The new name of the directory.</param>
        Task RenameDirectory(string oldName, string newName);

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
        ///     Moves all files and subdirectories related to nUpdate Administration from the current directory to the given destination directory.
        /// </summary>
        /// <param name="destinationPath">The destination directory to move the files and subdirectories to.</param>
        /// <param name="availableChannelNames">The available update channel names for selecting the specific update channels that need to be moved, too.</param>
        /// <remarks>This method should only affect specific update data used and created by nUpdate Administration (the master channel, all available update channels and update packages). Other files and/or directories should be ignored.</remarks>
        Task MoveContent(string destinationPath, IEnumerable<string> availableChannelNames);

        /// <summary>
        ///     Determines whether a file or directory exists on the server.
        /// </summary>
        /// <param name="destinationName">The name of the file or folder to check.</param>
        /// <returns><c>true</c> if the file or folder exists, otherwise <c>false</c>.</returns>
        Task<bool> Exists(string destinationName);

        /// <summary>
        ///     Determines whether a file or directory exists on the server.
        /// </summary>
        /// <param name="directoryPath">The directory in which the file should be existing.</param>
        /// <param name="destinationName">The name of the file or folder to check.</param>
        /// <returns><c>true</c> if the file or folder exists, otherwise <c>false</c>.</returns>
        Task<bool> Exists(string directoryPath, string destinationName);

        /// <summary>
        ///     Uploads a file onto the server.
        /// </summary>
        /// <param name="filePath">The local path of the file to upload.</param>
        /// <param name="progress">The <see cref="System.IProgress{T}"/> instance that should be used for reporting the upload progress.</param>
        Task UploadFile(string filePath, IProgress<ITransferProgressData> progress);

        // TODO: Docs and params
        /// <summary>
        ///     Uploads a file onto the server.
        /// </summary>
        /// <param name="fileStream">The <see cref="System.IO.Stream"/> containing the data to upload.</param>
        /// <param name="remotePath">The remote path of the file.</param>
        /// <param name="progress">The <see cref="System.IProgress{T}"/> instance that should be used for reporting the upload progress.</param>
        Task UploadFile(Stream fileStream, string remotePath, IProgress<ITransferProgressData> progress);

        /// <summary>
        ///     Uploads an update package onto the server.
        /// </summary>
        /// <param name="packagePath">The local path of the package.</param>
        /// <param name="guid">The <see cref="System.Guid"/> of the package for specifying the directory name.</param>
        /// <param name="progress">The <see cref="System.IProgress{T}"/> instance that should be used for reporting the upload progress.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance that should be used for cancelling the upload.</param>
        Task UploadPackage(string packagePath, Guid guid, CancellationToken cancellationToken, IProgress<ITransferProgressData> progress);
    }
}