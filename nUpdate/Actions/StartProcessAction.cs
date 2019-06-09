using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class StartProcessAction : IUpdateAction
    {
        public string Name => "StartProcess";
        public string Description => "Starts a process with optional arguments.";
        public string FilePath { get; set; }
        public IEnumerable<string> Arguments { get; set; }

        public Task Execute(object parameter)
        {
            return Task.Run(() =>
            {
                var pathProvider = (IUpdateActionPathProvider) parameter;
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = pathProvider.AssignPathVariables(FilePath),
                        Arguments = string.Join(" ", Arguments)
                    }
                };
                try
                {
                    process.Start();
                }
                catch (Win32Exception ex)
                {
                    if (ex.NativeErrorCode != 1223)
                        throw;
                }
            });
        }
    }
}
