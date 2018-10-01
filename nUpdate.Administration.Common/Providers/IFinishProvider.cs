using System;

namespace nUpdate.Administration.Common.Providers
{
    public interface IFinishProvider
    {
        void SetFinishAction(out Action finishAction);
    }
}
