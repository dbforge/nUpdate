using nUpdate.Installer;
using nUpdate.Operations;

namespace nUpdate.UpdateInstaller.Operations;

/// <summary>Starts and stops Windows services.</summary>
internal sealed class ServiceOperationHandler : IOperationHandler
{
    public OperationArea Area => OperationArea.Services;

    public int CountTasks(Operation operation) =>
        operation is null ? throw new ArgumentNullException(nameof(operation)) : 1;

    public void Execute(Operation operation, OperationContext context)
    {
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        switch (operation)
        {
            case StartServiceOperation start:
                context.Services.ServiceController.StartService(start.ServiceName, start.Arguments.ToArray());
                context.Report(InstallerText.ServiceStart, start.ServiceName);
                break;

            case StopServiceOperation stop:
                context.Services.ServiceController.StopService(stop.ServiceName);
                context.Report(InstallerText.ServiceStop, stop.ServiceName);
                break;

            default:
                throw new NotSupportedException($"The service operation {operation.Type} is not supported.");
        }
    }
}
