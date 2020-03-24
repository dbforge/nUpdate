// GitHubUpdateProvider.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octokit;

namespace nUpdate
{
    internal class GitHubUpdateProvider : UpdateProvider
    {
        private readonly GitHubClient _client;
        private readonly Dictionary<UpdatePackage, Uri> _packagesUris = new Dictionary<UpdatePackage, Uri>();

        public GitHubUpdateProvider(string repositoryAuthor, string repositoryName,
            Credentials authenticationCredentials, string publicKey, IVersion applicationVersion,
            UpdateChannelFilter updateChannelFilter)
            : base(publicKey, applicationVersion, updateChannelFilter)
        {
            _client = new GitHubClient(new ProductHeaderValue(ApplicationParameters.ProductName));
            RepositoryAuthor = repositoryAuthor ?? throw new ArgumentNullException(nameof(repositoryAuthor));
            RepositoryName = repositoryName ?? throw new ArgumentNullException(nameof(repositoryName));
            AuthenticationCredentials = authenticationCredentials ??
                                        throw new ArgumentNullException(
                                            nameof(authenticationCredentials)); // Do we actually need these?
        }

        public Credentials AuthenticationCredentials
        {
            get => _client.Credentials;
            set => _client.Credentials = value;
        }

        public int? RemainingRequests { get; private set; }

        public string RepositoryAuthor { get; set; }
        public string RepositoryName { get; set; }

        public DateTimeOffset? RequestLimitReset { get; private set; }

        public override async Task<UpdateCheckResult> CheckForUpdates(CancellationToken cancellationToken)
        {
            var apiInfo = _client.GetLastApiInfo();
            var rateLimit = apiInfo?.RateLimit;
            RemainingRequests = rateLimit?.Remaining;
            RequestLimitReset = rateLimit?.Reset;

            if (RemainingRequests.HasValue && RemainingRequests.Value < 1)
                throw new InvalidOperationException($"Server request limit reached until {RequestLimitReset}.");

            var releases = await _client.Repository.Release.GetAll(RepositoryAuthor, RepositoryName);
            var updatePackages = new List<UpdatePackage>();
            await releases.Where(r => !r.Draft).ForEachAsync(async r =>
            {
                var packageUpdateDataAsset = r.Assets.FirstOrDefault(a => a.Name.Equals("update.nupdgpkg"));
                if (packageUpdateDataAsset == null)
                    throw new InvalidOperationException(
                        $"The GitHub release for package {r.TagName} does not contain a package definition file and cannot be used with nUpdate.");

                var packageDefinition =
                    await new WebClientEx().DownloadStringTaskAsync(packageUpdateDataAsset.BrowserDownloadUrl);
                var currentPackage = JsonSerializer.Deserialize<UpdatePackage>(packageDefinition);
                updatePackages.Add(currentPackage);

                // Try to first get the URI independently from the package definition cause if it may change for any reason in the future, we are still able to download the package. Otherwise trust the data and rely on the specified URI.
                var packageAsset =
                    r.Assets.FirstOrDefault(a => a.Name.Equals($"{currentPackage.Guid}{Globals.PackageExtension}"));
                _packagesUris.Add(currentPackage,
                    packageAsset != null ? new Uri(packageAsset.BrowserDownloadUrl) : currentPackage.PackageUri);
            });

            var updateCheckResult = new UpdateCheckResult();
            await updateCheckResult.Initialize(updatePackages, ApplicationVersion, UpdateChannelFilter,
                cancellationToken, ClientConfiguration);
            return updateCheckResult;
        }

        public async Task DownloadPackage(UpdatePackage package, IProgress<UpdateProgressData> progress,
            CancellationToken cancellationToken)
        {
            var packageUri = _packagesUris[package];
            var localPackagePath = Path.Combine(Globals.ApplicationUpdateDirectory, package.Guid.ToString(),
                Globals.PackageExtension);
            await DownloadManager.DownloadFile(packageUri, localPackagePath, package.Size, cancellationToken, progress);
        }

        public override async Task DownloadUpdates(UpdateCheckResult checkResult, CancellationToken cancellationToken,
            IProgress<UpdateProgressData> progress)
        {
            long downloadedBytes = 0;
            var totalBytes = checkResult.Packages.Sum(p => p.Size);
            await checkResult.Packages.ForEachAsync(async p =>
            {
                var packageProgress = new Progress<UpdateProgressData>();
                long currentlyDownloadedBytes = 0;
                packageProgress.ProgressChanged += (sender, data) =>
                {
                    downloadedBytes += data.BytesReceived - currentlyDownloadedBytes;
                    currentlyDownloadedBytes += data.BytesReceived;
                    progress.Report(new UpdateProgressData(downloadedBytes,
                        // ReSharper disable once PossibleLossOfFraction
                        totalBytes, (float) (downloadedBytes / totalBytes) * 100));
                };
                await DownloadPackage(p, packageProgress, cancellationToken);
            });
        }

        public override Task<IEnumerable<string>> GetAvailableUpdateChannels()
        {
            throw new NotImplementedException();
        }
    }
}