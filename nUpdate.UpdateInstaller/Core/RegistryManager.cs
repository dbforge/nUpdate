// Copyright © Dominic Beger 2018

using System;
using System.Linq;
using Microsoft.Win32;

namespace nUpdate.UpdateInstaller.Core
{
    public class RegistryManager
    {
        /// <summary>
        ///     Creates a new sub key at a given key path.
        /// </summary>
        public static void CreateSubKey(string keyPath, string subKeyName)
        {
            var keyParts = keyPath.Split('\\');
            using (var key = GetRootKeyByName(keyParts[0]))
            {
                var subKeyPath = string.Join("\\", keyParts.Where(item => item != keyParts[0]));
                using (var subKey = key.OpenSubKey(subKeyPath, true))
                {
                    if (subKey == null)
                        throw new Exception($"The sub key \"{subKeyPath}\" couldn't be found.");

                    subKey.CreateSubKey(subKeyName);
                }
            }
        }

        /// <summary>
        ///     Deletes a sub key at the given path.
        /// </summary>
        /// <param name="keyPath">The path of the key to use.</param>
        /// <param name="subKeyName">The sub key to delete.</param>
        public static void DeleteSubKey(string keyPath, string subKeyName)
        {
            var keyParts = keyPath.Split('\\');
            using (var key = GetRootKeyByName(keyParts[0]))
            {
                var subKeyPath = string.Join("\\", keyParts.Where(item => item != keyParts[0]));
                using (var subKey = key.OpenSubKey(subKeyPath, true))
                {
                    if (subKey == null)
                        throw new Exception($"The sub key \"{subKeyPath}\" couldn't be found.");

                    subKey.DeleteSubKeyTree(subKeyName, false);
                }
            }
        }

        /// <summary>
        ///     Deletes a value of a name-value pair.
        /// </summary>
        /// <param name="keyPath">The path of the key to use.</param>
        /// <param name="valueName">The name of the value.</param>
        public static void DeleteValue(string keyPath, string valueName)
        {
            var keyParts = keyPath.Split('\\');
            using (var key = GetRootKeyByName(keyParts[0]))
            {
                var subKeyPath = string.Join("\\", keyParts.Where(item => item != keyParts[0]));
                using (var subKey = key.OpenSubKey(subKeyPath, true))
                {
                    if (subKey == null)
                        throw new Exception($"The sub key \"{subKeyPath}\" couldn't be found.");

                    subKey.DeleteValue(valueName, false);
                }
            }
        }

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
        ///     Sets the value of a name-value-pair. If the pair does not already exist, it will be created.
        /// </summary>
        /// <param name="keyPath">The path of the key to use.</param>
        /// <param name="valueName">The name of the value.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="valueKind">The kind/type of the value.</param>
        public static void SetValue(string keyPath, string valueName, object value, RegistryValueKind valueKind)
        {
            var keyParts = keyPath.Split('\\');
            using (var key = GetRootKeyByName(keyParts[0]))
            {
                var subKeyPath = string.Join("\\", keyParts.Where(item => item != keyParts[0]));
                using (var subKey = key.OpenSubKey(subKeyPath, true))
                {
                    if (subKey == null)
                        throw new Exception($"The sub key \"{subKeyPath}\" couldn't be found.");

                    var newValue = value;
                    switch (valueKind) // Special value kinds
                    {
                        case RegistryValueKind.Binary:
                            var binaryValueStrings = ((string) value).Split(',');
                            newValue = binaryValueStrings.Select(v => Convert.ToByte(v.Trim())).ToArray();
                            break;
                        case RegistryValueKind.MultiString:
                            newValue = ((string) value).Split(',').Select(s => s.Trim()).ToArray();
                            break;
                    }

                    subKey.SetValue(valueName, newValue, valueKind);
                }
            }
        }
    }
}