using nUpdate.Operations;

namespace nUpdate.UpdateInstaller.Operations;

/// <summary>Executes the operations of one <see cref="OperationArea" />.</summary>
public interface IOperationHandler
{
    OperationArea Area { get; }

    /// <summary>How many progress steps the operation contributes.</summary>
    int CountTasks(Operation operation);

    void Execute(Operation operation, OperationContext context);
}
