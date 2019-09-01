using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace nUpdate
{
    public interface IUpdateProvider
    {
        IUpdateDeliveryEndpoint UpdateDeliveryEndpoint { get; }
        IVersion ApplicationVersion { get; set; }
        CultureInfo LanguageCulture { get; set; }

        Task<UpdateCheckResult> CheckForUpdates(CancellationToken cancellationToken);
        Task DownloadUpdates(UpdateCheckResult checkResult, CancellationToken cancellationToken, IProgress<UpdateProgressData> progress);
        Task<VerificationResult> VerifyUpdates(UpdateCheckResult checkResult);
        Task InstallUpdates(UpdateCheckResult checkResult, Action terminateAction = null);

        void RemoveLocalPackage(UpdatePackage package);
        void RemoveLocalPackages(IEnumerable<UpdatePackage> packages);
        void RemoveLocalPackages(UpdateCheckResult checkResult);
    }
}
