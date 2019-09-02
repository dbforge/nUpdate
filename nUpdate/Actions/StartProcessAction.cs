using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using nUpdate.Actions.Exceptions;

namespace nUpdate.Actions
{
    public class StartProcessAction : IUpdateAction
    {
        public string Name => "StartProcess";
        public string Description => "Starts a process with optional arguments.";
        public bool ExecuteBeforeReplacingFiles { get; set; }
        public string FilePath { get; set; }
        public IEnumerable<string> Arguments { get; set; }

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
