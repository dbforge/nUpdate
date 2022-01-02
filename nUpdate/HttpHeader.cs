using System;
using System.Reflection;

namespace nUpdate
{
    public class HttpHeader
    {
        /// <summary>
        ///     Returns the default user agent string of the application.
        /// </summary>
        /// <returns>The default user agent string.</returns>
        public static string GetUserAgent()
        {
            AssemblyName assembly = Assembly.GetEntryAssembly()?.GetName() ?? 
                                    Assembly.GetCallingAssembly().GetName();

            string userAgent = $"{assembly.Name}/{assembly.Version} " +
                               $"({Environment.OSVersion}; {(Environment.Is64BitOperatingSystem ? "x64" : "x86")}; Runtime/{Environment.Version})";

            return userAgent;
        }
    }
}