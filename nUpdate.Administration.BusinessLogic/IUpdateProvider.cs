using System.Threading.Tasks;

namespace nUpdate.Administration.BusinessLogic
{
    public interface IUpdateProvider
    {
        Task UploadPackage();
    }
}
