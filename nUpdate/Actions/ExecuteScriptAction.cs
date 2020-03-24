// ExecuteScriptAction.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Reflection;
using System.Threading.Tasks;
using nUpdate.Actions.Exceptions;

namespace nUpdate.Actions
{
    public class ExecuteScriptAction : IUpdateAction
    {
        public string Code { get; set; }
        public string Description => "Executes a C# script.";

        public Task Execute()
        {
            return Task.Run(() =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                var provider = ServiceProviderHelper.CreateServiceProvider(assembly);
                if (provider == null)
                    throw new ServiceProviderMissingException();
                var appPathProvider =
                    (IUpdateActionAppPathProvider) provider.GetService(typeof(IUpdateActionAppPathProvider));
                if (appPathProvider == null)
                    throw new ServiceProviderMissingException(nameof(IUpdateActionAppPathProvider));

                var programFolder = appPathProvider.GetAppPath();
                var helper = new CodeDomHelper();
                helper.ExecuteScript(Code, new object[] {programFolder});
            });
        }

        public bool ExecuteBeforeReplacingFiles { get; set; }
        public string Name => "ExecuteScript";
    }
}