using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public interface IUpdateAction
    {
        string Name { get; }
        string Description { get; }
        bool ExecuteBeforeReplacingFiles { get; set; }
        Task Execute();
    }
}
