using System;
using System.Collections.Generic;
using nUpdate;
using nUpdate.Administration;
using nUpdate.Administration.ViewModels.NewProject;
using nUpdate.Administration.Views.NewProject;

[assembly: ServiceProvider(typeof(AdministrationServiceProvider))]

namespace nUpdate.Administration
{
    public class AdministrationServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services;

        public AdministrationServiceProvider()
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
            _services.Add(typeof(INewProjectProvider), new NewProjectProvider());
        }
    }
}
