// StopProcessAction.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Diagnostics;
using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class StopProcessAction : IUpdateAction
    {
        public string ProcessName { get; set; }
        public string Description => "Stops a process.";

        public Task Execute()
        {
            return Task.Run(() =>
            {
                var processes = Process.GetProcessesByName(ProcessName);
                foreach (var foundProcess in processes)
                    foundProcess.Kill();
            });
        }

        public bool ExecuteBeforeReplacingFiles { get; set; }
        public string Name => "StopProcess";
    }
}