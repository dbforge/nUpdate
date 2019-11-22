using nUpdate.Administration.Models.Ftp;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public interface INewProjectProvider : IFinishProvider
    {
        string GetFtpDirectory(FtpData data);
        string GetLocationDirectory(string initialDirectory);
        string GetUpdateDirectory();
    }
}
