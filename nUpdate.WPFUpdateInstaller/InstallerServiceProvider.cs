using System;
using System.Collections.Generic;
using nUpdate.UpdateInstaller.Client.GuiInterface;
using nUpdate.WPFUpdateInstaller;


[assembly: ServiceProvider(typeof(InstallerServiceProvider))]

namespace nUpdate.WPFUpdateInstaller
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
            var application = new App();
            application.InitializeComponent();
            
            _services.Add(typeof(IProgressReporter), new ProgressReporterService());
        }
    }
}
