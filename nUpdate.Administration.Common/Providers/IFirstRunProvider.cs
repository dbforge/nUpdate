namespace nUpdate.Administration.Common.Providers
{
    public interface IFirstRunProvider : IFinishProvider
    {
        bool Finish(FirstSetupData firstSetupData);
        void GetApplicationDataDirectoryCommandAction(ref string applicationDataDirectory);
        void GetDefaultProjectDirectoryCommandAction(ref string defaultProjectDirectory);
    }
}
