namespace nUpdate.Administration.ViewModels
{
    public interface IMainViewActionProvider : ILoadActionProvider
    {
        void CreateNewProject();
        bool CanEditMasterPassword();
    }
}
