// GitHubTransferProvider.cs, 10.09.2019
// Copyright (C) Dominic Beger 25.09.2019

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nUpdate.Administration.Common
{
    public class GitHubTransferProvider : ITransferProvider
    {
        public Task DeleteDirectory(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DirectoryExists(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FileExists(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IServerItem>> List(string path, bool recursive)
        {
            throw new NotImplementedException();
        }

        public Task MakeDirectory(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public Task Rename(string path, string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public Task<(bool, Exception)> TestConnection()
        {
            throw new NotImplementedException();
        }

        public ITransferData TransferData { get; set; }

        public Task UploadFile(string localFilePath, string remoteRelativePath,
            IProgress<ITransferProgressData> progress)
        {
            throw new NotImplementedException();
        }
    }
}