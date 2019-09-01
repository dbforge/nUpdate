using System;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate
{
    public interface IUpdateDeliveryEndpoint
    {
        Task<UpdateCheckResult> CheckForUpdates(IVersion applicationVersion, bool includePreRelease, CancellationToken cancellationToken);
        Task DownloadPackage(UpdatePackage package, IProgress<UpdateProgressData> progress, CancellationToken cancellationToken);
        Task DownloadPackages(UpdateCheckResult checkResult, IProgress<UpdateProgressData> progress, CancellationToken cancellationToken);

    }
}
