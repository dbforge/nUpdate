using System;
using System.Collections.Generic;
using nUpdate.Administration;
using nUpdate.Administration.Ftp;
using nUpdate.Administration.Http;
using nUpdate.Administration.TransferInterface;

[assembly: ServiceProvider(typeof (TransferServiceProvider))]

namespace nUpdate.Administration
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
            object service;
            return !_services.TryGetValue(serviceType, out service) ? null : service;
        }

        private void InitializeServices()
        {
            _services.Add(typeof (FtpTransferService), new FtpTransferService());
            _services.Add(typeof(HttpTransferService), new HttpTransferService());
        }
    }
}