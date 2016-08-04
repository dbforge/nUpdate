using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate.Administration.TransferInterface
{
    public interface ITransferProvider : IDisposable
    {
        /// <summary>
        ///     Determines whether a connection to the server can be established, or not.
        /// </summary>
        Task<bool> TestConnection();

        /// <summary>
        ///     Deletes a file on the server.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        Task DeleteFile(string fileName);

        /// <summary>
        ///     Deletes a file on the server which is located at the specified path.
        /// </summary>
        /// <param name="directoryPath">The path of the directory where the file is located.</param>
        /// <param name="fileName">The name of the file to delete.</param>
        Task DeleteFile(string directoryPath, string fileName);

        /// <summary>
        ///     Deletes a directory on the server.
        /// </summary>
        /// <param name="directoryPath">The name of the directory to delete.</param>
        Task DeleteDirectory(string directoryPath);

        /// <summary>
        ///     Lists the directories and files of the current directory as an <see cref="IEnumerable{FtpItem}"/>.
        /// </summary>
        Task<IEnumerable<FtpItem>> List(string path, bool recursive);

        /// <summary>
        ///     Renames a directory on the server.
        /// </summary>
        /// <param name="oldName">The old name of the directory.</param>
        /// <param name="newName">The new name of the directory.</param>
        Task RenameDirectory(string oldName, string newName);

        /// <summary>
        ///     Creates a new directory on the server.
        /// </summary>
        /// <param name="name">The name of the directory.</param>
        Task MakeDirectory(string name);

        /// <summary>
        ///     Moves all files and subdirectories from the current directory to the given aim directory, if they are update data.
        /// </summary>
        /// <param name="aimPath">The aim directory to move the files and subdirectories to.</param>
        Task MoveContent(string aimPath);

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
        /// <param name="progress">The <see cref="IProgress{TransferProgressEventArgs}"/> instance that should be used for reporting the upload progress.</param>
        Task UploadFile(string filePath, IProgress<TransferProgressEventArgs> progress);

        // TODO: Docs and params
        /// <summary>
        ///     Uploads a file onto the server.
        /// </summary>
        /// <param name="fileStream">The <see cref="Stream"/> containing the data to upload.</param>
        /// <param name="remotePath">The remote path of the file.</param>
        /// <param name="progress">The <see cref="IProgress{TransferProgressEventArgs}"/> instance that should be used for reporting the upload progress.</param>
        Task UploadFile(Stream fileStream, string remotePath, IProgress<TransferProgressEventArgs> progress);

        /// <summary>
        ///     Uploads an update package onto the server.
        /// </summary>
        /// <param name="packagePath">The local path of the package.</param>
        /// <param name="guid">The <see cref="Guid"/> of the package for specifying the directory name.</param>
        /// <param name="progress">The <see cref="IProgress{TransferProgressEventArgs}"/> instance that should be used for reporting the upload progress.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance that should be used for cancelling the upload.</param>
        Task UploadPackage(string packagePath, Guid guid, CancellationToken cancellationToken, IProgress<TransferProgressEventArgs> progress);
    }
}