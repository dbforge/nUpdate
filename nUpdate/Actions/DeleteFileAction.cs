using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using nUpdate.Actions.Exceptions;

namespace nUpdate.Actions
{
    public class DeleteFileAction : IUpdateAction
    {
        public string Name => "DeleteFile";
        public string Description => "Deletes a local file.";
        public bool ExecuteBeforeReplacingFiles { get; set; }
        public string DirectoryPath { get; set; }
        public IEnumerable<string> FileNames { get; set; }

        public Task Execute()
        {
            return Task.Run(() =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                var provider = ServiceProviderHelper.CreateServiceProvider(assembly);
                if (provider == null)
                    throw new ServiceProviderMissingException();
                var pathProvider = (IUpdateActionPathProvider)provider.GetService(typeof(IUpdateActionPathProvider));
                if (pathProvider == null)
                    throw new ServiceProviderMissingException(nameof(IUpdateActionPathProvider));
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
