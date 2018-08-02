using System;
using System.Collections.Generic;

namespace nUpdate.Updating
{
    public interface IStatisticUploadService
    {
        Guid ClientToken {get; set;}
        void UpdateRequested();
        void UpdateCheckResult(ICollection<UpdateConfiguration>results);
        void DownloadStarted();
        void DownloadFinished();

    }
}
