using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class StartServiceAction : IUpdateAction
    {
        public string Name => "StartService";
        public string Description => "Starts a service.";
        public bool ExecuteBeforeReplacingFiles { get; set; }
        public string ServiceName { get; set; }
        public IEnumerable<string> Arguments { get; set; }

        public Task Execute()
        {
            return Task.Run(() =>
            {
                ServiceManager.StartService(ServiceName, Arguments.ToArray());
            });
        }
    }
}
