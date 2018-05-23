// Copyright © Dominic Beger 2018

using System;
using System.ServiceProcess;

namespace nUpdate.UpdateInstaller.Core
{
    public class ServiceManager
    {
        /// <summary>
        ///     Restarts a running windows service.
        /// </summary>
        /// <param name="serviceName">The name of the service to restart.</param>
        public static void RestartService(string serviceName)
        {
            var serviceController = new ServiceController(serviceName);
            var millisec1 = Environment.TickCount;
            var timeout = TimeSpan.FromMilliseconds(5000);

            serviceController.Stop();
            serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

            var millisec2 = Environment.TickCount;
            timeout = TimeSpan.FromMilliseconds(5000 - (millisec2 - millisec1));

            serviceController.Start();
            serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        /// <summary>
        ///     Starts a windows service with the given name. If the service is already running, it will be restarted.
        /// </summary>
        /// <param name="serviceName">The name of the service to start.</param>
        /// <param name="arguments">The arguments to handle over.</param>
        public static void StartService(string serviceName, string[] arguments)
        {
            var serviceController = new ServiceController(serviceName);
            if (serviceController.Status == ServiceControllerStatus.Running) // Restart it
            {
                RestartService(serviceName);
            }
            else
            {
                var timeout = TimeSpan.FromMilliseconds(5000);

                if (arguments != null || arguments.Length != 0)
                    serviceController.Start(arguments);
                else
                    serviceController.Start();

                serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
        }

        /// <summary>
        ///     Stops a windows service with the given name.
        /// </summary>
        /// <param name="serviceName">The name of the service to stop.</param>
        public static void StopService(string serviceName)
        {
            var serviceController = new ServiceController(serviceName);
            var timeout = TimeSpan.FromMilliseconds(5000);

            serviceController.Stop();
            serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        }
    }
}