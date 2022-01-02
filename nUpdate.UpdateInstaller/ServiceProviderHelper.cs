// ServiceProviderHelper.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Linq;
using System.Reflection;
using nUpdate.UpdateInstaller.UIBase;

namespace nUpdate.UpdateInstaller
{
    internal class ServiceProviderHelper
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