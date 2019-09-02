using System.Reflection;
using System.Threading.Tasks;
using nUpdate.Actions.Exceptions;

namespace nUpdate.Actions
{
    public class ExecuteScriptAction : IUpdateAction
    {
        public string Name => "ExecuteScript";
        public string Description => "Executes a C# script.";
        public bool ExecuteBeforeReplacingFiles { get; set; }
        public string Code { get; set; }

        public Task Execute()
        {
            return Task.Run(() =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                var provider = ServiceProviderHelper.CreateServiceProvider(assembly);
                if (provider == null)
                    throw new ServiceProviderMissingException();
                var appPathProvider = (IUpdateActionAppPathProvider)provider.GetService(typeof(IUpdateActionAppPathProvider));
                if (appPathProvider == null)
                    throw new ServiceProviderMissingException(nameof(IUpdateActionAppPathProvider));

                string programFolder = appPathProvider.GetAppPath();
                var helper = new CodeDomHelper();
                helper.ExecuteScript(Code, new object[]{programFolder});
            });
        }
    }
}
