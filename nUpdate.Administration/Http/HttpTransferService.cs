using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Http
{
    internal class HttpTransferService : ITransferProvider
    {
        internal HttpData Data { get; set; }

        public Task DeleteDirectory(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFile(string directoryPath, string fileName)
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

        public Task<IEnumerable<FtpItem>> List(string path, bool recursive)
        {
            throw new NotImplementedException();
        }

        public Task MakeDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public Task MoveContent(string aimPath)
        {
            throw new NotImplementedException();
        }

        public Task RenameDirectory(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public Task TestConnection()
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(string filePath, IProgress<TransferProgressEventArgs> progress)
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(Stream fileStream, string remotePath, IProgress<TransferProgressEventArgs> progress)
        {
            throw new NotImplementedException();
        }

        public Task UploadPackage(string packagePath, string packageVersionString, CancellationToken cancellationToken, IProgress<TransferProgressEventArgs> progress)
        {
            throw new NotImplementedException();
        }

        Task<bool> ITransferProvider.TestConnection()
        {
            throw new NotImplementedException();
        }
    }
}