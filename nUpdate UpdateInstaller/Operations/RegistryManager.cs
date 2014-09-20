using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace nUpdate.UpdateInstaller.Operations
{
    internal class RegistryManager
    {
        /// <summary>
        ///     Returns the exception an operation has thrown. If the value equals 'null' no exception had been thrown.
        /// </summary>
        public List<Exception> RegistryExceptions { get; set; }

        public void Create(string keyName)
        {
            RegistryKey mykey;
            mykey = Registry.CurrentUser.CreateSubKey(keyName);
        }

        public void Delete(string keyName)
        {
            Registry.CurrentUser.DeleteSubKey(keyName);
        }

        public void SetValue(string keyName, string keyValue)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            key.SetValue("yourkey", "yourvalue");
        }
    }
}