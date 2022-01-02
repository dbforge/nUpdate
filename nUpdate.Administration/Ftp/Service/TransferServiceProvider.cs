// TransferServiceProvider.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Collections.Generic;
using nUpdate.Administration.Ftp.Service;
using nUpdate.Administration.TransferInterface;

[assembly: ServiceProvider(typeof(TransferServiceProvider))]

namespace nUpdate.Administration.Ftp.Service
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
            _services.Add(typeof(ITransferProvider), new FtpTransferService());
        }
    }
}