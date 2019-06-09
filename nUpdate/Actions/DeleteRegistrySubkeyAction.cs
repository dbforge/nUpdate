using System.Collections.Generic;
using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class DeleteRegistrySubkeyAction : IUpdateAction
    {
        public string Name => "DeleteRegistrySubkey";
        public string Description => "Deletes a registry subkey.";

        public string RegistryKey { get; set; }
        public IEnumerable<string> SubkeysToDelete { get; set; }

        public Task Execute(object parameter)
        {
            return Task.Run(() =>
            {
                foreach (var subKey in SubkeysToDelete)
                {
                    RegistryManager.DeleteSubKey(RegistryKey, subKey);
                }
            });
        }
    }
}
