using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace nUpdate.Core.Operations
{
    internal class RegistryManager
    {
        private readonly RegistryKey baseRegistryKey = Registry.LocalMachine;
        private readonly string subKey = String.Format("SOFTWARE\\{0}", Application.ProductName);

        /// <summary>
        ///     Returns the exception an operation has thrown. If the value equals 'null' no exception had been thrown.
        /// </summary>
        public Exception RegistryException { get; set; }

        /// <summary>
        ///     Writes an entry to the registry.
        /// </summary>
        /// <param name="keyName">The name of the sub key.</param>
        /// <param name="value">The value to use.</param>
        public void Write(string keyName, object value)
        {
            try
            {
                RegistryKey registryKey = baseRegistryKey;
                RegistryKey sk1 = registryKey.CreateSubKey(subKey);

                sk1.SetValue(keyName.ToUpper(), value);
            }
            catch (Exception ex)
            {
            }
        }
    }
}