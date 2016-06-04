// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Administration.Application;
using nUpdate.Administration.Ftp.Service;
using nUpdate.Administration.Http;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration
{
    // ReSharper disable once InconsistentNaming
    internal class TransferManager : ITransferProvider
    {
        private bool _disposed;
        private readonly ITransferProvider _transferProvider;
        private readonly UpdateProject _project;

        public TransferManager(ITransferData data)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var serviceProvider = ServiceProviderHelper.CreateServiceProvider(assembly);
            var ftpTransferProvider =
                            (FTPTransferService)serviceProvider.GetService(typeof(FTPTransferService));
            ftpTransferProvider.Data = (FTPData)data;
            _transferProvider = ftpTransferProvider;
        }

        public TransferManager(UpdateProject updateProject)
        {
            _project = updateProject;
            _transferProvider = GetTransferProvider();
        }

        private ITransferProvider GetTransferProvider()
        {
            if (string.IsNullOrWhiteSpace(Session.ActiveProject.TransferAssemblyFilePath))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var serviceProvider = ServiceProviderHelper.CreateServiceProvider(assembly);
                switch (Session.ActiveProject.Protocol)
                {
                    case TransferProtocol.FTP:
                        var ftpTransferProvider =
                            (FTPTransferService) serviceProvider.GetService(typeof (FTPTransferService));
                        ftpTransferProvider.Data = (FTPData)Session.ActiveProject.TransferData;
                        return ftpTransferProvider;
                    case TransferProtocol.HTTP:
                        var httpTransferProvider =
                            (HttpTransferProvider) serviceProvider.GetService(typeof (HttpTransferProvider));
                        httpTransferProvider.Data = (HttpData)Session.ActiveProject.TransferData;
                        return httpTransferProvider;
                }
            }
            else
            {
                var assembly = Assembly.LoadFrom(Session.ActiveProject.TransferAssemblyFilePath);
                var serviceProvider = ServiceProviderHelper.CreateServiceProvider(assembly) ?? GetDefaultServiceProvider();
                return (ITransferProvider)serviceProvider.GetService(typeof(ITransferProvider));
            }

            return null; // What
        }

        private IServiceProvider GetDefaultServiceProvider()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return ServiceProviderHelper.CreateServiceProvider(assembly);
        }
        
        public async Task DeleteFile(string fileName)
        {
            await _transferProvider.DeleteFile(fileName);
        }

        public async Task DeleteFile(string directoryPath, string fileName)
        {
            await _transferProvider.DeleteFile(directoryPath, fileName);
        }

        public async Task DeleteDirectory(string directoryPath)
        {
            await _transferProvider.DeleteDirectory(directoryPath);
        }

        public async Task RenameDirectory(string oldName, string newName)
        {
            await _transferProvider.RenameDirectory(oldName, newName);
        }

        public async Task MakeDirectory(string name)
        {
            await _transferProvider.MakeDirectory(name);
        }

        public async Task MoveContent(string aimPath)
        {
            await _transferProvider.MoveContent(aimPath);
        }
        
        public async Task UploadFile(string filePath, IProgress<TransferProgressEventArgs> progress)
        {
            await _transferProvider.UploadFile(filePath, progress);
        }

        public async Task UploadPackage(string packagePath, string packageVersion, CancellationToken cancellationToken, IProgress<TransferProgressEventArgs> progress)
        {
            await _transferProvider.UploadPackage(packagePath, packageVersion, cancellationToken, progress);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _transferProvider.Dispose();
            _disposed = true;
        }

        public async Task<bool> TestConnection()
        {
            return await _transferProvider.TestConnection();
        }

        public async Task<IEnumerable<FtpItem>> List(string path, bool recursive)
        {
            return await _transferProvider.List(path, recursive);
        }

        public async Task<bool> Exists(string destinationName)
        {
            return await _transferProvider.Exists(destinationName);
        }

        public async Task<bool> Exists(string directoryPath, string destinationName)
        {
            return await _transferProvider.Exists(directoryPath, destinationName);
        }

        public async Task UploadFile(Stream fileStream, string remotePath, IProgress<TransferProgressEventArgs> progress)
        {
            await _transferProvider.UploadFile(fileStream, remotePath, progress);
        }
    }
}