using System;
using System.Collections.Generic;
using nUpdate.UpdateInstaller.Client.GuiInterface;
using nUpdate.UpdateInstaller.Core;

[assembly: ServiceProvider(typeof(InstallerServiceProvider))]
namespace nUpdate.UpdateInstaller.Core
{
    public class InstallerServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services;

        public InstallerServiceProvider()
        {
            _services = new Dictionary<Type, object>();
            InitializeServices();
        }

        private void InitializeServices()
        {
            _services.Add(typeof(IProgressReporter), new ProgressReporterService());
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            object service;
            return !_services.TryGetValue(serviceType, out service) ? null : service;
        }
    }
}
