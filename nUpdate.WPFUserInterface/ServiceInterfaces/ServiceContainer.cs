using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace nUpdate.WPFUserInterface.ServiceInterfaces
{
    public class ServiceContainer
    {
        public static readonly ServiceContainer Instance = new ServiceContainer();

        private ServiceContainer()
        {
            _serviceMap = new Dictionary<Type, object>();
            _serviceMapLock = new object();
        }

        public void AddService<TServiceContract>(TServiceContract implementation)
            where TServiceContract : class
        {
            lock (_serviceMapLock)
            {
                _serviceMap[typeof(TServiceContract)] = implementation;
            }
        }

        public TServiceContract GetService<TServiceContract>()
            where TServiceContract : class
        {
            object service;
            lock (_serviceMapLock)
            {
                _serviceMap.TryGetValue(typeof(TServiceContract), out service);
            }
            return service as TServiceContract;
        }

        readonly Dictionary<Type, object> _serviceMap;
        readonly object _serviceMapLock;
    }
}
