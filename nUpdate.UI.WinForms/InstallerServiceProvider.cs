// InstallerServiceProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using nUpdate.UI.WinForms;
using nUpdate.UpdateInstaller.UserInterface;

[assembly: ServiceProvider(typeof(InstallerServiceProvider))]

namespace nUpdate.UI.WinForms
{
    public class InstallerServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services;

        public InstallerServiceProvider()
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
            _services.Add(typeof(IProgressReporter), new ProgressReporterService());
        }
    }
}