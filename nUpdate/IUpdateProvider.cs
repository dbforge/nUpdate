using System;
using System.Globalization;
using System.Threading.Tasks;

namespace nUpdate
{
    public interface IUpdateProvider
    {
        Uri MasterChannelUri { get; set; }
        string LowestUpdateChannel { get; set; }
        string ApplicationChannel { get; set; }
        UpdateVersion ApplicationVersion { get; set; }
        CultureInfo LanguageCulture { get; set; }

        Task<UpdateResult> BeginUpdateCheck();
        Task DownloadUpdates(IProgress<UpdateProgressData> progress);
        Task<VerificationResult> VerifyUpdates();
        Task InstallUpdates(Action terminateAction = null);
        void CancelUpdateCheck();
        void CancelDownload();
    }
}
