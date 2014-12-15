using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace nUpdate.UpdateInstaller.Core
{
    public class ServiceManager
    {
        /// <summary>
        ///     Starts a windows service with the given name. If the service is already running, it will be restarted.
        /// </summary>
        /// <param name="serviceName">The name of the service to start.</param>
        public static void StartService(string serviceName)
        {
            var serviceController = new ServiceController(serviceName);
            if (serviceController.Status == ServiceControllerStatus.Running) // Restart it
            {
                RestartService(serviceName);
            }
            else
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(5000);

                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
        }

        /// <summary>
        ///     Restarts a running windows service.
        /// </summary>
        /// <param name="serviceName">The name of the service to restart.</param>
        public static void RestartService(string serviceName)
        {
            var serviceController = new ServiceController(serviceName);
            int millisec1 = Environment.TickCount;
            TimeSpan timeout = TimeSpan.FromMilliseconds(5000);

            serviceController.Stop();
            serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

            int millisec2 = Environment.TickCount;
            timeout = TimeSpan.FromMilliseconds(5000 - (millisec2 - millisec1));

            serviceController.Start();
            serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        /// <summary>
        ///     Stops a windows service with the given name.
        /// </summary>
        /// <param name="serviceName">The name of the service to stop.</param>
        public static void StopService(string serviceName)
        {
            var serviceController = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(5000);

            serviceController.Stop();
            serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        }
    }
}
