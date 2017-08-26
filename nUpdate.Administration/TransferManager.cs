// Author: Dominic Beger (Trade/ProgTrade) 2017

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using nUpdate.Administration.Exceptions;
using nUpdate.Administration.Ftp;
using nUpdate.Administration.Http;

namespace nUpdate.Administration
{
    // ReSharper disable once InconsistentNaming
    public class TransferManager : ITransferProvider
    {
        // TODO: Certificate checks

        private readonly ITransferProvider _transferProvider;

        public TransferManager(UpdateProject project)
        {
            _transferProvider = GetTransferProvider(project);
        }

        public TransferManager(TransferProtocol protocol, ITransferData data)
        {
            _transferProvider = GetTransferProvider(protocol, data);
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

        public Task<bool> Exists(string destinationName)
        {
            return _transferProvider.Exists(destinationName);
        }

        public Task<bool> Exists(string directoryPath, string destinationName)
        {
            return _transferProvider.Exists(directoryPath, destinationName);
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

        public Task MoveContent(string destinationPath, IEnumerable<string> availableUpdateChannels)
        {
            return _transferProvider.MoveContent(destinationPath, availableUpdateChannels);
        }

        public Task RenameDirectory(string oldName, string newName)
        {
            return _transferProvider.RenameDirectory(oldName, newName);
        }

        public Task<bool> TestConnection()
        {
            return _transferProvider.TestConnection();
        }

        public Task UploadFile(string filePath, IProgress<ITransferProgressData> progress)
        {
            return _transferProvider.UploadFile(filePath, progress);
        }

        public Task UploadFile(Stream fileStream, string remotePath, IProgress<ITransferProgressData> progress)
        {
            return _transferProvider.UploadFile(fileStream, remotePath, progress);
        }

        public Task UploadPackage(string packagePath, Guid guid, CancellationToken cancellationToken,
            IProgress<ITransferProgressData> progress)
        {
            return _transferProvider.UploadPackage(packagePath, guid, cancellationToken, progress);
        }

        private ITransferProvider GetTransferProvider(UpdateProject project)
        {
            return GetTransferProvider(project.TransferProtocol, project.TransferData, project.TransferAssemblyFilePath);
        }

        private ITransferProvider GetTransferProvider(TransferProtocol protocol, ITransferData data,
            string transferAssemblyFilePath = null)
        {
            switch (protocol)
            {
                case TransferProtocol.FTP:
                    var ftpTransferProvider =
                        (FtpTransferService) GetDefaultServiceProvider().GetService(typeof (FtpTransferService));
                    ftpTransferProvider.Data = (FtpData) data;
                    return ftpTransferProvider;
                case TransferProtocol.HTTP:
                    var httpTransferProvider =
                        (HttpTransferService) GetDefaultServiceProvider().GetService(typeof (HttpTransferService));
                    httpTransferProvider.Data = (HttpData) data;
                    return httpTransferProvider;
                case TransferProtocol.Custom:
                    if (string.IsNullOrWhiteSpace(transferAssemblyFilePath))
                        throw new TransferProtocolException(
                            "The project uses a custom transfer protocol, but the path to the file containing the transfer services is missing.");
                    if (!transferAssemblyFilePath.IsValidPath())
                        throw new TransferProtocolException(
                            $"The project uses a custom transfer protocol, but the path to the file containing the transfer services is invalid: \"{transferAssemblyFilePath}\"");

                    var assembly = Assembly.LoadFrom(transferAssemblyFilePath);
                    var serviceProvider = ServiceProviderHelper.CreateServiceProvider(assembly) ??
                                          GetDefaultServiceProvider();
                    return (ITransferProvider) serviceProvider.GetService(typeof (ITransferProvider));
                default:
                    var availableProtocols =
                        Enum.GetValues(typeof (TransferProtocol))
                            .Cast<TransferProtocol>()
                            .Select(t => t.ToString())
                            .ToArray();
                    throw new TransferProtocolException(
                        $"The provided transfer protocol ({protocol}) is not defined. Available protocols are {string.Join(", ", availableProtocols, 0, availableProtocols.Length - 1) + " and " + availableProtocols.LastOrDefault()}.");
            }
        }

        private IServiceProvider GetDefaultServiceProvider()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return ServiceProviderHelper.CreateServiceProvider(assembly);
        }
    }
}