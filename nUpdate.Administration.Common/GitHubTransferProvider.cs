using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nUpdate.Administration.Common
{
    public class GitHubTransferProvider : ITransferProvider
    {
        public ITransferData TransferData { get; set; }
        public Task<(bool, Exception)> TestConnection()
        {
            throw new NotImplementedException();
        }

        public Task DeleteFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileInWorkingDirectory(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDirectory(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDirectoryInWorkingDirectory(string directoryName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IServerItem>> List(string path, bool recursive)
        {
            throw new NotImplementedException();
        }

        public Task RenameInWorkingDirectory(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public Task Rename(string path, string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public Task MakeDirectory(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task MakeDirectoryInWorkingDirectory(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FileExists(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FileExistsInWorkingDirectory(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DirectoryExists(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DirectoryExistsInWorkingDirectory(string destinationName)
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(string filePath, IProgress<ITransferProgressData> progress)
        {
            throw new NotImplementedException();
        }
    }
}
