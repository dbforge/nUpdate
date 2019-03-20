using System.Threading.Tasks;
using Microsoft.Win32;

namespace nUpdate.Actions
{
    public class SetRegistryValueAction : IUpdateAction
    {
        public string Name => "SetRegistryValue";
        public string Description => "Sets a value in the registry.";

        public string RegistryKey { get; set; }
        public string ValueName { get; set; }
        public object Value { get; set; }
        public RegistryValueKind ValueKind { get; set; }

        public Task Execute(object parameter)
        {
            return Task.Run(() =>
            {
                RegistryManager.SetValue(RegistryKey, ValueName, Value, ValueKind);
            });
        }
    }
}
