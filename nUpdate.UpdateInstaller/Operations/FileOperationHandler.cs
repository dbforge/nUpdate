using nUpdate.Installer;
using nUpdate.Operations;

namespace nUpdate.UpdateInstaller.Operations;

/// <summary>Deletes and renames files.</summary>
internal sealed class FileOperationHandler : IOperationHandler
{
    public OperationArea Area => OperationArea.Files;

    public int CountTasks(Operation operation)
    {
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));
        return operation is DeleteFilesOperation delete ? delete.Files.Count : 1;
    }

    public void Execute(Operation operation, OperationContext context)
    {
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var fileSystem = context.Services.FileSystem;
        switch (operation)
        {
            case DeleteFilesOperation delete:
                var directory = context.Paths.Resolve(delete.Directory);
                foreach (var fileName in delete.Files)
                {
                    var path = fileSystem.Path.Combine(directory, fileName);
                    if (fileSystem.File.Exists(path))
                        fileSystem.File.Delete(path);
                    context.Report(InstallerText.FileDeleting, fileName);
                }

                break;

            case RenameFileOperation rename:
                var source = context.Paths.Resolve(rename.Path);
                if (string.IsNullOrEmpty(rename.NewName))
                    throw new InvalidOperationException($"The rename operation for \"{rename.Path}\" has no new name.");
                var sourceName = fileSystem.Path.GetFileName(source);
                if (fileSystem.File.Exists(source))
                {
                    var target = source.Substring(0, source.Length - sourceName.Length) + rename.NewName;
                    if (fileSystem.File.Exists(target))
                        fileSystem.File.Delete(target);
                    fileSystem.File.Move(source, target);
                }

                context.Report(InstallerText.FileRenaming, sourceName, rename.NewName);
                break;

            default:
                throw new NotSupportedException($"The file operation {operation.Type} is not supported.");
        }
    }
}
