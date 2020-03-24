// CreateRegistrySubkeyAction.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Collections.Generic;
using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class CreateRegistrySubkeyAction : IUpdateAction
    {
        public string RegistryKey { get; set; }
        public IEnumerable<string> SubkeysToCreate { get; set; }
        public string Description => "Creates a registry subkey.";

        public Task Execute()
        {
            return Task.Run(() =>
            {
                foreach (var subKey in SubkeysToCreate) RegistryManager.CreateSubKey(RegistryKey, subKey);
            });
        }

        public bool ExecuteBeforeReplacingFiles { get; set; }
        public string Name => "CreateRegistrySubkey";
    }
}