using nUpdate.Installer;
using nUpdate.Operations;

namespace nUpdate.UpdateInstaller.Operations;

/// <summary>Starts processes, optionally waiting for them and failing on an error exit code, and terminates processes.</summary>
internal sealed class ProcessOperationHandler : IOperationHandler
{
    public OperationArea Area => OperationArea.Processes;

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
            case StartProcessOperation start:
                var path = context.Paths.Resolve(start.Path);
                if (start.WaitForExit)
                {
                    context.Status(InstallerText.ProcessWaiting, start.Path);
                    var exitCode = context.Services.ProcessService.Run(path, start.Arguments);
                    if (start.FailOnError && exitCode != 0)
                        throw new InvalidOperationException(context.Options.Text(InstallerText.ProcessExitCodeError,
                            start.Path, exitCode));
                }
                else
                {
                    context.Services.ProcessService.Start(path, start.Arguments);
                }

                context.Report(InstallerText.ProcessStart, start.Path);
                break;

            case TerminateProcessOperation terminate:
                context.Services.ProcessService.Kill(terminate.ProcessName);
                context.Report(InstallerText.ProcessStop, terminate.ProcessName);
                break;

            default:
                throw new NotSupportedException($"The process operation {operation.Type} is not supported.");
        }
    }
}
