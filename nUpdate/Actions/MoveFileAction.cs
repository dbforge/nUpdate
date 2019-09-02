using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using nUpdate.Actions.Exceptions;

namespace nUpdate.Actions
{
    public class MoveFileAction : IUpdateAction
    {
        public string Name => "MoveFile";
        public string Description => "Moves or renames a local file.";
        public bool ExecuteBeforeReplacingFiles { get; set; }
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }

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

                var sourceFilePath = pathProvider.AssignPathVariables(SourceFilePath);
                var destFilePath = pathProvider.AssignPathVariables(DestinationFilePath);
                
                if (File.Exists(sourceFilePath))
                    File.Move(sourceFilePath, destFilePath);
            });
        }
    }
}
