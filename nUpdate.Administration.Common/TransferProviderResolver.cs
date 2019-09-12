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
        private static readonly Dictionary<TransferProviderType, ITransferProvider> InternalTransferProviders =
            new Dictionary<TransferProviderType, ITransferProvider>
            {
                {TransferProviderType.Http, new HttpTransferProvider()},
                {TransferProviderType.Ftp, new FtpTransferProvider()},
                {TransferProviderType.GitHub, new GitHubTransferProvider() }
            };

        public static ITransferProvider ResolveInternal(TransferProviderType transferProviderType)
        {
            if (transferProviderType == TransferProviderType.Custom)
                throw new InvalidOperationException();
            return InternalTransferProviders[transferProviderType];
        }

        public static ITransferProvider ResolveCustom(string transferAssemblyFilePath, Type transferProviderClassType)
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
            var transferProvider = (ITransferProvider) serviceProvider.GetService(transferProviderClassType);
            if (transferProvider == null)
                throw new TransferProtocolException("No transfer provider is available in the specified assembly.");
            return transferProvider;
        }
    }
}
