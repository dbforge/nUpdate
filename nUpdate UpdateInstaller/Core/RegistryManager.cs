// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Linq;
using Microsoft.Win32;

namespace nUpdate.UpdateInstaller.Core
{
    public class RegistryManager
    {
        /// <summary>
        ///     Gets the name of the root key relating to the given name.
        /// </summary>
        /// <param name="name">The name of the root key.</param>
        /// <returns>Returns a new <see cref="RegistryKey" />-instance of the found relating key.</returns>
        private static RegistryKey GetRootKeyByName(string name)
        {
            switch (name)
            {
                case "HKEY_CLASSES_ROOT":
                    return Registry.ClassesRoot;
                case "HKEY_CURRENT_USER":
                    return Registry.CurrentUser;
                case "HKEY_LOCAL_MACHINE":
                    return Registry.LocalMachine;
            }
            return null;
        }

        /// <summary>
        ///     Creates a new sub key at a given key path.
        /// </summary>
        public static void CreateSubKey(string keyPath, string subKeyName)
        {
            var key = GetRootKeyByName(keyPath.Split('\\')[0]);
            key.CreateSubKey(subKeyName);
        }

        /// <summary>
        ///     Deletes a sub key at the given path.
        /// </summary>
        /// <param name="keyPath">The path of the key to use.</param>
        /// <param name="subKey">The sub key to delete.</param>
        public static void DeleteSubKey(string keyPath, string subKey)
        {
            var key = GetRootKeyByName(keyPath.Split('\\')[0]);
            key.OpenSubKey(String.Join("\\", keyPath.Split('\\').Where(item => item != key.Name)));
            key.DeleteSubKeyTree(subKey);
        }

        /// <summary>
        ///     Sets the value of a name-value-pair. If the pair does not already exist, it will be created.
        /// </summary>
        /// <param name="keyPath">The path of the key to use.</param>
        /// <param name="valueName">The name of the value.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="valueKind">The kind/type of the value.</param>
        public static void SetValue(string keyPath, string valueName, object value, RegistryValueKind valueKind)
        {
            Registry.SetValue(keyPath, valueName, value, valueKind);
        }

        /// <summary>
        ///     Deletes a value of a name-value pair.
        /// </summary>
        /// <param name="keyPath">The path of the key to use.</param>
        /// <param name="valueName">The name of the value.</param>
        public static void DeleteValue(string keyPath, string valueName)
        {
            var key = GetRootKeyByName(keyPath.Split('\\')[0]);
            key.OpenSubKey(String.Join("\\", keyPath.Split('\\').Where(item => item != key.Name)));
            key.DeleteValue(valueName, false);
        }
    }
}