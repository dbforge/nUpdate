using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class DeleteFileAction : IUpdateAction
    {
        public string Name => "DeleteFile";
        public string Description => "Deletes a local file.";
        public string DirectoryPath { get; set; }
        public IEnumerable<string> FileNames { get; set; }

        public Task Execute(object parameter)
        {
            return Task.Run(() =>
            {
                var pathProvider = (IUpdateActionPathProvider) parameter;
                var specializedPath = pathProvider.AssignPathVariables(DirectoryPath);

                foreach (var file in FileNames)
                {
                    string filePath = Path.Combine(specializedPath, file);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            });
        }
    }
}
