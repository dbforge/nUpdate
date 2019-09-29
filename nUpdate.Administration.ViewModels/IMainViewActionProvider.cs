using System.Collections.Generic;
using nUpdate.Administration.Common;

namespace nUpdate.Administration.ViewModels
{
    public interface IMainViewActionProvider : ILoadActionProvider
    {
        List<MainMenuItem> GetCollectionView();
        bool CanEditMasterPassword();
    }
}
