using System.Collections.Generic;

namespace nUpdate.Administration.Common.Providers
{
    public interface IMainViewActionProvider : ILoadActionProvider
    {
        List<MainMenuItem> GetCollectionView();
        bool CanEditMasterPassword();
    }
}
