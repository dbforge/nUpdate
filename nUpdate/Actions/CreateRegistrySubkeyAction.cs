using System.Collections.Generic;
using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class CreateRegistrySubkeyAction : IUpdateAction
    {
        public string Name => "CreateRegistrySubkey";
        public string Description => "Creates a registry subkey.";

        public string RegistryKey { get; set; }
        public IEnumerable<string> SubkeysToCreate { get; set; }

        public Task Execute(object parameter)
        {
            return Task.Run(() =>
            {
                foreach (var subKey in SubkeysToCreate)
                {
                    RegistryManager.CreateSubKey(RegistryKey, subKey);
                }
            });
        }
    }
}
