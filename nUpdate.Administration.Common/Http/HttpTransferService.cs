using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate.Administration.Common.Http
{
    internal class HttpTransferService : ITransferProvider
    {
        internal HttpData Data { get; set; }

        public Task DeleteDirectory(string directoryName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDirectoryWithPath(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileWithPath(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(string destinationName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(string directoryPath, string destinationName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IServerItem>> List(string path, bool recursive)
        {
            throw new NotImplementedException();
        }

        public Task MakeDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public Task MakeDirectoryWithPath(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task MoveContent(string destinationPath, IEnumerable<string> availableChannelNames)
        {
            throw new NotImplementedException();
        }

        public Task RenameDirectory(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TestConnection()
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(string filePath, IProgress<ITransferProgressData> progress)
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(Stream fileStream, string remotePath, IProgress<ITransferProgressData> progress)
        {
            throw new NotImplementedException();
        }

        public Task UploadPackage(string packagePath, Guid guid, CancellationToken cancellationToken, IProgress<ITransferProgressData> progress)
        {
            throw new NotImplementedException();
        }
    }
}