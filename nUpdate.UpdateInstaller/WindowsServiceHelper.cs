using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nUpdate.UpdateInstaller
{
    public static class WindowsServiceHelper
    {
        private static bool? _isRunningInServiceContext;

        /// <summary>
        /// Gets a value indicating, if the application runs in service context
        /// </summary>
        public static bool IsRunningInServiceContext => _isRunningInServiceContext ?? (_isRunningInServiceContext = DetermineIfRunningInServiceContext()).Value; 



        private static bool DetermineIfRunningInServiceContext()
        {
            if (!Environment.UserInteractive) return true;

            // https://stackoverflow.com/questions/1188658/how-can-a-c-sharp-windows-console-application-tell-if-it-is-run-interactively
            if (Console.OpenStandardInput(1) == Stream.Null) return true;


            //https://stackoverflow.com/questions/13296129/detect-if-application-is-running-under-system-account
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                if (identity.IsSystem) return true;
            }

            return false;
        }
    }
}
