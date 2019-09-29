using System;

namespace nUpdate.Administration.ViewModels
{
    public interface IFinishProvider
    {
        void SetFinishAction(out Action finishAction);
    }
}
