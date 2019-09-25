// TransferManager.cs, 27.07.2019
// Copyright (C) Dominic Beger 25.09.2019

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

        public TransferManager(TransferProviderType transferProviderType, ITransferData data,
            Type customTransferProviderClassType = null, string transferAssemblyFilePath = null)
        {
            _transferProvider = GetTransferProvider(transferProviderType, data, customTransferProviderClassType,
                transferAssemblyFilePath);
        }

        public Task DeleteDirectory(string relativeDirectoryPath)
        {
            return _transferProvider.DeleteDirectory(relativeDirectoryPath);
        }

        public Task DeleteFile(string relativeFileName)
        {
            return _transferProvider.DeleteFile(relativeFileName);
        }

        public Task<bool> DirectoryExists(string relativeDirectoryPath)
        {
            return _transferProvider.DirectoryExists(relativeDirectoryPath);
        }

        public Task<bool> FileExists(string relativeFilePath)
        {
            return _transferProvider.FileExists(relativeFilePath);
        }

        private ITransferProvider GetTransferProvider(UpdateProject project)
        {
            return GetTransferProvider(project.TransferProviderType, project.TransferData,
                project.CustomTransferProviderClassType, project.TransferAssemblyFilePath);
        }

        private ITransferProvider GetTransferProvider(TransferProviderType transferProviderType, ITransferData data,
            Type customTransferProviderClassType,
            string transferAssemblyFilePath)
        {
            var transferProvider = transferProviderType == TransferProviderType.Custom
                ? TransferProviderResolver.ResolveCustom(transferAssemblyFilePath, customTransferProviderClassType)
                : TransferProviderResolver.ResolveInternal(transferProviderType);
            transferProvider.TransferData = data;
            return transferProvider;
        }

        public Task<IEnumerable<IServerItem>> List(string relativeDirectoryPath, bool recursive)
        {
            return _transferProvider.List(relativeDirectoryPath, recursive);
        }

        public Task Rename(string relativePath, string oldName, string newName)
        {
            return _transferProvider.Rename(relativePath, oldName, newName);
        }

        public Task<(bool, Exception)> TestConnection()
        {
            return _transferProvider.TestConnection();
        }
    }
}