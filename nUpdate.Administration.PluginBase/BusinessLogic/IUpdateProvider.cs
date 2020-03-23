using System.Threading.Tasks;

namespace nUpdate.Administration.PluginBase.BusinessLogic
{
    public interface IUpdateProvider
    {
        Task UploadPackage();
    }
}
