// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Administration.Application;
using nUpdate.Administration.Exceptions;
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

        public TransferManager(UpdateProject project)
        {
            _transferProvider = GetTransferProvider(project);
        }
        
        public TransferManager(TransferProtocol protocol, ITransferData data)
        {
            _transferProvider = GetTransferProvider(protocol, data);
        }

        private ITransferProvider GetTransferProvider(UpdateProject project)
        {
            return GetTransferProvider(project.TransferProtocol, project.TransferData, project.TransferAssemblyFilePath);
        }

        private ITransferProvider GetTransferProvider(TransferProtocol protocol, ITransferData data, string transferAssemblyFilePath = null)
        {
            switch (protocol)
            {
                case TransferProtocol.FTP:
                    var ftpTransferProvider =
                        (FTPTransferService)GetDefaultServiceProvider().GetService(typeof(FTPTransferService));
                    ftpTransferProvider.Data = (FTPData)data;
                    return ftpTransferProvider;
                case TransferProtocol.HTTP:
                    var httpTransferProvider =
                        (HttpTransferProvider)GetDefaultServiceProvider().GetService(typeof(HttpTransferProvider));
                    httpTransferProvider.Data = (HttpData)data;
                    return httpTransferProvider;
                case TransferProtocol.Custom:
                    if (string.IsNullOrWhiteSpace(transferAssemblyFilePath))
                        throw new TransferProtocolException($"The project uses a custom transfer protocol, but the path to the file containing the transfer services is missing.");
                    if (!transferAssemblyFilePath.IsValidPath())
                        throw new TransferProtocolException($"The project uses a custom transfer protocol, but the path to the file containing the transfer services is invalid: \"{transferAssemblyFilePath}\"");

                    var assembly = Assembly.LoadFrom(transferAssemblyFilePath);
                    var serviceProvider = ServiceProviderHelper.CreateServiceProvider(assembly) ?? GetDefaultServiceProvider();
                    return (ITransferProvider)serviceProvider.GetService(typeof(ITransferProvider));
                default:
                    var availableProtocols =
                        Enum.GetValues(typeof(TransferProtocol)).Cast<TransferProtocol>().Select(t => t.ToString()).ToArray();
                    throw new TransferProtocolException($"The provided transfer protocol ({protocol}) is not defined. Available protocols are {string.Join(", ", availableProtocols, 0, availableProtocols.Length - 1) + " and " + availableProtocols.LastOrDefault()}.");
            }
        }

        private IServiceProvider GetDefaultServiceProvider()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return ServiceProviderHelper.CreateServiceProvider(assembly);
        }
        
        public Task DeleteFile(string fileName)
        {
            return _transferProvider.DeleteFile(fileName);
        }

        public Task DeleteFile(string directoryPath, string fileName)
        {
            return _transferProvider.DeleteFile(directoryPath, fileName);
        }

        public Task DeleteDirectory(string directoryPath)
        {
            return _transferProvider.DeleteDirectory(directoryPath);
        }

        public Task RenameDirectory(string oldName, string newName)
        {
            return _transferProvider.RenameDirectory(oldName, newName);
        }

        public Task MakeDirectory(string name)
        {
            return _transferProvider.MakeDirectory(name);
        }

        public Task MoveContent(string aimPath)
        {
            return _transferProvider.MoveContent(aimPath);
        }
        
        public Task UploadFile(string filePath, IProgress<TransferProgressEventArgs> progress)
        {
            return _transferProvider.UploadFile(filePath, progress);
        }

        public Task UploadPackage(string packagePath, string packageVersion, CancellationToken cancellationToken, IProgress<TransferProgressEventArgs> progress)
        {
            return _transferProvider.UploadPackage(packagePath, packageVersion, cancellationToken, progress);
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

        public Task<bool> TestConnection()
        {
            return _transferProvider.TestConnection();
        }

        public Task<IEnumerable<FtpItem>> List(string path, bool recursive)
        {
            return _transferProvider.List(path, recursive);
        }

        public Task<bool> Exists(string destinationName)
        {
            return _transferProvider.Exists(destinationName);
        }

        public Task<bool> Exists(string directoryPath, string destinationName)
        {
            return _transferProvider.Exists(directoryPath, destinationName);
        }

        public Task UploadFile(Stream fileStream, string remotePath, IProgress<TransferProgressEventArgs> progress)
        {
            return _transferProvider.UploadFile(fileStream, remotePath, progress);
        }
    }
}