using nUpdate.Installer;
using nUpdate.Operations;

namespace nUpdate.UpdateInstaller.Operations;

/// <summary>Creates and deletes registry keys, sets and deletes values.</summary>
internal sealed class RegistryOperationHandler : IOperationHandler
{
    public OperationArea Area => OperationArea.Registry;

    public int CountTasks(Operation operation)
    {
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));
        return operation switch
        {
            CreateRegistryKeysOperation create => create.SubKeys.Count,
            DeleteRegistryKeysOperation delete => delete.SubKeys.Count,
            SetRegistryValuesOperation set => set.Values.Count,
            DeleteRegistryValuesOperation deleteValues => deleteValues.Names.Count,
            _ => 1,
        };
    }

    public void Execute(Operation operation, OperationContext context)
    {
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var registry = context.Services.Registry;
        switch (operation)
        {
            case CreateRegistryKeysOperation create:
                foreach (var subKey in create.SubKeys)
                {
                    registry.CreateSubKey(create.Key, subKey);
                    context.Report(InstallerText.RegistrySubKeyCreate, subKey);
                }

                break;

            case DeleteRegistryKeysOperation delete:
                foreach (var subKey in delete.SubKeys)
                {
                    registry.DeleteSubKey(delete.Key, subKey);
                    context.Report(InstallerText.RegistrySubKeyDelete, subKey);
                }

                break;

            case SetRegistryValuesOperation set:
                foreach (var value in set.Values)
                {
                    registry.SetValue(set.Key, value);
                    context.Report(InstallerText.RegistryValueSet, value.Name, Describe(value.Value));
                }

                break;

            case DeleteRegistryValuesOperation deleteValues:
                foreach (var valueName in deleteValues.Names)
                {
                    registry.DeleteValue(deleteValues.Key, valueName);
                    context.Report(InstallerText.RegistryValueDelete, valueName);
                }

                break;

            default:
                throw new NotSupportedException($"The registry operation {operation.Type} is not supported.");
        }
    }

    /// <summary>The value for the progress text; <see cref="RegistryValue" /> guarantees the CLR type of each kind.</summary>
    private static string Describe(object? value) => value switch
    {
        null => string.Empty,
        long number => number.ToString(System.Globalization.CultureInfo.InvariantCulture),
        string[] strings => string.Join(", ", strings),
        byte[] bytes => Convert.ToBase64String(bytes),
        _ => (string)value,
    };
}
