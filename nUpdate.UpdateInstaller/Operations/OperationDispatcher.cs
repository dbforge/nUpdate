using nUpdate.Operations;

namespace nUpdate.UpdateInstaller.Operations;

/// <summary>Routes operations to the handler of their area.</summary>
public sealed class OperationDispatcher
{
    private readonly Dictionary<OperationArea, IOperationHandler> _handlers;

    public OperationDispatcher(IEnumerable<IOperationHandler> handlers)
    {
        if (handlers is null)
            throw new ArgumentNullException(nameof(handlers));
        _handlers = [];
        foreach (var handler in handlers)
        {
            if (_handlers.ContainsKey(handler.Area))
                throw new ArgumentException($"There is more than one handler for the area {handler.Area}.",
                    nameof(handlers));
            _handlers[handler.Area] = handler;
        }
    }

    public int CountTasks(IEnumerable<Operation> operations)
    {
        if (operations is null)
            throw new ArgumentNullException(nameof(operations));
        return operations.Sum(operation => Resolve(operation).CountTasks(operation));
    }

    public void Execute(IEnumerable<Operation> operations, OperationContext context)
    {
        if (operations is null)
            throw new ArgumentNullException(nameof(operations));
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        foreach (var operation in operations)
            Resolve(operation).Execute(operation, context);
    }

    private IOperationHandler Resolve(Operation operation) =>
        _handlers.TryGetValue(operation.Area, out var handler)
            ? handler
            : throw new NotSupportedException($"There is no handler for operations in the area {operation.Area}.");
}
