// SetRegistryValueAction.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Threading.Tasks;
using Microsoft.Win32;

namespace nUpdate.Actions
{
    public class SetRegistryValueAction : IUpdateAction
    {
        public string RegistryKey { get; set; }
        public object Value { get; set; }
        public RegistryValueKind ValueKind { get; set; }
        public string ValueName { get; set; }
        public string Description => "Sets a value in the registry.";

        public Task Execute()
        {
            return Task.Run(() => { RegistryManager.SetValue(RegistryKey, ValueName, Value, ValueKind); });
        }

        public bool ExecuteBeforeReplacingFiles { get; set; }
        public string Name => "SetRegistryValue";
    }
}