using System.Threading.Tasks;

namespace nUpdate.Administration.Common
{
    public interface IUpdateProvider
    {
        Task UploadPackage();
    }
}
