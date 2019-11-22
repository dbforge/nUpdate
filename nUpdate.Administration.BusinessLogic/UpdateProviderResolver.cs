using System;
using System.Collections.Generic;
using System.Reflection;
using nUpdate.Administration.BusinessLogic.Exceptions;
using nUpdate.Administration.BusinessLogic.Ftp;
using nUpdate.Administration.BusinessLogic.Http;
using nUpdate.Administration.Models;

namespace nUpdate.Administration.BusinessLogic
{
    public static class UpdateProviderResolver
    {
        private static readonly Dictionary<UpdateProviderType, IUpdateProvider> InternalTransferProviders =
            new Dictionary<UpdateProviderType, IUpdateProvider>
            {
                {UpdateProviderType.ServerOverHttp, new HttpServerUpdateProvider()},
                {UpdateProviderType.ServerOverFtp, new FtpServerUpdateProvider()},
                {UpdateProviderType.ServerOverSsh, new SshServerUpdateProvider()},
                {UpdateProviderType.GitHub, new GitHubUpdateProvider()},
            };


        public static IUpdateProvider Resolve(UpdateProject project)
        {
            return project.UpdateProviderType == UpdateProviderType.Custom
                ? ResolveCustom(project.TransferAssemblyFilePath, project.CustomTransferProviderClassType)
                : ResolveInternal(project.UpdateProviderType);
        }

        public static IUpdateProvider ResolveInternal(UpdateProviderType updateProtocolType)
        {
            if (updateProtocolType == UpdateProviderType.Custom)
                throw new InvalidOperationException();
            return InternalTransferProviders[updateProtocolType];
        }

        public static IUpdateProvider ResolveCustom(string transferAssemblyFilePath, Type transferProviderClassType)
        {
            if (string.IsNullOrWhiteSpace(transferAssemblyFilePath))
                throw new TransferProtocolException(
                    "The project uses a custom transfer provider, but the path to the file containing the transfer provider is missing.");
            if (!transferAssemblyFilePath.IsValidPath())
                throw new TransferProtocolException(
                    $"The project uses a custom transfer provider, but the path to the file containing the transfer provider is invalid: \"{transferAssemblyFilePath}\"");
            if (transferProviderClassType == null)
                throw new ArgumentNullException(nameof(transferProviderClassType));

            var assembly = Assembly.LoadFrom(transferAssemblyFilePath);
            var serviceProvider = ServiceProviderHelper.CreateServiceProvider(assembly);
            var transferProvider = (IUpdateProvider) serviceProvider.GetService(transferProviderClassType);
            if (transferProvider == null)
                throw new TransferProtocolException("No transfer provider is available in the specified assembly.");
            return transferProvider;
        }
    }
}
