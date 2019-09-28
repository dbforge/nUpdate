using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nUpdate.Administration.Common.Ssh
{
    public class SshTransferProvider : ITransferProvider
    {
        public ITransferData TransferData { get; set; }

        public Task DeleteDirectory(string relativeDirectoryPath)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFile(string relativeFilePath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DirectoryExists(string relativeDirectoryPath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FileExists(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IServerItem>> List(string relativeDirectoryPath, bool recursive)
        {
            throw new NotImplementedException();
        }

        public Task MakeDirectory(string relativePath)
        {
            throw new NotImplementedException();
        }

        public Task Rename(string relativePath, string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, Exception)> TestConnection()
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(string localFilePath, string remoteRelativePath, IProgress<ITransferProgressData> progress)
        {
            throw new NotImplementedException();
        }
    }
}
