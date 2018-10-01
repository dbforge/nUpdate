using nUpdate.Administration.Common.Ftp;

namespace nUpdate.Administration.Common.Providers
{
    public interface INewProjectProvider : IFinishProvider
    {
        string GetFtpDirectory(FtpData data);
        string GetLocationDirectory(string initialDirectory);
        string GetUpdateDirectory();
    }
}
