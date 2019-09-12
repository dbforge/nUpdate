using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nUpdate.Administration.Common
{
    internal class GitHubTransferProvider : ITransferProvider
    {
        public ITransferData TransferData { get; set; }
        public Task<(bool, Exception)> TestConnection()
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileWithPath(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDirectoryWithPath(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDirectory(string directoryName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IServerItem>> List(string path, bool recursive)
        {
            throw new NotImplementedException();
        }

        public Task Rename(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public Task RenameAtPath(string path, string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public Task MakeDirectoryWithPath(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task MakeDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FileExistsAtPath(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FileExists(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DirectoryExistsAtPath(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DirectoryExists(string destinationName)
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(string filePath, IProgress<ITransferProgressData> progress)
        {
            throw new NotImplementedException();
        }
    }
}
