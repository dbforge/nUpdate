// WebserverUpdateDeliveryEndpoint.cs, 27.07.2019
// Copyright (C) Dominic Beger 01.09.2019

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace nUpdate
{
    internal class WebserverUpdateDeliveryEndpoint : IHttpUpdateDeliveryEndpoint
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Uri PackageDataFile { get; set; }

        public async Task<UpdateCheckResult> CheckForUpdates(IVersion applicationVersion, bool includePreRelease,
            CancellationToken cancellationToken)
        {
            var packageData = Utility.IsHttpUri(PackageDataFile)
                ? await new WebClientEx().DownloadStringTaskAsync(PackageDataFile.ToString())
                : File.ReadAllText(PackageDataFile.ToString());
            var packages = JsonSerializer.Deserialize<IEnumerable<UpdatePackage>>(packageData);

            var result = new UpdateCheckResult();
            await result.Initialize(packages, applicationVersion, includePreRelease, cancellationToken);
            return result;
        }

        public async Task DownloadPackage(UpdatePackage package, IProgress<UpdateProgressData> progress, CancellationToken cancellationToken)
        {
            var packageUri = package.PackageUri;
            var localPackagePath = Path.Combine(Globals.ApplicationUpdateDirectory, package.Guid.ToString(),
                Globals.PackageExtension);

            if (Utility.IsHttpUri(packageUri))
            {
                Logger.Info($"Downloading package of version {package.Version}.");
                await DownloadManager.DownloadFile(package.PackageUri, localPackagePath, package.Size,
                    cancellationToken, progress);
            }
            else
            {
                File.Copy(packageUri.ToString(),localPackagePath);
            }
        }

        public Task DownloadPackages(UpdateCheckResult checkResult, IProgress<UpdateProgressData> progress,
            CancellationToken cancellationToken)
        {
            long downloadedBytes = 0;
            long totalBytes = checkResult.Packages.Sum(p => p.Size);
            return checkResult.Packages.ForEachAsync(async p =>
            {
                var packageProgress = new Progress<UpdateProgressData>();
                long currentlyDownloadedBytes = 0;
                packageProgress.ProgressChanged += (sender, data) =>
                {
                    downloadedBytes += data.BytesReceived - currentlyDownloadedBytes;
                    currentlyDownloadedBytes += data.BytesReceived;
                    progress.Report(new UpdateProgressData(downloadedBytes,
                        // ReSharper disable once PossibleLossOfFraction
                        totalBytes, (float)(downloadedBytes / totalBytes) * 100));
                };
                await DownloadPackage(p, packageProgress, cancellationToken);
            });
        }
    }
}