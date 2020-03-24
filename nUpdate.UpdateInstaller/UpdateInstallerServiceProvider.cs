// UpdateInstallerServiceProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using nUpdate;
using nUpdate.Actions;
using nUpdate.UpdateInstaller;

[assembly: ServiceProvider(typeof(UpdateInstallerServiceProvider))]

namespace nUpdate.UpdateInstaller
{
    public class UpdateInstallerServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services;

        public UpdateInstallerServiceProvider()
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
            _services.Add(typeof(IUpdateActionPathProvider), new UpdateActionPathProvider());
            _services.Add(typeof(IUpdateActionAppPathProvider), new UpdateActionAppPathProvider());
        }
    }
}