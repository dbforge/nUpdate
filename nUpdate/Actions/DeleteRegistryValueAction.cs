using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class DeleteRegistryValueAction : IUpdateAction
    {
        public string Name => "DeleteRegistryValue";
        public string Description => "Deletes a value in the registry.";
        public bool ExecuteBeforeReplacingFiles { get; set; }

        public string RegistryKey { get; set; }
        public string ValueName { get; set; }

        public Task Execute()
        {
            return Task.Run(() =>
            {
                RegistryManager.DeleteValue(RegistryKey, ValueName);
            });
        }
    }
}
