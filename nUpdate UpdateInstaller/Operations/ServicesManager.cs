using System;
using System.ServiceProcess;

namespace nUpdate.UpdateInstaller.Operations
{
    public class ServicesManager
    {
        /// <summary>
        ///     Returns the exception that was caused by an operation. If the value equals 'null' no exception had been returned.
        /// </summary>
        public Exception ServiceException { get; set; }

        /// <summary>
        ///     Starts a new windows service.
        /// </summary>
        public void StartService(string serviceName, int timeoutMilliseconds)
        {
            var service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                ServiceException = ex;
            }
        }

        /// <summary>
        ///     Stops a running windows service.
        /// </summary>
        public void StopService(string serviceName, int timeoutMilliseconds)
        {
            var service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                ServiceException = ex;
            }
        }

        /// <summary>
        ///     Restarts a running service.
        /// </summary>
        public void RestartService(string serviceName, int timeoutMilliseconds)
        {
            var service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                ServiceException = ex;
            }
        }
    }
}