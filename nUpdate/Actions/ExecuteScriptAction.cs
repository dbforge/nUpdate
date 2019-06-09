using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public class ExecuteScriptAction : IUpdateAction
    {
        public string Name => "ExecuteScript";
        public string Description => "Executes a C# script.";
        public string Code { get; set; }

        public Task Execute(object parameter)
        {
            return Task.Run(() =>
            {
                string programFolder = (string) parameter;
                var helper = new CodeDomHelper();
                helper.ExecuteScript(Code, new object[]{programFolder});
            });
        }
    }
}
