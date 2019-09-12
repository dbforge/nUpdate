using System;
using System.Collections.Generic;
using System.Reflection;
using nUpdate.Administration.Common.Exceptions;
using nUpdate.Administration.Common.Ftp;
using nUpdate.Administration.Common.Http;

namespace nUpdate.Administration.Common
{
    public static class TransferProviderResolver
    {
        private static readonly Dictionary<TransferProviderType, Type> InternalTransferProviders =
            new Dictionary<TransferProviderType, Type>
            {
                {TransferProviderType.Http, typeof(HttpTransferProvider)},
                {TransferProviderType.Ftp, typeof(FtpTransferProvider)},
                {TransferProviderType.GitHub, typeof(GitHubTransferProvider) }
            };

        public static ITransferProvider ResolveInternal(TransferProviderType transferProviderType)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var serviceProvider = ServiceProviderHelper.CreateServiceProvider(assembly);
            return (ITransferProvider)serviceProvider.GetService(InternalTransferProviders[transferProviderType]);
        }

        public static ITransferProvider ResolveCustom(string transferAssemblyFilePath, Type transferProviderClassType)
        {
            if (string.IsNullOrWhiteSpace(transferAssemblyFilePath))
                throw new TransferProtocolException(
                    "The project uses a custom transfer provider, but the path to the file containing the transfer provider is missing.");
            if (!transferAssemblyFilePath.IsValidPath())
                throw new TransferProtocolException(
                    $"The project uses a custom transfer provider, but the path to the file containing the transfer provider is invalid: \"{transferAssemblyFilePath}\"");

            var assembly = Assembly.LoadFrom(transferAssemblyFilePath);
            var serviceProvider = ServiceProviderHelper.CreateServiceProvider(assembly);
            var transferProvider = (ITransferProvider) serviceProvider.GetService(transferProviderClassType);
            if (transferProvider == null)
                throw new TransferProtocolException("No transfer provider is available in the specified assembly.");
            return transferProvider;
        }
    }
}
