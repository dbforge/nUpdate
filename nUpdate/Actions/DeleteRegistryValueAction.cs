using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class DeleteRegistryValueAction : IUpdateAction
    {
        public string Name => "DeleteRegistryValue";
        public string Description => "Deletes a value in the registry.";

        public string RegistryKey { get; set; }
        public string ValueName { get; set; }

        public Task Execute(object parameter)
        {
            return Task.Run(() =>
            {
                RegistryManager.DeleteValue(RegistryKey, ValueName);
            });
        }
    }
}
