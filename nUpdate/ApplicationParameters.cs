// Copyright © Dominic Beger 2017

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace nUpdate
{
    internal static class ApplicationParameters
    {
        private static string _executablePath;
        private static string _productName;
        private static Version _productVersion;
        private static string _startupPath;

        internal static string ExecutablePath
        {
            get
            {
                if (!string.IsNullOrEmpty(_executablePath))
                    return _executablePath;

                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                    throw new InvalidOperationException("The entry assembly ould not be specified.");

                _executablePath = entryAssembly.Location;
                return _executablePath;
            }
        }

        internal static string ProductName
        {
            get
            {
                if (!string.IsNullOrEmpty(_productName))
                    return _productName;

                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                    throw new InvalidOperationException("The entry assembly could not be specified.");

                var attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attrs.Length <= 0)
                    throw new InvalidOperationException(
                        $"The entry assembly does not define an {nameof(AssemblyProductAttribute)}. Its product name cannot be determined.");

                _productName = ((AssemblyProductAttribute) attrs[0]).Product;
                return _productName;
            }
        }

        internal static Version ProductVersion
        {
            get
            {
                if (_productVersion != null)
                    return _productVersion;

                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                    throw new InvalidOperationException("The entry assembly could not be specified.");

                var attrs =
                    entryAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                if (attrs.Length <= 0)
                    throw new InvalidOperationException(
                        $"The entry assembly does not define an {nameof(AssemblyInformationalVersionAttribute)}. Its product version cannot be determined.");

                var informationalVersionString = ((AssemblyInformationalVersionAttribute) attrs[0])
                    .InformationalVersion;
                if (!string.IsNullOrEmpty(informationalVersionString))
                {
                    _productVersion = new Version(informationalVersionString);
                }
                else
                {
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(ExecutablePath);
                    _productVersion = new Version(fileVersionInfo.ProductVersion);
                }

                return _productVersion;
            }
        }

        internal static string StartupPath
        {
            get
            {
                if (string.IsNullOrEmpty(_startupPath))
                    _startupPath = Path.GetDirectoryName(ExecutablePath);
                return _startupPath;
            }
        }
    }
}