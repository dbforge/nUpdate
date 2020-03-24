// DeleteRegistryValueAction.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class DeleteRegistryValueAction : IUpdateAction
    {
        public string RegistryKey { get; set; }
        public string ValueName { get; set; }
        public string Description => "Deletes a value in the registry.";

        public Task Execute()
        {
            return Task.Run(() => { RegistryManager.DeleteValue(RegistryKey, ValueName); });
        }

        public bool ExecuteBeforeReplacingFiles { get; set; }
        public string Name => "DeleteRegistryValue";
    }
}