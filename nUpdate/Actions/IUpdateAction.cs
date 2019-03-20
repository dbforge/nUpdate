using System.Threading.Tasks;

namespace nUpdate.Actions
{
    public interface IUpdateAction
    {
        string Name { get; }
        string Description { get; }
        Task Execute(object parameter);
    }
}
