using System;
using System.Collections.Generic;
using nUpdate;
using nUpdate.Administration.Common;
using nUpdate.Administration.Common.Ftp;
using nUpdate.Administration.Common.Http;

[assembly: ServiceProvider(typeof (TransferServiceProvider))]

namespace nUpdate.Administration.Common
{
    public class TransferServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services;

        public TransferServiceProvider()
        {
            _services = new Dictionary<Type, object>();
            InitializeServices();
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));
            return !_services.TryGetValue(serviceType, out var service) ? null : service;
        }

        private void InitializeServices()
        {
            _services.Add(typeof (FtpTransferProvider), new FtpTransferProvider());
            _services.Add(typeof(HttpTransferProvider), new HttpTransferProvider());
        }
    }
}