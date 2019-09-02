using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class StopServiceAction : IUpdateAction
    {
        public string Name => "StopProcess";
        public string Description => "Stops a process.";
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
