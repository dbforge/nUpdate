using System;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate
{
    internal class GitHubUpdateDeliveryEndpoint : IHttpUpdateDeliveryEndpoint
    {
        public string RepositoryUrl { get; set; }

        public Task<UpdateCheckResult> CheckForUpdates(IVersion applicationVersion, bool includePreRelease, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DownloadPackage(UpdatePackage package, IProgress<UpdateProgressData> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DownloadPackages(UpdateCheckResult checkResult, IProgress<UpdateProgressData> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UpdatePackage> GetPackage(string versionData, IProgress<UpdateProgressData> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
