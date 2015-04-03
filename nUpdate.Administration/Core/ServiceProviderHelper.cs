using System;
using System.Linq;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration.Core
{
    public class ServiceProviderHelper
    {
        public static IServiceProvider CreateServiceProvider(System.Reflection.Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var attribute =
                assembly.GetCustomAttributes(typeof (ServiceProviderAttribute), false)
                    .Cast<ServiceProviderAttribute>()
                    .SingleOrDefault();

            if (attribute == null)
                return null;

            return (IServiceProvider) Activator.CreateInstance(attribute.ServiceType);
        }
    }
}
