using System.Diagnostics;
using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class StopProcessAction : IUpdateAction
    {
        public string Name => "StopProcess";
        public string Description => "Stops a process.";
        public string ProcessName { get; set; }

        public Task Execute(object parameter)
        {
            return Task.Run(() =>
            {
                var processes = Process.GetProcessesByName(ProcessName);
                foreach (var foundProcess in processes)
                    foundProcess.Kill();
            });
        }
    }
}
