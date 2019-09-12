// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nUpdate.Administration.Common
{
    // ReSharper disable once InconsistentNaming
    public class TransferManager
    {
        // TODO: Certificate checks

        private readonly ITransferProvider _transferProvider;

        public TransferManager(UpdateProject project)
        {
            _transferProvider = GetTransferProvider(project);
        }

        public TransferManager(TransferProviderType transferProviderType, ITransferData data, Type customTransferProviderClassType = null, string transferAssemblyFilePath = null)
        {
            _transferProvider = GetTransferProvider(transferProviderType, data, customTransferProviderClassType, transferAssemblyFilePath);
        }

        public Task DeleteDirectory(string directoryName)
        {
            return _transferProvider.DeleteDirectory(directoryName);
        }

        public Task DeleteDirectoryWithPath(string directoryPath)
        {
            return _transferProvider.DeleteDirectoryWithPath(directoryPath);
        }

        public Task DeleteFile(string fileName)
        {
            return _transferProvider.DeleteFile(fileName);
        }

        public Task DeleteFileWithPath(string filePath)
        {
            return _transferProvider.DeleteFileWithPath(filePath);
        }

        public Task<IEnumerable<IServerItem>> List(string path, bool recursive)
        {
            return _transferProvider.List(path, recursive);
        }

        public Task MakeDirectory(string directoryName)
        {
            return _transferProvider.MakeDirectory(directoryName);
        }

        public Task MakeDirectoryWithPath(string directoryPath)
        {
            return _transferProvider.MakeDirectoryWithPath(directoryPath);
        }

        public Task UploadFile(string filePath, IProgress<ITransferProgressData> progress)
        {
            return _transferProvider.UploadFile(filePath, progress);
        }

        private ITransferProvider GetTransferProvider(UpdateProject project)
        {
            return GetTransferProvider(project.TransferProviderType, project.TransferData, project.CustomTransferProviderClassType, project.TransferAssemblyFilePath);
        }

        private ITransferProvider GetTransferProvider(TransferProviderType transferProviderType, ITransferData data, Type customTransferProviderClassType,
            string transferAssemblyFilePath)
        {
            var transferProvider = transferProviderType == TransferProviderType.Custom
                ? TransferProviderResolver.ResolveCustom(transferAssemblyFilePath, customTransferProviderClassType)
                : TransferProviderResolver.ResolveInternal(transferProviderType);
            transferProvider.TransferData = data;
            return transferProvider;
        }

        public Task<(bool, Exception)> TestConnection()
        {
            return _transferProvider.TestConnection();
        }

        public Task Rename(string oldName, string newName)
        {
            return _transferProvider.Rename(oldName, newName);
        }

        public Task RenameAtPath(string path, string oldName, string newName)
        {
            return _transferProvider.RenameAtPath(path, oldName, newName);
        }

        public Task<bool> FileExistsAtPath(string filePath)
        {
            return _transferProvider.FileExistsAtPath(filePath);
        }

        public Task<bool> FileExists(string fileName)
        {
            return _transferProvider.FileExists(fileName);
        }

        public Task<bool> DirectoryExistsAtPath(string directoryPath)
        {
            return _transferProvider.DirectoryExistsAtPath(directoryPath);
        }

        public Task<bool> DirectoryExists(string destinationName)
        {
            return _transferProvider.DirectoryExists(destinationName);
        }
    }
}