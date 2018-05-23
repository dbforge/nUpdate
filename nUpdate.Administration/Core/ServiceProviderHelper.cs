// Copyright © Dominic Beger 2018

using System;
using System.Linq;
using System.Reflection;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Core
{
    public class ServiceProviderHelper
    {
        public static IServiceProvider CreateServiceProvider(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var attribute =
                assembly.GetCustomAttributes(typeof(ServiceProviderAttribute), false)
                    .Cast<ServiceProviderAttribute>()
                    .SingleOrDefault();

            if (attribute == null)
                return null;

            return (IServiceProvider) Activator.CreateInstance(attribute.ServiceType);
        }
    }
}