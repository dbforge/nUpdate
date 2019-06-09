using System.IO;
using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class MoveFileAction : IUpdateAction
    {
        public string Name => "MoveFile";
        public string Description => "Moves or renames a local file.";
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }

        public Task Execute(object parameter)
        {
            return Task.Run(() =>
            {
                var pathProvider = (IUpdateActionPathProvider)parameter;
                var sourceFilePath = pathProvider.AssignPathVariables(SourceFilePath);
                var destFilePath = pathProvider.AssignPathVariables(DestinationFilePath);
                
                if (File.Exists(sourceFilePath))
                    File.Move(sourceFilePath, destFilePath);
            });
        }
    }
}
