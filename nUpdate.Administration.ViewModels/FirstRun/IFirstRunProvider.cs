using nUpdate.Administration.Models;

namespace nUpdate.Administration.ViewModels.FirstRun
{
    public interface IFirstRunProvider : IFinishProvider
    {
        bool Finish(FirstSetupData firstSetupData);
        void GetApplicationDataDirectoryCommandAction(ref string applicationDataDirectory);
        void GetDefaultProjectDirectoryCommandAction(ref string defaultProjectDirectory);
    }
}
